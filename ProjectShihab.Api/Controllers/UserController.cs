using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Repos;
using ProjectShihab.Shared;

namespace ProjectShihab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserRepo repo) : ControllerBase
    {
        // Registration and trainer creation live in the Web app; the API never returns password hashes.
        [HttpGet]
        public IActionResult GetAll(string? role)
        {
            var result = repo.GetAll(role);
            result.Data?.ForEach(u => u.PasswordHash = string.Empty);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            var result = repo.GetById(id);
            if (result.Data != null)
                result.Data.PasswordHash = string.Empty;
            return Ok(result);
        }

        [HttpPut("{id}/Status")]
        public IActionResult SetStatus(int id, bool status)
        {
            var result = repo.SetStatus(id, status);
            if (result.Data != null)
                result.Data.PasswordHash = string.Empty;
            return Ok(result);
        }
    }
}
