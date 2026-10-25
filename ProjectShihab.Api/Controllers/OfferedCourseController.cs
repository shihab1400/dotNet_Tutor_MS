using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Repos;

namespace ProjectShihab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferedCourseController(OfferedCourseRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll(int? tutorId, bool onlyActive = false) => Ok(repo.GetAll(tutorId, onlyActive));

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id) => Ok(repo.GetById(id));

        // The repo rejects tutors that hold no certificate for the course.
        [HttpPost]
        public IActionResult Save(OfferedCourse model) => Ok(repo.Save(model, model.TutorId));
    }
}
