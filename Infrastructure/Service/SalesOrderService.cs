using Infrastructure.Service.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class SalesOrderService
    {
        private readonly IConfiguration _configuration;

        public SalesOrderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PostSalesOrderAsync(string salesOrderNo)
        {
            try
            {
                string Company = "";
                var context = new NAVService(_configuration, Company);
                int EntryNo = 1;
                var salesOrder = new NAV.SWDWebIntegrationEntry
                {
                    Entry_No = EntryNo,
                    Entry_Type = "Create Sales",
                    Document_Type = 1,

                };
                context.Context.AddToSWDWebIntegrationEntry(salesOrder);
                TaskFactory<DataServiceResponse> taskFactory = new TaskFactory<DataServiceResponse>();
                var response = await taskFactory.FromAsync(context.Context.BeginSaveChanges(null, null), iar => context.Context.EndSaveChanges(iar));


                var post = new SWSoapService.SWDWebIntegration_PortClient();

                post.Endpoint.Address =
           new EndpointAddress(new Uri(_configuration[Company + ":SoapUrl"] + "/" + _configuration[Company + ":Company"] + "/Codeunit/SWDWebIntegration"),
           new DnsEndpointIdentity(""));

                post.ClientCredentials.UserName.UserName = _configuration[Company + ":UserName"];
                post.ClientCredentials.UserName.Password = _configuration[Company + ":Password"];
                post.ClientCredentials.Windows.ClientCredential.UserName = _configuration[Company + ":UserName"]; ;
                post.ClientCredentials.Windows.ClientCredential.Password = _configuration[Company + ":Password"];
                post.ClientCredentials.Windows.ClientCredential.Domain = _configuration[Company + ":Domain"];
                post.ClientCredentials.Windows.AllowedImpersonationLevel =
                System.Security.Principal.TokenImpersonationLevel.Impersonation;                
                var result = await post.FnCreateSalesOrderAsync(EntryNo);

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
