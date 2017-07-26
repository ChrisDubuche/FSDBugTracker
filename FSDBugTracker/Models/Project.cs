using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSDBugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //Nav
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        //Con
        public Project()
        {
            this.Tickets = new HashSet<Ticket>();
            this.Users = new HashSet<ApplicationUser>();
        }

    }
}