using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.Repository.Query;
using Infrastructure.Repository.Query.Base;
using Infrastructure.Service.Config;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using NAV;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Services.Client;
using System.Linq;
using System.ServiceModel;
using static iTextSharp.text.pdf.AcroFields;
using static Org.BouncyCastle.Math.EC.ECCurve;

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
        public async Task<string> RawMatItemAsync(string company, long companyid, string type)
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
                        //BalanceQuantity = b.Remaining_Quantity,
                        ExpiryDate = b.g_dteExpDate,
                        LocationCode = b.Location_Code,
                        ManufacturingDate = b.g_dteMfgDate,
                        // QuantityOnHand = b.Remaining_Quantity,
                        // NavQuantity = b.Remaining_Quantity,

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

        public async Task<List<Core.Entities.ItemBatchInfo>> NavItemBatchAsync(string company)
        {
            List<Core.Entities.ItemBatchInfo> ItemBatchInfo = new List<Core.Entities.ItemBatchInfo>();
            try
            {

                int pageSize = 1000;
                int page = 0;
                while (true)
                {
                    var context = new NAVService(_configuration, company);
                    var nquery = context.Context.ItemBatchInfo.Skip(page * pageSize).Take(pageSize);
                    DataServiceQuery<NAV.ItemBatchInfo> query = (DataServiceQuery<NAV.ItemBatchInfo>)nquery;

                    TaskFactory<IEnumerable<NAV.ItemBatchInfo>> taskFactory = new TaskFactory<IEnumerable<NAV.ItemBatchInfo>>();
                    IEnumerable<NAV.ItemBatchInfo> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));

                    var prodCodes = result.ToList();
                    prodCodes.ForEach(b =>
                    {
                        var exits = ItemBatchInfo.Where(f => f.BatchNo == b.Batch_No && f.LocationCode == b.Location_Code).Count();
                        if (exits == 0)
                        {
                            ItemBatchInfo.Add(new Core.Entities.ItemBatchInfo
                            {
                                ItemNo = b.Item_No,
                                ItemDescription = b.Description,
                                BatchNo = b.Batch_No,
                                // BalanceQuantity = b.Remaining_Quantity,
                                ExpiryDate = b.g_dteExpDate,
                                LocationCode = b.Location_Code,
                                ManufacturingDate = b.g_dteMfgDate,
                                // QuantityOnHand = b.Remaining_Quantity,
                                //  NavQuantity = b.Remaining_Quantity,

                            });
                        }
                    });
                    if (prodCodes.Count < 1000)
                        break;
                    page++;
                }
                return ItemBatchInfo;
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
        public async Task<List<Navitems>> GetNavItemsAdd(ViewPlants company)
        {
            List<Navitems> navItems = new List<Navitems>();
            if (company == null)
                return null;
            var context = new NAVService(_configuration, company.NavCompanyName);


            int pageSize = 1000;
            int page = 0;
            while (true)
            {
                var nquery = context.Context.ItemList.Skip(page * pageSize).Take(pageSize);
                DataServiceQuery<ItemList> query = (DataServiceQuery<ItemList>)nquery;

                TaskFactory<IEnumerable<ItemList>> taskFactory = new TaskFactory<IEnumerable<ItemList>>();
                IEnumerable<ItemList> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));

                var itemList = result.ToList();
                itemList.ForEach(item =>
                {
                    navItems.Add(new Navitems
                    {
                        No = item.No,
                        RelatedItemNo = item.Related_Item_No,
                        Description = item.Description,
                        Description2 = item.Description_2,
                        ItemType = item.Type,
                        StatusCodeId = item.Blocked.GetValueOrDefault(false) ? 2 : 1,
                        Inventory = item.Inventory,
                        InternalRef = item.Internal_Ref,
                        ItemRegistration = item.Item_Registration,
                        ExpirationCalculation = item.Expiration_Calculation,
                        BatchNos = item.Batch_Nos,
                        ProductionRecipeNo = item.Production_Recipe_No,
                        Qcenabled = item.QC_Enabled,
                        //SafetyLeadTime = item.Safety_Lead_Time,
                        ProductionBomno = item.Production_BOM_No,
                        RoutingNo = item.Routing_No,
                        BaseUnitofMeasure = item.Base_Unit_of_Measure,
                        UnitCost = item.Unit_Cost,
                        UnitPrice = item.Unit_Price,
                        VendorNo = item.Replenishment_System,
                        //VendorItemNo = item.Vendor_Item_No,
                        ItemCategoryCode = item.Item_Category_Code,
                        ItemTrackingCode = item.Item_Tracking_Code,
                        Qclocation = item.QC_Location,
                        //LastSyncDate = item.Last_Date_Modified,
                        PurchaseUom = item.Purch_Unit_of_Measure,
                        ShelfLife = item.Expiration_Calculation,
                    });
                });
                if (itemList.Count < 1000)
                    break;
                page++;
            }
            return navItems;
        }
        private int GetWeekNumberOfMonth(DateTime date)
        {
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            int firstDay = (int)firstDayOfMonth.DayOfWeek;
            if (firstDay == 0)
            {
                firstDay = 7;
            }
            double d = (firstDay + date.Day - 1) / 7.0;
            return d > 5 ? (int)Math.Floor(d) : (int)Math.Ceiling(d);
        }
        public async Task<string> GetNAVStockBalance(StockBalanceSearch searchModel)
        {
            try
            {
                var companyListData = await _postSalesOrderQueryRepository.GetSotckBalanceItemsListAsync(searchModel);
                string query = string.Empty;
                query += UpdateSotckBalance(searchModel, companyListData);
                query += UpdateKIVQty(searchModel, companyListData);
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception("Stock balance update failed with some unexpected errors!", ex);
            }
        }
        public string UpdateSotckBalance(StockBalanceSearch searchModel, SotckBalanceItemsList companyListData)
        {
            string query = string.Empty;
            var companyList = companyListData.PlantData.ToList();
            if (companyList != null && companyList.Count() > 0)
            {
                var company = companyList.FirstOrDefault(f => f.PlantID == searchModel.CompanyId.GetValueOrDefault(0));
                if (company != null)
                {
                    var Company = company.NavCompanyName;

                    var context = new NAVService(_configuration, Company);

                    var post = new SWWebIntegration.WebIntegration_PortClient();

                    post.Endpoint.Address = new EndpointAddress(new Uri(_configuration[Company + ":SoapUrl"] + "/" + _configuration[Company + ":Company"] + "/Codeunit/WebIntegration"), new DnsEndpointIdentity(""));

                    post.ClientCredentials.UserName.UserName = _configuration[Company + ":UserName"];
                    post.ClientCredentials.UserName.Password = _configuration[Company + ":Password"];
                    post.ClientCredentials.Windows.ClientCredential.UserName = _configuration[Company + ":UserName"]; ;
                    post.ClientCredentials.Windows.ClientCredential.Password = _configuration[Company + ":Password"];
                    post.ClientCredentials.Windows.ClientCredential.Domain = _configuration[Company + ":Domain"];
                    post.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

                    var inventoryItems = companyListData.NavitemsData;
                    //var firstDayCurrentMonth = new DateTime(searchModel.StkMonth.Year, searchModel.StkMonth.Month, 1);
                    //var lastDayLastMonth = firstDayCurrentMonth.AddDays(-1);

                    var month = searchModel.StkMonth.Month;// lastDayLastMonth.Month;
                    var year = searchModel.StkMonth.Year;// lastDayLastMonth.Year;

                    var weekofMonth = GetWeekNumberOfMonth(searchModel.StkMonth);
                    int count = 0;
                    int totalItems = inventoryItems.Count();
                    List<NavitemStockBalance> navitemStockBalances = new List<NavitemStockBalance>();

                    if (totalItems > 0)
                    {
                        inventoryItems.ForEach(f =>
                        {
                            var stockdate = searchModel.StkMonth;

                            var itemCount = count + " of " + totalItems;
                            //notify client progress update
                            var itemName = string.Format("{0} {1}-{2}  {3}", itemCount, f.No, f.Description, "from NAV Stock balance");

                            var stockBalance = post.FnCalculateInventoryAsync(f.No, stockdate).Result.return_value;
                            var fmqty = post.FnCalculateFMInventoryAsync(f.No, stockdate).Result.return_value;
                            var reqty = post.FnCalculateRWInventoryAsync(f.No, stockdate).Result.return_value;
                            var wipqty = post.FnCalculateWIPInventoryAsync(f.No, stockdate).Result.return_value;
                            var notStartqty = post.FnCalcNotStartInventoryAsync(f.No, stockdate).Result.return_value;

                            var kivQty = 0;

                            var stockNav = companyListData.NavitemStockBalance.FirstOrDefault(d => d.ItemId == f.ItemId && d.StockBalMonth.Value.Month == month && d.StockBalMonth.Value.Year == year && d.StockBalWeek == weekofMonth);
                            if (stockNav == null)
                            {
                                var stockdates = stockdate.ToString("yyyy-mm-dd");
                                var AddedByUserId = 1; var StatusCodeId = 1; var RejectQuantity = 0; var Supply1ProcessQty = 0; var SupplyWipqty = 0;
                                query += "INSERT INTO NavitemStockBalance(ItemId,AddedByUserId,AddedDate,StatusCodeId,Quantity,GlobalQty,ReworkQty,Wipqty,Kivqty,RejectQuantity,StockBalWeek,StockBalMonth,Supply1ProcessQty,SupplyWipqty,NotStartInvQty)VALUES " +
                                "(" + f.ItemId + "," + AddedByUserId + ",'GetDate()'," + StatusCodeId + "," + (stockBalance > 0 ? stockBalance : 0) + "," + (fmqty > 0 ? fmqty : 0) + "," + (reqty > 0 ? reqty : 0) + "," + (kivQty > 0 ? kivQty : 0) + "," + RejectQuantity + "," + weekofMonth + ",'" + stockdates + "'," + Supply1ProcessQty + "," + SupplyWipqty + "," + (notStartqty > 0 ? notStartqty : 0) + ");\n\r";
                            }
                            else
                            {
                                query += "Update  NavitemStockBalance SET Quantity=" + (stockBalance > 0 ? stockBalance : 0) + ",StockBalWeek=" + weekofMonth + ",Wipqty=" + (wipqty > 0 ? wipqty : 0) + ",ReworkQty=" + (reqty > 0 ? reqty : 0) + ",GlobalQty=" + (fmqty > 0 ? fmqty : 0) + ",Kivqty=" + (kivQty > 0 ? kivQty : 0) + ",NotStartInvQty=" + (notStartqty > 0 ? notStartqty : 0) + " WHERE ID =" + stockNav.NavStockBalanceId + ";\n\r";
                            }
                            count++;
                        });
                    }
                }
            }
            return query;
        }
        public string UpdateKIVQty(StockBalanceSearch searchModel, SotckBalanceItemsList companyListData)
        {
            try
            {
                string querys = string.Empty;
                var companyList = companyListData.PlantData.ToList();
                if (companyList != null && companyList.Count() > 0)
                {
                    var company = companyList.FirstOrDefault(f => f.PlantID == searchModel.CompanyId.GetValueOrDefault(0));
                    if (company != null)
                    {
                        var Company = company.NavCompanyName;

                        var custMasts = companyListData.Navcustomer;
                        var inventoryItems = companyListData.NavitemsData;

                        if (inventoryItems != null && inventoryItems.Count() > 0)
                        {
                            inventoryItems.ForEach(f =>
                            {
                                var context = new NAVService(_configuration, Company);

                                var nquery = context.Context.SalesOrderLineKIV.Where(cs => cs.No == f.No);
                                DataServiceQuery<SalesOrderLineKIV> query = (DataServiceQuery<SalesOrderLineKIV>)nquery;
                                TaskFactory<IEnumerable<SalesOrderLineKIV>> taskFactory = new TaskFactory<IEnumerable<SalesOrderLineKIV>>();
                                IEnumerable<SalesOrderLineKIV> salesResults = taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar)).GetAwaiter().GetResult();
                                var salesList = salesResults.ToList();

                                var month = searchModel.StkMonth.Month;// lastDayLastMonth.Month;
                                var year = searchModel.StkMonth.Year;// lastDayLastMonth.Year;
                                var stockdate = searchModel.StkMonth;
                                var weekofMonth = GetWeekNumberOfMonth(searchModel.StkMonth);


                                salesList.ForEach(c =>
                                {
                                    var itemMas = f;
                                    var custMas = custMasts.FirstOrDefault(i => i.Code == c.Sell_to_Customer_No);
                                    var stockNav = companyListData.DistStockBalanceKiv.FirstOrDefault(d => d.ItemNo == f.No && d.StockBalMonth.Value.Month == month && d.StockBalMonth.Value.Year == year && d.StockBalWeek == weekofMonth && d.CustomerId == custMas.CustomerId);
                                    if (stockNav == null)
                                    {
                                        var stockdates = stockdate.ToString("yyyy-mm-dd");
                                        querys += "INSERT INTO DistStockBalanceKiv(ItemId,ItemNo,CompanyId,CustomerId,Quantity,CustomerNo,StockBalWeek,StockBalMonth)VALUES " +
                               "(" + itemMas?.ItemId + "," + c.No + "," + custMas?.CustomerId + "" + (c.Outstanding_Quantity > 0 ? c.Outstanding_Quantity : 0) + "," + (c.Sell_to_Customer_Name) + "," + weekofMonth + ",'" + stockdates + "');\n\r";

                                    }
                                    else
                                    {

                                        var Quantity = stockNav.Quantity + c.Outstanding_Quantity;
                                        querys += "Update  DynamicForm SET Quantity=" + Quantity + " WHERE ID =" + stockNav.DistKivId + ";\n\r";
                                    }
                                });
                            });
                        }

                    }
                }
                return querys;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Core.Entities.FinishedProdOrderLine>> FinishedProdOrderLineAsync(string company, long companyid, List<Core.Entities.FinishedProdOrderLine> finishedProdOrderLines, List<Navitems> navitems)
        {
            List<Core.Entities.FinishedProdOrderLine> finishedProdOrderLine = new List<Core.Entities.FinishedProdOrderLine>();
            try
            {

                int pageSize = 1000;
                int page = 0;
                while (true)
                {
                    var context = new NAVService(_configuration, company);
                    var nquery = context.Context.FinishedProdOrderLine.Skip(page * pageSize).Take(pageSize);
                    DataServiceQuery<NAV.FinishedProdOrderLine> query = (DataServiceQuery<NAV.FinishedProdOrderLine>)nquery;

                    TaskFactory<IEnumerable<NAV.FinishedProdOrderLine>> taskFactory = new TaskFactory<IEnumerable<NAV.FinishedProdOrderLine>>();
                    IEnumerable<NAV.FinishedProdOrderLine> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));

                    var prodCodes = result.ToList();
                    prodCodes.ForEach(b =>
                    {
                        if (b.Line_No > 0)
                        {
                            if (!string.IsNullOrEmpty(b.Item_No))
                            {
                                var exitsData = finishedProdOrderLine.Where(p => p.ItemNo == b.Item_No && p.ReplanRefNo == b.Replan_Ref_No && p.ProdOrderNo == b.Prod_Order_No).Count();
                                if (exitsData == 0)
                                {
                                    var itemsExits = navitems.FirstOrDefault(f => f.No.Trim().ToLower() == b.Item_No.Trim().ToLower());
                                    var exist = finishedProdOrderLines.Where(p => p.ItemNo == b.Item_No && p.ReplanRefNo == b.Replan_Ref_No && p.CompanyId == companyid && p.ProdOrderNo == b.Prod_Order_No).FirstOrDefault();
                                    if (exist == null)
                                    {
                                        finishedProdOrderLine.Add(new Core.Entities.FinishedProdOrderLine
                                        {
                                            ItemNo = b.Item_No,
                                            Status = b.Status,
                                            ProdOrderNo = b.Prod_Order_No,
                                            OrderLineNo = b.Line_No,
                                            Description = b.Description,
                                            Description2 = b.Description_2,
                                            ReplanRefNo = b.Replan_Ref_No,
                                            StartingDate = b.Starting_Date == DateTime.MinValue ? null : b.Starting_Date,
                                            BatchNo = b.Batch_No,
                                            ManufacturingDate = b.Manufacturing_Date == DateTime.MinValue ? null : b.Manufacturing_Date,
                                            ExpirationDate = b.Expiration_Date == DateTime.MinValue ? null : b.Expiration_Date,
                                            ProductCode = b.Product_Code,
                                            ProductName = b.Product_Name,
                                            CompanyId = companyid,
                                            OptStatus = b.g_optStatus,
                                            ItemId = itemsExits?.ItemId,
                                        });
                                    }
                                    else
                                    {
                                        finishedProdOrderLine.Add(new Core.Entities.FinishedProdOrderLine
                                        {
                                            FinishedProdOrderLineId = exist.FinishedProdOrderLineId,
                                            ItemNo = b.Item_No,
                                            Status = b.Status,
                                            ProdOrderNo = b.Prod_Order_No,
                                            OrderLineNo = b.Line_No,
                                            Description = b.Description,
                                            Description2 = b.Description_2,
                                            ReplanRefNo = b.Replan_Ref_No,
                                            StartingDate = b.Starting_Date == DateTime.MinValue ? null : b.Starting_Date,
                                            BatchNo = b.Batch_No,
                                            ManufacturingDate = b.Manufacturing_Date == DateTime.MinValue ? null : b.Manufacturing_Date,
                                            ExpirationDate = b.Expiration_Date == DateTime.MinValue ? null : b.Expiration_Date,
                                            ProductCode = b.Product_Code,
                                            ProductName = b.Product_Name,
                                            CompanyId = companyid,
                                            OptStatus = b.g_optStatus,
                                            ItemId = itemsExits?.ItemId,
                                        });
                                    }
                                }
                            }
                        }
                    });
                    if (prodCodes.Count < 1000)
                        break;
                    page++;
                }
                return finishedProdOrderLine;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<NavprodOrderLine>> GetNAVProdOrderLine(string company, long companyid, List<NavprodOrderLine> productionLinelist)
        {

            List<NavprodOrderLine> prodNotStartList = new List<NavprodOrderLine>();
            var fromMonth = DateTime.Today;
            var year = fromMonth.Year - 1;
            var tomonth = DateTime.Today.AddMonths(6);
            var context = new NAVService(_configuration, company);
            int pageSize = 1000;
            int page = 0;

            while (true)
            {
                var nquery = context.Context.ProdOrderLineList.Where(w => w.Status == "Released").Skip(page * pageSize).Take(pageSize);
                DataServiceQuery<NAV.ProdOrderLineList> query = (DataServiceQuery<NAV.ProdOrderLineList>)nquery;

                TaskFactory<IEnumerable<NAV.ProdOrderLineList>> taskFactory = new TaskFactory<IEnumerable<NAV.ProdOrderLineList>>();
                IEnumerable<NAV.ProdOrderLineList> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));

                var prodCodes = result.ToList();
                prodCodes.ForEach(f =>
                {
                    if (f.Line_No > 0)
                    {
                        string refNo = String.Empty;
                        if (!string.IsNullOrEmpty(f.Prod_Order_No))
                        {
                            var refPlanNo = f.Prod_Order_No.Split("-");
                            if (refPlanNo.Length == 1)
                            {
                                refNo = refPlanNo[0];
                            }
                            else
                            {
                                refNo = refPlanNo[0] + "-" + refPlanNo[1];
                            }
                            if (!string.IsNullOrEmpty(refNo))
                            {
                                var exitsData = prodNotStartList.Where(s => s.ItemNo == f.Item_No && s.RePlanRefNo == refNo && s.ProdOrderNo == f.Prod_Order_No).Count();
                                if (exitsData == 0)
                                {
                                    var exist = productionLinelist.Where(p => p.ItemNo == f.Item_No && p.RePlanRefNo == refNo && p.CompanyId == companyid && p.ProdOrderNo == f.Prod_Order_No).FirstOrDefault();
                                    if (exist == null)
                                    {
                                        var prodNotStart = new NavprodOrderLine
                                        {
                                            RePlanRefNo = refNo,
                                            CompanyId = companyid,
                                            ProdOrderNo = f.Prod_Order_No,
                                            OrderLineNo = f.Line_No,
                                            ItemNo = f.Item_No,
                                            Description = f.Description,
                                            Description1 = f.Description_2,
                                            CompletionDate = f.Completion_Date == DateTime.MinValue ? null : f.Completion_Date,
                                            RemainingQuantity = f.Remaining_Quantity,
                                            BatchNo = f.Batch_No,
                                            Status = f.Status,
                                            OutputQty = f.Finished_Quantity,
                                            StartDate = f.Starting_Date == DateTime.MinValue ? null : f.Starting_Date,
                                            LastSyncDate = DateTime.Now,
                                        };

                                        prodNotStartList.Add(prodNotStart);
                                    }
                                }
                            }
                        }
                    }
                });
                if (prodCodes.Count < 1000)
                    break;
                page++;
            }

            return prodNotStartList;
        }


        public async Task<List<NavprodOrderLine>> GetFinishedProdOrderLineToNAVProdOrderLine(string company, long companyid, List<NavprodOrderLine> productionLinelist)
        {

            List<NavprodOrderLine> prodNotStartList = new List<NavprodOrderLine>();
            var fromMonth = DateTime.Today;
            var year = fromMonth.Year - 1;
            var tomonth = DateTime.Today.AddMonths(6);
            var context = new NAVService(_configuration, company);
            int pageSize = 1000;
            int page = 0;

            while (true)
            {
                var nquery = context.Context.FinishedProdOrderLine.Where(w => w.g_optStatus == "Approved" || w.g_optStatus == "").Skip(page * pageSize).Take(pageSize);
                DataServiceQuery<NAV.FinishedProdOrderLine> query = (DataServiceQuery<NAV.FinishedProdOrderLine>)nquery;

                TaskFactory<IEnumerable<NAV.FinishedProdOrderLine>> taskFactory = new TaskFactory<IEnumerable<NAV.FinishedProdOrderLine>>();
                IEnumerable<NAV.FinishedProdOrderLine> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));

                var prodCodes = result.ToList();
                prodCodes.ForEach(f =>
                {
                    if (f.Line_No > 0)
                    {
                        string refNo = f.Replan_Ref_No;
                        if (!string.IsNullOrEmpty(refNo))
                        {
                            var exitsData = prodNotStartList.Where(s => s.ItemNo == f.Item_No && s.RePlanRefNo == refNo && s.ProdOrderNo == f.Prod_Order_No).Count();
                            if (exitsData == 0)
                            {
                                var exist = productionLinelist.Where(p => p.ItemNo == f.Item_No && p.RePlanRefNo == refNo && p.CompanyId == companyid && p.ProdOrderNo == f.Prod_Order_No).FirstOrDefault();
                                if (exist == null)
                                {
                                    var prodNotStart = new NavprodOrderLine
                                    {
                                        RePlanRefNo = refNo,
                                        CompanyId = companyid,
                                        ProdOrderNo = f.Prod_Order_No,
                                        OrderLineNo = f.Line_No,
                                        ItemNo = f.Item_No,
                                        Description = f.Description,
                                        Description1 = f.Description_2,
                                        //  CompletionDate = f.Completion_Date == DateTime.MinValue ? null : f.Completion_Date,
                                        //  RemainingQuantity = f.Remaining_Quantity,
                                        BatchNo = f.Batch_No,
                                        Status = "Released",
                                        //  OutputQty = f.Finished_Quantity,
                                        StartDate = f.Starting_Date == DateTime.MinValue ? null : f.Starting_Date,
                                        LastSyncDate = DateTime.Now,
                                    };

                                    prodNotStartList.Add(prodNotStart);
                                }
                            }
                        }

                    }
                });
                if (prodCodes.Count < 1000)
                    break;
                page++;
            }

            return prodNotStartList;
        }

        public async Task<List<SoCustomer>> NavVendorAsync(string company, long companyid, List<SoCustomer> navvendors)
        {
            try
            {
                List<SoCustomer> navVendorList = new List<SoCustomer>();
                int pageSize = 1000;
                int page = 0;
                while (true)
                {
                    var context = new NAVService(_configuration, company);
                    var nquery = context.Context.Vendor.Skip(page * pageSize).Take(pageSize);
                    DataServiceQuery<NAV.Vendor> query = (DataServiceQuery<NAV.Vendor>)nquery;

                    TaskFactory<IEnumerable<NAV.Vendor>> taskFactory = new TaskFactory<IEnumerable<NAV.Vendor>>();
                    IEnumerable<NAV.Vendor> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));

                    var prodCodes = result.ToList();
                    prodCodes.ForEach(b =>
                    {
                        var exitsData = navVendorList.Where(f => f.ShipCode == b.No && f.CompanyId == companyid).Count();
                        if (exitsData == 0)
                        {
                            var exits = navvendors.Where(f => f.ShipCode == b.No && f.CompanyId == companyid).FirstOrDefault();
                            if (exits == null)
                            {
                                navVendorList.Add(new SoCustomer
                                {
                                    ShipCode = b.No,
                                    CustomerName = b.Name,
                                    Address1 = b.Address,
                                    Address2 = b.Address_2,
                                    PostCode = b.Post_Code,
                                    City = b.City,
                                    StateCode = b.Location_Code,
                                    CompanyId = companyid,
                                    Type = "Vendor"
                                });
                            }
                            else
                            {
                                navVendorList.Add(exits);
                            }
                        }
                    });
                    if (prodCodes.Count < 1000)
                        break;
                    page++;
                }
                return navVendorList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<Core.Entities.RawMatPurch>> GetRawMatPurchAsync(string company, long companyid, List<Core.Entities.RawMatPurch> rawMatPurches, List<Navitems> navitems)
        {
            try
            {
                List<Core.Entities.RawMatPurch> rawMatPurchList = new List<Core.Entities.RawMatPurch>();
                int pageSize = 1000;
                int page = 0;
                while (true)
                {
                    var context = new NAVService(_configuration, company);
                    var nquery = context.Context.RawMatPurch.Skip(page * pageSize).Take(pageSize);
                    DataServiceQuery<NAV.RawMatPurch> query = (DataServiceQuery<NAV.RawMatPurch>)nquery;

                    TaskFactory<IEnumerable<NAV.RawMatPurch>> taskFactory = new TaskFactory<IEnumerable<NAV.RawMatPurch>>();
                    IEnumerable<NAV.RawMatPurch> result = await taskFactory.FromAsync(query.BeginExecute(null, null), iar => query.EndExecute(iar));

                    var prodCodes = result.ToList();
                    prodCodes.ForEach(b =>
                    {
                        var exitsData = rawMatPurchList.Where(f => f.ItemNo == b.Item_No && f.CompanyId == companyid && f.BatchNo == b.Batch_No).Count();
                        if (exitsData == 0)
                        {
                            var itemsExits = navitems.FirstOrDefault(f => f.CompanyId == companyid && f.No.Trim().ToLower() == b.Item_No.Trim().ToLower());
                            var exits = rawMatPurches.Where(f => f.ItemNo == b.Item_No && f.CompanyId == companyid && b.Batch_No == f.BatchNo).FirstOrDefault();
                            if (exits == null)
                            {
                                rawMatPurchList.Add(new Core.Entities.RawMatPurch
                                {
                                    ItemNo = b.Item_No,
                                    Description = b.Description,
                                    Description2 = b.Description_2,
                                    Quantity = b.Quantity,
                                    ManufacturingDate = b.Manufacturing_Date == DateTime.MinValue ? null : b.Manufacturing_Date,
                                    ExpirationDate = b.Expiration_Date == DateTime.MinValue ? null : b.Expiration_Date,
                                    UnitOfMeasureCode = b.Unit_of_Measure_Code,
                                    BatchNo = b.Batch_No,
                                    CompanyId = companyid,
                                    ItemId = itemsExits?.ItemId
                                });
                            }
                            else
                            {
                                rawMatPurchList.Add(exits);
                            }
                        }
                    });
                    if (prodCodes.Count < 1000)
                        break;
                    page++;
                }
                return rawMatPurchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
