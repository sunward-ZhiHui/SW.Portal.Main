using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace AC.SD.Core.Services
{
    public class EmailAutoRefresh
    {
        private readonly IConfiguration _config;
        private readonly IJSRuntime _jsRuntime;


        public event EventHandler<HelloEventArgs>? HelloChanged;
        private string? _hello;
        public string HelloMessage => _hello ?? "No Message Set";

        public EmailAutoRefresh(IConfiguration config, IJSRuntime jsRuntime)
        {
            _config = config;
            _jsRuntime = jsRuntime;
        }

        public void NotifyHelloChanged(string? hello)
        {
            _hello = hello;
            this.HelloChanged?.Invoke(this, new(hello));
        }
    }

    public class HelloEventArgs : EventArgs
    {
        public string? Hello { get; set; }

        public HelloEventArgs(string? hello)
            => Hello = hello;
    }
}
