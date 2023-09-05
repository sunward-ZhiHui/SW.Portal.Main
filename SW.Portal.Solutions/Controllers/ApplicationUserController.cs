using Core.Repositories.Query;
using Infrastructure.Repository.Query;
using Microsoft.AspNetCore.Mvc;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : Controller
    {
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;

        public ApplicationUserController(IApplicationUserQueryRepository applicationUserQueryRepository)
        {
            _applicationUserQueryRepository = applicationUserQueryRepository;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(string loginId, string password)
        {
            var response = await _applicationUserQueryRepository.LoginAuth(loginId, password);
            if (response != null)
                return Ok(response);
            else
                return NotFound("Invalid User");
        }

        [HttpGet]
        public string Get()
        {
            return "Test";
        }
    }
}
