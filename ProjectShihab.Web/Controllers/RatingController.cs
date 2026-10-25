using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Repos;
using ProjectShihab.Shared;

namespace ProjectShihab.Web.Controllers
{
    [Authorize(Roles = $"{Roles.Admin},{Roles.Tutor}")]
    public class RatingController(RatingRepo repo, CurrentUserHelper currentUser) : Controller
    {
        // Tutors see ratings they received; admin sees all.
        public IActionResult Index()
        {
            var result = repo.GetAll(tutorId: currentUser.Role == Roles.Tutor ? currentUser.UserId : null);
            if (result.HasError)
                ViewBag.Error = result.Message;
            return View(result.Data ?? []);
        }
    }
}
