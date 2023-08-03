using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorldLibrary.Web.Repositories;

namespace WorldLibrary.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhysicalLibrariesController : ControllerBase
    {
        private readonly IPhysicalLibraryRepository _physicalLibraryRepository;

        public PhysicalLibrariesController(IPhysicalLibraryRepository physicalLibraryRepository)
        {
            _physicalLibraryRepository = physicalLibraryRepository;
        }
        [HttpGet]
        public IActionResult GetPhysicalLibraries()
        {
            return Ok(_physicalLibraryRepository.GetAll());
        }
    }
}
