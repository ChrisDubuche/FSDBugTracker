using FSDBugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static FSDBugTracker.EmailService;

namespace FSDBugTracker.Helpers
{
    public class TicketsHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private UserProjectHelper projHelper = new UserProjectHelper();

        private UserManager<ApplicationUser>
            userManager = new UserManager<ApplicationUser>
            (new UserStore<ApplicationUser>
                (new ApplicationDbContext()));

        public ICollection<Ticket> GetUserOwnedTickets(string userId)
        {
            return db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
        }

        public ICollection<Ticket> GetAssignedTickets(string userId)
        {
            return db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
        }
        
        public ICollection<Ticket> GetPMTickets(string userId)
        {
            var pmTickets = new List<Ticket>();
            if (roleHelper.IsUserInRole(userId, "Project Manager"))
            {
                foreach (var project in projHelper.ListUserProjects(userId))
                {
                    foreach (var ticket in db.Tickets.Where(t => t.ProjectId == project.Id))
                    {
                        pmTickets.Add(ticket);
                    }
                }
            }
            return pmTickets;
        }

        public ICollection<Ticket> GetMyTickets()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var myTickets = new List<Ticket>();

            switch (myRole)
            {
                case "SuperUser":
                case "Admin":
                case "Developer":
                    myTickets = db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
                    break;
                case "Project Manager":
                    foreach (var project in projHelper.ListUserProjects(userId))
                    {
                        foreach (var ticket in db.Tickets.Where(t => t.ProjectId == project.Id))
                        {
                            myTickets.Add(ticket);
                        }
                    }
                    break;
                case "Submitter":
                    myTickets = db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
                    break;
                default:
                    break;
            }

            return myTickets;
        }

        
        public bool IsUserOnTicket(string userId, int ticketId)
        {
            return db.Tickets.FirstOrDefault(t => t.Id == ticketId).AssignedToUserId == userId;
        }


        public void AddUserToTicket(string userId, int ticketId)
        {
            //TODO:  Verify that this code is correct
            Ticket tix = db.Tickets.Find(ticketId);
            tix.AssignedToUserId = userId;
              

            db.Tickets.Attach(tix);
            db.SaveChanges();
        }

        public void RemoveUserFromTicket(string userId, int ticketId)
        {
            if (IsUserOnTicket(userId, ticketId))
            {
                Ticket tix = db.Tickets.Find(ticketId);
                var delUser = db.Users.Find(userId);

                tix.Users.Remove(delUser);
                db.Entry(tix).State = EntityState.Modified; 
                db.SaveChanges();
            }
        }

        #region TicketNotification Helper(s)
        //Abstracting away the work necessary to record a Notification
        public static async Task GenerateNotificationAsync(int ticketUpdateId, Ticket oldTicket, Ticket newTicket)
        {
            //I need to generate Notifications when the AssignedToUserId changes...that's all
            //Be carefull of NULLs
            var oldAssignedToId = oldTicket.AssignedToUserId;
            var newAssignedToId = newTicket.AssignedToUserId;
            var assignedUser = db.Users.Find(newAssignedToId);
            var currentUser = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            var newNotification = new Notification();
            newNotification.SenderId = HttpContext.Current.User.Identity.GetUserId();
            newNotification.SentDate = DateTime.Now;
            //newTicketNotification.Id = ticketHistoryId;
            newNotification.TicketId = oldTicket.Id;
            newNotification.NotifyReason = string.Format("Ticket Assignment for Ticket - {0} has changed", oldTicket.Title);
            newNotification.TicketUpdateId = ticketUpdateId;
            var notifcationBody = new StringBuilder();
            //Send Email Notifcation
            var notificationMessage = new NotificationEmailMessage();
            notificationMessage.Source = currentUser.Email;//HttpContext.Current.User.Identity.GetUserId();//You may need to be passing the current user's email here, instead of their id
            notificationMessage.Subject = newNotification.NotifyReason;
            if (oldAssignedToId == null)
            {
                //Case 1: Initial Ticket Assignement - oldTicket.AssignedToUserId = null and newTicket.AssignedToUserId has a value          
                newNotification.RecipientId = newAssignedToId;
                //Building the body of my Notification
                notifcationBody.Clear();
                notifcationBody.AppendLine("You have been assigned to a Ticket! Please read the following Ticket Details");
                notifcationBody.AppendLine("Ticket Title: " + newTicket.Title);
                notifcationBody.AppendLine("Ticket for Project Id: " + newTicket.ProjectId);
                newNotification.NotificationBody = notifcationBody.ToString();
                db.Notifications.Add(newNotification);
                db.SaveChanges();
                //Send Email Notifcation
                notificationMessage.Destination = newNotification.RecipientId;//pass the email address here, not the id
                notificationMessage.Body = newNotification.NotificationBody;
                await SendNotificationEmailAsync(notificationMessage);
            }
            else if ((oldAssignedToId != null && newAssignedToId != null) && (oldAssignedToId != newAssignedToId))
            {
                //Case 2: Reassigning a Ticket: oldTicket.TicketId has a value and newTicket.TicketId has a value but they are different
                //Notifcation 1: Assignment Notification
                newNotification.RecipientId = newAssignedToId;
                notifcationBody.Clear();
                notifcationBody.AppendLine("You have been assigned to a Ticket! Please read the following Ticket Details");
                notifcationBody.AppendLine("Ticket Title: " + newTicket.Title);
                notifcationBody.AppendLine("Ticket for Project Id: " + newTicket.ProjectId);////////////This notification is the exact same as the one below it
                newNotification.NotificationBody = notifcationBody.ToString();
                db.Notifications.Add(newNotification);
                db.SaveChanges();
                //Send Email Notifcation
                notificationMessage.Destination = assignedUser.Email;//.RecipientId;//pass the email address here, not the id
                notificationMessage.Body = newNotification.NotificationBody;
                await SendNotificationEmailAsync(notificationMessage);
                //Notifcation 2: UnAssignment Notification
                newNotification.RecipientId = oldAssignedToId;
                notifcationBody.Clear();
                notifcationBody.AppendLine(string.Format("Ticket Title: {0} has been reassigned to {1}", newTicket.Title, UserHelper.GetUserNameFromId(newAssignedToId)));
                newNotification.NotificationBody = notifcationBody.ToString();//Right here
                db.Notifications.Add(newNotification);
                db.SaveChanges();
                //Send Email Notifcation
                notificationMessage.Destination = assignedUser.Email; //newTicketNotification.RecipientId;//you need te be passing the email address here, not the Id
                notificationMessage.Body = newNotification.NotificationBody;
                await EmailService.SendNotificationEmailAsync(notificationMessage);
            }
            else if (oldAssignedToId != null && newAssignedToId == null)
            {
                //Case 3: Ticket Unassignment - oldTicket.AssignedToUserId has a value but newTicket.AssignedToUserId is null
                //Notifcation 1: Assignment Notification
                newNotification.RecipientId = oldAssignedToId;
                notifcationBody.Clear();
                notifcationBody.AppendLine(string.Format("You have been removed from Ticket Title: {0}", newTicket.Title));
                notifcationBody.AppendLine("Ticket Title: " + newTicket.Title);
                notifcationBody.AppendLine("Ticket for Project Id: " + newTicket.ProjectId);
                newNotification.NotificationBody = notifcationBody.ToString();
                db.Notifications.Add(newNotification);
                db.SaveChanges();
                //Send Email Notifcation
                notificationMessage.Destination = assignedUser.Email; //newTicketNotification.RecipientId;
                notificationMessage.Body = newNotification.NotificationBody;
                await EmailService.SendNotificationEmailAsync(notificationMessage);
            }
            var property = db.TicketUpdates.Find(ticketUpdateId).Property;
            //You can create your TicketNotifications based off of this property
            //You'd probably need to redo the code above to fit into this kind of structure
            //Right now, I am going to just wrap this in a mock if statement
            if (true)//You can add all the other properties to this switch statement that you'd want to create notifications for and send emails for and this would be a pretty 
                     //clean and easy way to handle it. Does that make sense?absolutely, sir.
                     //Ok. Also, you should probably try to go over your code above for the assignement notifications and try to make sure there aren't duplicates being made
                     //I would wait and take care of that after you add all the other properties in down here and get them taken care of.sure sir.I am vary happy that u help me a lot, sir.
                     //I'm glad to help. Your project is looking very good. You're doing a nice job. Keep it up!yes sir .wishing u a g9, sir. Thanks Tim. I hope you have a good night
                     //too and I will see you tomorrow. I can help you more tomorrow night and this weekend if you need it.Wowooowowow.Iam vey happy that you are available, sir. 
                     //Great. Well, I will talk to you tomorrow. Have a great night.u do the same !!!
            {
                Notification notification = new Notification()
                {
                    UpdateId = ticketUpdateId,
                    SenderId = currentUser.Id,
                    SentDate = DateTime.Now,
                    TicketUpdateId = ticketUpdateId,
                    TicketId = newTicket.Id
                };
                switch (property)
                {
                    case "title":
                        notification.NotifyReason = "Ticket title change";
                        notification.RecipientId = newTicket.AssignedToUserId;
                        notification.NotificationBody = $"The title of a ticket that you are assigned to has changed from \"{oldTicket.Title}\" to \"{newTicket.Title}\"";
                        break;
                    default:
                        break;
                }
                db.Notifications.Add(notification);
                db.SaveChanges();
                NotificationEmailMessage email = new NotificationEmailMessage()
                {
                    Body = notification.NotificationBody,
                    Destination = assignedUser.Email,
                    Source = currentUser.Email,
                    Subject = notification.NotifyReason
                };
                await EmailService.SendNotificationEmailAsync(email);
            }
        }
        #endregion
    }
}