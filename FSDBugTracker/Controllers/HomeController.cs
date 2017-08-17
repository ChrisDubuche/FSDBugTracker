using FSDBugTracker.Helpers;
using FSDBugTracker.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FSDBugTracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private UserProjectHelper projectHelper = new UserProjectHelper();
        private UserProjectHelper projHelper = new UserProjectHelper();


        //new helper to assign PM users to tickets
        private MyUserHelper userHelper = new MyUserHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #region Assigning Roles
        // GET: Admin
        public ActionResult AssignRoles(string id)
        {
            ViewBag.UserId = id;
            var userRoles = roleHelper.ListUserRoles(id);
            ViewBag.Roles = new MultiSelectList(db.Roles, "Name", "Name", userRoles);
            return View();
        }

        [HttpPost]
        public ActionResult AssignRoles(string userId, List<string> roles)
        {
            //Unassign the user from all roles
            foreach (var role in roleHelper.ListUserRoles(userId))
            {
                roleHelper.RemoveUserFromRole(userId, role);
            }

            if (roles != null)
            {
                //Assign user to selected roles
                foreach (var role in roles)
                {
                    roleHelper.AddUserToRole(userId, role);
                }
            }

            return RedirectToAction("Index", "Users");
        }
        #endregion

        #region Project stuff
        // GET: Admin
        public ActionResult AssignProjects(string userId)
        {
            ViewBag.UserId = userId;
            var userProjects = projectHelper.ListUserProjects(userId);
            var userProjectIds = userProjects.Select(p => p.Id);
            ViewBag.Projects = new MultiSelectList(db.Projects, "Id", "Name", userProjectIds);
            return View();
        }

        [HttpPost]
        public ActionResult AssignProjects(string userId, List<int> projects)
        {
            foreach (var project in projectHelper.ListUserProjects(userId))
            {
                projectHelper.RemoveUserFromProject(userId, project.Id);
            }

            if (projects != null)
            {
                foreach (var projectId in projects)
                {
                    projectHelper.AddUserToProject(userId, projectId);
                }
            }

            return RedirectToAction("Index", "Users");

        }
        #endregion

        #region Assigning Users to projects
        [Authorize(Roles = "SuperUser, Project Manager, Admin")]
        public ActionResult AssignUsers(int projectId)
        {
            var assignedIds = projectHelper.UsersOnProject(projectId).Select(u => u.Id);
            ViewBag.ProjectId = projectId;
            ViewBag.AssignedUsers = new SelectList(db.Users, "Id", "FirstName", assignedIds);
            return View();
        }

        [HttpPost]
        public ActionResult AssignUsers(int projectId, List<string> AssignedUsers)
        {
            //Unassign All assigned users
            var projectUsers = projectHelper.UsersOnProject(projectId).ToList();
            foreach (var user in projectUsers)
            {
                projectHelper.RemoveUserFromProject(user.Id, projectId);
            }

            //If any users are selected we will re-add them to the project
            if (AssignedUsers  != null)
            {
                foreach (var userId in AssignedUsers)
                {
                    projHelper.AddUserToProject(userId, projectId);
                }

            }
                     
            return RedirectToAction("Index", "Projects");
        }
        #endregion
    }
}
