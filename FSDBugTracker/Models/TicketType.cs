using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSDBugTracker.Models
{
    public class TicketType
    {
        public int Id { get; set; }

        [Display(Name = "Type")]
        public string Name { get; set; }

        //Nav
        public virtual ICollection<Ticket> Tickets { get; set; }

        //Con
        public TicketType()
        {
            this.Tickets = new HashSet<Ticket>();
        }
    }
}