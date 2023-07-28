using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using Microsoft.Extensions.Configuration;

namespace AC.SD.Core.ReportProvider
{
    public class ReportDataConnectionProviderService : IConnectionProviderService
    {
        private readonly IConfiguration _configuration;
        public ReportDataConnectionProviderService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public SqlDataConnection LoadConnection(string connectionName)
        {
            string localConnectionName = "Report_Connection";
            return new SqlDataConnection
            {
                Name = connectionName,
                ConnectionString = _configuration.GetConnectionString(localConnectionName)
            };
        }
    }
}
