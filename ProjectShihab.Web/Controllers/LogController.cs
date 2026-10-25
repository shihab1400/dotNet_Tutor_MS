using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Models;
using ProjectShihab.Repos;

namespace ProjectShihab.Web.Controllers
{
    // Only the admin can see the audit log.
    [Authorize(Roles = Roles.Admin)]
    public class LogController(LogRepo repo) : Controller
    {
        public IActionResult Index(LogFilterModel filter)
        {
            var result = repo.Search(filter.Search, filter.Role, filter.From, filter.To, filter.Page, out var totalPages);
            if (result.HasError)
                ViewBag.Error = result.Message;
            filter.TotalPages = totalPages;
            filter.Page = Math.Clamp(filter.Page, 1, totalPages);
            ViewBag.Filter = filter;
            return View(result.Data ?? []);
        }
    }
}
