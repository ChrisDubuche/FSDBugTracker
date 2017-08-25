using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSDBugTracker.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }

        [Display(Name = "Priority")]
        public string Name { get; set; }

        //Nav
        public virtual ICollection<Ticket> Tickets { get; set; }

        //Constructor
        public TicketPriority()
        {
            this.Tickets = new HashSet<Ticket>();
        }
    }
}