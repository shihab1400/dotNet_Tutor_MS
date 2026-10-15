using ProjectShihab.Models;
using ProjectShihab.Repos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProjectShihab.Entities;

namespace ProjectShihab.Web.Controllers
{
    public class AuthController(UserRepo repo) : Controller
    {
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid == false)
                return View(model);

            var result = repo.Authenticate(model.Email, model.Password);
            if (result.HasError || result.Data == null)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, result.Data.UserName),
                new Claim(ClaimTypes.Role, result.Data.Role),
                new Claim("Email", result.Data.Email),
                new Claim("UserId", result.Data.Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, "AsAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("AsAuth", principal);

            switch (result.Data.Role)
            {
                case "Student":
                    return RedirectToAction("Index", "OfferedCourse");
                default:
                    return RedirectToAction("Denied");

            }

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AsAuth");
            return RedirectToAction("Login");
        }

        public IActionResult Denied()
        {
            return View();
        }


        public IActionResult Registration()
        {
            return View(new User());
        }

        [HttpPost]
        public IActionResult Registration(User model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var result = repo.Save(model);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }
            else
            {
                if (result.Data != null) TempData["Success"] = $"Data#{result.Data.Id} saved successfully.";
                return RedirectToAction("Login");
            }
        }

    }
}
