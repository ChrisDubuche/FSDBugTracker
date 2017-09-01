using FSDBugTracker.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FSDBugTracker.Helpers
{
    public class UserProjectHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.ProjectUsers.Any(u => u.Id == userId);
            return (flag);
        }

        public ICollection<Project> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            var projects = user.Projects.ToList();
            return (projects);
        }

        public void AddUserToProject(string userId, int projectId)
        {

            if (!IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var newUser = db.Users.Find(userId);

                proj.ProjectUsers.Add(newUser);
                db.SaveChanges();
            }
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            if (IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var delUser = db.Users.Find(userId);

                proj.ProjectUsers.Remove(delUser);
                db.Entry(proj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public ICollection<ApplicationUser> UsersOnProject(int projectId)
        {
            var users = new List<ApplicationUser>();
            var project = db.Projects.FirstOrDefault(p => p.Id == projectId);
            foreach (var projectUser in project.ProjectUsers)
            {
                users.Add(projectUser);
            }

            return users;
        }

        public ICollection<ApplicationUser> UsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }

        public void AddUsersToProject(List<string> userIds, int projectId)
        {
            foreach (var user in userIds)
            {
                if (!IsUserOnProject(user, projectId))
                {
                    Project proj = db.Projects.Find(projectId);
                    var newUser = db.Users.Find(user);
                    proj.ProjectUsers.Add(newUser);
                }
            }
        }
    }
}