using FSDBugTracker.Models;
using System.Collections.Generic;
using System.Linq;

namespace FSDBugTracker.Helpers
{
    public class TicketsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ICollection<Ticket> GetUserOwnedTickets(string userId)
        {
            return db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
        }

        public ICollection<Ticket> GetAssignedTickets(string userId)
        {
            return db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
        }
    }
}