using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FSDBugTracker.Models;

namespace FSDBugTracker.Controllers
{
    public class TicketStatusesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketStatuses
        [Authorize]
        public ActionResult Index()
        {
            return View(db.TicketStatuses.ToList());
        }

        // GET: TicketStatuses/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketStatus ticketStatus = db.TicketStatuses.Find(id);
            if (ticketStatus == null)
            {
                return HttpNotFound();
            }
            return View(ticketStatus);
        }

        // GET: TicketStatuses/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TicketStatuses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] TicketStatus ticketStatus)
        {
            if (ModelState.IsValid)
            {
                db.TicketStatuses.Add(ticketStatus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticketStatus);
        }

        // GET: TicketStatuses/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketStatus ticketStatus = db.TicketStatuses.Find(id);
            if (ticketStatus == null)
            {
                return HttpNotFound();
            }
            return View(ticketStatus);
        }

        // POST: TicketStatuses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] TicketStatus ticketStatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketStatus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticketStatus);
        }

        // GET: TicketStatuses/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketStatus ticketStatus = db.TicketStatuses.Find(id);
            if (ticketStatus == null)
            {
                return HttpNotFound();
            }
            return View(ticketStatus);
        }

        // POST: TicketStatuses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketStatus ticketStatus = db.TicketStatuses.Find(id);
            db.TicketStatuses.Remove(ticketStatus);
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
