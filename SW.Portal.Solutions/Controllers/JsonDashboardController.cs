using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace SW.Portal.Solutions.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class JsonDashboardController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public JsonDashboardController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("list")]
        public IActionResult GetJsonDashboards()
        {
            string folderPath = Path.Combine(_env.WebRootPath, "data");
            var files = Directory.GetFiles(folderPath, "*.json");

            var dashboards = files.Select(f => new DashboardItem
            {
                Id = Path.GetFileNameWithoutExtension(f),
                DisplayName = Path.GetFileName(f)
            }).ToList();

            return Ok(dashboards);
        }

        public class DashboardItem
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
        }
    }

}
