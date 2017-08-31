using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FSDBugTracker.Models;
using FSDBugTracker.Helpers;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace FSDBugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private TicketsHelper ticketHelper = new TicketsHelper();
        private UserProjectHelper projectHelper = new UserProjectHelper();

        // GET: Tickets
        [Authorize]
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include
                (t => t.AssignedToUser).Include
                (t => t.OwnerUser).Include
                (t => t.Project).Include
                (t => t.TicketPriority).Include
                (t => t.TicketStatus).Include
                (t => t.TicketType);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        [NoDirectAccess]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter, SuperUser")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [NoDirectAccess]
        [Authorize(Roles = "Submitter, SuperUser")]
        public ActionResult Create()
        {

            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities.Where(tp => tp.TicketPriorityName != null), "Id", "TicketPriorityName");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses.Where(ts => ts.TicketStatusName != null), "Id", "TicketStatusName");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes.Where(tt => tt.TicketTypeName != null), "Id", "TicketTypeName");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = DateTimeOffset.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [NoDirectAccess]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter, SuperUser")]
        public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Ticket ticket = db.Tickets.Find(id);
        //    if (ticket == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    var allDevs = roleHelper.UsersInRole("Developer");
        //    ViewBag.AssignedToUserId = new SelectList(allDevs, "Id", "FirstName", ticket.AssignedToUserId);

        //    ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
        //    ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
        //    ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
        //    ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
        //    ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
        //    return View(ticket);
        //}
        {
            var userId = User.Identity.GetUserId();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)

            {
                return HttpNotFound();
            }

            var userRolesAssigned = roleHelper.UsersInRole("Developer");

            ViewBag.AssignedToUserId = new SelectList(userRolesAssigned, "Id", "FirstName", ticket.AssignedToUserId);//make an instatiate Selectlist called viewbag AssignToUserId that pass in list of users w/Id and firsst Name Select with the box
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            if (User.IsInRole("Admin"))
            {
                return View(ticket);
            }
            if (User.IsInRole("Project Manager"))
            {
                if (projectHelper.IsUserOnProject(userId, ticket.ProjectId))
                {
                    return View(ticket);
                }
            }
            else
            {
                if (userId == ticket.OwnerUserId || userId == ticket.AssignedToUserId)
                {
                    return View(ticket);
                }

            }
            return View(ticket); 
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        //{
        //    var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
        //    if (ModelState.IsValid)
        //    {
        //        ticket.Updated = DateTimeOffset.Now;
        //        db.Entry(ticket).State = EntityState.Modified;
        //        db.SaveChanges();
        //        // await ... call tickethelper method for generating notification
        //        return RedirectToAction("Index");
        //    }

        //    var allDevs = roleHelper.UsersInRole("Developer");
        //    ViewBag.AssignedToUserId = new SelectList(allDevs, "Id", "FirstName", ticket.AssignedToUserId);

        //    ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
        //    ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
        //    ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
        //    ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
        //    ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
        //    return View(ticket);
        //}

        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                #region
                Ticket oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

                List<TicketUpdate> updates = GetTicketUpdates(oldTicket, ticket);
                if (updates.Count > 0)
                    foreach (TicketUpdate update in updates)
                    {
                        db.TicketUpdates.Add(update);
                        
                        db.SaveChanges();

                        await TicketsHelper.GenerateNotificationAsync(update.Id, oldTicket, ticket);
                    }

                #endregion
                ticket.Updated = DateTimeOffset.Now;

                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                if (!User.IsInRole("Admin"))
                {
                    RedirectToAction("Index"); //fix this
                }
                return RedirectToAction("Index");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "TkPriorityName", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "TkStatusName", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "TkTypeName", ticket.TicketTypeId);
            return View(ticket);
        }
        #region
        private List<TicketUpdate> GetTicketUpdates(Ticket oldTicket, Ticket ticket)
        {
            List<TicketUpdate> updates = new List<TicketUpdate>();

            if (oldTicket.AssignedToUserId != ticket.AssignedToUserId)
                updates.Add(new TicketUpdate()
                {
                    ChangedDate = DateTime.Now,
                    NewValue = db.Users.FirstOrDefault(u => u.Id == ticket.AssignedToUserId).DisplayName,
                    OldValue = !string.IsNullOrWhiteSpace(oldTicket.AssignedToUserId) ? db.Users.FirstOrDefault(u => u.Id == oldTicket.AssignedToUserId).DisplayName : null,
                    Property = "assigned user",
                    TicketId = ticket.Id,
                    UserId = User.Identity.GetUserId()
                });

            if (oldTicket.Title != ticket.Title)
                updates.Add(new TicketUpdate()
                {
                    ChangedDate = DateTime.Now,
                    NewValue = ticket.Title,
                    OldValue = oldTicket.Title,
                    Property = "title",
                    TicketId = ticket.Id,
                    UserId = User.Identity.GetUserId()
                });

            if (oldTicket.Description != ticket.Description)
                updates.Add(new TicketUpdate()
                {
                    ChangedDate = DateTime.Now,
                    NewValue = ticket.Description,
                    OldValue = oldTicket.Description,
                    Property = "description",
                    TicketId = ticket.Id,
                    UserId = User.Identity.GetUserId()
                });

            if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                updates.Add(new TicketUpdate()
                {
                    ChangedDate = DateTime.Now,
                    NewValue = db.TicketPriorities.FirstOrDefault(p => p.Id == ticket.TicketPriorityId).TicketPriorityName,
                    OldValue = oldTicket.TicketPriority.TicketPriorityName,
                    Property = "priority",
                    TicketId = ticket.Id,
                    UserId = User.Identity.GetUserId()
                });

            if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                updates.Add(new TicketUpdate()
                {
                    ChangedDate = DateTime.Now,
                    NewValue = db.TicketStatuses.FirstOrDefault(s => s.Id == ticket.TicketStatusId).TicketStatusName,
                    OldValue = oldTicket.TicketStatus.TicketStatusName,
                    Property = "status",
                    TicketId = ticket.Id,
                    UserId = User.Identity.GetUserId()
                });

            if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                updates.Add(new TicketUpdate()
                {
                    ChangedDate = DateTime.Now,
                    NewValue = db.TicketTypes.FirstOrDefault(t => t.Id == ticket.TicketTypeId).TicketTypeName,
                    OldValue = oldTicket.TicketType.TicketTypeName,
                    Property = "type",
                    TicketId = ticket.Id,
                    UserId = User.Identity.GetUserId()
                });

            return updates;
        }
        #endregion

        // GET: Tickets/Delete/5
        [NoDirectAccess]
        [Authorize(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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

        #region Assigning Devs to tickets
        [NoDirectAccess]
        [Authorize(Roles = "SuperUser, Project Manager, Admin")]
        public ActionResult AssignDevs(int ticketId)
        {
            ViewBag.TicketId = ticketId;
            var currentlyAssigned = db.Tickets.FirstOrDefault(t => t.Id == ticketId).AssignedToUserId;
            var allDevs = roleHelper.UsersInRole("Developer");
            ViewBag.AssignedDevs = new SelectList(allDevs, "Id", "FirstName", currentlyAssigned);
            
            return View();
        }

        [HttpPost]
        public ActionResult AssignDevs(int TicketId, string AssignedDevs) 
        {
            //var allDevs = roleHelper.UsersInRole("Developer");
            //ViewBag.TicketId = new SelectList(allDevs, "Id", "FirstName", TicketId);

            //if (AssignedDevs != null)
            //{
            //    foreach (var userId in AssignedDevs)
            //    {
            //        ticketHelper.AddUserToTicket(userId, TicketId);
            //    }

            //}
            ticketHelper.AddUserToTicket(AssignedDevs, TicketId);


            return RedirectToAction("Index", "Tickets");
        }
        #endregion
    }
}