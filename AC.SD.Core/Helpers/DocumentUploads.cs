using DevExpress.XtraPrinting.Native;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;

namespace AC.SD.Core.Helpers
{
    public  class DocumentUploads
    {
        private readonly IConfiguration _appConfiguration;
        public DocumentUploads(IConfiguration configuration)
        {
            _appConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public string GetUploadUrl(string url)
        {
            var mc = new DocumentUploads(_appConfiguration);
            var configuredUrl = _appConfiguration.GetSection("DocumentsUrl")["BaseUrl"];
            var safeUrl = configuredUrl + url;
            mc.GetUploadUrl(safeUrl);
            return mc.ToString();
        }
    }
}
