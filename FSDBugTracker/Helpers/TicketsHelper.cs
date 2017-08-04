using FSDBugTracker.Models;
using System.Collections.Generic;
using System.Linq;

namespace FSDBugTracker.Helpers
{
    public class TicketsHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private UserProjectHelper projHelper = new UserProjectHelper();

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
    }
}