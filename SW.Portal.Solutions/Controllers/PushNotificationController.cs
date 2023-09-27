using Core.FCM;
using Microsoft.AspNetCore.Mvc;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushNotificationController : Controller
    {
        private readonly IFcm _fcm;
        public PushNotificationController(IFcm fcm)
        {
            _fcm = fcm;
        }

        [HttpGet]
        public async Task<string> SendMessage()
        {
            var result = await _fcm.SendMessageAsync("/topics/news", "My Message Title", "Message Data", "");
            return result.Message;
        }
    }
}
