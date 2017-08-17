using FSDBugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSDBugTracker.Helpers
{
    public class MyUserHelper
    {
        //incomplete alt method
        #region
        //ApplicationDbContext db = new ApplicationDbContext();

        //public bool IsAssignedOnTicket(string userId, int ticketId)
        //{
        //    var ticket = db.Tickets.Find(string userId, int ticketId)
        //    var flag = ticket.Users.Any(u => u.Id == userId);
        //    return (flag);
        //}
    #endregion

        private UserManager<ApplicationUser>
            userManager = new UserManager<ApplicationUser>
            (new UserStore<ApplicationUser>
                (new ApplicationDbContext()));

        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsAssignedOnTicket(string userId, int ticketId)
        {            
            return db.Tickets.Any(t => t.AssignedToUserId == userId && t.Id == ticketId);
        }
    }
}