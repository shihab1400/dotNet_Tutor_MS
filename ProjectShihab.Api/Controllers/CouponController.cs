using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Repos;

namespace ProjectShihab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController(CouponRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll() => Ok(repo.GetAll());

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id) => Ok(repo.GetById(id));

        [HttpPost]
        public IActionResult Save(Coupon model) => Ok(repo.Save(model));

        // Preview the discounted price for an amount without consuming the coupon.
        [HttpGet("Quote")]
        public IActionResult Quote(decimal amount, string? code) => Ok(repo.Quote(amount, code));
    }
}
