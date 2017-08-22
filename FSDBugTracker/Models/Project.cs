using System;
using System.Collections.Generic;

namespace FSDBugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }
        //public ApplicationUser ProjectUser { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        //Nav
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> ProjectUsers { get; set; }
      
        //Con
        public Project()
        {
            this.Tickets = new HashSet<Ticket>();
            this.ProjectUsers = new HashSet<ApplicationUser>();
        }

    }
}