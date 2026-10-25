using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class TrainingRepo(AsDbContext context, CouponRepo couponRepo, PaymentRepo paymentRepo, LogRepo logRepo)
    {
        public Result<List<TrainingOffering>> GetOfferings(int? trainerId = null, bool onlyActive = false)
        {
            try
            {
                var query = context.TrainingOfferings.Include(e => e.Course).ThenInclude(c => c!.Group)
                    .Include(e => e.Trainer).AsQueryable();
                if (trainerId != null)
                    query = query.Where(e => e.TrainerId == trainerId);
                if (onlyActive)
                    query = query.Where(e => e.Status && e.EndDate >= DateTime.Today);
                return Result<List<TrainingOffering>>.Ok(query.OrderByDescending(e => e.Id).ToList());
            }
            catch (Exception e)
            {
                return Result<List<TrainingOffering>>.Fail(e.Message);
            }
        }

        public Result<TrainingOffering> GetOffering(int id)
        {
            try
            {
                return Result<TrainingOffering>.Ok(context.TrainingOfferings.Include(e => e.Course).ThenInclude(c => c!.Group)
                    .Include(e => e.Trainer).FirstOrDefault(e => e.Id == id));
            }
            catch (Exception e)
            {
                return Result<TrainingOffering>.Fail(e.Message);
            }
        }

        public Result<TrainingOffering> SaveOffering(TrainingOffering model, int trainerId)
        {
            try
            {
                if (model.EndDate < model.StartDate)
                    return Result<TrainingOffering>.Fail("End date cannot be before start date.");
                if (context.TrainingOfferings.Any(e => e.CourseId == model.CourseId && e.TrainerId == trainerId && e.Id != model.Id))
                    return Result<TrainingOffering>.Fail("You already offer training for this course.");

                var objToSave = context.TrainingOfferings.Find(model.Id);
                var isNew = objToSave == null;
                if (objToSave != null && objToSave.TrainerId != trainerId)
                    return Result<TrainingOffering>.Fail("You can only edit your own training.");
                if (objToSave == null)
                {
                    objToSave = new TrainingOffering { TrainerId = trainerId };
                    context.TrainingOfferings.Add(objToSave);
                }

                objToSave.CourseId = model.CourseId;
                objToSave.Price = model.Price;
                objToSave.StartDate = model.StartDate;
                objToSave.EndDate = model.EndDate;
                objToSave.Status = model.Status;
                context.SaveChanges();

                logRepo.Add(isNew ? "Create Training" : "Update Training", "TrainingOffering", objToSave.Id, $"CourseId: {objToSave.CourseId}, Price: {objToSave.Price}");
                return Result<TrainingOffering>.Ok(objToSave);
            }
            catch (Exception e)
            {
                return Result<TrainingOffering>.Fail(e.Message);
            }
        }

        public Result<List<TrainingEnrollment>> GetEnrollments(int? trainerId = null, int? tutorId = null)
        {
            try
            {
                var query = context.TrainingEnrollments.Include(e => e.Tutor).Include(e => e.Payment)
                    .Include(e => e.TrainingOffering).ThenInclude(t => t!.Course).AsQueryable();
                if (trainerId != null)
                    query = query.Where(e => e.TrainingOffering!.TrainerId == trainerId);
                if (tutorId != null)
                    query = query.Where(e => e.TutorId == tutorId);
                return Result<List<TrainingEnrollment>>.Ok(query.OrderByDescending(e => e.Id).ToList());
            }
            catch (Exception e)
            {
                return Result<List<TrainingEnrollment>>.Fail(e.Message);
            }
        }

        public Result<PriceQuote> Quote(int offeringId, string? couponCode)
        {
            var offering = context.TrainingOfferings.Find(offeringId);
            if (offering == null)
                return Result<PriceQuote>.Fail("Training not found.");
            return couponRepo.Quote(offering.Price, couponCode);
        }

        public Result<TrainingEnrollment> Enroll(int tutorId, int offeringId, string? couponCode)
        {
            try
            {
                var offering = context.TrainingOfferings.Find(offeringId);
                if (offering == null || !offering.Status || offering.EndDate < DateTime.Today)
                    return Result<TrainingEnrollment>.Fail("This training is not available.");
                if (context.TrainingEnrollments.Any(e => e.TutorId == tutorId && e.TrainingOfferingId == offeringId))
                    return Result<TrainingEnrollment>.Fail("You are already enrolled in this training.");

                var quote = couponRepo.Quote(offering.Price, couponCode);
                if (quote.HasError || quote.Data == null)
                    return Result<TrainingEnrollment>.Fail(quote.Message);

                using var tx = context.Database.BeginTransaction();
                var payment = paymentRepo.Pay(tutorId, PaymentKinds.Training, offeringId, quote.Data);
                var enrollment = new TrainingEnrollment
                {
                    TutorId = tutorId,
                    TrainingOffering = offering,
                    Payment = payment,
                    Status = EnrollmentStatus.Enrolled,
                    EnrolledAt = DateTime.Now
                };
                context.TrainingEnrollments.Add(enrollment);
                context.SaveChanges();
                tx.Commit();

                logRepo.Add("Enroll Training", "TrainingEnrollment", enrollment.Id, $"Paid {payment.FinalAmount} (txn {payment.TransactionRef})");
                return Result<TrainingEnrollment>.Ok(enrollment);
            }
            catch (Exception e)
            {
                return Result<TrainingEnrollment>.Fail(e.Message);
            }
        }

        public Result<TrainingEnrollment> Complete(int enrollmentId, int trainerId)
        {
            try
            {
                var enrollment = context.TrainingEnrollments.Include(e => e.TrainingOffering).FirstOrDefault(e => e.Id == enrollmentId);
                if (enrollment == null || enrollment.TrainingOffering!.TrainerId != trainerId)
                    return Result<TrainingEnrollment>.Fail("Enrollment not found.");
                if (enrollment.Status == EnrollmentStatus.Completed)
                    return Result<TrainingEnrollment>.Fail("Training is already completed.");

                enrollment.Status = EnrollmentStatus.Completed;
                enrollment.CompletedAt = DateTime.Now;
                context.SaveChanges();

                logRepo.Add("Complete Training", "TrainingEnrollment", enrollment.Id, $"TutorId: {enrollment.TutorId}");
                return Result<TrainingEnrollment>.Ok(enrollment);
            }
            catch (Exception e)
            {
                return Result<TrainingEnrollment>.Fail(e.Message);
            }
        }

        public Result<Certificate> IssueCertificate(int enrollmentId, int trainerId)
        {
            try
            {
                var enrollment = context.TrainingEnrollments.Include(e => e.TrainingOffering).FirstOrDefault(e => e.Id == enrollmentId);
                if (enrollment == null || enrollment.TrainingOffering!.TrainerId != trainerId)
                    return Result<Certificate>.Fail("Enrollment not found.");
                if (enrollment.Status != EnrollmentStatus.Completed)
                    return Result<Certificate>.Fail("Complete the training before issuing a certificate.");
                if (context.Certificates.Any(e => e.TrainingEnrollmentId == enrollmentId))
                    return Result<Certificate>.Fail("Certificate already issued.");

                var certificate = new Certificate
                {
                    TrainingEnrollmentId = enrollment.Id,
                    TutorId = enrollment.TutorId,
                    TrainerId = trainerId,
                    CourseId = enrollment.TrainingOffering.CourseId,
                    CertificateNo = "CERT-" + Guid.NewGuid().ToString("N")[..10].ToUpper(),
                    IssuedAt = DateTime.Now
                };
                context.Certificates.Add(certificate);
                context.SaveChanges();

                logRepo.Add("Issue Certificate", "Certificate", certificate.Id, $"{certificate.CertificateNo} to TutorId {certificate.TutorId}");
                return Result<Certificate>.Ok(certificate);
            }
            catch (Exception e)
            {
                return Result<Certificate>.Fail(e.Message);
            }
        }

        public Result<List<Certificate>> GetCertificates(int? tutorId = null, int? trainerId = null)
        {
            try
            {
                var query = context.Certificates.Include(e => e.Course).ThenInclude(c => c!.Group)
                    .Include(e => e.Tutor).Include(e => e.Trainer).AsQueryable();
                if (tutorId != null)
                    query = query.Where(e => e.TutorId == tutorId);
                if (trainerId != null)
                    query = query.Where(e => e.TrainerId == trainerId);
                return Result<List<Certificate>>.Ok(query.OrderByDescending(e => e.Id).ToList());
            }
            catch (Exception e)
            {
                return Result<List<Certificate>>.Fail(e.Message);
            }
        }

        public Result<Certificate> GetCertificate(int id)
        {
            try
            {
                return Result<Certificate>.Ok(context.Certificates.Include(e => e.Course).ThenInclude(c => c!.Group)
                    .Include(e => e.Tutor).Include(e => e.Trainer).FirstOrDefault(e => e.Id == id));
            }
            catch (Exception e)
            {
                return Result<Certificate>.Fail(e.Message);
            }
        }
    }
}
