using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;

namespace ProjectShihab.Repos
{
    public record StatCard(string Label, string Value, string Icon, string Tone);

    public record ActivityItem(string Title, string Sub, DateTime When);

    public class DashboardStats
    {
        public List<StatCard> Cards { get; } = [];
        public List<ActivityItem> Activity { get; } = [];
        public string ActivityTitle { get; set; } = "Recent activity";
    }

    public class PublicStats
    {
        public int Tutors { get; set; }
        public int Students { get; set; }
        public int Courses { get; set; }
        public double AverageRating { get; set; }
        public List<OfferedCourse> Featured { get; set; } = [];
        public Dictionary<int, (double Average, int Count)> Ratings { get; set; } = [];
        public List<Group> Groups { get; set; } = [];
        public List<Course> AllCourses { get; set; } = [];
    }

    public class DashboardRepo(AsDbContext context, RatingRepo ratingRepo)
    {
        public PublicStats GetPublic()
        {
            var stats = new PublicStats();
            try
            {
                stats.Tutors = context.Certificates.Select(c => c.TutorId).Distinct().Count();
                stats.Students = context.Users.Count(u => u.Role == Roles.Student && u.Status);
                stats.Courses = context.Courses.Count();
                stats.AverageRating = context.Ratings.Any() ? context.Ratings.Average(r => r.RatingValue) : 0;
                stats.Ratings = ratingRepo.GetTutorAverages();
                stats.Groups = context.Groups.OrderBy(g => g.Title).ToList();
                stats.AllCourses = context.Courses.OrderBy(c => c.Subject).ToList();
                stats.Featured = context.OfferedCourses.Include(o => o.Course).ThenInclude(c => c!.Group).Include(o => o.User)
                    .Where(o => o.Status && o.EndDate >= DateTime.Today).ToList()
                    .OrderByDescending(o => stats.Ratings.TryGetValue(o.TutorId, out var r) ? r.Average : 0)
                    .Take(6).ToList();
            }
            catch
            {
            }
            return stats;
        }

        public DashboardStats Get(int userId, string role)
        {
            var s = new DashboardStats();
            try
            {
                switch (role)
                {
                    case Roles.Admin: FillAdmin(s); break;
                    case Roles.Trainer: FillTrainer(s, userId); break;
                    case Roles.Tutor: FillTutor(s, userId); break;
                    case Roles.Student: FillStudent(s, userId); break;
                }
            }
            catch
            {
            }
            return s;
        }

        private void FillAdmin(DashboardStats s)
        {
            s.Cards.Add(new("Students", context.Users.Count(u => u.Role == Roles.Student).ToString(), "bi-mortarboard", "tone-brand"));
            s.Cards.Add(new("Tutors", context.Users.Count(u => u.Role == Roles.Tutor).ToString(), "bi-person-video3", "tone-info"));
            s.Cards.Add(new("Trainers", context.Users.Count(u => u.Role == Roles.Trainer).ToString(), "bi-person-workspace", "tone-warning"));
            s.Cards.Add(new("Revenue", "৳ " + context.Payments.Sum(p => (decimal?)p.FinalAmount).GetValueOrDefault().ToString("n0"), "bi-cash-stack", "tone-success"));
            s.ActivityTitle = "Latest system activity";
            s.Activity.AddRange(context.Logs.OrderByDescending(l => l.Id).Take(8).ToList()
                .Select(l => new ActivityItem(l.Action, $"{l.UserName} · {l.Role}{(l.Details != null ? " · " + l.Details : "")}", l.CreatedAt)));
        }

        private void FillTrainer(DashboardStats s, int id)
        {
            var enrollments = context.TrainingEnrollments.Where(e => e.TrainingOffering!.TrainerId == id);
            s.Cards.Add(new("My trainings", context.TrainingOfferings.Count(t => t.TrainerId == id).ToString(), "bi-easel", "tone-brand"));
            s.Cards.Add(new("Enrolled tutors", enrollments.Count().ToString(), "bi-people", "tone-info"));
            s.Cards.Add(new("Awaiting completion", enrollments.Count(e => e.Status == EnrollmentStatus.Enrolled).ToString(), "bi-hourglass-split", "tone-warning"));
            s.Cards.Add(new("Certificates issued", context.Certificates.Count(c => c.TrainerId == id).ToString(), "bi-award", "tone-success"));
            s.ActivityTitle = "Recent enrollments";
            s.Activity.AddRange(enrollments.Include(e => e.Tutor).Include(e => e.TrainingOffering).ThenInclude(t => t!.Course)
                .OrderByDescending(e => e.Id).Take(6).ToList()
                .Select(e => new ActivityItem($"{e.Tutor?.UserName} enrolled", $"{e.TrainingOffering?.Course?.Subject} · {e.Status}", e.EnrolledAt)));
        }

        private void FillTutor(DashboardStats s, int id)
        {
            var avg = context.Ratings.Where(r => r.TutorId == id).Select(r => (double?)r.RatingValue).Average();
            s.Cards.Add(new("Certificates", context.Certificates.Count(c => c.TutorId == id).ToString(), "bi-award", "tone-warning"));
            s.Cards.Add(new("Courses offered", context.OfferedCourses.Count(o => o.TutorId == id).ToString(), "bi-journal-bookmark", "tone-brand"));
            s.Cards.Add(new("Students", context.CourseEnrollments.Count(e => e.OfferedCourse!.TutorId == id).ToString(), "bi-people", "tone-info"));
            s.Cards.Add(new("Avg. rating", avg == null ? "—" : avg.Value.ToString("0.0") + " ★", "bi-star", "tone-success"));
            s.ActivityTitle = "Recent student enrollments";
            s.Activity.AddRange(context.CourseEnrollments.Include(e => e.Student).Include(e => e.OfferedCourse).ThenInclude(o => o!.Course)
                .Where(e => e.OfferedCourse!.TutorId == id).OrderByDescending(e => e.Id).Take(6).ToList()
                .Select(e => new ActivityItem($"{e.Student?.UserName} enrolled", $"{e.OfferedCourse?.Course?.Subject} · {e.Status}", e.EnrolledAt)));
        }

        private void FillStudent(DashboardStats s, int id)
        {
            var mine = context.CourseEnrollments.Where(e => e.StudentId == id);
            var rated = context.Ratings.Where(r => r.StudentId == id).Select(r => r.CourseEnrollmentId);
            s.Cards.Add(new("Enrolled courses", mine.Count().ToString(), "bi-journal-bookmark", "tone-brand"));
            s.Cards.Add(new("Completed", mine.Count(e => e.Status == EnrollmentStatus.Completed).ToString(), "bi-check2-circle", "tone-success"));
            s.Cards.Add(new("Awaiting your rating", mine.Count(e => e.Status == EnrollmentStatus.Completed && !rated.Contains(e.Id)).ToString(), "bi-star", "tone-warning"));
            s.Cards.Add(new("Total spent", "৳ " + context.Payments.Where(p => p.UserId == id).Sum(p => (decimal?)p.FinalAmount).GetValueOrDefault().ToString("n0"), "bi-wallet2", "tone-info"));
            s.ActivityTitle = "My courses";
            s.Activity.AddRange(mine.Include(e => e.OfferedCourse).ThenInclude(o => o!.Course).Include(e => e.OfferedCourse).ThenInclude(o => o!.User)
                .OrderByDescending(e => e.Id).Take(6).ToList()
                .Select(e => new ActivityItem(e.OfferedCourse?.Course?.Subject ?? "", $"with {e.OfferedCourse?.User?.UserName} · {e.Status}", e.EnrolledAt)));
        }
    }
}
