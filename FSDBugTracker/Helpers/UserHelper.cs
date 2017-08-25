using FSDBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSDBugTracker.Helpers
{
    public static class UserHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetUserNameFromId(string userId)
        {
            ApplicationUser user = new ApplicationUser();
            if (userId.Contains("@"))
            {
                user = db.Users.FirstOrDefault(u => u.Email == userId);
            }
            else
            {
                user = db.Users.FirstOrDefault(u => u.Id == userId);
            }
            var fullName = user.FirstName + " " + user.LastName;
            return fullName;
        }
        //Check for issues in this code
        public static string GetUserEmailFromId(string userId)
        {
            ApplicationUser user = new ApplicationUser();
            if (userId.Contains("@"))
            {
                user = db.Users.FirstOrDefault(u => u.Email == userId);
            }
            else
            {
                user = db.Users.FirstOrDefault(u => u.Id == userId);
            }
            return user.Email;
        }
    }
}