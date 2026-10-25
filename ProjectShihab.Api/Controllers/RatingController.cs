using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Repos;

namespace ProjectShihab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController(RatingRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll(int? tutorId, int? studentId) => Ok(repo.GetAll(tutorId, studentId));
    }
}
