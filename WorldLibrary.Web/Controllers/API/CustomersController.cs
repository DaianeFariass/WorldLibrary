using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        [HttpGet]
        public IActionResult GetBooks()
        {
            return Ok(_customerRepository.GetAll());
        }
    }
}
