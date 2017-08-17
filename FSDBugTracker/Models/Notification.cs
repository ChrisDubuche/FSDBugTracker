using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSDBugTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public DateTimeOffset SentDate { get; set; }
      
        public string NotifyReason { get; set; }
        public string NotificationBody { get; set; }

        //FK's
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string RecipientId { get; set; }

        public int TicketUpdateId { get; set; }

        //Nav
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
        public virtual TicketUpdate TicketUpdate { get; set; }

        //public ICollection<TicketUpdate> TicketUpdates { get; set; }

        //public Notification()
        //{
        //    this.TicketUpdates = new HashSet<TicketUpdate>();
        //}

    }
}