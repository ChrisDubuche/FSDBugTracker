using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FSDBugTracker.Models;

namespace FSDBugTracker.Controllers
{
    public class TicketUpdatesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketUpdates
        [Authorize]
        public ActionResult Index()
        {
            var ticketUpdates = db.TicketUpdates.Include(t => t.Ticket).Include(t => t.User);
            return View(ticketUpdates.ToList());
        }

        // GET: TicketUpdates/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketUpdate ticketUpdate = db.TicketUpdates.Find(id);
            if (ticketUpdate == null)
            {
                return HttpNotFound();
            }
            return View(ticketUpdate);
        }

        // GET: TicketUpdates/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Titlte");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: TicketUpdates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Property,OldValue,NewValue,ChangedDate,TicketId,UserId")] TicketUpdate ticketUpdate)
        {
            if (ModelState.IsValid)
            {
                db.TicketUpdates.Add(ticketUpdate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Titlte", ticketUpdate.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketUpdate.UserId);
            return View(ticketUpdate);
        }

        // GET: TicketUpdates/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketUpdate ticketUpdate = db.TicketUpdates.Find(id);
            if (ticketUpdate == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Titlte", ticketUpdate.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketUpdate.UserId);
            return View(ticketUpdate);
        }

        // POST: TicketUpdates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Property,OldValue,NewValue,ChangedDate,TicketId,UserId")] TicketUpdate ticketUpdate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketUpdate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Titlte", ticketUpdate.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketUpdate.UserId);
            return View(ticketUpdate);
        }

        // GET: TicketUpdates/Delete/5
        [Authorize(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketUpdate ticketUpdate = db.TicketUpdates.Find(id);
            if (ticketUpdate == null)
            {
                return HttpNotFound();
            }
            return View(ticketUpdate);
        }

        // POST: TicketUpdates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketUpdate ticketUpdate = db.TicketUpdates.Find(id);
            db.TicketUpdates.Remove(ticketUpdate);
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
