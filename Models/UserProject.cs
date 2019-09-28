using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TicketKeeper.Models
{
    public class UserProject
    {
        public int Id { get; set; }
        [DisplayName("Project Title")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [DisplayName("User Mail")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}