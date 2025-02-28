using Microsoft.AspNetCore.Mvc;
using SW.Portal.Solutions.Services;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : Controller
    {
        public ReminderController()
        {
            ReminderBackgroundService.OnReminderTriggered += HandleReminderTriggered;
        }

        private void HandleReminderTriggered(string title, string message)
        {
            // You can log the reminder or perform an action
            Console.WriteLine($"Reminder Triggered: {title} - {message}");
        }
    }
}
