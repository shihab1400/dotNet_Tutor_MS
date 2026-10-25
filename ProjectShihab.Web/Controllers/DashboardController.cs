using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Repos;
using ProjectShihab.Shared;

namespace ProjectShihab.Web.Controllers
{
    [Authorize]
    public class DashboardController(DashboardRepo repo, CurrentUserHelper currentUser) : Controller
    {
        public IActionResult Index()
        {
            return View(repo.Get(currentUser.UserId, currentUser.Role));
        }
    }
}
