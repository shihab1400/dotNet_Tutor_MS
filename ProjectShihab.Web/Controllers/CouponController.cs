using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Repos;

namespace ProjectShihab.Web.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class CouponController(CouponRepo repo) : Controller
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
            var model = id == 0 ? new Coupon { ExpiryDate = DateTime.Today.AddMonths(1) } : repo.GetById(id).Data;
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(Coupon model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = repo.Save(model);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            TempData["Success"] = "Coupon saved.";
            return RedirectToAction("Index");
        }
    }
}
