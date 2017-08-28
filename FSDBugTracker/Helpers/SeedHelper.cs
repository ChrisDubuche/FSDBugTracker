using FSDBugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FSDBugTracker.Helpers


{
    public class SeedHelper
    {
        private static ApplicationDbContext context = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

        public void CreateRandomUsers(int number)
        {
            var userManager = new UserManager<ApplicationUser>(
             new UserStore<ApplicationUser>(context));

            for (var i = 0; i < number; i++)
            {
                var firstName = "Test";
                var lastName = string.Concat("User", i + 1);
                var random = new Random();

                var email = string.Concat(firstName, lastName, "@mailinator.com");

                if (!context.Users.Any(u => u.Email == email))
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                    }, "Abc&123!");
                }

                //Assign user to a random role
                var rolesList = context.Roles.Where(r => r.Name != "Admin" && r.Name != "Super User").ToList();
                var userId = userManager.FindByEmail(email).Id;
                userManager.AddToRole(userId, rolesList[random.Next(0, rolesList.Count())].Name);
            }
        }

        public void CreateRandomProjects(int number, int tickets)
        {
            for (var i = 0; i < number; i++)
            {
                var name = string.Concat("Project", i + 1);
                var random = new Random();

                var project = new Project();
                project.Created = DateTime.Now;
                project.Name = name;
                project.Id = i + 1;

                var userList = userManager.Users.ToList();

                //assign 2 developers to each project
                var devList = new List<ApplicationUser>();
                foreach (var user in userList)
                {
                    if (userManager.IsInRole(user.Id, "Developer"))
                    {
                        devList.Add(user);
                    }
                }
                project.ProjectUsers.Add(devList[random.Next(0, devList.Count())]);
                project.ProjectUsers.Add(devList[random.Next(0, devList.Count())]);

                //assign the project manager
                var pmList = new List<ApplicationUser>();
                foreach (var user in userList)
                {
                    if (userManager.IsInRole(user.Id, "Project Manager"))
                    {
                        pmList.Add(user);
                    }
                }
                var pm = pmList[random.Next(0, pmList.Count())];
                project.ProjectUsers.Add(pm);
                project.OwnerId = pm.FullName;

                context.Projects.Add(project);
                context.SaveChanges();

                CreateRandomTickets(project.Id, tickets);
            }
        }

        public void CreateRandomTickets(int projectId, int number)
        {
            //Create 'number' tickets per project
            for (var i = 0; i < number; i++)
            {
                var title = string.Concat("Ticket", i + 1);
                var random = new Random();

                var ticket = new Ticket();
                ticket.Created = DateTimeOffset.Now;
                ticket.Title = title;
                ticket.Id = i + 1;
                ticket.ProjectId = projectId;
                ticket.Description = "This is a seeded ticket.";

                var userList = userManager.Users.ToList();

                //assign the ticket a random submitter
                var subList = new List<ApplicationUser>();
                foreach (var user in userList)
                {
                    if (userManager.IsInRole(user.Id, "Submitter"))
                    {
                        subList.Add(user);
                    }
                }
                ticket.OwnerUserId = subList[random.Next(0, subList.Count())].Id;

                //assign the ticket a random developer and set initial status
                var devList = new List<ApplicationUser>();
                foreach (var user in userList)
                {
                    if (userManager.IsInRole(user.Id, "Developer") && context.Projects.FirstOrDefault(p => p.Id == projectId).ProjectUsers.Any(u => u.Id == user.Id))
                    {
                        devList.Add(user);
                    }
                }
                ticket.AssignedToUserId = devList[random.Next(0, devList.Count())].Id;
                ticket.TicketStatusId = context.TicketStatuses.FirstOrDefault(t => t.TicketStatusName == "Open/Assigned").Id;

                //assign a random type
                var typeList = context.TicketTypes.Select(t => t.Id).ToList();
                ticket.TicketTypeId = typeList[random.Next(0, typeList.Count())];

                //assign a random priority
                var priorityList = context.TicketPriorities.Select(t => t.Id).ToList();
                ticket.TicketPriorityId = priorityList[random.Next(0, priorityList.Count())];

                context.Tickets.Add(ticket);
                context.SaveChanges();
            }
        }
    }
}