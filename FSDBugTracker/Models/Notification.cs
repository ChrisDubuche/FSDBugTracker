using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSDBugTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Display(Name = "Sent Date Time")]
        public DateTimeOffset SentDate { get; set; }

        [Display(Name = "Notify Reason")]
        public string NotifyReason { get; set; }
        public int UpdateId { get; set; }

        [Display(Name = "Notification Body")]
        public string NotificationBody { get; set; }

        public bool Archived { get; set; } // review JT's controller/view folders' code

        //FK's
        public int TicketId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }

        public int TicketUpdateId { get; set; }

        //Nav
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
        public virtual TicketUpdate TicketUpdate { get; set; }

       

    }
}