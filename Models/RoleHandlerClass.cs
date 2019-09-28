using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TicketKeeper.Models
{
    public class RoleHandlerClass
    {
        private protected UserManager<IdentityUser> userManager { get; set; }
        private protected RoleManager<IdentityRole> RoleManager { get; set; }
        private protected ApplicationDbContext db = new ApplicationDbContext();
        public RoleHandlerClass()
        {
            userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());
            // Initilizing manager instances.
        }

        //public void SeedRolesInDatabase()
        //{
        //    RoleManager.Create(new IdentityRole("Admin"));
        //    RoleManager.Create(new IdentityRole("ProjectManager"));
        //    RoleManager.Create(new IdentityRole("Developer"));
        //    RoleManager.Create(new IdentityRole("Submitter"));
        //}

        //Assign/unassign users to/from roles

        // If runing for first time. Run this SeedRolesInDatabase() manually to seed database with proper role names.
        // If you add new roles to UserRole enum then please add it to Above method.
        // Above method only needs to be run if you are starting a new database.

        public class UserEmail
        {
            public string Email { get; set; }

            public string Role { get; set; }
        }

        public List<UserEmail> GetAllUsersWithEmails(List<string> userIds)
        {
            List<UserEmail> userEmails = new List<UserEmail>();
            userIds.ForEach(id =>
            {
                var email = db.Users.Find(id).Email;
                //userEmail.Role = 
                var listOfUserRoles = GetUserRoles(id);
                if (listOfUserRoles.Count == 1)
                {
                    UserEmail userEmail = new UserEmail();
                    userEmail.Email = email;
                    userEmail.Role = listOfUserRoles[0];
                    userEmails.Add(userEmail);
                }
                else
                {
                    listOfUserRoles.ForEach(r =>
                    {
                        UserEmail userEmail = new UserEmail();
                        userEmail.Email = email;
                        userEmail.Role = r;
                        userEmails.Add(userEmail);
                    });
                }
            });

            return userEmails;

        }

        public List<string> GetAllRoles()
        {
            List<string> allRoles = RoleManager.Roles.Select(x => x.Name).ToList();

            return allRoles;
        }

        public List<string> GetAllAdmins()
        {

            List<string> allAdmins = db.Users.ToList().Where(x => userManager.IsInRole(x.Id, "Admin")).Select(x => x.Email).ToList();
            return allAdmins;
        }

        public List<string> GetAllProjectManagers()
        {

            List<string> allManagers = db.Users.ToList().Where(x => userManager.IsInRole(x.Id, "ProjectManager")).Select(x => x.Email).ToList();
            return allManagers;
        }

        public List<ApplicationUser> GetAllAdmins(bool v)
        {

            List<ApplicationUser> allAdmins = db.Users.ToList().Where(x => userManager.IsInRole(x.Id, "Admin")).ToList();
            return allAdmins;
        }

        public List<ApplicationUser> GetAllProjectManagers(bool v)
        {

            List<ApplicationUser> allManagers = db.Users.ToList().Where(x => userManager.IsInRole(x.Id, "ProjectManager")).ToList();
            return allManagers;
        }

        public List<string> GetAllDevelopers()
        {

            List<string> allDeveloper = db.Users.ToList().Where(x => userManager.IsInRole(x.Id, "Developer")).Select(x => x.Email).ToList();
            return allDeveloper;
        }

        public List<string> GetAllSubmitters()
        {

            List<string> allSubmitter = db.Users.ToList().Where(x => userManager.IsInRole(x.Id, "Submitter")).Select(x => x.Email).ToList();
            return allSubmitter;
        }

        public List<string> AllUnassignedUser()
        {
            List<string> allUnassignedUsers = db.Users.ToList().Where(x => (!userManager.IsInRole(x.Id, "Submitter")) && (!userManager.IsInRole(x.Id, "ProjectManager")) && (!userManager.IsInRole(x.Id, "Developer")) && (!userManager.IsInRole(x.Id, "Admin"))).Select(x => x.Email).ToList();
            return allUnassignedUsers;

        }



        public string GetUserRole(string userId)
        {
            if (userManager.FindById(userId) == null)
            {
                return "";
            }
            var userRoles = userManager.GetRoles(userId).ToList().FirstOrDefault();
            return userRoles;
        }

        public List<string> GetUserRoles(string userId)
        {
            var userRoles = userManager.GetRoles(userId);
            return userRoles.ToList();
        }

        public bool IsUserExist(string userId)
        {
            if (userManager.FindById(userId) != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method accepts a userid and role name and returns true if user is in role else returns false.
        /// </summary>
        /// <returns></returns>        
        public bool IsUserInRole(string userId, UserRole role)
        {
            var roleName = role.ToString();
            var user = userManager.FindById(userId);
            if (user != null && (userManager.IsInRole(userId, roleName)))
            {
                return true;
            }
            return false;
        }


        public bool IsUserInRole(string userId, string role)
        {
            var user = userManager.FindById(userId);
            if (user != null && (userManager.IsInRole(userId, role)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Assigning Role By instance of UserRole to user having userId.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>

        //Administrators and Project Managers must be able to Assign user to projects

        // When adding new admin please uncomment below authorization. Or login to existing admin to add it.
        public bool AssignUserToRole(string userId, string role)
        {

            var user = userManager.FindById(userId);
            if (user == null)
            {
                return false;
            }
            if ((userManager.IsInRole(userId, role)))
            {
                return true;
            }
            var result = userManager.AddToRole(userId, role).Succeeded;
            return result;
        }

        /// <summary>
        /// Removing a Role of instance of UserRole from user having userid.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>

        //Administrators and Project Managers must be able to UnAssign user to projects
        public bool UnAssignUserToRole(string userId, string role)
        {
            var user = userManager.FindById(userId);
            if (user == null)
            {
                return false;
            }
            if (!userManager.IsInRole(userId, role))
            {
                return true;
            }
            var result = userManager.RemoveFromRole(userId, role).Succeeded;
            return result;
        }
    }
}