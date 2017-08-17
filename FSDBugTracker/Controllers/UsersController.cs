using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FSDBugTracker.Models;
using FSDBugTracker.Helpers;
using System.Collections.Generic;

namespace FSDBugTracker.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private UserProjectHelper projectHelper = new UserProjectHelper();

        // GET: Users
        [Authorize]
        public ActionResult Index(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return View(db.Users.ToList());
            }
            else
            {
                return View(roleHelper.UsersInRole(roleName));
            }
            
        }

        // GET: Users/Details/5
        [Authorize]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: Users/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,DisplayName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicationUser);
        }

        // GET: Users/Edit/5
        [Authorize]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,DisplayName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: Users/Delete/5
        [NoDirectAccess]
        [Authorize(Roles = "SuperUser")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Role stuff
        // GET: Admin
        [NoDirectAccess]
        [Authorize(Roles = "Admin, SuperUser")]
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




    }
}
