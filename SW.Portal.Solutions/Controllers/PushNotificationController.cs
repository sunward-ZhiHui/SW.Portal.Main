using Core.FCM;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Plugin.Firebase.CloudMessaging;
using static DevExpress.XtraPivotGrid.Data.FieldValueItemsGenerator;
using System.Text;
using SW.Portal.Solutions.Models;

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
            //var result = await _fcm.SendMessageAsync("/topics/news", "My Message Title", "Message Data", "");

            //await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            //var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

            var token = "dc1Aen-PR5O1CcrX5if_vT:APA91bGPs_gdFgcUzrp1yImhaPchliNYHgEY4awBlkb_w3j177gYKYIgGF_d3BZTQHzHwCkD-othFiMixLLJtXw24cT9aBNy_80vPb-i08fgKwlOeZ0CyvzTeGzTQdxCpf3a7OW8kzdI";

            var androidNotificationObject = new Dictionary<string, string>();
            var pushNotificationRequest = new PostItem
            {
                notification = new NotificationMessageBody
                {
                    title = "Title",
                    body = "Welcome to all"
                },
                data = androidNotificationObject,
                registration_ids = new List<string> { token }
            };

            string url = "https://fcm.googleapis.com/fcm/send";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("key", "=" + "AAAAeo31_Is:APA91bFPh3rj_ZrmfurBTfz_Ahw_Ojo9rA4oNIFoaNThAHUhwtq515F19qD9ICngHp5qs1IBQ1ZePalvD8YOzCKF-va991eN02_TEZtgAE4AWM5hku9rDdQoEZvT47l3mE67LcGpKMuz");

                string serializeRequest = JsonConvert.SerializeObject(pushNotificationRequest);
                var response = await client.PostAsync(url, new StringContent(serializeRequest, Encoding.UTF8, "application/json"));
                //if (response.StatusCode == System.Net.HttpStatusCode.OK)
                //{
                //    //await App.Current.MainPage.DisplayAlert("Notification sent", "notification sent", "OK");
                //}
            }
            return "ok";
        }
    }
}
