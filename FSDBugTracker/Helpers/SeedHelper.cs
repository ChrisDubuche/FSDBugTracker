using FSDBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSDBugTracker.Helpers
{
    public class SeedHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserProjectHelper projHelper = new UserProjectHelper();

        private void ClearDB()
        {
            var projects = db.Projects;
            var tickets = db.Tickets;
            var ticketComments = db.TicketComments;
            var ticketAttachments = db.TicketAttachments;
            var ticketUpdates = db.TicketUpdates;
            var notifications = db.Notifications;

            foreach (var project in projects)
            {
                db.Projects.Remove(project);
            }

            foreach (var ticket in tickets)
            {
                db.Tickets.Remove(ticket);
            }

            foreach (var ticketComment in ticketComments)
            {
                db.TicketComments.Remove(ticketComment);
            }

            foreach (var ticketAttachment in ticketAttachments)
            {
                db.TicketAttachments.Remove(ticketAttachment);
            }

            foreach (var ticketUpdate in ticketUpdates)
            {
                db.TicketUpdates.Remove(ticketUpdate);
            }

            foreach (var notification in notifications)
            {
                db.Notifications.Remove(notification);
            }

            db.SaveChanges();
        }

        public void ProjectsTicketsCreator()
        {
            ClearDB();
            var projectManager = db.Users.FirstOrDefault(pm => pm.UserName == "demoprojectmanager@mailinator.com");
            var developer = db.Users.FirstOrDefault(pm => pm.UserName == "demodeveloper@mailinator.com");
            var submitter = db.Users.FirstOrDefault(pm => pm.UserName == "demosubmitter@mailinator.com");
            var admin = db.Users.FirstOrDefault(pm => pm.UserName == "demoadmin@mailinator.com");
            Random rnd = new Random();

            for (int p = 1; p < 11; p++)
            {
                //Create New Project
                var project = new Project();
                project.Name = "Auto-Gen Project " + p;
                project.Description = "Automatically generated project for test purposes only";

                project.ProjectUsers.Add(projectManager);
                project.ProjectUsers.Add(developer);
                project.ProjectUsers.Add(submitter);
                project.ProjectUsers.Add(admin);

                db.Projects.Add(project);
                db.SaveChanges();

                //Create Tickets for project
                for (int t = 1; t <= 5; t++)
                {
                    var ticket = new Ticket();
                    ticket.Title = "Auto-Gen Ticket " + t + " for " + project.Name; ;
                    ticket.Description = "Automatically generated ticket for test purposes only";
                    ticket.Created = DateTimeOffset.Now;
                    ticket.ProjectId = project.Id;
                    ticket.OwnerUserId = submitter.Id;
                    ticket.AssignedToUserId = developer.Id;

                    //Assign Ticket Status
                    int pickStatus = rnd.Next(1, 5);
                    var ticketStatus = new TicketStatus();

                    switch (pickStatus)
                    {
                        case 1:
                            ticketStatus = db.TicketStatuses.FirstOrDefault(ts => ts.Name == "Open/Assigned");
                            break;
                        case 2:
                            ticketStatus = db.TicketStatuses.FirstOrDefault(ts => ts.Name == "Resolved");
                            break;
                        case 3:
                            ticketStatus = db.TicketStatuses.FirstOrDefault(ts => ts.Name == "Waiting For Info");
                            break;
                        case 4:
                            ticketStatus = db.TicketStatuses.FirstOrDefault(ts => ts.Name == "Closed");
                            break;
                    }

                    ticket.TicketStatusId = ticketStatus.Id;

                    //Assign ticket Priority
                    int pickPriority = rnd.Next(1, 6);
                    var ticketPriority = new TicketPriority();

                    switch (pickPriority)
                    {
                        case 1:
                            ticketPriority = db.TicketPriorities.FirstOrDefault(ts => ts.Name == "Critical");
                            break;
                        case 2:
                            ticketPriority = db.TicketPriorities.FirstOrDefault(ts => ts.Name == "Higher");
                            break;
                        case 3:
                            ticketPriority = db.TicketPriorities.FirstOrDefault(ts => ts.Name == "High");
                            break;
                        case 4:
                            ticketPriority = db.TicketPriorities.FirstOrDefault(ts => ts.Name == "Medium");
                            break;
                        case 5:
                            ticketPriority = db.TicketPriorities.FirstOrDefault(ts => ts.Name == "Low");
                            break;
                    }

                    ticket.TicketPriorityId = ticketPriority.Id;

                    //Assign Ticket Type

                    int pickType = rnd.Next(1, 6);
                    var ticketType = new TicketType();

                    switch (pickType)
                    {
                        case 1:
                            ticketType = db.TicketTypes.FirstOrDefault(ts => ts.Name == "Bug");
                            break;
                        case 2:
                            ticketType = db.TicketTypes.FirstOrDefault(ts => ts.Name == "Task");
                            break;
                        case 3:
                            ticketType = db.TicketTypes.FirstOrDefault(ts => ts.Name == "Informational");
                            break;
                        case 4:
                            ticketType = db.TicketTypes.FirstOrDefault(ts => ts.Name == "Feature Request");
                            break;
                        case 5:
                            ticketType = db.TicketTypes.FirstOrDefault(ts => ts.Name == "Call For Documentation");
                            break;
                    }

                    ticket.TicketTypeId = ticketType.Id;

                    db.Tickets.Add(ticket);
                    db.SaveChanges();

                    for (int ta = 1; ta < 6; ta++)
                    {
                        int pickAttach = rnd.Next(1, 6);
                        var ticketAttachment = new TicketAttachment();

                        ticketAttachment.TicketId = ticket.Id;
                        ticketAttachment.Description = "Auto-Gen Ticket Attachment " + ta + ", " + ticket.Description;
                        ticketAttachment.Created = DateTimeOffset.Now;
                        ticketAttachment.UserId = developer.Id;

                        switch (pickAttach)
                        {
                            case 1:
                                ticketAttachment.MediaUrl = "/Assets/attachments/-----------";
                                break;
                            case 2:
                                ticketAttachment.MediaUrl = "/Assets/attachments/-----------";
                                break;
                            case 3:
                                ticketAttachment.MediaUrl = "/Assets/attachments/-----------";
                                break;
                            case 4:
                                ticketAttachment.MediaUrl = "/Assets/attachments/-----------";
                                break;
                            case 5:
                                ticketAttachment.MediaUrl = "/Assets/attachments/-----------";
                                break;
                        }

                        db.TicketAttachments.Add(ticketAttachment);
                        db.SaveChanges();
                    }

                    for (int tc = 1; tc < 6; tc++)
                    {
                        int pickComment = rnd.Next(1, 6);
                        var ticketComment = new TicketComment();

                        ticketComment.TicketId = ticket.Id;
                        ticketComment.Comment = "Auto-Generated Ticket Comment " + tc + ", " + ticket.Description;
                        ticketComment.Created = DateTimeOffset.Now;
                        ticketComment.UserId = developer.Id;

                        db.TicketComments.Add(ticketComment);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}