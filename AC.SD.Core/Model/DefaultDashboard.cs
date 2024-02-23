using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using Microsoft.AspNetCore.DataProtection;

namespace AC.SD.Core.Model
{
    public class DefaultDashboard : DashboardController
    {
        public DefaultDashboard(DashboardConfigurator configurator, IDataProtectionProvider? dataProtectionProvider = null)
       : base(configurator, dataProtectionProvider)
        {
        }
    }
}
