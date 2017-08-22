using FSDBugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

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












    }
}