using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class RatingRepo(AsDbContext context, LogRepo logRepo)
    {
        public Result<List<Rating>> GetAll(int? tutorId = null, int? studentId = null)
        {
            try
            {
                var query = context.Ratings.Include(e => e.Student).Include(e => e.Tutor)
                    .Include(e => e.OfferedCourse).ThenInclude(o => o!.Course).AsQueryable();
                if (tutorId != null)
                    query = query.Where(e => e.TutorId == tutorId);
                if (studentId != null)
                    query = query.Where(e => e.StudentId == studentId);
                return Result<List<Rating>>.Ok(query.OrderByDescending(e => e.Id).ToList());
            }
            catch (Exception e)
            {
                return Result<List<Rating>>.Fail(e.Message);
            }
        }

        // tutorId -> (average, count)
        public Dictionary<int, (double Average, int Count)> GetTutorAverages()
        {
            try
            {
                return context.Ratings.GroupBy(e => e.TutorId)
                    .Select(g => new { g.Key, Avg = g.Average(r => r.RatingValue), Cnt = g.Count() })
                    .ToDictionary(e => e.Key, e => (e.Avg, e.Cnt));
            }
            catch
            {
                return new Dictionary<int, (double, int)>();
            }
        }

        public Result<Rating> Save(int courseEnrollmentId, int studentId, int ratingValue, string? feedback)
        {
            try
            {
                if (ratingValue < 1 || ratingValue > 5)
                    return Result<Rating>.Fail("Rating must be between 1 and 5.");

                var enrollment = context.CourseEnrollments.Include(e => e.OfferedCourse)
                    .FirstOrDefault(e => e.Id == courseEnrollmentId && e.StudentId == studentId);
                if (enrollment == null)
                    return Result<Rating>.Fail("Enrollment not found.");
                if (enrollment.Status != EnrollmentStatus.Completed)
                    return Result<Rating>.Fail("You can rate a course only after completing it.");
                if (context.Ratings.Any(e => e.CourseEnrollmentId == courseEnrollmentId))
                    return Result<Rating>.Fail("You have already rated this course.");

                var rating = new Rating
                {
                    CourseEnrollmentId = courseEnrollmentId,
                    StudentId = studentId,
                    TutorId = enrollment.OfferedCourse!.TutorId,
                    OfferedCourseId = enrollment.OfferedCourseId,
                    RatingValue = ratingValue,
                    Feedback = feedback,
                    CreatedAt = DateTime.Now
                };
                context.Ratings.Add(rating);
                context.SaveChanges();

                logRepo.Add("Rate Course", "Rating", rating.Id, $"{ratingValue} star(s) for OfferedCourseId {rating.OfferedCourseId}");
                return Result<Rating>.Ok(rating);
            }
            catch (Exception e)
            {
                return Result<Rating>.Fail(e.Message);
            }
        }
    }
}
