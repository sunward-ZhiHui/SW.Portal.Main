using ClosedXML.Excel;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NAV;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;
using static Infrastructure.Repository.Query.ACEntrysQueryRepository;
namespace Infrastructure.Repository.Query
{
    public class DistStockBalanceQueryRepository : DbConnector, IDistStockBalanceQueryRepository
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public DistStockBalanceQueryRepository(IConfiguration configuration, IWebHostEnvironment host)
            : base(configuration)
        {
            _hostingEnvironment = host;
        }

        public async Task<IReadOnlyList<NavStockBalanceModel>> GetAllDistStockBalanceAsync(StockBalanceSearch searchModel)
        {
            try
            {
                List<NavStockBalanceModel> navStockBalanceModels = new List<NavStockBalanceModel>();
                var month = searchModel.StkMonth.Month;
                var year = searchModel.StkMonth.Year;
                var weekofMonth = GetWeekNumberOfMonth(searchModel.StkMonth);
                var parameters = new DynamicParameters();
                parameters.Add("WeekNumberOfMonth", weekofMonth);
                parameters.Add("CompanyID", searchModel.CompanyId);
                parameters.Add("Month", month);
                parameters.Add("Year", year);
                List<long> distItemsID = new List<long>();
                List<DistStockBalance> DistStockBalance = new List<DistStockBalance>();
                var query = "select * from DistStockBalance where  Month(StockBalMonth) = @Month and Year(StockBalMonth)=@Year AND (StockBalWeek=@WeekNumberOfMonth OR StockBalWeek is null)";
                using (var connection = CreateConnection())
                {
                    DistStockBalance = (await connection.QueryAsync<DistStockBalance>(query, parameters)).ToList();
                    distItemsID = DistStockBalance.Select(s => s.DistItemId).ToList();
                }
                navStockBalanceModels = await GetAcItems(DistStockBalance, distItemsID, searchModel);
                return navStockBalanceModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<NavStockBalanceModel>> GetAcItems(List<DistStockBalance> distItems, List<long> DistItemIds, StockBalanceSearch searchModel)
        {
            try
            {
                List<NavStockBalanceModel> navStockBalances = new List<NavStockBalanceModel>();
                List<Navitems> navitems = new List<Navitems>(); List<NavItemCitemList> navItemCList = new List<NavItemCitemList>();
                DistItemIds = DistItemIds != null && DistItemIds.Count() > 0 ? DistItemIds : new List<long>() { -1 };
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", searchModel.CompanyId);
                List<Acitems> navStockBalance = new List<Acitems>();
                var query = "select * from Acitems where StatusCodeID=1 AND CompanyId=@CompanyID  AND DistAcid IN(" + string.Join(',', DistItemIds) + ") order by DistAcid desc";
                using (var connection = CreateConnection())
                {
                    navStockBalance = (await connection.QueryAsync<Acitems>(query, parameters)).ToList();
                    navStockBalance.ForEach(s =>
                    {
                        NavStockBalanceModel navStockBalanceModel = new NavStockBalanceModel
                        {
                            AvnavStockBalID = s.DistAcid,
                            StockBalMonth = distItems?.FirstOrDefault(f => f.DistItemId == s.DistAcid).StockBalMonth,
                            ItemDecs = s.ItemDesc,
                            RemainingQty = distItems?.FirstOrDefault(f => f.DistItemId == s.DistAcid).Quantity,
                            Dist = s.DistName,
                            ItemCode = s.ItemNo,
                            StatusCodeID = s.StatusCodeId,
                            CompanyId = s.CompanyId,
                        };
                        navStockBalances.Add(navStockBalanceModel);
                    });
                    var acItemIDs = navStockBalances.Select(c => c.AvnavStockBalID).ToList();
                    acItemIDs = acItemIDs != null && acItemIDs.Count() > 0 ? acItemIDs : new List<long>() { -1 };
                    var query1 = "select * from NavItemCitemList where  NavItemCustomerItemId IN(" + string.Join(',', acItemIDs) + ")";
                    navItemCList = (await connection.QueryAsync<NavItemCitemList>(query1, parameters)).ToList();
                    if (navItemCList.Count > 0)
                    {
                        var ItemIds = navItemCList.Select(s => s.NavItemId).Distinct().ToList();
                        ItemIds = ItemIds != null && ItemIds.Count() > 0 ? ItemIds : new List<long?>() { -1 };
                        var query2 = "select t1.* from NAVItems t1\r\nJOIN GenericCodes t2 ON t1.GenericCodeId=t2.GenericCodeId Where ItemId IN(" + string.Join(',', ItemIds) + ")";
                        navitems = (await connection.QueryAsync<Navitems>(query2, parameters)).ToList();
                    }
                }
                navStockBalances.ForEach(a =>
                {
                    var navCItem = navItemCList.FirstOrDefault(c => c.NavItemCustomerItemId == a.AvnavStockBalID);
                    if (navCItem != null)
                    {
                        var navItem = navitems.FirstOrDefault(n => n.ItemId == navCItem.NavItemId);
                        var groupName = navItem != null ? navItem.CategoryId.GetValueOrDefault(0) : 0;
                        a.PackSize = navItem != null ? navItem.PackSize.ToString() : string.Empty;
                        a.Packuom = navItem != null ? navItem.PackUom : string.Empty;
                        a.Uom = navItem != null ? navItem.BaseUnitofMeasure : string.Empty;
                        a.SWItemNo = navItem != null ? navItem.No : string.Empty;
                        a.SWDesc = navItem != null ? navItem.Description : string.Empty;
                        a.SWDesc2 = navItem != null ? navItem.Description2 : string.Empty;
                        a.InternalRefNo = navItem != null ? navItem.InternalRef : "";
                        a.ItemGroup = navItem != null ? navItem.ItemCategoryCode : "";
                    }
                });
                return navStockBalances;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
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
        public async Task<IReadOnlyList<NavitemStockBalance>> GetAllNavItemStockBalanceAsync(NavitemStockBalance value)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("WeekNumberOfMonth", value.WeekNumberOfMonth);
                parameters.Add("CompanyID", value.CompanyID);
                parameters.Add("Month", value.Month);
                parameters.Add("Year", value.Year);
                var query = @"Select NIB.*,NI.No as ItemNo,NI.Description as ItemDescription,NI.Description2 as ItemDescription2,NI.InternalRef as InternalRef,
                            NI.ItemCategoryCode as Category,NI.PurchaseUOM as UOM,NI.PackSize as PackSize,NI.PackUOM as PackUOM,NIB.Quantity,NIB.StockBalMonth, NI.StatusCodeID as NavStatusCodeID,NIB.StockBalWeek From NavItemStockBalance NIB 
                            LEFT Join NavItems NI on NI.ItemID = NIB.ItemID where NI.StatusCodeID = 1 And NIB.StockBalWeek = @WeekNumberOfMonth and NI.CompanyId = @CompanyID and Month(NIB.StockBalMonth) = @Month and Year(NIB.StockBalMonth)=@Year";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<NavitemStockBalance>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<NavItemStockBalanceModel>> GetNavItemStockBalanceById(long? id)
        {
            try
            {
                List<NavItemStockBalanceModel> navStockBalanceModels = new List<NavItemStockBalanceModel>();
                var parameters = new DynamicParameters();
                parameters.Add("ItemId", id);
                var query = "select t1.*,t2.No as ItemName from NavitemStockBalance t1  LEFT JOIN Navitems t2 ON t1.ItemId=t2.ItemId where t1.ItemId=@ItemId;";
                using (var connection = CreateConnection())
                {
                    navStockBalanceModels = (await connection.QueryAsync<NavItemStockBalanceModel>(query, parameters)).ToList();
                }
                return navStockBalanceModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DistStockBalanceModel>> GetNavDistStockBalanceById(long? id)
        {
            try
            {
                List<DistStockBalanceModel> navStockBalanceModels = new List<DistStockBalanceModel>();
                var parameters = new DynamicParameters();
                parameters.Add("DistItemId", id);
                var query = "select t1.*,t2.DistName,t2.ItemNo as ItemName,t1.POQuantity as POqty from DistStockBalance t1\r\nLEFT JOIN Acitems t2 ON t1.DistItemId=t2.DistACID where t1.DistItemId=@DistItemId;";
                using (var connection = CreateConnection())
                {
                    navStockBalanceModels = (await connection.QueryAsync<DistStockBalanceModel>(query, parameters)).ToList();
                }
                return navStockBalanceModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Acitems> InsertAcItems(Acitems acitems, DistStockBalance distStockBalance)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {

                        var parameters = new DynamicParameters();
                        parameters.Add("DistName", acitems.DistName, DbType.String);
                        parameters.Add("Acqty", acitems.Acqty);
                        parameters.Add("ItemDesc", acitems.ItemDesc, DbType.String);
                        parameters.Add("ItemNo", acitems.ItemNo, DbType.String);
                        parameters.Add("Acmonth", acitems.Acmonth, DbType.DateTime);
                        parameters.Add("CompanyId", acitems.CompanyId);
                        parameters.Add("StatusCodeId", acitems.StatusCodeId);
                        parameters.Add("CustomerId", acitems.CustomerId);
                        parameters.Add("AddedDate", acitems.AddedDate);
                        parameters.Add("AddedByUserId", acitems.AddedByUserId);
                        var query = "INSERT INTO Acitems(DistName,Acqty,ItemDesc,ItemNo,Acmonth,CompanyId,StatusCodeId,CustomerId,AddedDate,AddedByUserId)  " +
                              "OUTPUT INSERTED.DistAcid VALUES " +
                             "(@DistName,@Acqty,@ItemDesc,@ItemNo,@Acmonth,@CompanyId,@StatusCodeId,@CustomerId,@AddedDate,@AddedByUserId);\n\r";
                        acitems.DistAcid = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);



                        var parameters1 = new DynamicParameters();
                        parameters1.Add("DistAcid", acitems.DistAcid);
                        parameters1.Add("Quantity", distStockBalance.Quantity);
                        parameters1.Add("StockBalMonth", distStockBalance.StockBalMonth, DbType.DateTime);
                        parameters1.Add("AddedDate", distStockBalance.AddedDate, DbType.DateTime);
                        parameters1.Add("AddedByUserId", distStockBalance.AddedByUserID);
                        parameters1.Add("StockBalWeek", distStockBalance.StockBalWeek);
                        var query1 = "INSERT INTO DistStockBalance(DistItemId,Quantity,StockBalMonth,StockBalWeek,AddedDate,AddedByUserId)  " +
                              "OUTPUT INSERTED.DistStockBalanceId VALUES " +
                             "(@DistAcid,@Quantity,@StockBalMonth,@StockBalWeek,@AddedDate,@AddedByUserId);\n\r";
                        distStockBalance.DistStockBalanceId = await connection.QuerySingleOrDefaultAsync<long>(query1, parameters1);
                        return acitems;
                    }


                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }

                }


            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<DistStockBalance> UpadatetDistStockBalance(DistStockBalance distStockBalance)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters1 = new DynamicParameters();
                        parameters1.Add("DistStockBalanceId", distStockBalance.DistStockBalanceId);
                        parameters1.Add("Quantity", distStockBalance.Quantity);
                        parameters1.Add("ModifiedDate", distStockBalance.ModifiedDate, DbType.DateTime);
                        parameters1.Add("ModifiedByUserID", distStockBalance.ModifiedByUserID);
                        var query1 = "update DistStockBalance set Quantity=@Quantity,ModifiedDate=@ModifiedDate,ModifiedByUserID=@ModifiedByUserID where DistStockBalanceId=@DistStockBalanceId;";
                        distStockBalance.DistStockBalanceId = await connection.ExecuteAsync(query1, parameters1);
                        return distStockBalance;
                    }


                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }

                }


            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<DistStockBalance> UpadatetAcItems(DistStockBalance distStockBalance)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters1 = new DynamicParameters();
                        parameters1.Add("DistItemId", distStockBalance.DistItemId);
                        parameters1.Add("Quantity", distStockBalance.Quantity);
                        parameters1.Add("StockBalMonth", distStockBalance.StockBalMonth, DbType.DateTime);
                        parameters1.Add("AddedDate", distStockBalance.AddedDate);
                        parameters1.Add("AddedByUserId", distStockBalance.AddedByUserID);
                        parameters1.Add("StockBalWeek", distStockBalance.StockBalWeek);
                        var query1 = "INSERT INTO DistStockBalance(DistItemId,Quantity,StockBalMonth,StockBalWeek,AddedDate,AddedByUserId)  " +
                                                      "OUTPUT INSERTED.DistStockBalanceId VALUES " +
                                                     "(@DistItemId,@Quantity,@StockBalMonth,@StockBalWeek,@AddedDate,@AddedByUserId);\n\r";

                        distStockBalance.DistStockBalanceId = await connection.QuerySingleOrDefaultAsync<long>(query1, parameters1);
                        return distStockBalance;
                    }


                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }

                }


            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<StockBalanceSearch> UploadStockBalance(StockBalanceSearch TenderOrderModel)
        {
            try
            {
                var SessionId = Guid.NewGuid(); long? companyId = TenderOrderModel.CompanyId; long? userId = TenderOrderModel.UserId;
                string sheetName = "ACImport"; var acItems = new List<ACImportModel>();
                DataTable dt = new DataTable();
                byte[] byteArray = TenderOrderModel.ByteData;

                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    using (XLWorkbook workbook = new XLWorkbook(stream))
                    {
                        IXLWorksheet worksheet = workbook.Worksheet(1);
                        bool FirstRow = true;
                        //Range for reading the cells based on the last cell used.  
                        string readRange = "1:1";
                        foreach (IXLRow row in worksheet.RowsUsed())
                        {
                            //If Reading the First Row (used) then add them as column name  
                            if (FirstRow)
                            {
                                //Checking the Last cellused for column generation in datatable  
                                readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                                foreach (IXLCell cell in row.Cells(readRange))
                                {
                                    dt.Columns.Add(cell.Value.ToString());
                                }
                                FirstRow = false;
                            }
                            else
                            {
                                //Adding a Row in datatable  
                                dt.Rows.Add();
                                int cellIndex = 0;
                                //Updating the values of datatable  
                                foreach (IXLCell cell in row.Cells(readRange))
                                {
                                    dt.Rows[dt.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                                    cellIndex++;
                                }
                            }
                        }
                        //If no data in Excel file  
                        if (FirstRow)
                        {
                            // throw new AppException("Please Check Sheet Name or File format/Version!. Excel Export will not support lower versions(.xls).", null);
                        }
                    }
                }
                int rowCount = dt.Rows.Count;
                if (rowCount > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        acItems.Add(new ACImportModel
                        {
                            DistName = row.ItemArray[0].ToString(),
                            ItemNo = row.ItemArray[1].ToString(),
                            Description = row.ItemArray[2].ToString(),
                            QtyOnHand = row.ItemArray[3].ToString(),
                            Location = row.ItemArray[4].ToString(),
                            Month = row.ItemArray[5].ToString(),
                        });
                    }
                }
                /*using (var fs = new MemoryStream(TenderOrderModel.ByteData))
                {
                    using (var package = new ExcelPackage(fs))
                    {
                        var oSheet = package.Workbook.Worksheets[sheetName];
                        if (oSheet != null)
                        {
                            ExcelWorksheet worksheet = oSheet;
                            var rowCount = worksheet.Dimension.Rows;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null && worksheet.Cells[row, 6].Value != null)
                                {
                                    acItems.Add(new ACImportModel
                                    {
                                        DistName = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        ItemNo = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                        Description = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                        QtyOnHand = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                        Location = worksheet.Cells[row, 5].Value?.ToString().Trim(),
                                        Month = worksheet.Cells[row, 6].Value.ToString().Trim(),
                                    });
                                }
                            }
                        }
                        else
                        {
                        }
                    }
                }*/
                var query = "select * from Navcustomer;";
                query += "select * from Acitems;";
                // query += "select * from DistStockBalance;";
                List<Navcustomer> customers = new List<Navcustomer>(); List<Acitems> acitems = new List<Acitems>();
                //List<DistStockBalance> DistStockBalances = new List<DistStockBalance>();
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query);
                    customers = results.Read<Navcustomer>().ToList();
                    acitems = results.Read<Acitems>().ToList(); //DistStockBalances = results.Read<DistStockBalance>().ToList();
                }
                acItems.ForEach(async ac =>
                {
                    long? custId = null;
                    var cust = customers.FirstOrDefault(cu => cu.Name == ac.DistName);
                    if (cust != null)
                    {
                        custId = cust.CustomerId;
                    }
                    else
                    {

                    }
                    var acMonth = TenderOrderModel.StkMonth;
                    var qty = acItems.Where(q => q.ItemNo == ac.ItemNo && q.DistName == ac.DistName).Sum(s => decimal.Parse(s.QtyOnHand));

                    var exist = acitems.FirstOrDefault(d => d.CustomerId == custId && d.CompanyId == companyId && d.ItemNo == ac.ItemNo);
                    if (exist == null)
                    {
                        var acitem = new Acitems
                        {
                            DistName = ac.DistName,
                            Acqty = qty,
                            ItemDesc = ac.Description,
                            ItemNo = ac.ItemNo,
                            Acmonth = acMonth,
                            StatusCodeId = 2,
                            CompanyId = companyId,
                            CustomerId = custId,
                            AddedDate = DateTime.Now,
                            AddedByUserId = userId,
                        };
                        var stockBal = new DistStockBalance
                        {
                            Quantity = qty,
                            StockBalMonth = acMonth,
                            AddedDate = DateTime.Now,
                            AddedByUserID = userId,
                        };
                        stockBal.StockBalWeek = GetWeekNumberOfMonth(stockBal.StockBalMonth);
                        await InsertAcItems(acitem, stockBal);
                    }
                    else
                    {

                        exist.ModifiedByUserId = userId;
                        exist.ModifiedDate = DateTime.Now;
                        var stockBalWeek = GetWeekNumberOfMonth(acMonth);
                        var DistStockBalances = await GetNavDistStockBalanceOneById(exist.DistAcid, acMonth);
                        var stkbalances = DistStockBalances.Where(fd => fd.DistItemId == exist.DistAcid && fd.StockBalMonth.Month == acMonth.Month && fd.StockBalMonth.Year == acMonth.Year);
                        DistStockBalance stkbalance = null;
                        if (stkbalances.Any(c => c.StockBalWeek == stockBalWeek))
                        {
                            stkbalance = stkbalances.FirstOrDefault(s => s.StockBalWeek == stockBalWeek);
                        }
                        if (stkbalance == null)
                        {
                            var stockBal = new DistStockBalance
                            {
                                DistItemId = exist.DistAcid,
                                Quantity = qty,
                                StockBalMonth = acMonth,
                                AddedDate = DateTime.Now,
                                AddedByUserID = userId,
                            };
                            stockBal.StockBalWeek = stockBalWeek;
                            await UpadatetAcItems(stockBal);
                        }
                        else
                        {
                            stkbalance.Quantity = qty;
                            stkbalance.ModifiedDate = DateTime.Now;
                            stkbalance.ModifiedByUserID = userId;
                            await UpadatetDistStockBalance(stkbalance);
                        }
                    }
                });

            }
            catch (Exception ex)
            {
            }
            return TenderOrderModel;
        }
        public async Task<List<DistStockBalance>> GetNavDistStockBalanceOneById(long? id, DateTime acMonth)
        {
            try
            {
                List<DistStockBalance> navStockBalanceModels = new List<DistStockBalance>();
                var parameters = new DynamicParameters();
                parameters.Add("DistItemId", id);

                parameters.Add("Month", acMonth.Month);
                parameters.Add("Year", acMonth.Year);
                var query = "select t1.* from DistStockBalance t1 where t1.DistItemId=@DistItemId  AND MONTH(t1.StockBalMonth) = @Month AND YEAR(t1.StockBalMonth)=@Year;";
                using (var connection = CreateConnection())
                {
                    navStockBalanceModels = (await connection.QueryAsync<DistStockBalance>(query, parameters)).ToList();
                }
                return navStockBalanceModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<NavItemStockBalanceModel> UpdateNavItemStockBalance(NavItemStockBalanceModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("NavStockBalanceId", value.NavStockBalanceId);
                    parameters.Add("Quantity", value.Quantity);
                    parameters.Add("ItemId", value.ItemId);
                    parameters.Add("RejectQuantity", value.RejectQuantity);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    parameters.Add("StockBalMonth", value.StockBalMonth, DbType.DateTime);
                    if (value.NavStockBalanceId > 0)
                    {
                        var query = "UPDATE NavitemStockBalance SET ModifiedByUserID=@ModifiedByUserID,Quantity=@Quantity,RejectQuantity=@RejectQuantity,ModifiedDate=@ModifiedDate\r" +
                            "WHERE NavStockBalanceId = @NavStockBalanceId";
                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var stockBalWeek = GetWeekNumberOfMonth(value.StockBalMonth.Value);
                        parameters.Add("StockBalWeek", stockBalWeek);
                        var query1 = "INSERT INTO NavitemStockBalance(ItemId,RejectQuantity,Quantity,StockBalMonth,StockBalWeek,AddedDate,AddedByUserId)  " +
                                                    "OUTPUT INSERTED.NavStockBalanceId VALUES " +
                                                   "(@ItemId,@RejectQuantity,@Quantity,@StockBalMonth,@StockBalWeek,@AddedDate,@AddedByUserId);\n\r";
                        await connection.QuerySingleOrDefaultAsync<long>(query1, parameters);
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<NavItemStockBalanceModel> DeleteNavItemStockBalance(NavItemStockBalanceModel navItemStockBalanceModel)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("NavStockBalanceId", navItemStockBalanceModel.NavStockBalanceId);
                        var query = "DELETE FROM NavitemStockBalance WHERE NavStockBalanceId= @NavStockBalanceId;";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return navItemStockBalanceModel;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<Acitems>> GetNoACItemsList()
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("StatusCodeID", 2);
                var query = @"select t1.* from ACItems t1 where t1.StatusCodeID=@StatusCodeID;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Acitems>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Acitems> UpdateNoACItems(Acitems acitems)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DistAcid", acitems.DistAcid);
                        parameters.Add("StatusCodeId", 1);
                        var query = "update Acitems set StatusCodeId=@StatusCodeId WHERE DistAcid= @DistAcid;";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return acitems;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DistStockBalanceModel> UpdateDistStockBalance(DistStockBalanceModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("DistStockBalanceId", value.DistStockBalanceId);
                    parameters.Add("Quantity", value.Quantity);
                    parameters.Add("POQuantity", value.PoQty);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("StockBalMonth", value.StockBalMonth, DbType.DateTime);
                    parameters.Add("DistItemId", value.DistItemId);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    if (value.DistStockBalanceId > 0)
                    {
                        var query = "UPDATE DistStockBalance SET ModifiedByUserID=@ModifiedByUserID,Quantity=@Quantity,POQuantity=@POQuantity,ModifiedDate=@ModifiedDate\r" +
                            "WHERE DistStockBalanceId = @DistStockBalanceId";
                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var stockBalWeek = GetWeekNumberOfMonth(value.StockBalMonth.Value);
                        parameters.Add("StockBalWeek", stockBalWeek);
                        var query1 = "INSERT INTO DistStockBalance(POQuantity,DistItemId,Quantity,StockBalMonth,StockBalWeek,AddedDate,AddedByUserId)  " +
                                                     "OUTPUT INSERTED.DistStockBalanceId VALUES " +
                                                    "(@POQuantity,@DistItemId,@Quantity,@StockBalMonth,@StockBalWeek,@AddedDate,@AddedByUserId);\n\r";
                        await connection.QuerySingleOrDefaultAsync<long>(query1, parameters);
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DistStockBalanceModel> DeleteDistStockBalance(DistStockBalanceModel navItemStockBalanceModel)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DistStockBalanceId", navItemStockBalanceModel.DistStockBalanceId);
                        var query = "DELETE FROM DistStockBalance WHERE DistStockBalanceId= @DistStockBalanceId;";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return navItemStockBalanceModel;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
