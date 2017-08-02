using FSDBugTracker.Helpers;
using FSDBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSDBugTracker.Controllers
{
    public class HomeController : Controller
    {
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

        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();

        // GET: Admin
        public ActionResult AssignRoles(string userId)
        {
            ViewBag.UserId = userId;
            var userRoles = roleHelper.ListUserRoles(userId);
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
    }

}