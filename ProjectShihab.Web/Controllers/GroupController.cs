using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Repos;

namespace ProjectShihab.Web.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class GroupController(GroupRepo repo) : Controller
    {
        public IActionResult Index()
        {
            var result = repo.GetAll();
            if (result.HasError)
                ViewBag.Error = result.Message;
            return View(result.Data ?? []);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Save(Group model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Title is required.";
                return RedirectToAction("Index");
            }

            var result = repo.Save(model);
            if (result.HasError)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = "Group saved.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var result = repo.Delete(id);
            if (result.HasError)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = "Group deleted.";
            return RedirectToAction("Index");
        }
    }
}
