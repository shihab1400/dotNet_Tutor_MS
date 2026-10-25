using Microsoft.EntityFrameworkCore;
using ProjectShihab.Entities;

namespace ProjectShihab.Data
{
    public class AsDbContext(DbContextOptions<AsDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<TrainingOffering> TrainingOfferings { get; set; }
        public DbSet<TrainingEnrollment> TrainingEnrollments { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<OfferedCourse> OfferedCourses { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SQL Server rejects multiple cascade paths, so every FK restricts deletes.
            foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            foreach (var prop in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
                prop.SetPrecision(18);
            foreach (var prop in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
                prop.SetScale(2);

            modelBuilder.Entity<User>().HasIndex(e => e.Email).IsUnique();
            modelBuilder.Entity<Group>().HasIndex(e => e.Title).IsUnique();
            modelBuilder.Entity<Course>().HasIndex(e => new { e.GroupId, e.Subject }).IsUnique();
            modelBuilder.Entity<Coupon>().HasIndex(e => e.Code).IsUnique();
            modelBuilder.Entity<OfferedCourse>().HasIndex(e => new { e.CourseId, e.TutorId }).IsUnique();
            modelBuilder.Entity<TrainingOffering>().HasIndex(e => new { e.CourseId, e.TrainerId }).IsUnique();
            modelBuilder.Entity<TrainingEnrollment>().HasIndex(e => new { e.TutorId, e.TrainingOfferingId }).IsUnique();
            modelBuilder.Entity<CourseEnrollment>().HasIndex(e => new { e.StudentId, e.OfferedCourseId }).IsUnique();
            modelBuilder.Entity<Certificate>().HasIndex(e => e.TrainingEnrollmentId).IsUnique();
            modelBuilder.Entity<Certificate>().HasIndex(e => e.CertificateNo).IsUnique();
            modelBuilder.Entity<Rating>().HasIndex(e => e.CourseEnrollmentId).IsUnique();
            modelBuilder.Entity<Log>().HasIndex(e => e.CreatedAt);

            modelBuilder.Entity<User>().ToTable(t => t.HasCheckConstraint("CK_User_Role", "[Role] in ('Admin','Trainer','Tutor','Student')"));
            modelBuilder.Entity<Coupon>().ToTable(t => t.HasCheckConstraint("CK_Coupon_Type", "[Type] in ('Percentage','Fixed')"));
            modelBuilder.Entity<Rating>().ToTable(t => t.HasCheckConstraint("CK_Rating_Value", "[Rating] between 1 and 5"));
            modelBuilder.Entity<Material>().ToTable(t => t.HasCheckConstraint("CK_Material_Target", "([OfferedCourseId] is null and [TrainingOfferingId] is not null) or ([OfferedCourseId] is not null and [TrainingOfferingId] is null)"));
        }
    }
}
