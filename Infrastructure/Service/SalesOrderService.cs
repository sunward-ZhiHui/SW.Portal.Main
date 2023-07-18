using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Infrastructure.Service.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.Services.Client;
using System.Linq;
using System.ServiceModel;

namespace Infrastructure.Service
{
    public class SalesOrderService : QueryRepository<PostSalesOrder>, ISalesOrderService
    {
        private readonly IConfiguration _configuration;

        //public SalesOrderService(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}
        public SalesOrderService(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task PostSalesOrderAsync(PostSalesOrder postSalesOrder)
        {
            try
            {
                var soLinequery = "select  * from view_SoSalesOrderLine where SoSalesOrderId =@SoSalesOrderId";

                var soLineparameters = new DynamicParameters();
                soLineparameters.Add("SoSalesOrderId", postSalesOrder.SoSalesOrderID, DbType.Int64);

                var soLine = new List<View_SoSalesOrderLine>();
                using (var connection = CreateConnection())
                {
                    soLine.AddRange((await connection.QueryAsync<View_SoSalesOrderLine>(soLinequery, soLineparameters)));
                }
                var itemGroupByCountry = soLine.GroupBy(g => g.NavCompanyName).ToList();
                foreach (var so in itemGroupByCountry)
                {
                    Random random = new Random();
                    int EntryNo = random.Next();
                    string? Company = so.Key;

                    var groupId = Guid.NewGuid();
                    var context = new NAVService(_configuration, Company);
                    //int EntryNo = (int)(sale?.SoSalesOrderId.GetValueOrDefault());
                    foreach (var sale in so.ToList())
                    {

                        var salesOrder = new NAV.SWDWebIntegrationEntry
                        {
                            Entry_No = EntryNo,
                            Entry_Type = "Create Sales",
                            Document_Type = 1,
                            Customer_No = sale.ShipCode,
                            Item_No = sale.No,
                            Unit_of_Measure_Code = sale.BaseUnitofMeasure,
                            Posting_Date = DateTime.Now,
                            Quantity = sale.Qty,
                            Group_ID = groupId
                        };
                        context.Context.AddToSWDWebIntegrationEntry(salesOrder);
                        TaskFactory<DataServiceResponse> taskFactory = new TaskFactory<DataServiceResponse>();
                        var response = await taskFactory.FromAsync(context.Context.BeginSaveChanges(null, null), iar => context.Context.EndSaveChanges(iar));

                    }
                    var post = new SWSoapService.SWDWebIntegration_PortClient();
                    post.Endpoint.Address =
               new EndpointAddress(new Uri(_configuration[Company + ":SoapUrl"] + "/" + _configuration[Company + ":Company"] + "/Codeunit/SWDWebIntegration"),
               new DnsEndpointIdentity(string.Empty));

                    post.ClientCredentials.UserName.UserName = _configuration[Company + ":UserName"];
                    post.ClientCredentials.UserName.Password = _configuration[Company + ":Password"];
                    post.ClientCredentials.Windows.ClientCredential.UserName = _configuration[Company + ":UserName"]; ;
                    post.ClientCredentials.Windows.ClientCredential.Password = _configuration[Company + ":Password"];
                    post.ClientCredentials.Windows.ClientCredential.Domain = _configuration[Company + ":Domain"];
                    post.ClientCredentials.Windows.AllowedImpersonationLevel =
                    System.Security.Principal.TokenImpersonationLevel.Impersonation;
                    var navSalesOrder = await post.FnCreateSalesOrderAsync(EntryNo);
                }
            }
            catch (Exception exp)
            {
                //var properties = exp.GetType()
                //            .GetProperties();
                //var fields = properties
                //                 .Select(property => new {
                //                     Name = property.Name,
                //                     Value = property.GetValue(exp, null)
                //                 })
                //                 .Select(x => String.Format(
                //                     "{0} = {1}",
                //                     x.Name,
                //                     x.Value != null ? x.Value.ToString() : String.Empty
                //                 ));
                //var error = String.Join("\n", fields);

                throw exp;
            }
        }
    }
}
