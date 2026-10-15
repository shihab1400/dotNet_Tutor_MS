using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectShihab.Data;
using ProjectShihab.Repos;

namespace ProjectShihab.Web.Controllers
{
    [Authorize]
    public class RatingController(RatingRepo ratingRepo) : Controller
    {
        public IActionResult Index()
        {
            var result = ratingRepo.GetAll();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
            }
            return View(result.Data);
        }
    }
}
