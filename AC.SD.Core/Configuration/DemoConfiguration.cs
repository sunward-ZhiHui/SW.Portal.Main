using System;
using System.Collections.Generic;
using DevExpress.Blazor.DocumentMetadata;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace AC.SD.Core.Configuration
{
    public class DemoConfiguration
    {
        public const string DocBaseUrl = "https://docs.devexpress.com/Blazor/";
        public static readonly string PagesFolderName = "Pages";
        public static readonly string DescriptionsFolderName = "Descriptions";
        protected DemoConfiguration()
        {
        }
        public DemoConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;            
        }

        private IConfiguration Configuration { get; set; }       

        public bool IsServerSide =>
#if SERVER_BLAZOR
            true;
#else
            false;
#endif
        public virtual bool ShowOnlyReporting => false;
        public Dictionary<string, string> Redirects { get; private set; }

        public T GetConfigurationValue<T>(string key)
        {
            return Configuration.GetValue<T>(key);
        }

       
        
    }
}
