using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservesController : ControllerBase
    {
        private readonly IReserveRepository _reserveRepository;

        public ReservesController(IReserveRepository reserveRepository)
        {
            _reserveRepository = reserveRepository;
        }
        [HttpGet]
        public IActionResult GetReserves()
        {
            return Ok(_reserveRepository.GetAll());
        }
    }
}
