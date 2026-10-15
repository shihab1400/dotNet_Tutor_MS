using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectShihab.Data;
using ProjectShihab.Repos;

namespace ProjectShihab.Web.Controllers
{
    [Authorize]
    public class RecommendationController(RecommendationRepo recommendationRepo) : Controller
    {
        public IActionResult Index()
        {
            var result = recommendationRepo.GetAll();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
            }
            return View(result.Data);
        }
    }
}
