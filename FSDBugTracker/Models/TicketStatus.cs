﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSDBugTracker.Models
{
    public class TicketStatus
    {
        public int Id { get; set; }

        [Display(Name = "Status")]
        public string TicketStatusName { get; set; }

        //Nav
        public virtual ICollection<Ticket> Tickets { get; set; }

        //Constructor
        public TicketStatus()
        {
            this.Tickets = new HashSet<Ticket>();
        }


    }

}