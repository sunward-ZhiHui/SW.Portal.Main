using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace AC.SD.Core.Services
{
    public class FirebaseMessagingService
    {
        private readonly IConfiguration _config;
        private readonly IJSRuntime _jsRuntime;

        public FirebaseMessagingService(IConfiguration config, IJSRuntime jsRuntime)
        {
            _config = config;
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeFirebaseMessagingAsync()
        {
            await _jsRuntime.InvokeVoidAsync("firebase.initializeApp", _config["FirebaseConfig"]);
        }

        public async Task RequestPermissionForNotificationsAsync()
        {
            await _jsRuntime.InvokeVoidAsync("firebase.messaging().requestPermission");
        }
    }
}
