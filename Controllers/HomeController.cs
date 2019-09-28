using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketKeeper.Models;

namespace TicketKeeper.Controllers
{
    public class HomeController : Controller
    {
        private protected ApplicationDbContext db = new ApplicationDbContext();

        private protected RoleHandlerClass RoleHelper = new RoleHandlerClass();

        private protected ProjectHandler ProjectHandler = new ProjectHandler();

        private protected TicketHandler TicketHandler = new TicketHandler();

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var roles = RoleHelper.GetUserRoles(userId);
                if (roles.Count == 1)
                {
                    return RedirectToAction(roles[0]);
                }
                else if (roles.Count > 1)
                {
                    return RedirectToAction("ChoseDashboard");
                }
            }

            return View();
        }

        public ActionResult ChoseDashboard()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.Email = db.Users.Find(userId).Email;
            ViewBag.Roles = RoleHelper.GetUserRoles(userId);
            return View();
        }

        public ActionResult Admin()
        {
            string adminId = User.Identity.GetUserId();

            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Email = db.Users.Find(adminId).Email;

            return View();
        }


        public ActionResult AssignRole()
        {
            var users = db.Users.ToList();

            ViewBag.users = users;
            ViewBag.UnassignedUsers = RoleHelper.AllUnassignedUser();
            ViewBag.Admins = RoleHelper.GetAllAdmins();
            ViewBag.ProjectManagers = RoleHelper.GetAllProjectManagers();
            ViewBag.Developers = RoleHelper.GetAllDevelopers();
            ViewBag.Submitters = RoleHelper.GetAllSubmitters();
            ViewBag.RoleList = RoleHelper.GetAllRoles();
            return View();
        }
        [HttpPost, ActionName("AssignRole")]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRoleConfirm(string Id, string Name)
        {
            RoleHelper.AssignUserToRole(Id, Name);
            return RedirectToAction("AssignRole");
        }

        public ActionResult UnAssignRole()
        {
            var users = db.Users.ToList();
            ViewBag.users = users;
            ViewBag.UnassignedUsers = RoleHelper.AllUnassignedUser();
            ViewBag.Admins = RoleHelper.GetAllAdmins();
            ViewBag.ProjectManagers = RoleHelper.GetAllProjectManagers();
            ViewBag.Developers = RoleHelper.GetAllDevelopers();
            ViewBag.Submitters = RoleHelper.GetAllSubmitters();
            ViewBag.RoleList = RoleHelper.GetAllRoles();
            return View();
        }


        [HttpPost, ActionName("UnAssignRole")]
        [ValidateAntiForgeryToken]
        public ActionResult UnAssignRoleConfirm(string Id, string Name)
        {
            RoleHelper.UnAssignUserToRole(Id, Name);
            return RedirectToAction("UnAssignRole");
        }

        public ActionResult ProjectManager()
        {
            if (!User.IsInRole("ProjectManager") && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Email = db.Users.Find(User.Identity.GetUserId()).Email;
            return View();
        }

        public ActionResult MyProjects()
        {
            if (!User.IsInRole("ProjectManager") && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            string userId = User.Identity.GetUserId();
            ViewBag.Email = db.Users.Find(userId).Email;
            var allProjects = ProjectHandler.GetAllProjects(User.Identity.GetUserId());
            var MyprojectList = new List<Project>();
            var allProjectsWithoutManager = new List<Project>();
            allProjects.ForEach(p =>
            {
                if (p.ApplicationUserId == userId)
                {
                    MyprojectList.Add(p);

                }
                else
                {
                    allProjectsWithoutManager.Add(p);
                }
            });
            ViewBag.MyProjects = MyprojectList;
            return View(allProjectsWithoutManager);
        }


        public ActionResult CreateProject()
        {
            if (!User.IsInRole("ProjectManager") && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Email = db.Users.Find(User.Identity.GetUserId()).Email;

            return View();
        }
        [HttpPost, ActionName("CreateProject")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProjectConfirm([Bind(Include = "ProjectDiscripTion,ProjectName")] Project project)
        {
            // User Accessing this page will only be project managers.

            project.ApplicationUserId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
            }
            return RedirectToAction("ProjectManager");
        }
        public ActionResult EditProject(int? id)
        {
            if (!User.IsInRole("ProjectManager") && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Email = db.Users.Find(User.Identity.GetUserId()).Email;
            var allApplicableManager = RoleHelper.GetAllProjectManagers(true);
            allApplicableManager.AddRange(RoleHelper.GetAllAdmins(true));
            ViewBag.ApplicationUserId = allApplicableManager.Distinct();
            Project p = db.Projects.Find(id);
            return View(p);
        }
        [HttpPost, ActionName("EditProject")]
        [ValidateAntiForgeryToken]
        public ActionResult EditProjectConfirm([Bind(Include = "ApplicationUserId,ProjectDiscripTion,ProjectName")] Project project)
        {
            ProjectHandler.EditProject(project);

            return RedirectToAction("MyProjects");
        }

        public ActionResult ProjectDetails(int? id)
        {
            if (!User.IsInRole("ProjectManager") && !User.IsInRole("Admin") || id == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Email = db.Users.Find(User.Identity.GetUserId()).Email;
            Project p = db.Projects.Find(id);
            return View(p);
        }

        public ActionResult ArchiveProject(int? id)
        {
            if (!User.IsInRole("ProjectManager") && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Email = db.Users.Find(User.Identity.GetUserId()).Email;
            return View();
        }

        public ActionResult AddUserToProject(int? id)
        {
            if (!User.IsInRole("ProjectManager") && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Email = db.Users.Find(User.Identity.GetUserId()).Email;
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.ProjectTitle = db.Projects.Find(id).ProjectName;
            return View();
        }

        [HttpPost, ActionName("AddUserToProject")]
        [ValidateAntiForgeryToken]
        public ActionResult AddUserToProjectConfirm(int? id, [Bind(Include = "ApplicationUserId")] UserProject userProject)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            ProjectHandler.AssignUserToProject(userProject.ApplicationUserId, Convert.ToInt32(id));
            return RedirectToAction("MyProjects");
        }

        public ActionResult RemoveUserFromProject(int? id)
        {
            if (!User.IsInRole("ProjectManager") && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Email = db.Users.Find(User.Identity.GetUserId()).Email;
            var allusersOfProject = db.UserProjects.ToList().Where(x => x.ProjectId == Convert.ToInt32(id)).Select(x => x.ApplicationUserId).ToList();
            ViewBag.ApplicationUserId = new SelectList(db.Users.ToList().Where(x => allusersOfProject.Contains(x.Id)), "Id", "Email");
            ViewBag.ProjectTitle = db.Projects.Find(id).ProjectName;
            ViewBag.UserWithRoles = RoleHelper.GetAllUsersWithEmails(allusersOfProject);

            return View();
        }

        [HttpPost, ActionName("RemoveUserFromProject")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveUserToProjectConfirm(int? id, [Bind(Include = "ApplicationUserId")] UserProject userProject)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            ProjectHandler.UnAssignUserToProject(userProject.ApplicationUserId, Convert.ToInt32(id));
            return RedirectToAction("MyProjects");
        }

        public ActionResult Developer()
        {
            if (!User.IsInRole("Developer"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Email = db.Users.Find(User.Identity.GetUserId()).Email;
            return View();
        }

        public ActionResult Submitter()
        {
            if (!User.IsInRole("Submitter"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Email = db.Users.Find(User.Identity.GetUserId()).Email;
            return View();
        }

        public ActionResult MyProjectsDS()
        {
            string userId = User.Identity.GetUserId();
            ViewBag.Email = db.Users.Find(userId).Email;
            var allProjects = ProjectHandler.GetAllProjects(User.Identity.GetUserId());

            return View(allProjects);
        }

        public List<TicketForJQuery> GetTicketsForRoles(string userId, string role)
        {
            List<TicketForJQuery> tickets = new List<TicketForJQuery>();
            if (role == "Admin")
            {
                tickets.AddRange(TicketHandler.GetAllTicketsForAdmin());
            }
            else if (role == "ProjectManager")
            {
                tickets.AddRange(TicketHandler.GetAllTicketsForProjectManager(userId));
            }
            else if (role == "Developer")
            {
                tickets.AddRange(TicketHandler.GetAllTicketsForDeveloper(userId));
            }
            else if (role == "Submitter")
            {
                tickets.AddRange(TicketHandler.GetAllTicketsForSubmitter(userId));
            }
            return tickets;
        }

        public ActionResult MyTickets()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            string userId = User.Identity.GetUserId();
            var roles = RoleHelper.GetUserRoles(userId);
            var tickets = new List<TicketForJQuery>();
            ViewBag.Email = db.Users.Find(userId).Email;
            if (roles.Count == 1)
            {
                GetTicketsForRoles(userId, roles[0]);
            }
            else if (roles.Count == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (roles.Contains("Admin"))
                {
                    tickets.AddRange(GetTicketsForRoles(userId, "Admin"));
                }
                if (roles.Contains("ProjectManager"))
                {
                    tickets.AddRange(GetTicketsForRoles(userId, "ProjectManager"));
                }
                if (roles.Contains("Developer"))
                {
                    tickets.AddRange(GetTicketsForRoles(userId, "Developer"));
                }
                if (roles.Contains("Submitter"))
                {
                    tickets.AddRange(GetTicketsForRoles(userId, "Submitter"));
                }
                // A user will never have same roles twise..
            }
            ViewBag.Submitter = User.IsInRole("Submitter");

            return View(tickets);
        }

        public ActionResult CreateTickets()
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Index");
            }
            if (!User.IsInRole("Submitter"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "PriorityName", "PriorityName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName", "ProjectName");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "StatusName", "StatusName");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "TypeName", "TypeName");
            return View();
        }

        [HttpPost, ActionName("CreateTickets")]
        [ValidateAntiForgeryToken]
        public ActionResult CreatTicketConfirm([Bind(Include = "Id,Title,Discription,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticketRef)
        {
            Ticket ticket = new Ticket();
            ticket.AssignedToUserId = "";
            ticket.Created = DateTime.Now;
            ticket.Updated = DateTime.Now;
            ticket.OwenerUserId = User.Identity.GetUserId();
            ticket.Discription = ticketRef.Discription;
            ticket.ProjectId = ticketRef.ProjectId;
            ticket.TicketPriorityId = ticketRef.TicketPriorityId;
            ticket.TicketStatusId = ticketRef.TicketStatusId;
            ticket.TicketTypeId = ticketRef.TicketTypeId;
            ticket.Title = ticketRef.Title;

            TicketHandler.CreateTicket(ticket);
            return RedirectToAction("Index");
        }

        public ActionResult AssignTickets(int? id)
        {
            if (id == null || !User.Identity.IsAuthenticated || (!User.IsInRole("Admin") && !User.IsInRole("ProjectManager")))
            {
                return RedirectToAction("Index");
            }

            Ticket ticket = db.Tickets.Find(id);
            ViewBag.ApplicationUserId = db.Users.ToList();

            return View(ticket);
        }
        [HttpPost, ActionName("AssignTickets")]
        [ValidateAntiForgeryToken]
        public ActionResult AssignedTicketConfirm(int Id, string AssignedToUserId)
        {
            Ticket ticket = db.Tickets.Find(Id);
            if (ticket == null || db.Users.Find(AssignedToUserId) == null)
            {
                return RedirectToAction("Index");
            }

            ticket.AssignedToUserId = AssignedToUserId;
            ticket.Updated = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("MyTickets");
        }

        public ActionResult EditTicket(int? id)
        {
            string userId = User.Identity.GetUserId();
            if (id == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            Ticket ticket = db.Tickets.Find(id);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "PriorityName", "PriorityName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName", "ProjectName");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "StatusName", "StatusName");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "TypeName", "TypeName");

            if (User.IsInRole("Admin"))
            {
                ViewBag.CanChangeStatus = true;
                return View(ticket);
            }
            else if (User.IsInRole("ProjectManager") && TicketHandler.IsTicketBelongToProjectManager(userId, ticket.Id))
            {
                ViewBag.CanChangeStatus = true;
                return View(ticket);
            }
            else if (User.IsInRole("Developer") && TicketHandler.IsTicketBelongToDeveloper(userId, ticket.Id))
            {
                ViewBag.CanChangeStatus = false;
                return View(ticket);
            }
            else if (User.IsInRole("Submitter") && TicketHandler.IsTicketBelongToSubmitter(userId, ticket.Id))
            {
                ViewBag.CanChangeStatus = false;
                return View(ticket);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost, ActionName("EditTicket")]
        [ValidateAntiForgeryToken]
        public ActionResult EditTicketConfirm([Bind(Include = "Id,Title,Discription,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticketRef)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            TicketHandler.EditExistingTicket(ticketRef);

            return RedirectToAction("MyTickets");
        }


        public ActionResult TicketDetails(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Ticket t = db.Tickets.Find(id);
            TicketForJQuery ticketForJQueryOBJ = new TicketForJQuery();

            ticketForJQueryOBJ.AssignedTo = t.AssignedToUserId != null && t.AssignedToUserId != "" ? db.Users.Find(t.AssignedToUserId).Email : "Not Assigned";
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

            return View(ticketForJQueryOBJ);
        }

        public ActionResult AddComments(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Ticket t = db.Tickets.Find(id);
            ViewBag.TicketTitle = t.Title;


            return View();
        }

        [HttpPost, ActionName("AddComments")]
        [ValidateAntiForgeryToken]
        public ActionResult AddCommentsConfirm(int? id, [Bind(Include = "Id,Comment")] TicketComments ticketComment)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Ticket t = db.Tickets.Find(id);
            ticketComment.TicketId = t.Id;
            ticketComment.UserId = User.Identity.GetUserId();
            string userId = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                TicketHandler.AddCommentToTicketForAdmin(ticketComment);
            }
            else if (User.IsInRole("ProjectManager"))
            {
                TicketHandler.AddCommentToTicketForProjectManager(ticketComment, userId);
            }
            else if (User.IsInRole("Developer"))
            {
                TicketHandler.AddCommentToTicketForDeveloper(ticketComment, userId);
            }
            else if (User.IsInRole("Submitter"))
            {
                TicketHandler.AddCommentToTicketForSubmitter(ticketComment, userId);
            }


            return RedirectToAction("MyTickets");
        }

        public ActionResult AddAttachment(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Ticket t = db.Tickets.Find(id);
            ViewBag.TicketTitle = t.Title;


            return View();
        }

        [HttpPost, ActionName("AddAttachment")]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttachmentConfirm(int? id, HttpPostedFileBase File, string Discription)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Ticket t = db.Tickets.Find(id);
            string userId = User.Identity.GetUserId();
            bool confirmRole = false;
            if (User.IsInRole("Admin"))
            {
                confirmRole = true;
            }
            else if (User.IsInRole("ProjectManager") && TicketHandler.IsTicketBelongToProjectManager(userId, t.Id))
            {
                confirmRole = true;
            }
            else if (User.IsInRole("Developer") && TicketHandler.IsTicketBelongToDeveloper(userId, t.Id))
            {
                confirmRole = true;
            }
            else if (User.IsInRole("Submitter") && TicketHandler.IsTicketBelongToSubmitter(userId, t.Id))
            {
                confirmRole = true;
            }
            else
            {
                return RedirectToAction("Index");
            }
            if (confirmRole == true)
            {
                TicketAttachments ticketAttachment = new TicketAttachments();
                ticketAttachment.UserId = db.Users.Find(User.Identity.GetUserId()).Email;
                ticketAttachment.Created = DateTime.Now;
                ticketAttachment.Discription = Discription;
                ticketAttachment.TicketId = t.Id;

                string FileName = Path.GetFileName(File.FileName);
                //string fileExtention = Path.GetExtension(File.FileName);

                ticketAttachment.FilePath = "~/AttachedFile" + FileName;
                var path = Path.Combine(Server.MapPath("~/AttachedFile"), FileName);
                File.SaveAs(path);
                if (ModelState.IsValid)
                {

                    db.TicketAttachments.Add(ticketAttachment);
                    db.SaveChanges();
                    return RedirectToAction("MyTickets");
                }

            }
            return RedirectToAction("Index");
        }

    }
}