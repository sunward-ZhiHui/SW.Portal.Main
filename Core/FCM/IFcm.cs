using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.FCM
{
    public interface IFcm
    {
        string ProjectID { get; set; }
        string Apikey { get; set; }
        string EndPointUrl { get; set; }
        Task<(bool IsSuccess, string Message)> SubscribeTopicsAsync(List<string> registationToken, string topic);
        Task<(bool IsSuccess, string Message)> UnSubscribeTopicsAsync(List<string> registationToken, string topic);
        Task<(bool IsSuccess, string Message)> SendMessageAsync(string validRegistationTokenOrTopic, string messageTitle, string messageData, string messageLink);
    }
}
