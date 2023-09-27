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

        [HttpGet]
        [Route("sendall")]
        public async Task<int> Get()
        {
            //Replace with your generated public/private key
            var publicKey = "BPA3UiGqKpwA8yJ9VkcEWcu7F0Up49US3P_EW91YyPz-smHe-D35nD0Ay9Cg7QtE_AHDQb-Gj479oTcRAG-R2-Q";
            var privateKey = "vrhLl8tfUjeYsrvZ6I0ekzGT-WkGnFoQ02jzNWDgJE8";

            //give a website URL or mailto:your-mail-id
            var vapidDetails = new VapidDetails("http://mkumaran.net", publicKey, privateKey);
            var webPushClient = new WebPushClient();

            foreach (var subscription in _subscriptions)
            {
                var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);

                try
                {
                    var payload = JsonSerializer.Serialize(new
                    {
                        message = "this text is from server",
                        url = "open this URL when user clicks on notification"
                    });
                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error sending push notification: " + ex.Message);
                    //return -1;
                }
            }

            return _subscriptions.Count();
        }
    }
}
