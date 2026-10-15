using ProjectShihab.Entities;
using ProjectShihab.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjectShihab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController(GroupRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = repo.GetAll();
            return Ok(result);
        }
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            var result = repo.GetById(id);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult Save(Group model)
        {
            var result = repo.Save(model);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = repo.Delete(id);
            return Ok(success);
        }
    }
}
