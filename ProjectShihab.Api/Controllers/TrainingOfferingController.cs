using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Repos;

namespace ProjectShihab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingOfferingController(TrainingRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll(int? trainerId, bool onlyActive = false) => Ok(repo.GetOfferings(trainerId, onlyActive));

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id) => Ok(repo.GetOffering(id));

        [HttpPost]
        public IActionResult Save(TrainingOffering model) => Ok(repo.SaveOffering(model, model.TrainerId));
    }
}
