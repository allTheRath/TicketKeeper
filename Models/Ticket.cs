using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TicketKeeper.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Discription { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Created On")]
        public DateTime Created { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Updated On")]
        public DateTime? Updated { get; set; }

        [DisplayName("For Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        [DisplayName("Type Of")]
        public int TicketTypeId { get; set; }
        public virtual TicketType TicketName { get; set; }

        [DisplayName("Priority")]
        public int TicketPriorityId { get; set; }
        public virtual TicketPriority PriorityName { get; set; }

        [DisplayName("Progress")]
        public int TicketStatusId { get; set; }
        public virtual TicketStatus StatusName { get; set; }

        [DisplayName("Who Created")]
        public string OwenerUserId { get; set; }
        public virtual ApplicationUser OwenerUser { get; set; }

        [DisplayName("Assigned To")]
        public string AssignedToUserId { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }

        public virtual ICollection<TicketAttachments> TicketAttachments { get; set; }
        public virtual ICollection<TicketComments> TicketComments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
    }

    public class TicketForJQuery
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Discription { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string ProjectTitle { get; set; }
        public string Type { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string OwenerUser { get; set; }
        public string AssignedTo { get; set; }
    }

    public class TicketType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }

    public class TicketStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
    }

    public class TicketPriority
    {
        public int Id { get; set; }
        public string PriorityName { get; set; }
    }

    public class TicketAttachments
    {
        public int Id { get; set; }
        [DisplayName("Ticket Title")]
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        [DisplayName("Upload File")]
        public string FilePath { get; set; }
        public string Discription { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }
        [DisplayName("User Who Attached")]
        public string UserId { get; set; }
        public virtual ApplicationUser AttachmentOfUser { get; set; }

    }

    public class TicketAttachmentWithFile
    {
        [DisplayName("Ticket Title")]
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public string Discription { get; set; }
    }

    public class TicketComments
    {
        public int Id { get; set; }
        [DisplayName("Write Comment")]
        public string Comment { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Commented On")]
        public DateTime Created { get; set; }
        [DisplayName("Ticket Title")]
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        [DisplayName("Who Commented")]
        public string UserId { get; set; }
        public virtual ApplicationUser Commentor { get; set; }
    }


    public class TicketWithListOfCommentsAndAttachmentsModel
    {
        [DisplayName("Ticket Title")]
        public string Title { get; set; }
        public string Discription { get; set; }
        [DisplayName("Created On")]
        public DateTime Created { get; set; }
        [DisplayName("Updated On")]
        public DateTime? Updated { get; set; }
        public Project Project { get; set; }
        [DisplayName("Ticket Type")]
        public string TicketName { get; set; }
        [DisplayName("Priority")]
        public string PriorityName { get; set; }
        [DisplayName("Progress")]
        public string StatusName { get; set; }
        [DisplayName("Who Created")]
        public string OwenerUser { get; set; }
        [DisplayName("Assigned User")]
        public string AssignedUser { get; set; }
        public List<TicketComments> Comments { get; set; }
        public List<TicketAttachments> Attachments { get; set; }
        public TicketWithListOfCommentsAndAttachmentsModel()
        {
            Comments = new List<TicketComments>();
            Attachments = new List<TicketAttachments>();
        }
    }

    public class TicketHistory
    {
        public int Id { get; set; }
        [DisplayName("Ticket Title")]
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        public string Property { get; set; }
        [DisplayName("Old Value")]
        public string OldValue { get; set; }
        [DisplayName("Updated Value")]
        public string NewValue { get; set; }
        [Required]
        [DisplayName("Changed On")]
        public DateTime Changed { get; set; }
        [DisplayName("Who Changed")]
        public string UserId { get; set; }
        public virtual ApplicationUser OwnerOfEdit { get; set; }
    }

    public class TicketNotification
    {
        public int Id { get; set; }
        [DisplayName("Ticket Title")]
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        [DisplayName("Notified To")]
        public string UserId { get; set; }
        public virtual ApplicationUser NotifyUser { get; set; }

    }
}