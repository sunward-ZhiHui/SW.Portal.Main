using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query;
using Infrastructure.Repository.Query.Base;
using Infrastructure.Service.Config;
using Microsoft.Extensions.Configuration;
using NAV;
using System;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Services.Client;
using System.Linq;
using System.ServiceModel;

namespace Infrastructure.Service
{
    public class SalesOrderService : QueryRepository<PostSalesOrder>, ISalesOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly IPostSalesOrderQueryRepository _postSalesOrderQueryRepository;

        //public SalesOrderService(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}
        public SalesOrderService(IConfiguration configuration, IPostSalesOrderQueryRepository postSalesOrderQueryRepository) : base(configuration)
        {
            _configuration = configuration;
            _postSalesOrderQueryRepository = postSalesOrderQueryRepository;
        }
        public async Task<string> RawMatItemAsync(string company,long companyid,string type)
        {
            try
            {
                //int pageSize = 1000;
                //int page = 0;
                //while (true)
                //{
                var context = new NAVService(_configuration, company);
                var nquery = context.Context.RawMatItemList;
                DataServiceQuery<NAV.RawMatItemList> query = (DataServiceQuery<NAV.RawMatItemList>)nquery;

                TaskFactory<IEnumerable<NAV.RawMatItemList>> taskFactory = new TaskFactory<IEnumerable<NAV.RawMatItemList>>();
                IEnumerable<NAV.RawMatItemList> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));

                var prodCodes = result.ToList();

                var ItemBatchInfo = new List<Core.Entities.RawMatItemList>();

                prodCodes.ForEach(b =>
                {
                    ItemBatchInfo.Add(new Core.Entities.RawMatItemList
                    {
                        ItemNo = b.No,
                        Description = b.Description,
                        Description2 = b.Description_2,
                        Inventory = b.Inventory,
                        InternalRef = b.Internal_Ref,
                        ItemRegistration = b.Item_Registration,
                        BatchNos = b.Batch_Nos,
                        PSOItemNo = b.PSO_Item_No,
                        ProductionRecipeNo = b.Production_Recipe_No,
                        SafetyLeadTime = b.Safety_Lead_Time,
                        ProductionBOMNo = b.Production_BOM_No,
                        RoutingNo = b.Routing_No,
                        BaseUnitofMeasure = b.Base_Unit_of_Measure,
                        StandardCost = b.Standard_Cost,
                        UnitCost = b.Unit_Cost,
                        LastDirectCost = b.Last_Direct_Cost,
                        ItemCategoryCode = b.Item_Category_Code,
                        ProductGroupCode = b.Product_Group_Code,
                        CompanyId = companyid,
                        Type = type
                    });
                });

                var lsst = await _postSalesOrderQueryRepository.InsertRawMatItemList(ItemBatchInfo);

                return lsst;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> PackagingItemAsync(string company, long companyid, string type)
        {
            try
            {
                //int pageSize = 1000;
                //int page = 0;
                //while (true)
                //{
                var context = new NAVService(_configuration, company);
                var nquery = context.Context.PackagingItemList;
                DataServiceQuery<NAV.PackagingItemList> query = (DataServiceQuery<NAV.PackagingItemList>)nquery;

                TaskFactory<IEnumerable<NAV.PackagingItemList>> taskFactory = new TaskFactory<IEnumerable<NAV.PackagingItemList>>();
                IEnumerable<NAV.PackagingItemList> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));

                var prodCodes = result.ToList();

                var ItemBatchInfo = new List<Core.Entities.RawMatItemList>();

                prodCodes.ForEach(b =>
                {
                    ItemBatchInfo.Add(new Core.Entities.RawMatItemList
                    {
                        ItemNo = b.No,
                        Description = b.Description,
                        Description2 = b.Description_2,
                        Inventory = b.Inventory,
                        InternalRef = b.Internal_Ref,
                        BaseUnitofMeasure = b.Base_Unit_of_Measure,                       
                        ItemCategoryCode = b.Item_Category_Code,
                        CompanyId = companyid,
                        Type = type

                    });
                });
                var lsst = await _postSalesOrderQueryRepository.InsertRawMatItemList(ItemBatchInfo);

                return lsst;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> ProcessItemAsync(string company, long companyid, string type)
        {
            try
            {
                //int pageSize = 1000;
                //int page = 0;
                //while (true)
                //{
                var context = new NAVService(_configuration, company);
                var nquery = context.Context.ProcessItemList;
                DataServiceQuery<NAV.ProcessItemList> query = (DataServiceQuery<NAV.ProcessItemList>)nquery;

                TaskFactory<IEnumerable<NAV.ProcessItemList>> taskFactory = new TaskFactory<IEnumerable<NAV.ProcessItemList>>();
                IEnumerable<NAV.ProcessItemList> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));

                var prodCodes = result.ToList();

                var ItemBatchInfo = new List<Core.Entities.RawMatItemList>();

                prodCodes.ForEach(b =>
                {
                    ItemBatchInfo.Add(new Core.Entities.RawMatItemList
                    {
                        ItemNo = b.No,
                        Description = b.Description,
                        Description2 = b.Description_2,
                        Inventory = b.Inventory,
                        InternalRef = b.Internal_Ref,
                        ItemRegistration = b.Item_Registration,
                        BatchNos = b.Batch_Nos,
                        PSOItemNo = b.PSO_Item_No,
                        ProductionRecipeNo = b.Production_Recipe_No,
                        SafetyLeadTime = b.Safety_Lead_Time,
                        ProductionBOMNo = b.Production_BOM_No,
                        RoutingNo = b.Routing_No,
                        BaseUnitofMeasure = b.Base_Unit_of_Measure,
                        StandardCost = b.Standard_Cost,
                        UnitCost = b.Unit_Cost,
                        LastDirectCost = b.Last_Direct_Cost,
                        ItemCategoryCode = b.Item_Category_Code,
                        ProductGroupCode = b.Product_Group_Code,
                        CompanyId = companyid,
                        Type = type

                    });
                });
                var lsst = await _postSalesOrderQueryRepository.InsertRawMatItemList(ItemBatchInfo);

                return lsst;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Core.Entities.ItemBatchInfo>> SyncBatchAsync(string company, string itemNo)
        {
            try
            {
                //int pageSize = 1000;
                //int page = 0;
                //while (true)
                //{
                var context = new NAVService(_configuration, company);
                var nquery = context.Context.ItemBatchInfo.Where(f => f.Item_No == itemNo);
                DataServiceQuery<NAV.ItemBatchInfo> query = (DataServiceQuery<NAV.ItemBatchInfo>)nquery;

                TaskFactory<IEnumerable<NAV.ItemBatchInfo>> taskFactory = new TaskFactory<IEnumerable<NAV.ItemBatchInfo>>();
                IEnumerable<NAV.ItemBatchInfo> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));

                var prodCodes = result.ToList();

                var ItemBatchInfo = new List<Core.Entities.ItemBatchInfo>();

                prodCodes.ForEach(b =>
                {
                    ItemBatchInfo.Add(new Core.Entities.ItemBatchInfo
                    {
                        BatchNo = b.Batch_No,
                        BalanceQuantity = b.Remaining_Quantity,
                        ExpiryDate = b.Expiration_Date,
                        LocationCode = b.Location_Code,
                        ManufacturingDate = b.Manufacturing_Date,
                        QuantityOnHand = b.Remaining_Quantity,
                        NavQuantity = b.Remaining_Quantity,

                    });
                });
                return ItemBatchInfo;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
