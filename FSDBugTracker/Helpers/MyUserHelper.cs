using FSDBugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;

namespace FSDBugTracker.Helpers
{
    public class MyUserHelper
    {
        #region

        private UserManager<ApplicationUser>
            userManager = new UserManager<ApplicationUser>
            (new UserStore<ApplicationUser>
                (new ApplicationDbContext()));

        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsAssignedOnTicket(string userId, int ticketId)
        {            
            return db.Tickets.Any(t => t.AssignedToUserId == userId && t.Id == ticketId);
        }

        #endregion
    }
}