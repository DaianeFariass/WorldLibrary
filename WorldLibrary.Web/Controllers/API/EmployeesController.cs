using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        [HttpGet]
        public IActionResult GetEmployess()
        {
            return Ok(_employeeRepository.GetAll());
        }
    }
}
