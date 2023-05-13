using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        [HttpPost("[action]")]
        public ActionResult Upload(IFormFile myFile)
        {
            try
            {
                // Write code that saves the 'myFile' file.
                // Don't rely on or trust the FileName property without validation.
            }
            catch
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpGet]
        public string Get()
        {
            return "Test";
        }
    }
}
