using Microsoft.EntityFrameworkCore;
using ProjectShihab.Entities;

namespace ProjectShihab.Data
{
    public class AsDbContext(DbContextOptions<AsDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<OfferedCourse> OfferedCourses { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Rating> Ratings { get; set; }

       
    }
}
