using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace TicketKeeper.Models
{
    public class ProjectHandler
    {
        //Create projects
        private protected ApplicationDbContext db { get; set; }
        private protected RoleHandlerClass RoleHandler { get; set; }

        public ProjectHandler()
        {
            db = new ApplicationDbContext();
            RoleHandler = new RoleHandlerClass();
        }


        public List<ApplicationUser> GetProjectManagers()
        {
            // get all users that are project manager..
            var result = new List<ApplicationUser>();
            db.Users.Select(x => x.Id).ToList().ForEach(uId =>
            {
                if (RoleHandler.IsUserInRole(uId, "ProjectManager"))
                {

                    result.Add(db.Users.Find(uId));
                }
            });

            return result;

        }

        //Organization of resources / project managers or admin only
        public void CreateProject(Project project)
        {
            db.Projects.Add(project);
            // Creating a Project.
        }

        // Assign/unassign users to/from projects
        // Here there is a class called UserProject which should be use.
        // Organization of resources / project managers or admin only

        public void AssignUserToProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            if (user == null || project == null)
            {
                return;
            }
            UserProject userProject = new UserProject();
            userProject.ProjectId = project.Id;
            userProject.Project = project;
            userProject.ApplicationUserId = user.Id;
            userProject.ApplicationUser = user;
            db.UserProjects.Add(userProject);
            db.SaveChanges();
        }

        public void UnAssignUserToProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            if (user == null || project == null)
            {
                return;
            }
            UserProject userProject = db.UserProjects.Where(x => x.ProjectId == project.Id && x.ApplicationUserId == user.Id).FirstOrDefault();
            db.UserProjects.Remove(userProject);
            db.SaveChanges();
        }

        /// <summary>
        /// Returns a project based on given id.
        /// </summary>
        /// <param name="projectId"></param>
        public Project GetProject(int projectId)
        {
            var project = db.Projects.Find(projectId);
            if (project == null)
            {
                return new Project();
            }

            return project;
        }

        //Administrators and Project Managers must be able to edit existing projects

        public void EditProject(Project project)
        {
            var ExistingProject = db.Projects.Find(project.Id);
            // Get existing project from database.
            if (project == null)
            {
                return;
            }

            ExistingProject.ProjectDiscripTion = project.ProjectDiscripTion;
            ExistingProject.ProjectName = project.ProjectName;
            ExistingProject.ApplicationUserId = project.ApplicationUserId;

            db.SaveChanges();
        }

        //Administrators, Project Managers, Developers, and Submitters must be able to view a list of projects they are assigned to. 
        //Administrators and Project Managers must be able to view a separate list of all projects.
        /// <summary>
        /// This method returns All project belonging to user.
        /// If user is admin or project manager then all projects are returned.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Project> GetAllProjects(string userId)
        {
            var result = new List<Project>();
            var user = db.Users.Find(userId);
            if (user == null)
            {
                return result;
            }

            if (RoleHandler.IsUserInRole(userId, "Admin") || RoleHandler.IsUserInRole(userId, "ProjectManager"))
            {
                return db.Projects.ToList();
                // All projects are returned if user is admin or project manager.
            }
            else
            {
                // regular user Only projects which user is already in are returned.
                var projectIdList = db.UserProjects.ToList().Where(x => x.ApplicationUserId == userId).Select(x => x.ProjectId).Distinct().ToList();
                projectIdList.ForEach(projectId =>
                {
                    var project = db.Projects.Find(projectId);
                    if (project != null)
                    {
                        result.Add(project);
                    }
                });

            }

            return result;
        }



    }
}



