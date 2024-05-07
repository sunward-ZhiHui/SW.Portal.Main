using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Reporting.Core.Services
{
    public class ReportingCustomConfigurationProvider
    {
        readonly IWebHostEnvironment hostingEnvironment;
        Dictionary<string, string> connectionStrings;
        string localConnectionName = "Report_Connection";
        public ReportingCustomConfigurationProvider(IWebHostEnvironment hostingEnvironment, IConfiguration config)
        {
            this.hostingEnvironment = hostingEnvironment;
            var dirPath = config.GetValue<string>("DataSourcesFolder");
            connectionStrings = new Dictionary<string, string>
            {

                [$"ConnectionStrings:Report_Connection"] = config.GetConnectionString(localConnectionName),
                [$"ConnectionStrings:JsonConnection"] = config.GetConnectionString("JsonConnection"),
                [$"ConnectionStrings:SunwardNAV01"] = config.GetConnectionString("SunwardNAV01"),
                [$"ConnectionStrings:SWUAT"] = config.GetConnectionString("SWUAT"),
                [$"ConnectionStrings:localhost_SW_Int_LIVE_Connection"] = config.GetConnectionString("localhost_SW_Int_LIVE_Connection"),
                //[$"ConnectionStrings:NWindConnectionString"] = $"XpoProvider=SQLite;Data Source={dirPath}/nwind.db",
                //[$"ConnectionStrings:VehiclesDBConnectionString"] = $"XpoProvider=SQLite;Data Source={dirPath}/vehicles.db",
                //[$"ConnectionStrings:HomesConnectionString"] = $"XpoProvider=SQLite;Data Source={dirPath}/homes.db",
                //[$"ConnectionStrings:ContactsConnectionString"] = $"XpoProvider=SQLite;Data Source={dirPath}/Contacts.db",
                //[$"ConnectionStrings:DevAvConnectionString"] = $"XpoProvider=SQLite;Data Source={dirPath}/devav.sqlite3",
                //[$"ConnectionStrings:CountriesConnectionString"] = $"XpoProvider=SQLite;Data Source={dirPath}/Countries.db"
            };
        }
        public IDictionary<string, string> GetGlobalConnectionStrings()
        {
            return new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddInMemoryCollection(connectionStrings)
                .AddEnvironmentVariables()
                .Build()
                .GetSection("ConnectionStrings")
                .AsEnumerable(true)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public IConfigurationSection GetReportDesignerWizardConfigurationSection()
        {
            return new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddInMemoryCollection(connectionStrings)
                .Build()
                .GetSection("ConnectionStrings");
        }
    }
}
