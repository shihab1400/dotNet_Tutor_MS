using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectShihab.Entities;
using ProjectShihab.Repos;

namespace ProjectShihab.Web.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class CourseController(CourseRepo repo, GroupRepo groupRepo) : Controller
    {
        public IActionResult Index()
        {
            var result = repo.GetAll();
            if (result.HasError)
                ViewBag.Error = result.Message;
            return View(result.Data ?? []);
        }

        public IActionResult Edit(int id)
        {
            var model = id == 0 ? new Course() : repo.GetById(id).Data;
            if (model == null)
                return RedirectToAction("Index");
            FillGroups(model.GroupId);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(Course model)
        {
            ModelState.Remove(nameof(Course.Group));
            ModelState.Remove(nameof(Course.User));
            if (!ModelState.IsValid)
            {
                FillGroups(model.GroupId);
                return View(model);
            }

            var result = repo.Save(model);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                FillGroups(model.GroupId);
                return View(model);
            }

            TempData["Success"] = "Course saved.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var result = repo.Delete(id);
            if (result.HasError)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = "Course deleted.";
            return RedirectToAction("Index");
        }

        private void FillGroups(int selected)
        {
            ViewBag.Groups = new SelectList(groupRepo.GetAll().Data ?? [], "Id", "Title", selected);
        }
    }
}
