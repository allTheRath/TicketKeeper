using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TicketKeeper.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string ProjectDiscripTion { get; set; }        
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ProjectManager { get; set; }
        public virtual ICollection<UserProject> ListOfUsers { get; set; }
        public virtual ICollection<Ticket> ListOfTickets { get; set; }

    }
}