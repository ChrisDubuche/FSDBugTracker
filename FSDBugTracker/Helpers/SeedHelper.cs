using FSDBugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSDBugTracker.Helpers
{
    public class SeedHelper
    {
        //TODO - Appropriate Async Await keywords in this class to prevent seed method from timing out
        private static ApplicationDbContext context = new ApplicationDbContext();
        private UserProjectHelper projHelper = new UserProjectHelper();
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        public async Task SeedData()
        {

            #region Role Creation
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //New Demo Role section
            var seededRoles = new List<string> {
                    "Admin",
                    "Project Manager",
                    "Developer",
                    "Submitter",
                    "Demo Admin",
                    "Demo Project Manager",
                    "Demo Submitter",
                    "Demo Developer"
                };
            var task = Task.Run(() => {
                foreach (var role in seededRoles)
                {
                    if (!context.Roles.Any(r => r.Name == role))
                    {
                        roleManager.Create(new IdentityRole
                        {
                            Name = role
                        });
                    }
                }
            });
            task.Wait();

            #endregion

            #region User creation
            var UserManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            //New Demo Role section
            var seededUsers = new List<string> {
                    "FSDAdmin@mailinator.com",
                    "FSDProjectManager@mailinator.com",
                    "FSDSubmitter@mailinator.com",
                    "FSDDeveloper@mailinator.com",
                    "DemoAdministrator@mailinator.com",
                    "DemoProjectManager@mailinator.com",
                    "DemoSubmitter@mailinator.com",
                    "DemoDeveloper@mailinator.com"
                };

            task =  Task.Run(() =>
            {
                foreach (var user in seededUsers)
                {
                    if (!context.Users.Any(u => u.Email == user))
                    {
                        UserManager.Create(new ApplicationUser
                        {
                            UserName = user,
                            Email = user,
                            FirstName = user.Split('@')[0],
                            LastName = "Seeded",
                            DisplayName = string.Concat("Seeded ", user.Split('@')[0])
                        }, "Abc&123!");
                    }
                }
            });
            task.Wait();

            if (!context.Users.Any(u => u.Email == "UnAssignedUser@mailinator.com"))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = "UnAssignedUser@mailinator.com",
                    Email = "UnAssignedUser@mailinator.com",
                    FirstName = "UnAssigned",
                    LastName = "User",
                    DisplayName = "UnAssigned User"
                }, "Abc&123!");
            }
            #endregion

            #region Role Assignment
            var userRolesDict = new Dictionary<string, string>()
                {
                    { "FSDAdmin@mailinator.com", "Admin"},
                    { "FSDProjectManager@mailinator.com", "Project Manager"},
                    { "FSDSubmitter@mailinator.com", "Submitter"},
                    { "FSDDeveloper@mailinator.com", "Developer"},
                    { "DemoAdministrator@mailinator.com", "Demo Admin"},
                    { "DemoProjectManager@mailinator.com", "Demo Project Manager"},
                    { "DemoSubmitter@mailinator.com", "Demo Submitter"},
                    { "DemoDeveloper@mailinator.com", "Demo Developer"}
                };

            var userId = string.Empty;

            task =  Task.Run(() =>
                    {
                        foreach (var userRole in userRolesDict)
                        {
                            userId = UserManager.FindByEmail(userRole.Key).Id;
                            UserManager.AddToRole(userId, userRole.Value);
                        }
                    });
            task.Wait();

            userId = UserManager.FindByEmail("UnAssignedUSer@mailinator.com").Id;
            UserManager.AddToRole(userId, "Developer");
            #endregion

            #region Lookup Table seeding
            var logMsg = new StringBuilder();

            var ticketStatuses = new List<string>();
            ticketStatuses.Add("Open/Unassigned");
            ticketStatuses.Add("Open/Assigned");
            ticketStatuses.Add("Waiting For Info");
            ticketStatuses.Add("In Progress");
            ticketStatuses.Add("Resolved");
            ticketStatuses.Add("Closed");
            ticketStatuses.Add("Archived");

            task =  Task.Run(() =>
            {
                foreach (var status in ticketStatuses)
                {
                    var ticketStatus = new TicketStatus();
                    ticketStatus.TicketStatusName = status;

                    context.TicketStatuses.Add(ticketStatus);
                    context.SaveChanges();
                }
            });
            task.Wait();

            var ticketTypes = new List<string>();
            ticketTypes.Add("Bug");
            ticketTypes.Add("Task");
            ticketTypes.Add("Informational");
            ticketTypes.Add("Documentation Requested");

            task =  Task.Run(() =>
            {
                foreach (var type in ticketTypes)
                {
                    var ticketType = new TicketType();
                    ticketType.TicketTypeName = type;

                    context.TicketTypes.Add(ticketType);
                    context.SaveChanges();
                }
            });
            task.Wait();

            var ticketPriorities = new List<string>();
            ticketPriorities.Add("Immediate");
            ticketPriorities.Add("Highest");
            ticketPriorities.Add("High");
            ticketPriorities.Add("Medium");
            ticketPriorities.Add("Low");

            task = Task.Run(() =>
            {
                foreach (var priority in ticketPriorities)
                {
                    var ticketPriority = new TicketPriority();
                    ticketPriority.TicketPriorityName = priority;

                    context.TicketPriorities.Add(ticketPriority);
                    context.SaveChanges();
                }
            });
            task.Wait();
            #endregion

            await LoadProjectsAndTickets(true);
        }

        public async Task LoadProjectsAndTickets(bool demo)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var projectName = demo ? "Seeded Demo Project # " : "Seeded Project # ";
            var projectUserIds = new List<string>();
            var assignedToUserId = UserManager.FindByEmail("UnAssignedUser@mailinator.com").Id;
            var ownerUserId = demo ? UserManager.FindByEmail("DemoSubmitter@mailinator.com").Id : UserManager.FindByEmail("FSDSubmitter@mailinator.com").Id;
            var ownerId = demo ? UserManager.FindByEmail("DemoProjectManager@mailinator.com").Id : UserManager.FindByEmail("FSDProjectManager@mailinator.com").Id;


            #region 30 Test Projects created
            var seededProject = new Project();

            var task = Task.Run(() =>
            {
                for (var loop = 1; loop <= 3; loop++)
                {
                    seededProject.Name = string.Concat(projectName, loop);
                    seededProject.Created = DateTime.Now;
                    context.Projects.Add(seededProject);
                    context.SaveChanges();
                }
            });
            task.Wait();
            #endregion

            #region Assigning users to Test Projects
            var allProjects = context.Projects.AsNoTracking().ToList();

            task = Task.Run(() =>
            {
                foreach (var project in allProjects)
                {
                    if (project.Name.Contains("Demo"))
                    {
                        projectUserIds.Add(UserManager.FindByEmail("DemoAdmin@mailinator.com").Id);
                        projectUserIds.Add(UserManager.FindByEmail("DemoProjectManager@mailinator.com").Id);
                        projectUserIds.Add(UserManager.FindByEmail("DemoSubmitter@mailinator.com").Id);
                        projectUserIds.Add(UserManager.FindByEmail("DemoDeveloper@mailinator.com").Id);
                    }
                    else
                    {
                        projectUserIds.Add(UserManager.FindByEmail("FSDAdmin@mailinator.com").Id);
                        projectUserIds.Add(UserManager.FindByEmail("FSDProjectManager@mailinator.com").Id);
                        projectUserIds.Add(UserManager.FindByEmail("FSDSubmitter@mailinator.com").Id);
                        projectUserIds.Add(UserManager.FindByEmail("FSDDeveloper@mailinator.com").Id);
                    }
                    projHelper.AddUsersToProject(projectUserIds, project.Id);

                    for (var ticketloop = 1; ticketloop <= 10; ticketloop++)
                    {
                        var ticket = new Ticket
                        {
                            ProjectId = project.Id,
                            Title = string.Format("Seeded Ticket #{0} for Project: {1}", ticketloop, string.Concat("Seeded_Project_#", ticketloop)),
                            Description = "This is a Seeded Ticket created by the Configuration class for testing",
                            Created = DateTime.Now,
                            AssignedToUserId = assignedToUserId,
                            OwnerUserId = ownerUserId,
                            TicketPriorityId = context.TicketPriorities.AsNoTracking().FirstOrDefault(p => p.TicketPriorityName == "High").Id,
                            TicketStatusId = context.TicketStatuses.AsNoTracking().FirstOrDefault(p => p.TicketStatusName == "Open/UnAssigned").Id,
                            TicketTypeId = context.TicketTypes.AsNoTracking().FirstOrDefault(p => p.TicketTypeName == "Bug").Id
                        };

                        context.Tickets.Add(ticket);
                        context.SaveChanges();
                    }
                    #endregion
                }
            });
            task.Wait();
        }
    }
}


#region
//public void CreateRandomUsers(int number)
//{
//    var userManager = new UserManager<ApplicationUser>(
//     new UserStore<ApplicationUser>(context));

//    for (var i = 0; i < number; i++)
//    {
//        var firstName = "Test";
//        var lastName = string.Concat("User", i + 1);
//        var random = new Random();

//        var email = string.Concat(firstName, lastName, "@mailinator.com");

//        if (!context.Users.Any(u => u.Email == email))
//        {
//            userManager.Create(new ApplicationUser
//            {
//                UserName = email,
//                Email = email,
//                FirstName = firstName,
//                LastName = lastName,
//            }, "Abc&123!");
//        }

//        //Assign user to a random role
//        var rolesList = context.Roles.Where(r => r.Name != "Admin" && r.Name != "Super User").ToList();
//        var userId = userManager.FindByEmail(email).Id;
//        userManager.AddToRole(userId, rolesList[random.Next(0, rolesList.Count())].Name);
//    }
//}

//public void CreateRandomProjects(int number, int tickets)
//{
//    for (var i = 0; i < number; i++)
//    {
//        var name = string.Concat("Project", i + 1);
//        var random = new Random();

//        var project = new Project();
//        project.Created = DateTime.Now;
//        project.Name = name;
//        project.Id = i + 1;

//        var userList = userManager.Users.ToList();

//        //assign 2 developers to each project
//        var devList = new List<ApplicationUser>();
//        foreach (var user in userList)
//        {
//            if (userManager.IsInRole(user.Id, "Developer"))
//            {
//                devList.Add(user);
//            }
//        }
//        project.ProjectUsers.Add(devList[random.Next(0, devList.Count())]);
//        project.ProjectUsers.Add(devList[random.Next(0, devList.Count())]);

//        //assign the project manager
//        var pmList = new List<ApplicationUser>();
//        foreach (var user in userList)
//        {
//            if (userManager.IsInRole(user.Id, "Project Manager"))
//            {
//                pmList.Add(user);
//            }
//        }
//        var pm = pmList[random.Next(0, pmList.Count())];
//        project.ProjectUsers.Add(pm);
//        project.OwnerId = pm.FullName;

//        context.Projects.Add(project);
//        context.SaveChanges();

//        CreateRandomTickets(project.Id, tickets);
//    }
//}

//public void CreateRandomTickets(int projectId, int number)
//{
//    //Create 'number' tickets per project
//    for (var i = 0; i < number; i++)
//    {
//        var title = string.Concat("Ticket", i + 1);
//        var random = new Random();

//        var ticket = new Ticket();
//        ticket.Created = DateTimeOffset.Now;
//        ticket.Title = title;
//        ticket.Id = i + 1;
//        ticket.ProjectId = projectId;
//        ticket.Description = "This is a seeded ticket.";

//        var userList = userManager.Users.ToList();

//        //assign the ticket a random submitter
//        var subList = new List<ApplicationUser>();
//        foreach (var user in userList)
//        {
//            if (userManager.IsInRole(user.Id, "Submitter"))
//            {
//                subList.Add(user);
//            }
//        }
//        ticket.OwnerUserId = subList[random.Next(0, subList.Count())].Id;

//        //assign the ticket a random developer and set initial status
//        var devList = new List<ApplicationUser>();
//        foreach (var user in userList)
//        {
//            if (userManager.IsInRole(user.Id, "Developer") && context.Projects.FirstOrDefault(p => p.Id == projectId).ProjectUsers.Any(u => u.Id == user.Id))
//            {
//                devList.Add(user);
//            }
//        }
//        ticket.AssignedToUserId = devList[random.Next(0, devList.Count())].Id;
//        ticket.TicketStatusId = context.TicketStatuses.FirstOrDefault(t => t.TicketStatusName == "Open/Assigned").Id;

//        //assign a random type
//        var typeList = context.TicketTypes.Select(t => t.Id).ToList();
//        ticket.TicketTypeId = typeList[random.Next(0, typeList.Count())];

//        //assign a random priority
//        var priorityList = context.TicketPriorities.Select(t => t.Id).ToList();
//        ticket.TicketPriorityId = priorityList[random.Next(0, priorityList.Count())];

//        context.Tickets.Add(ticket);
//        context.SaveChanges();
//    }
//}
#endregion

