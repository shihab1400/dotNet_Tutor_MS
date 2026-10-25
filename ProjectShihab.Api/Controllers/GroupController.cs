using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Repos;

namespace ProjectShihab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController(GroupRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll() => Ok(repo.GetAll());

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id) => Ok(repo.GetById(id));

        [HttpPost]
        public IActionResult Save(Group model) => Ok(repo.Save(model));

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) => Ok(repo.Delete(id));
    }
}
