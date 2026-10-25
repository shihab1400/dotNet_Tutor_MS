using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Repos;
using ProjectShihab.Web.Models;
using System.Diagnostics;

namespace ProjectShihab.Web.Controllers
{
    public class HomeController(DashboardRepo dashboardRepo) : Controller
    {
        // Public landing page: the only page a guest can see besides Login / Register.
        public IActionResult Index()
        {
            return View(dashboardRepo.GetPublic());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
