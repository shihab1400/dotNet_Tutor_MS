using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Models;
using ProjectShihab.Repos;
using System.Security.Claims;

namespace ProjectShihab.Web.Controllers
{
    public class AuthController(UserRepo repo, LogRepo logRepo) : Controller
    {
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");
            return View(new LoginModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = repo.Authenticate(model.Email, model.Password);
            if (result.HasError || result.Data == null)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, result.Data.UserName),
                new(ClaimTypes.Role, result.Data.Role),
                new("Email", result.Data.Email),
                new("UserId", result.Data.Id.ToString())
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "AsAuth"));
            await HttpContext.SignInAsync("AsAuth", principal);

            return RedirectToAction("Index", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            logRepo.Add("Logout", "User", null, null);
            await HttpContext.SignOutAsync("AsAuth");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Denied()
        {
            return View();
        }

        public IActionResult Register(string? role)
        {
            return View(new RegisterModel { Role = role == Roles.Tutor ? Roles.Tutor : Roles.Student });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Register(RegisterModel model)
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
                Address = model.Address,
                Role = model.Role
            };
            var result = repo.Register(user, model.Password);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            TempData["Success"] = "Registration successful. Please log in.";
            return RedirectToAction("Login");
        }
    }
}
