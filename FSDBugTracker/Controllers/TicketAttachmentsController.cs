using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FSDBugTracker.Models;
using FSDBugTracker.Helpers;
using System.IO;
using System;
using Microsoft.AspNet.Identity;
using System.Web;

namespace FSDBugTracker.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketAttachments
        [Authorize]
        public ActionResult Index()
        {
            var ticketAttachments = db.TicketAttachments.Include(t => t.Ticket).Include(t => t.User);
            return View(ticketAttachments.ToList());
        }

        // GET: TicketAttachments/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MediaUrl,Description,Created,TicketId,UserId")] TicketAttachment ticketAttachment, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var fullName = DateTime.Now.ToString("hh.mm.ss.ffffff") + "_" + fileName;// this is a technique for allowing multiple images with same name and reference properly. it's appending current date in millisecond
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fullName));
                    ticketAttachment.MediaUrl = "/Uploads/" + fullName;
                    ticketAttachment.User = db.Users.Find(User.Identity.GetUserId());
                    ticketAttachment.Created = DateTime.Now;
                    db.TicketAttachments.Add(ticketAttachment);
                    db.TicketUpdates.Add(new TicketUpdate()
                    {
                        ChangedDate = DateTime.Now,
                        NewValue = null,
                        OldValue = null,
                        Property = "attachment",
                        TicketId = ticketAttachment.TicketId,
                        UserId = ticketAttachment.User.Id
                    });
                    db.SaveChanges();
                    return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
                }

                TempData["invalid"] = "message";
                return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
            }
            return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
        }
        // GET: TicketAttachments/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MediaUrl,Description,Created,TicketId,UserId")] TicketAttachment ticketAttachment, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var fullName = DateTime.Now.ToString("hh.mm.ss.ffffff") + "_" + fileName;// this is a technique for allowing multiple images with same name and reference properly. it's appending current date in millisecond
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fullName));
                    ticketAttachment.MediaUrl = "/Uploads/" + fullName;
                }


                db.Entry(ticketAttachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
            ViewBag.userId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);

            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            var ticketId = ticketAttachment.TicketId;
            db.TicketAttachments.Remove(ticketAttachment);
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
