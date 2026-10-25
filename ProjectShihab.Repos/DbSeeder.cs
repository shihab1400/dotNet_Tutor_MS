using Microsoft.AspNetCore.Identity;
using ProjectShihab.Data;
using ProjectShihab.Entities;

namespace ProjectShihab.Repos
{
    public static class DbSeeder
    {
        public const string AdminEmail = "admin@adn.com";
        public const string AdminPassword = "Admin@123";

        // Idempotent: seeds the admin account and a few groups/courses only when missing.
        public static void Seed(AsDbContext context)
        {
            var admin = context.Users.FirstOrDefault(e => e.Role == Roles.Admin);
            if (admin == null)
            {
                admin = new User
                {
                    UserName = "Administrator",
                    Email = AdminEmail,
                    Gender = "Others",
                    ContactNumber = "00000000000",
                    Dob = new DateTime(2000, 1, 1),
                    Address = "Head Office",
                    Role = Roles.Admin,
                    Status = true,
                    CreatedAt = DateTime.Now
                };
                admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, AdminPassword);
                context.Users.Add(admin);
                context.SaveChanges();
            }

            if (context.Groups.Any())
                return;

            string[] groups = ["Class 8", "Class 9", "CSE", "EEE"];
            foreach (var title in groups)
                context.Groups.Add(new Group { Title = title });
            context.SaveChanges();

            (string Group, string Subject, decimal Price)[] courses =
            [
                ("Class 8", "Mathematics", 1500), ("Class 8", "English", 1200),
                ("Class 9", "Physics", 1800), ("Class 9", "Chemistry", 1800),
                ("CSE", "Algorithms", 3000), ("CSE", "Database Systems", 3000),
                ("EEE", "Circuit Analysis", 3200), ("EEE", "Digital Electronics", 3200)
            ];
            foreach (var c in courses)
            {
                context.Courses.Add(new Course
                {
                    GroupId = context.Groups.First(g => g.Title == c.Group).Id,
                    Subject = c.Subject,
                    BasePrice = c.Price,
                    CreatedBy = admin.Id,
                    CreatedAt = DateTime.Now
                });
            }
            context.SaveChanges();
        }
    }
}
