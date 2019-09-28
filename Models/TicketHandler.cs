using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TicketKeeper.Models
{
    public class TicketHandler
    {
        private protected ApplicationDbContext db { get; set; }
        private protected RoleHandlerClass RoleHandler { get; set; }

        public TicketHandler()
        {
            db = new ApplicationDbContext();
            RoleHandler = new RoleHandlerClass();
        }

        //Create tickets
        //Log software issue instance
        /// <summary>
        /// Create a Ticket.
        /// </summary>
        /// <param name="ticket"></param>


        public void CreateTicket(Ticket ticket)
        {

            db.Tickets.Add(ticket);
        }



        /// <summary>
        /// Create ticket comments
        /// </summary>
        /// <param name="comments"></param>
        public void CreateTicketComment(TicketComments comments)
        {
            comments.Created = DateTime.Now;
            db.TicketComments.Add(comments);
            db.SaveChanges();
        }

        /// <summary>
        /// Authenticated users must be able to view a list of all tickets 
        /// </summary>
        [Authorize]
        public List<Ticket> GetListOfAllTickets()
        {
            return db.Tickets.ToList();
        }

        /// <summary>
        /// Project Managers must be able to view a list of all tickets belonging to the projects to which they are assigned
        /// </summary>
        public List<TicketForJQuery> GetAllTicketsForProjectManager(string userID)
        {
            var result = new List<Ticket>();
            var user = db.Users.Find(userID);
            if (user == null)
            {
                return new List<TicketForJQuery>();
            }

            result.AddRange(db.Tickets.Where(t => db.Projects.Find(t.ProjectId).ApplicationUserId == userID).ToList());

            var resultForPM = ConvertToTicketJQueryModel(result);
            return resultForPM;
        }

        public List<Ticket> AssignUserWithEmail(List<Ticket> tickets)
        {
            tickets.ForEach(t =>
            {
                if (t.AssignedToUserId != null && t.AssignedToUserId != "")
                {
                    t.AssignedToUserId = db.Users.Find(t.AssignedToUserId).Email;
                }
            });
            return tickets;
        }

        public List<Ticket> GetAllTicketForPM(string userID)
        {
            var result = new List<Ticket>();
            var user = db.Users.Find(userID);
            if (user == null)
            { return result; }
            result.AddRange(db.Tickets.Where(t => t.Project.ApplicationUserId == userID).ToList());
            return AssignUserWithEmail(result);

        }

        /// <summary>
        /// List tickets by owner Id who is submitter.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public List<TicketForJQuery> GetAllTicketsForSubmitter(string ownerId)
        {
            var result = new List<Ticket>();
            var user = db.Users.Find(ownerId);
            if (user == null)
            {
                return new List<TicketForJQuery>();
            }

            db.Tickets.Where(x => x.OwenerUserId == ownerId).ToList().ForEach(t =>
            {
                result.Add(t);
            });
            var resultForSM = ConvertToTicketJQueryModel(result);
            return resultForSM;
        }

        public bool IsTicketBelongToSubmitter(string submitterId, int ticketId)
        {
            Ticket ticket = db.Tickets.Find(ticketId);
            if (ticket.OwenerUserId == submitterId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsTicketBelongToProjectManager(string projectManagerId, int ticketId)
        {
            Ticket ticket = db.Tickets.Find(ticketId);
            if (db.Projects.Find(ticket.ProjectId).ApplicationUserId == projectManagerId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsTicketBelongToDeveloper(string developerId, int ticketId)
        {
            Ticket ticket = db.Tickets.Find(ticketId);
            if (ticket.AssignedToUserId == developerId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// List tickets by assignedUser Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<TicketForJQuery> GetAllTicketsForDeveloper(string userId)
        {
            var result = new List<Ticket>();
            var user = db.Users.Find(userId);
            if (user == null)
            {
                return new List<TicketForJQuery>();
            }

            db.Tickets.Where(x => x.AssignedToUserId == userId).ToList().ForEach(t =>
            {
                result.Add(t);
            });

            var resultForDP = ConvertToTicketJQueryModel(result);
            return resultForDP;
        }

        private protected List<TicketForJQuery> ConvertToTicketJQueryModel(List<Ticket> ticket)
        {
            List<TicketForJQuery> ticketsForJQuery = new List<TicketForJQuery>();

            ticket.ForEach(t =>
            {
                TicketForJQuery ticketForJQueryOBJ = new TicketForJQuery();
                ticketForJQueryOBJ.AssignedTo = t.AssignedUser != null ? t.AssignedUser.Email : "Not Assigned";
                ticketForJQueryOBJ.Created = t.Created;
                ticketForJQueryOBJ.Discription = t.Discription;
                ticketForJQueryOBJ.OwenerUser = t.OwenerUser.Email;
                ticketForJQueryOBJ.Priority = t.PriorityName.PriorityName;
                ticketForJQueryOBJ.ProjectTitle = t.Project.ProjectName;
                ticketForJQueryOBJ.Status = t.StatusName.StatusName;
                ticketForJQueryOBJ.Title = t.Title;
                ticketForJQueryOBJ.Type = t.TicketName.TypeName;
                ticketForJQueryOBJ.Updated = t.Updated;
                ticketForJQueryOBJ.Id = t.Id;

                ticketsForJQuery.Add(ticketForJQueryOBJ);
            });

            return ticketsForJQuery;
        }

        /// <summary>
        /// Get all Tickets
        /// </summary>
        /// <returns></returns>
        public List<TicketForJQuery> GetAllTicketsForAdmin()
        {
            var result = ConvertToTicketJQueryModel(db.Tickets.ToList());
            return result;
        }


        /// <summary>
        ///   Assign tickets to user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ticketId"></param>

        public void AssignTicketToUserByAdmin(string userId, int ticketId)
        {
            var user = db.Users.Find(userId);
            Ticket ticket = db.Tickets.Find(ticketId);
            if (user == null || ticket == null)
            {
                return;
            }

            ticket.AssignedToUserId = userId;
            ticket.AssignedUser = user;
            db.SaveChanges();
        }

        public void AssignTicketToUserByProjectManager(string userId, int ticketId)
        {
            var user = db.Users.Find(userId);
            Ticket ticket = db.Tickets.Find(ticketId);
            if (user == null || ticket == null)
            {
                return;
            }

            ticket.AssignedToUserId = userId;
            ticket.AssignedUser = user;
            db.SaveChanges();
        }

        /// <summary>
        /// Assign tickets to user
        /// </summary>
        /// <param name="ticketId"></param>
        public void EditExistingTicket(Ticket ticket)
        {
            Ticket existingTicket = db.Tickets.Find(ticket.Id);
            existingTicket.Title = ticket.Title;
            existingTicket.TicketName = ticket.TicketName;
            existingTicket.Discription = ticket.Discription;
            existingTicket.Updated = DateTime.Now;
            db.SaveChanges();
        }

        /// <summary>
        /// Create ticket attachments
        /// </summary>
        /// <param name="attachments"></param>
        public void CreateTicketAttachment(TicketAttachments attachments)
        {
            attachments.Created = DateTime.Now;
            db.TicketAttachments.Add(attachments);
            db.SaveChanges();
        }

        /// <summary>
        ///List comments, attachments per ticket 
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        public TicketWithListOfCommentsAndAttachmentsModel GetListOfAllCommentsAndAttachmentsOfTicket(int ticketId)
        {
            TicketWithListOfCommentsAndAttachmentsModel model = new TicketWithListOfCommentsAndAttachmentsModel();
            Ticket ticket = db.Tickets.Find(ticketId);
            if (ticket == null)
            {
                return model;
            }
            model.AssignedUser = ticket.AssignedUser.Email;
            model.Created = ticket.Created;
            model.Discription = ticket.Discription;
            model.OwenerUser = ticket.OwenerUser.Email;
            model.PriorityName = ticket.PriorityName.PriorityName;
            model.StatusName = ticket.StatusName.StatusName;
            model.TicketName = ticket.TicketName.TypeName;
            model.Title = ticket.Title;
            model.Updated = ticket.Updated;

            ticket.TicketAttachments.ToList().ForEach(attach =>
            {
                model.Attachments.Add(attach);
            });
            ticket.TicketComments.ToList().ForEach(comment =>
            {
                model.Comments.Add(comment);
            });

            return model;
        }

        /// <summary>
        /// List history of a ticket’s changes
        /// </summary>
        /// <param name="ticketId"></param>
        public List<TicketHistory> GetHistoriesOfTicket(int ticketId)
        {
            Ticket ticket = db.Tickets.Find(ticketId);
            var result = new List<TicketHistory>();
            if (ticket == null)
            {
                return result;
            }

            db.TicketHistories.Where(x => x.TicketId == ticketId).ToList().ForEach(history =>
            {
                result.Add(history);
            });
            return result;
        }

        //Ticket Comments


        /// <summary>
        /// Administrators must be able to add Comments to any ticket
        /// </summary>
        /// <param name="ticket"></param>
        public void AddCommentToTicket(TicketComments comment)
        {
            Ticket ticket = db.Tickets.Find(comment.TicketId);
            if (ticket == null)
            {
                return;
            }
            db.TicketComments.Add(comment);
            db.SaveChanges();
        }
        /// <summary>
        ///Project Managers must be able to add Comments to tickets belonging to Projects to which they are assigned
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        public void AddCommentToTicketForProjectManager(TicketComments comment, string userId)
        {
            Ticket ticket = db.Tickets.Find(comment.TicketId);
            if (ticket == null)
            {
                return;
            }
            var userProject = db.UserProjects.Where(x => x.ApplicationUserId == userId && x.ProjectId == comment.Ticket.ProjectId).FirstOrDefault();
            if (userProject != null)
            {
                db.TicketComments.Add(comment);
                db.SaveChanges();

            }
        }

        /// <summary>
        ///Developers must be able to add Comments to tickets to which they are assigned
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        public void AddCommentToTicketForDeveloper(TicketComments comment, string userId)
        {
            Ticket ticket = db.Tickets.Find(comment.TicketId);
            if (ticket == null)
            {
                return;
            }
            if (ticket.AssignedToUserId == userId)
            {
                db.TicketComments.Add(comment);
                db.SaveChanges();
            }
        }

        //Submitters must be able to add Comments to tickets they own
        public void AddCommentToTicketForSubmitter(TicketComments comment, string userId)
        {
            Ticket ticket = db.Tickets.Find(comment.TicketId);
            if (ticket == null)
            {
                return;
            }
            if (ticket.OwenerUserId == userId)
            {
                db.TicketComments.Add(comment);
                db.SaveChanges();
            }
        }

        //Ticket Attachments 
        //	Administrators must be able to add Attachments to any ticket
        public void AddAttachmentByAdmin(TicketAttachments attachment)
        {
            db.TicketAttachments.Add(attachment);
            db.SaveChanges();

        }


        //Project Managers must be able to add Attachments to tickets belonging to Projects to which they are assigned
        public void AddAttachmentByProjectManager(TicketAttachments attachment, string projectManagerId)
        {
            if (attachment.Ticket.Project.ApplicationUserId == projectManagerId)
            {
                db.TicketAttachments.Add(attachment);
                db.SaveChanges();

            }
        }


        //Developers must be able to add Attachments to tickets to which they are assigned
        public void AddAttachmentByDeveloper(TicketAttachments attachment, string developerId)
        {
            if (attachment.Ticket.AssignedToUserId == developerId)
            {
                db.TicketAttachments.Add(attachment);
                db.SaveChanges();
            }
        }

        //Submitters must be able to add Attachments to tickets they own
        public void AddAttachmentBySubmitter(TicketAttachments attachment, string submitterId)
        {
            if (attachment.Ticket.OwenerUserId == submitterId)
            {
                db.TicketAttachments.Add(attachment);
                db.SaveChanges();
            }
        }

        //Ticket Histories

        //A new History object must be created for each property change made to a ticket 
        //(History objects need not be created for the addition of comments or attachments)

        public void AddHistoryOfTicket(TicketHistory ticketHistory)
        {
            db.TicketHistories.Add(ticketHistory);
            db.SaveChanges();
        }

        //Ticket Notifications 


        public void CreateTicketNotification(TicketNotification ticketNotification)
        {
            db.TicketNotifications.Add(ticketNotification);
            db.SaveChanges();
        }

        //Developers must be notified each time they are assigned to a ticket
        public List<TicketNotification> GetAllNotificationsForDeveloper(string userId)
        {
            var result = new List<TicketNotification>();
            var user = db.Users.Find(userId);
            if (user == null)
            {
                return result;
            }

            result.AddRange(db.TicketNotifications.Where(x => x.UserId == userId).ToList());

            return result;
        }

        //	Developers must be notified each time a ticket to 
        //which they are assigned is modified by another user (including the addition of comments and attachments)

        //[Authorize(Roles = "Developer")]
        //public List<TicketNotification> GetAllNotificationsForDeveloperModifyByOther(string userId)
        //{
        //    var result = new List<TicketNotification>();
        //    var user = db.Users.Find(userId);
        //    if (user == null)
        //    {
        //        return result;
        //    }

        //    result.AddRange(db.TicketNotifications.Where(x => x.UserId == userId).ToList());

        //    return result;
        //}


        //A ticket Details page must provide a summary of all ticket information, including a list of all comments, attachments, histories



        public List<TicketWithListOfCommentsAndAttachmentsModel> GetAllAssignedTicketWithDetails(string userId)
        {
            //GetListOfAllCommentsAndAttachmentsOfTicket()
            var result = new List<TicketWithListOfCommentsAndAttachmentsModel>();
            db.Tickets.Where(x => x.AssignedToUserId == userId).Select(x => x.Id).ToList().ForEach(ticketId =>
            {
                result.Add(GetListOfAllCommentsAndAttachmentsOfTicket(ticketId));
            });

            return result;
        }

        public List<TicketWithListOfCommentsAndAttachmentsModel> GetAllOwnedTicketWithDetails(string userId)
        {
            //GetListOfAllCommentsAndAttachmentsOfTicket()
            var result = new List<TicketWithListOfCommentsAndAttachmentsModel>();
            db.Tickets.Where(x => x.OwenerUserId == userId).Select(x => x.Id).ToList().ForEach(ticketId =>
            {
                result.Add(GetListOfAllCommentsAndAttachmentsOfTicket(ticketId));
            });

            return result;
        }



    }

}