﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FSDBugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string MediaUrl { get; set; }



        //[FK]
        [DisplayName("Project Name")]
        public int ProjectId { get; set; }

        [Display(Name = "Ticket Type")]
        public int TicketTypeId { get; set; }

        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }

        [Display(Name = "Owner Name")]
        public string OwnerUserId { get; set; }

        [Display(Name = "Assigned Name")]
        public string AssignedToUserId { get; set; }

        //[Nav]            
        public virtual Project Project { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketUpdate> TicketUpdates { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        //Con
        public Ticket()
        {
            this.TicketAttachments = new HashSet<TicketAttachment>();
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketUpdates = new HashSet<TicketUpdate>();
            this.Notifications = new HashSet<Notification>();
        }
    }
}