using Core.Entities;
using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Views;
using Core.EntityModels;

using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static Org.BouncyCastle.Math.EC.ECCurve;
using ClosedXML.Excel;

namespace Infrastructure.Repository.Query
{
    public class TenderOrdersQueryRepository : DbConnector, ITenderOrdersQueryRepository
    {
        public TenderOrdersQueryRepository(IConfiguration configuration)
            : base(configuration)
        {
        }
        public async Task<IReadOnlyList<TenderOrderModel>> GetAllByAsync()
        {
            try
            {
                var query = "select * from SimulationAddhoc";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<TenderOrderModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<TenderOrderModel> InsertOrUpdateTenderOrder(TenderOrderModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("SimualtionAddhocID", value.SimualtionAddhocID);
                    parameters.Add("DocumentNo", value.DocumentNo, DbType.String);
                    parameters.Add("ExternalDocNo", value.ExternalDocNo, DbType.String);
                    parameters.Add("OutstandingQty", value.OutstandingQty);
                    parameters.Add("PromisedDate", value.PromisedDate, DbType.DateTime);
                    parameters.Add("ShipmentDate", value.ShipmentDate, DbType.DateTime);
                    if (value.SimualtionAddhocID > 0)
                    {
                        var query = "UPDATE SimulationAddhoc SET DocumentNo=@DocumentNo,ExternalDocNo=@ExternalDocNo,OutstandingQty=@OutstandingQty,PromisedDate=@PromisedDate,ShipmentDate=@ShipmentDate\r" +
                            "WHERE SimualtionAddhocID = @SimualtionAddhocID";
                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {

                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<TenderOrderModel> DeleteTenderOrder(TenderOrderModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("SimualtionAddhocID", value.SimualtionAddhocID);
                        var query = "DELETE FROM SimulationAddhoc WHERE SimualtionAddhocID= @SimualtionAddhocID;";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return value;
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
        /*public async Task<IActionResult> Upload(IFormCollection files)
        {
            var SessionId = Guid.NewGuid();
            var plant = files["plantId"].ToString();
            var user = files["userId"].ToString();
            long userId = !string.IsNullOrEmpty(user) ? long.Parse(user) : -1;
            var companyId = !string.IsNullOrEmpty(plant) ? long.Parse(plant) : -1;
            DataTable dt = new DataTable();
            files.Files.ToList().ForEach(f =>
            {

                var file = f;
                var fs = file.OpenReadStream();
                var br = new BinaryReader(fs);
                Byte[] document = br.ReadBytes((Int32)fs.Length);

                var fileName = file.FileName;

                var serverPath = _hostingEnvironment.ContentRootPath + @"\AppUpload\" + fileName;
                System.IO.File.WriteAllBytes(serverPath, document);
                var fileInfo = new FileInfo(serverPath);
                using (XLWorkbook workbook = new XLWorkbook(serverPath))
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
            });

            var tenderOrders = (from DataRow row in dt.Rows

                                select new TenderOrderModel
                                {
                                    Categories = row["Catogories"].ToString(),
                                    CustomerName = row["CustomerName"].ToString(),
                                    Description = row["Description"].ToString(),
                                    Description1 = row["Description2"].ToString(),
                                    DocumantType = row["DocumentType"].ToString(),
                                    DocumentNo = row["DocumentNo"].ToString(),
                                    ExternalDocNo = row["ExternalDocumentNo"].ToString(),
                                    ItemNo = row["ItemNo"].ToString(),
                                    OutstandingQty = decimal.Parse(row["OutstandingQuantity"].ToString()),
                                    PromisedDate = GetDate(row["PromisedDeliveryDate"].ToString()),
                                    SelltoCustomerNo = row["SelltoCustomerNo"].ToString(),
                                    ShipmentDate = GetDate(row["ShipmentDate"]?.ToString()),
                                    UOMCode = row["UnitofMeasureCode"].ToString(),
                                    Company = row["Company"].ToString(),
                                }).ToList();

            if (tenderOrders.Count > 0)
            {


                await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM SimulationAddhoc");

                var items = _context.Navitems.Select(s => new { s.ItemId, s.No, s.CompanyId }).AsNoTracking().ToList();
                int count = 0;
                int totalItems = tenderOrders.Count();
                var addhocList = new List<SimulationAddhoc>();
                tenderOrders.ForEach(f =>
                {
                    var itemCount = count + " of " + totalItems;
                    //notify client progress update
                    var itemName = string.Format("{0} {1}-{2}  {3}", itemCount, f.ItemNo, f.Description, "tender items");
                    _hub.Clients.Group(userId.ToString()).SendAsync("progress", itemName);
                    var companyId = f.Company == "SWJB" ? 1 : 2;
                    var simAddhoc = new SimulationAddhoc
                    {
                        Categories = f.Categories,
                        CustomerName = f.CustomerName,
                        Description = f.Description,
                        Description1 = f.Description1,
                        DocumantType = f.DocumantType,
                        DocumentNo = f.DocumentNo,
                        ExternalDocNo = f.ExternalDocNo,
                        ItemId = items.FirstOrDefault(i => i.No == f.ItemNo && i.CompanyId == companyId)?.ItemId,
                        ItemNo = f.ItemNo,
                        OutstandingQty = f.OutstandingQty,
                        PromisedDate = f.PromisedDate,
                        SelltoCustomerNo = f.SelltoCustomerNo,
                        ShipmentDate = f.ShipmentDate,
                        Uomcode = f.UOMCode,
                    };
                    //_context.SimulationAddhoc.Add(simAddhoc);
                    addhocList.Add(simAddhoc);
                    count++;
                });

            }
            return Content(SessionId.ToString());
        }*/
    }
}
