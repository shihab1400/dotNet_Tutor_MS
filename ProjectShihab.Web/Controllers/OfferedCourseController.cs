using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectShihab.Data;
using ProjectShihab.Repos;

namespace ProjectShihab.Web.Controllers
{
    [Authorize]
    public class OfferedCourseController(OfferedCourseRepo offeredCourseRepo) : Controller
    {
        public IActionResult Index()
        {
            var result = offeredCourseRepo.GetAll();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
            }
            return View(result.Data);
        }

        public IActionResult Apply(int dataId)
        {
            var result = offeredCourseRepo.GetById(dataId);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Payment", dataId);
        }
    }
}
