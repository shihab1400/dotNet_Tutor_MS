using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Models;
using ProjectShihab.Repos;

namespace ProjectShihab.Web.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class UserController(UserRepo repo) : Controller
    {
        public IActionResult Index(string? role)
        {
            var result = repo.GetAll(role);
            if (result.HasError)
                ViewBag.Error = result.Message;
            ViewBag.Role = role;
            return View(result.Data ?? []);
        }

        public IActionResult CreateTrainer()
        {
            return View(new RegisterModel { Role = Roles.Trainer });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateTrainer(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                Gender = model.Gender,
                ContactNumber = model.ContactNumber,
                Dob = model.Dob,
                Address = model.Address
            };
            var result = repo.CreateTrainer(user, model.Password);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            TempData["Success"] = $"Trainer {result.Data!.UserName} created.";
            return RedirectToAction("Index", new { role = Roles.Trainer });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SetStatus(int id, bool status)
        {
            var result = repo.SetStatus(id, status);
            if (result.HasError)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = status ? "User activated." : "User deactivated.";
            return RedirectToAction("Index");
        }
    }
}
