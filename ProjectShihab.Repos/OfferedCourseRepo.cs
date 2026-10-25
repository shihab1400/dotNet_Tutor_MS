using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class OfferedCourseRepo(AsDbContext context, CouponRepo couponRepo, PaymentRepo paymentRepo, LogRepo logRepo)
    {
        public Result<List<OfferedCourse>> GetAll(int? tutorId = null, bool onlyActive = false)
        {
            try
            {
                var query = context.OfferedCourses.Include(e => e.Course).ThenInclude(c => c!.Group)
                    .Include(e => e.User).AsQueryable();
                if (tutorId != null)
                    query = query.Where(e => e.TutorId == tutorId);
                if (onlyActive)
                    query = query.Where(e => e.Status && e.EndDate >= DateTime.Today);
                return Result<List<OfferedCourse>>.Ok(query.OrderByDescending(e => e.Id).ToList());
            }
            catch (Exception e)
            {
                return Result<List<OfferedCourse>>.Fail(e.Message);
            }
        }

        public Result<OfferedCourse> GetById(int id)
        {
            try
            {
                return Result<OfferedCourse>.Ok(context.OfferedCourses.Include(e => e.Course).ThenInclude(c => c!.Group)
                    .Include(e => e.User).FirstOrDefault(e => e.Id == id));
            }
            catch (Exception e)
            {
                return Result<OfferedCourse>.Fail(e.Message);
            }
        }

        // A tutor may only offer a course once a trainer has certified them for it.
        public Result<OfferedCourse> Save(OfferedCourse model, int tutorId)
        {
            try
            {
                if (model.EndDate < model.StartDate)
                    return Result<OfferedCourse>.Fail("End date cannot be before start date.");
                if (!context.Certificates.Any(e => e.TutorId == tutorId && e.CourseId == model.CourseId))
                    return Result<OfferedCourse>.Fail("You need a training certificate for this course before offering it.");
                if (context.OfferedCourses.Any(e => e.CourseId == model.CourseId && e.TutorId == tutorId && e.Id != model.Id))
                    return Result<OfferedCourse>.Fail("You already offer this course.");

                var objToSave = context.OfferedCourses.Find(model.Id);
                var isNew = objToSave == null;
                if (objToSave != null && objToSave.TutorId != tutorId)
                    return Result<OfferedCourse>.Fail("You can only edit your own offered course.");
                if (objToSave == null)
                {
                    objToSave = new OfferedCourse { TutorId = tutorId };
                    context.OfferedCourses.Add(objToSave);
                }

                objToSave.CourseId = model.CourseId;
                objToSave.Price = model.Price;
                objToSave.StartDate = model.StartDate;
                objToSave.EndDate = model.EndDate;
                objToSave.Status = model.Status;
                context.SaveChanges();

                logRepo.Add(isNew ? "Offer Course" : "Update Offered Course", "OfferedCourse", objToSave.Id, $"CourseId: {objToSave.CourseId}, Price: {objToSave.Price}");
                return Result<OfferedCourse>.Ok(objToSave);
            }
            catch (Exception e)
            {
                return Result<OfferedCourse>.Fail(e.Message);
            }
        }

        public Result<List<CourseEnrollment>> GetEnrollments(int? studentId = null, int? tutorId = null)
        {
            try
            {
                var query = context.CourseEnrollments.Include(e => e.Student).Include(e => e.Payment)
                    .Include(e => e.OfferedCourse).ThenInclude(o => o!.Course).ThenInclude(c => c!.Group)
                    .Include(e => e.OfferedCourse).ThenInclude(o => o!.User).AsQueryable();
                if (studentId != null)
                    query = query.Where(e => e.StudentId == studentId);
                if (tutorId != null)
                    query = query.Where(e => e.OfferedCourse!.TutorId == tutorId);
                return Result<List<CourseEnrollment>>.Ok(query.OrderByDescending(e => e.Id).ToList());
            }
            catch (Exception e)
            {
                return Result<List<CourseEnrollment>>.Fail(e.Message);
            }
        }

        public Result<PriceQuote> Quote(int offeredCourseId, string? couponCode)
        {
            var offered = context.OfferedCourses.Find(offeredCourseId);
            if (offered == null)
                return Result<PriceQuote>.Fail("Course not found.");
            return couponRepo.Quote(offered.Price, couponCode);
        }

        public Result<CourseEnrollment> Enroll(int studentId, int offeredCourseId, string? couponCode)
        {
            try
            {
                var offered = context.OfferedCourses.Find(offeredCourseId);
                if (offered == null || !offered.Status || offered.EndDate < DateTime.Today)
                    return Result<CourseEnrollment>.Fail("This course is not available.");
                if (context.CourseEnrollments.Any(e => e.StudentId == studentId && e.OfferedCourseId == offeredCourseId))
                    return Result<CourseEnrollment>.Fail("You are already enrolled in this course.");
                // The tutor's certificate must still exist (defensive; offering requires it).
                if (!context.Certificates.Any(e => e.TutorId == offered.TutorId && e.CourseId == offered.CourseId))
                    return Result<CourseEnrollment>.Fail("This tutor is not certified for the course.");

                var quote = couponRepo.Quote(offered.Price, couponCode);
                if (quote.HasError || quote.Data == null)
                    return Result<CourseEnrollment>.Fail(quote.Message);

                using var tx = context.Database.BeginTransaction();
                var payment = paymentRepo.Pay(studentId, PaymentKinds.Course, offeredCourseId, quote.Data);
                var enrollment = new CourseEnrollment
                {
                    StudentId = studentId,
                    OfferedCourse = offered,
                    Payment = payment,
                    Status = EnrollmentStatus.Enrolled,
                    EnrolledAt = DateTime.Now
                };
                context.CourseEnrollments.Add(enrollment);
                context.SaveChanges();
                tx.Commit();

                logRepo.Add("Enroll Course", "CourseEnrollment", enrollment.Id, $"Paid {payment.FinalAmount} (txn {payment.TransactionRef})");
                return Result<CourseEnrollment>.Ok(enrollment);
            }
            catch (Exception e)
            {
                return Result<CourseEnrollment>.Fail(e.Message);
            }
        }

        public Result<CourseEnrollment> Complete(int enrollmentId, int tutorId)
        {
            try
            {
                var enrollment = context.CourseEnrollments.Include(e => e.OfferedCourse).FirstOrDefault(e => e.Id == enrollmentId);
                if (enrollment == null || enrollment.OfferedCourse!.TutorId != tutorId)
                    return Result<CourseEnrollment>.Fail("Enrollment not found.");
                if (enrollment.Status == EnrollmentStatus.Completed)
                    return Result<CourseEnrollment>.Fail("Course is already completed.");

                enrollment.Status = EnrollmentStatus.Completed;
                enrollment.CompletedAt = DateTime.Now;
                context.SaveChanges();

                logRepo.Add("Complete Course", "CourseEnrollment", enrollment.Id, $"StudentId: {enrollment.StudentId}");
                return Result<CourseEnrollment>.Ok(enrollment);
            }
            catch (Exception e)
            {
                return Result<CourseEnrollment>.Fail(e.Message);
            }
        }
    }
}
