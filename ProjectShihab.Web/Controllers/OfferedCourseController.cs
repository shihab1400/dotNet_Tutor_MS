using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectShihab.Entities;
using ProjectShihab.Repos;
using ProjectShihab.Shared;

namespace ProjectShihab.Web.Controllers
{
    [Authorize]
    public class OfferedCourseController(OfferedCourseRepo repo, TrainingRepo trainingRepo, RatingRepo ratingRepo, CurrentUserHelper currentUser) : Controller
    {
        // Tutor: own offered courses. Student: courses open for enrollment. Admin: everything.
        [Authorize(Roles = $"{Roles.Admin},{Roles.Tutor},{Roles.Student}")]
        public IActionResult Index()
        {
            var tutor = currentUser.Role == Roles.Tutor;
            var result = tutor ? repo.GetAll(tutorId: currentUser.UserId) : repo.GetAll(onlyActive: currentUser.Role == Roles.Student);
            if (result.HasError)
                ViewBag.Error = result.Message;

            ViewBag.Averages = ratingRepo.GetTutorAverages();
            if (currentUser.Role == Roles.Student)
            {
                var mine = repo.GetEnrollments(studentId: currentUser.UserId).Data ?? [];
                ViewBag.EnrolledIds = mine.Select(e => e.OfferedCourseId).ToHashSet();
            }
            return View(result.Data ?? []);
        }

        [Authorize(Roles = Roles.Tutor)]
        public IActionResult Edit(int id)
        {
            var model = id == 0
                ? new OfferedCourse { StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(2) }
                : repo.GetById(id).Data;
            if (model == null || (id != 0 && model.TutorId != currentUser.UserId))
                return RedirectToAction("Index");
            FillCourses(model.CourseId);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Tutor)]
        public IActionResult Edit(OfferedCourse model)
        {
            ModelState.Remove(nameof(OfferedCourse.Course));
            ModelState.Remove(nameof(OfferedCourse.User));
            if (!ModelState.IsValid)
            {
                FillCourses(model.CourseId);
                return View(model);
            }

            var result = repo.Save(model, currentUser.UserId);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                FillCourses(model.CourseId);
                return View(model);
            }

            TempData["Success"] = "Course saved.";
            return RedirectToAction("Index");
        }

        // Tutor: students enrolled in my courses.
        [Authorize(Roles = Roles.Tutor)]
        public IActionResult Students()
        {
            var result = repo.GetEnrollments(tutorId: currentUser.UserId);
            if (result.HasError)
                ViewBag.Error = result.Message;
            return View(result.Data ?? []);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Tutor)]
        public IActionResult Complete(int id)
        {
            var result = repo.Complete(id, currentUser.UserId);
            if (result.HasError)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = "Course marked as completed for the student.";
            return RedirectToAction("Students");
        }

        // Only courses the tutor holds a certificate for can be offered.
        private void FillCourses(int selected)
        {
            var certs = trainingRepo.GetCertificates(tutorId: currentUser.UserId).Data ?? [];
            var courses = certs.DistinctBy(c => c.CourseId)
                .Select(c => new { Id = c.CourseId, Name = $"{c.Course?.Group?.Title} - {c.Course?.Subject}" });
            ViewBag.Courses = new SelectList(courses, "Id", "Name", selected);
        }
    }
}
