using DocumentViewer.Helpers;
using DocumentViewer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Policy;
using Microsoft.AspNetCore.Http;

namespace DocumentViewer.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;
        public LoginController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index(string url)
        {
            HttpContext.Session.Remove("invalid");
            if (!string.IsNullOrEmpty(url))
            {
                HttpContext.Session.SetString("url", url);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(LoginModel loginModel)
        {
            HttpContext.Session.Remove("invalid");
            var url = HttpContext.Session.GetString("url");
            if (loginModel.Password != null)
            {
                var password = EncryptDecryptPassword.Encrypt(loginModel.Password);
                var users = _context.ApplicationUser.SingleOrDefault(w => w.LoginId == loginModel.UserName && w.LoginPassword == password);
                if (users != null)
                {
                    var employee = _context.Employee.SingleOrDefault(w => w.UserId == users.UserId);
                    if (employee != null)
                    {
                        var appMaster = _context.ApplicationMasterDetail.SingleOrDefault(w => w.ApplicationMasterDetailId == employee.AcceptanceStatus)?.Value;
                        var acceptanceStatus = appMaster != null ? appMaster.ToLower() : null;
                        if (acceptanceStatus != "resign" || employee.AcceptanceStatus == null)
                        {
                            HttpContext.Session.SetString("user_id", users.UserId.ToString());
                            HttpContext.Session.SetString("user_name", users.UserName?.ToString());
                            HttpContext.Session.SetString("session_id", users.SessionId?.ToString());
                            return Redirect("Home?url=" + url);
                        }
                        else
                        {
                            HttpContext.Session.SetString("invalid", "Invalid User");
                        }
                    }
                    else
                    {
                        HttpContext.Session.SetString("invalid", "Invalid User");
                    }
                }
                else
                {
                    HttpContext.Session.SetString("invalid", "Invalid User");
                }
            }
            return View();
        }
        [HttpGet("Logout")]
        public void Logout()
        {
            HttpContext.Session.Clear();
        }
    }
}
