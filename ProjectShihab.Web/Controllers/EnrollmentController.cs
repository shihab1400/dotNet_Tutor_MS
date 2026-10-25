using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Models;
using ProjectShihab.Repos;
using ProjectShihab.Shared;

namespace ProjectShihab.Web.Controllers
{
    // Student: my courses, and rating a completed one.
    [Authorize(Roles = Roles.Student)]
    public class EnrollmentController(OfferedCourseRepo repo, RatingRepo ratingRepo, CurrentUserHelper currentUser) : Controller
    {
        public IActionResult Index()
        {
            var result = repo.GetEnrollments(studentId: currentUser.UserId);
            if (result.HasError)
                ViewBag.Error = result.Message;
            ViewBag.Rated = (ratingRepo.GetAll(studentId: currentUser.UserId).Data ?? []).Select(r => r.CourseEnrollmentId).ToHashSet();
            return View(result.Data ?? []);
        }

        public IActionResult Rate(int id)
        {
            var enrollment = (repo.GetEnrollments(studentId: currentUser.UserId).Data ?? []).FirstOrDefault(e => e.Id == id);
            if (enrollment == null)
                return RedirectToAction("Index");
            return View(new RatingModel { CourseEnrollmentId = id, CourseTitle = enrollment.OfferedCourse?.Course?.Subject ?? "" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Rate(RatingModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = ratingRepo.Save(model.CourseEnrollmentId, currentUser.UserId, model.Rating, model.Feedback);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            TempData["Success"] = "Thanks for your rating.";
            return RedirectToAction("Index");
        }
    }
}
