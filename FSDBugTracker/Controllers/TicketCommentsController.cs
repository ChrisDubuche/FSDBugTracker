using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FSDBugTracker.Models;
using FSDBugTracker.Helpers;
using System;
using Microsoft.AspNet.Identity;

namespace FSDBugTracker.Controllers
{
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketsHelper TicketHelper = new TicketsHelper();
        private UserProjectHelper projectHelper = new UserProjectHelper();


        // GET: TicketComments
        [Authorize(Roles = "Admin, SuperUser")]
        public ActionResult Index()
        {
            var ticketComments = db.TicketComments.Include(t => t.Ticket).Include(t => t.User);
            return View(ticketComments.ToList());
        }

        // GET: TicketComments/Details/5
        [Authorize(Roles = "Admin, SuperUser")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        [Authorize(Roles = "Admin, SuperUser")]
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Comment,Created,TicketId,UserId")] TicketComment ticketComment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.TicketComments.Add(ticketComment);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
        //    return View(ticketComment);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int Id, string Comment)
        {
            var comment = new TicketComment();
            comment.TicketId = Id;
            comment.Comment = Comment;

            comment.UserId = User.Identity.GetUserId();
            comment.Created = DateTimeOffset.Now;

            db.TicketComments.Add(comment);
            db.TicketUpdates.Add(new TicketUpdate()
            {
                ChangedDate = DateTime.Now,
                NewValue = null,
                OldValue = null,
                Property = "comment",
                TicketId = Id,
                UserId = comment.UserId
            });

            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = Id });
        }

        // GET: TicketComments/Edit/5
        //[Authorize]
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketComment ticketComment = db.TicketComments.Find(id);
        //    if (ticketComment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
        //    return View(ticketComment);
        //}
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);

            var userId = User.Identity.GetUserId();

            if (User.IsInRole("Admin") || User.IsInRole("Project Manager") || (User.IsInRole("SuprUser")))
            {
                return View(ticketComment);
            }
            if (User.IsInRole("Developer"))
            {
                if (ticketComment.Ticket.OwnerUserId == userId)
                {
                    return View(ticketComment);
                }
            }

            if (User.IsInRole("Project Manager"))
            {
                if (projectHelper.IsUserOnProject(userId, ticketComment.Ticket.ProjectId))
                {
                    return View(ticketComment);
                }
            }
            else
            {
                if (userId == ticketComment.UserId)
                {
                    return View(ticketComment);
                }

            }
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Comment,Created,TicketId,UserId")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        //[Authorize]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketComment ticketComment = db.TicketComments.Find(id);
        //    if (ticketComment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticketComment);
        //}
        [Authorize]
        public ActionResult Delete(int? id)
        {
            var userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Admin") || User.IsInRole("Project Manager") || (User.IsInRole("SuperUser")))
            {
                return View(ticketComment);
            }
            if (User.IsInRole("Developer"))
            {
                if (ticketComment.Ticket.AssignedToUserId == userId)
                {
                    return View(ticketComment);
                }
            }

            if (User.IsInRole("Project Manager"))
            {
                if (projectHelper.IsUserOnProject(userId, ticketComment.Ticket.ProjectId))
                {
                    return View(ticketComment);
                }
            }
            else
            {
                if (userId == ticketComment.UserId)
                {
                    return View(ticketComment);
                }

            }
            return RedirectToAction("Edit", "Tickets");
        }

        // POST: TicketComments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    TicketComment ticketComment = db.TicketComments.Find(id);
        //    db.TicketComments.Remove(ticketComment);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            TicketComment ticketComment = db.TicketComments.Find(id);

            var ticketId = ticketComment.Ticket.Id;
            db.TicketComments.Remove(ticketComment);
            db.SaveChanges();

            return RedirectToAction("Details", "Tickets", new { id = ticketId });

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
