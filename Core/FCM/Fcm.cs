using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.FCM
{
    public class Fcm : IFcm
    {
        public string ProjectID { get; set; }
        public string Apikey { get; set; }
        public string EndPointUrl { get; set; } = "https://fcm.googleapis.com/fcm/send";
        public async Task<(bool IsSuccess, string Message)> SubscribeTopicsAsync(List<string> registationToken, string topic)
        {
            bool isSuccess = false;
            string message = string.Empty;

            using (var client = new HttpClient())
            {
                //https://medium.com/@selvaganesh93/firebase-cloud-messaging-important-rest-apis-be79260022b5
                client.BaseAddress = new Uri("https://iid.googleapis.com/iid/v1:batchAdd");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={Apikey}");

                var jsondata = @"{
                               ""to"": ""/topics/" + topic + @""",
                               ""registration_tokens"": [" + string.Join(",", registationToken.Select(o => "\"" + o + "\"")).TrimEnd(',') + @"],
                               }";

                var stringContent = new StringContent(jsondata, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    isSuccess = true;
                    message = await response.Content.ReadAsStringAsync();

                }
                else
                {
                    message = await response.Content.ReadAsStringAsync();
                }

                return (isSuccess, message);
            }
        }
        public async Task<(bool IsSuccess, string Message)> UnSubscribeTopicsAsync(List<string> registationToken, string topic)
        {
            bool isSuccess = false;
            string message = string.Empty;

            using (var client = new HttpClient())
            {
                //https://medium.com/@selvaganesh93/firebase-cloud-messaging-important-rest-apis-be79260022b5
                client.BaseAddress = new Uri("https://iid.googleapis.com/iid/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={Apikey}");

                var jsondata = @"{
                               ""to"": """ + topic + @""",
                               ""registration_tokens"": [" + string.Join(",", registationToken.Select(o => "\"" + o + "\"")).TrimEnd(',') + @"],
                               }";

                var stringContent = new StringContent(jsondata, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("v1:batchRemove", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    isSuccess = true;
                    message = await response.Content.ReadAsStringAsync();

                }
                else
                {
                    message = await response.Content.ReadAsStringAsync();
                }

                return (isSuccess, message);
            }
        }
        public async Task<(bool IsSuccess, string Message)> SendMessageAsync(string validRegistationTokenOrTopic, string messageTitle, string messageData, string messageLink)
        {
            bool isSuccess = false;
            string responsemessage = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EndPointUrl);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={Apikey}");


                string jsondata = @"{ ""notification"": {
                            ""title"": """ + messageTitle + @""",
                            ""body"": """ + messageData + @""",
                            " + (!string.IsNullOrEmpty(messageLink) ? ("\"click_action\" : \"" + messageLink + "\"") : "") + @"
                            },
                          ""to"" : """ + validRegistationTokenOrTopic + @"""
                        }";


                var stringContent = new StringContent(jsondata, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    isSuccess = true;
                    responsemessage = await response.Content.ReadAsStringAsync();

                }
                else
                {
                    responsemessage = await response.Content.ReadAsStringAsync();
                }

                return (isSuccess, responsemessage);
            }
        }

    }


}
