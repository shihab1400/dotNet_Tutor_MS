using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectShihab.Entities;
using ProjectShihab.Repos;
using ProjectShihab.Shared;

namespace ProjectShihab.Web.Controllers
{
    [Authorize]
    public class TrainingController(TrainingRepo repo, CourseRepo courseRepo, CurrentUserHelper currentUser) : Controller
    {
        // Trainer: own trainings. Tutor: trainings open for enrollment. Admin: everything.
        public IActionResult Index()
        {
            var result = currentUser.Role switch
            {
                Roles.Trainer => repo.GetOfferings(currentUser.UserId),
                Roles.Tutor => repo.GetOfferings(onlyActive: true),
                _ => repo.GetOfferings()
            };
            if (result.HasError)
                ViewBag.Error = result.Message;

            if (currentUser.Role == Roles.Tutor)
            {
                var mine = repo.GetEnrollments(tutorId: currentUser.UserId).Data ?? [];
                ViewBag.EnrolledIds = mine.Select(e => e.TrainingOfferingId).ToHashSet();
            }
            return View(result.Data ?? []);
        }

        [Authorize(Roles = Roles.Trainer)]
        public IActionResult Edit(int id)
        {
            var model = id == 0
                ? new TrainingOffering { StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(1) }
                : repo.GetOffering(id).Data;
            if (model == null || (id != 0 && model.TrainerId != currentUser.UserId))
                return RedirectToAction("Index");
            FillCourses(model.CourseId);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Trainer)]
        public IActionResult Edit(TrainingOffering model)
        {
            ModelState.Remove(nameof(TrainingOffering.Course));
            ModelState.Remove(nameof(TrainingOffering.Trainer));
            if (!ModelState.IsValid)
            {
                FillCourses(model.CourseId);
                return View(model);
            }

            var result = repo.SaveOffering(model, currentUser.UserId);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                FillCourses(model.CourseId);
                return View(model);
            }

            TempData["Success"] = "Training saved.";
            return RedirectToAction("Index");
        }

        // Trainer: tutors enrolled in my trainings (complete + certify).
        [Authorize(Roles = Roles.Trainer)]
        public IActionResult Enrollments()
        {
            var result = repo.GetEnrollments(trainerId: currentUser.UserId);
            if (result.HasError)
                ViewBag.Error = result.Message;
            ViewBag.Issued = (repo.GetCertificates(trainerId: currentUser.UserId).Data ?? []).Select(c => c.TrainingEnrollmentId).ToHashSet();
            return View(result.Data ?? []);
        }

        [Authorize(Roles = Roles.Tutor)]
        public IActionResult MyTrainings()
        {
            var result = repo.GetEnrollments(tutorId: currentUser.UserId);
            if (result.HasError)
                ViewBag.Error = result.Message;
            return View(result.Data ?? []);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Trainer)]
        public IActionResult Complete(int id)
        {
            var result = repo.Complete(id, currentUser.UserId);
            if (result.HasError)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = "Training marked as completed.";
            return RedirectToAction("Enrollments");
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Trainer)]
        public IActionResult Certify(int id)
        {
            var result = repo.IssueCertificate(id, currentUser.UserId);
            if (result.HasError)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = $"Certificate {result.Data!.CertificateNo} issued.";
            return RedirectToAction("Enrollments");
        }

        private void FillCourses(int selected)
        {
            var courses = (courseRepo.GetAll().Data ?? [])
                .Select(c => new { c.Id, Name = $"{c.Group?.Title} - {c.Subject}" });
            ViewBag.Courses = new SelectList(courses, "Id", "Name", selected);
        }
    }
}
