using FSDBugTracker.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSDBugTracker.Helpers
{
    public class CounterHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private static UserRolesHelper roleHelper = new UserRolesHelper();
        private static UserProjectHelper projHelper = new UserProjectHelper();

        //Ticket Counters
        public static int CountTicketsByStatus(string status)
        {
            int count = 0;
            if (string.IsNullOrEmpty(status))
            {
                count = db.Tickets.Count();
            }
            else
            {
                count = db.Tickets.Where(t => t.TicketStatus.Name == status).Count();
            }
            return count;
        }

        public static int CountTicketsByType(string type)
        {
            int count = 0;
            if (string.IsNullOrEmpty(type))
            {
                count = db.Tickets.Count();
            }
            else
            {
                count = db.Tickets.Where(t => t.TicketType.Name == type).Count();
            }
            return count;
        }

        public static int CountTicketsByPriority(string priority)
        {
            int count = 0;
            if (string.IsNullOrEmpty(priority))
            {
                count = db.Tickets.Count();
            }
            else
            {
                count = db.Tickets.Where(t => t.TicketPriority.Name == priority).Count();
            }
            return count;
        }

        public static int CountMyTickets(string type)
        {
            int count = 0;
            var userId = HttpContext.Current.User.Identity.GetUserId(); 

            switch (type)
            {
                case "":
                    count = db.Tickets.Where(t => t.OwnerUserId == userId).Count() +
                            db.Tickets.Where(t => t.AssignedToUserId == userId).Count();
                    break;
                case "Owned":
                    count = db.Tickets.Where(t => t.OwnerUserId == userId).Count();
                    break;
                case "Assigned":
                    count = db.Tickets.Where(t => t.AssignedToUserId == userId).Count();
                    break;
                default:
                    break;
            }
            return count;
        }

        public static int CountMyProjects()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return db.Projects.Where(p => p.OwnerId == userId).Count();
        }

        public static int CountMyProjectTickets()
        {
            int count = 0;
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var projHelper = new UserProjectHelper();
            var myProjectTickets = new List<Ticket>();

            return count;
        }

        //Notification Counters
        public static int CountNotifications()
        {
            return db.Notifications.Count();
        }

        //Attachment Counters
        public static int CountAttachments()
        {
            return db.TicketAttachments.Count();
        }

        //History Counters
        public static int CountHistory()
        {
            return db.TicketUpdates.Count();
        }

        //Count Users by Role
        public static int CountUsersByRole(string roleName)
        {
            return roleHelper.UsersInRole(roleName).Count();
        }
    }
}
