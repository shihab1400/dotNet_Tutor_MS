using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Models;
using ProjectShihab.Repos;
using ProjectShihab.Shared;

namespace ProjectShihab.Web.Controllers
{
    [Authorize]
    public class PaymentController(PaymentRepo repo, TrainingRepo trainingRepo, OfferedCourseRepo offeredCourseRepo, CurrentUserHelper currentUser) : Controller
    {
        // Admin sees every payment, everyone else sees their own.
        public IActionResult Index()
        {
            var result = repo.GetAll(currentUser.Role == Roles.Admin ? null : currentUser.UserId);
            if (result.HasError)
                ViewBag.Error = result.Message;
            return View(result.Data ?? []);
        }

        // Tutors pay for trainings, students pay for courses.
        [Authorize(Roles = $"{Roles.Tutor},{Roles.Student}")]
        public IActionResult Checkout(string kind, int id, string? couponCode)
        {
            var model = Build(kind, id, couponCode);
            if (model == null)
            {
                TempData["Error"] = "Nothing to pay for.";
                return RedirectToAction("Index", "Dashboard");
            }
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Tutor},{Roles.Student}")]
        public IActionResult Pay(string kind, int id, string? couponCode)
        {
            if (kind == PaymentKinds.Training && currentUser.Role == Roles.Tutor)
            {
                var result = trainingRepo.Enroll(currentUser.UserId, id, couponCode);
                if (result.HasError)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction("Checkout", new { kind, id, couponCode });
                }
                TempData["Success"] = "Payment successful. You are enrolled in the training.";
                return RedirectToAction("MyTrainings", "Training");
            }

            if (kind == PaymentKinds.Course && currentUser.Role == Roles.Student)
            {
                var result = offeredCourseRepo.Enroll(currentUser.UserId, id, couponCode);
                if (result.HasError)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction("Checkout", new { kind, id, couponCode });
                }
                TempData["Success"] = "Payment successful. You are enrolled in the course.";
                return RedirectToAction("Index", "Enrollment");
            }

            return RedirectToAction("Denied", "Auth");
        }

        private CheckoutModel? Build(string kind, int id, string? couponCode)
        {
            var model = new CheckoutModel { Kind = kind, TargetId = id, CouponCode = couponCode };
            Result<PriceQuote> quote;

            if (kind == PaymentKinds.Training && currentUser.Role == Roles.Tutor)
            {
                var offering = trainingRepo.GetOffering(id).Data;
                if (offering == null) return null;
                model.Title = $"Training: {offering.Course?.Subject}";
                model.Subtitle = $"Trainer {offering.Trainer?.UserName} - {offering.StartDate:dd MMM yyyy} to {offering.EndDate:dd MMM yyyy}";
                quote = trainingRepo.Quote(id, couponCode);
            }
            else if (kind == PaymentKinds.Course && currentUser.Role == Roles.Student)
            {
                var offered = offeredCourseRepo.GetById(id).Data;
                if (offered == null) return null;
                model.Title = $"Course: {offered.Course?.Subject}";
                model.Subtitle = $"Tutor {offered.User?.UserName} - {offered.StartDate:dd MMM yyyy} to {offered.EndDate:dd MMM yyyy}";
                quote = offeredCourseRepo.Quote(id, couponCode);
            }
            else
            {
                return null;
            }

            if (quote.HasError || quote.Data == null)
            {
                // Show the undiscounted price together with the reason the coupon was rejected.
                model.CouponMessage = quote.Message;
                var plain = kind == PaymentKinds.Training
                    ? trainingRepo.Quote(id, null)
                    : offeredCourseRepo.Quote(id, null);
                model.Original = model.Final = plain.Data?.Original ?? 0;
            }
            else
            {
                model.Original = quote.Data.Original;
                model.Discount = quote.Data.Discount;
                model.Final = quote.Data.Final;
                if (quote.Data.Coupon != null)
                    model.CouponMessage = $"Coupon {quote.Data.Coupon.Code} applied.";
            }
            return model;
        }
    }
}
