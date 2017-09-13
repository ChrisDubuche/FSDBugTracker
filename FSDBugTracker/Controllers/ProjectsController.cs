using System.Linq;
using System.Net;
using System.Web.Mvc;
using FSDBugTracker.Models;
using System;
using Microsoft.AspNet.Identity;
using FSDBugTracker.Helpers;
using System.Collections.Generic;

namespace FSDBugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private UserProjectHelper projHelper = new UserProjectHelper();
        // GET: Projects
        public ActionResult Index()
        {
            var allProjects = db.Projects.ToList();
            return View(allProjects);
        }

        // GET: Projects
        [Authorize]
        public ActionResult OwnedIndex()
        {
            var userId = User.Identity.GetUserId();

            //I am only allowing a user to occupy a single role
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var myProjects = new List<Project>();

            switch (myRole)
            {
                case "SuperUser":
                case "Admin":
                case "Project Manager":
                    myProjects = db.Projects.Where(p => p.OwnerId == userId).ToList();
                    break;
                case "Developer":
                case "Submitter":
                    myProjects = projHelper.ListUserProjects(userId).ToList();
                    break;
                default:
                    break;
            }
            return View("Index", myProjects);
        }

        // GET: Projects/Details/5
        [NoDirectAccess]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter, SuperUser")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [NoDirectAccess]
        [Authorize(Roles = "Admin, Project Manager, SuperUser")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project) //List<string> Developer, List<string> Admin, List<string> ProjectManager, List<string> Submitter
        {
            if (ModelState.IsValid)
            {
                project.Created = DateTime.Now;
                project.OwnerId = User.Identity.GetUserId();
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [NoDirectAccess]
        [Authorize(Roles = "Admin, Project Manager, SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Updated = DateTime.Now;
                db.Projects.Attach(project);
                db.Entry(project).Property("Name").IsModified = true;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [NoDirectAccess]
        [Authorize(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
    }
}
