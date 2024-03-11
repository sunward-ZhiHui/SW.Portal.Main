using Microsoft.AspNetCore.Mvc;
using SW.Portal.Solutions.Services;
using System.Text.Json;
using WebPush;

namespace SW.Portal.Solutions.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : Controller
    {
        private static List<NotificationSubscription> _subscriptions = new();

        [HttpPost]
        [Route("subscribe")]
        public int Post(NotificationSubscription notificationSubscription)
        {
            _subscriptions.Add(notificationSubscription);
            return _subscriptions.Count();
        }       
    }
}
