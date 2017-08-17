using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSDBugTracker.Models
{
    public class TicketUpdate
    {
        public int Id { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTimeOffset ChangedDate { get; set; }

        //FK's
        public int TicketId { get; set; }
        public string UserId { get; set; }
        
        //Nav
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        //Constructor
        public TicketUpdate()
        {
            this.Notifications = new HashSet<Notification>();
        }
    }
}