using DevExpress.DataAccess.Web;
using DevExpress.DataAccess.Wizard.Services;
using Microsoft.Extensions.Configuration;

namespace AC.SD.Core.ReportProvider
{
    public class ReportDataConnectionProviderFactory : IConnectionProviderFactory
    {
        private readonly IConfiguration _configuration;
        public ReportDataConnectionProviderFactory(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public IConnectionProviderService Create()
        {
            return new ReportDataConnectionProviderService(_configuration);
        }
    }

}
