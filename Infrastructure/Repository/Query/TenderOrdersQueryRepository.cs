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
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Hosting;
using Google.Type;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.JSInterop;
using System.IO;

namespace Infrastructure.Repository.Query
{
    public class TenderOrdersQueryRepository : DbConnector, ITenderOrdersQueryRepository
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public TenderOrdersQueryRepository(IConfiguration configuration, IWebHostEnvironment host)
            : base(configuration)
        {
            _hostingEnvironment = host;
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
        private async Task<long> InsertOrUpdate(string? TableName, string? PrimareyKeyName, long PrimareyKeyId, DynamicParameters parameters)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var query = string.Empty;
                    if (PrimareyKeyId > 0)
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "UPDATE " + TableName + " SET\r";
                            var names = string.Empty;
                            if (subDynamic.ParameterNames is not null)
                            {
                                foreach (var keyValue in subDynamic.ParameterNames)
                                {
                                    names += keyValue + "=";
                                    names += "@" + keyValue + ",";
                                }
                            }
                            query += names.TrimEnd(',') + "\rwhere " + PrimareyKeyName + " = " + PrimareyKeyId + ";";
                        }
                    }
                    else
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "INSERT INTO " + TableName + "(\r";
                            var values = string.Empty;
                            var names = string.Empty;
                            if (subDynamic.ParameterNames is not null)
                            {
                                foreach (var keyValue in subDynamic.ParameterNames)
                                {
                                    names += keyValue + ",";
                                    values += "@" + keyValue + ",";
                                }
                            }
                            query += names.TrimEnd(',') + ")\rOUTPUT INSERTED." + PrimareyKeyName + " VALUES(" + values.TrimEnd(',') + ");";
                        }
                    }
                    if (!string.IsNullOrEmpty(query))
                    {
                        if (PrimareyKeyId > 0)
                        {
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            PrimareyKeyId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                    }
                }
                return PrimareyKeyId;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private System.DateTime? GetDate(string dateString)
        {
            if (!string.IsNullOrEmpty(dateString))
            {
                System.DateTime date = System.DateTime.Parse(dateString);
                //double d = double.Parse(dateString);
                //DateTime date = DateTime.FromOADate(d);
                return date;
            }
            return null;
        }
        public async Task<TenderOrderModel> UploadTenderOrder(TenderOrderModel TenderOrderModel)
        {
            try
            {
                var SessionId = Guid.NewGuid();
                string folderName = _hostingEnvironment.ContentRootPath + @"\AppUpload";
                string newFolderName = "ConvertExcel";
                string serverPath = System.IO.Path.Combine(folderName, newFolderName);
                if (!System.IO.Directory.Exists(serverPath))
                {
                    System.IO.Directory.CreateDirectory(serverPath);
                }
                string FromLocation = serverPath + @"\" + SessionId + ".xlsx";
                File.WriteAllBytes(FromLocation, TenderOrderModel.ByteData);
                DataTable dt = new DataTable();
                using (XLWorkbook workbook = new XLWorkbook(FromLocation))
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
                List<DynamicParameters> dynamicParameters = new List<DynamicParameters>();
                int rowCount = dt.Rows.Count;
                if (rowCount > 0)
                {
                    var items = await GetNavItemsAllByAsync(TenderOrderModel.CompanyId);
                    foreach (DataRow row in dt.Rows)
                    {
                        var parameters = new DynamicParameters();
                        var ItemNo = row["ItemNo"].ToString();
                        var itemIds = items.FirstOrDefault(i => i.No == ItemNo && i.CompanyId == TenderOrderModel.CompanyId)?.ItemId;
                        parameters.Add("ItemId", itemIds);
                        parameters.Add("DocumantType", row["DocumentType"].ToString(), DbType.String);
                        parameters.Add("CustomerName", row["CustomerName"].ToString(), DbType.String);
                        parameters.Add("Categories", row["Catogories"].ToString(), DbType.String);
                        parameters.Add("DocumentNo", row["DocumentNo"].ToString(), DbType.String);
                        parameters.Add("ExternalDocNo", row["ExternalDocumentNo"].ToString(), DbType.String);
                        parameters.Add("Description", row["Description"].ToString(), DbType.String);
                        parameters.Add("Description1", row["Description2"].ToString(), DbType.String);
                        parameters.Add("ItemNo", ItemNo, DbType.String);
                        parameters.Add("OutstandingQty", row["OutstandingQuantity"].ToString(), DbType.String);
                        parameters.Add("PromisedDate", GetDate(row["PromisedDeliveryDate"].ToString()), DbType.DateTime);
                        parameters.Add("SelltoCustomerNo", row["SelltoCustomerNo"].ToString(), DbType.String);
                        parameters.Add("ShipmentDate", GetDate(row["ShipmentDate"].ToString()), DbType.DateTime);
                        parameters.Add("UOMCode", row["UnitofMeasureCode"].ToString(), DbType.String);
                        //parameters.Add("Company", row["Company"].ToString(), DbType.String);
                        dynamicParameters.Add(parameters);
                    }
                    if (dynamicParameters.Count > 0)
                    {
                        await DeleteTenderOrder();
                        dynamicParameters.ForEach(async a =>
                        {
                            await InsertOrUpdate("SimulationAddhoc", "SimualtionAddhocID", 0, a);
                        });
                    }
                }
                System.IO.File.Delete(FromLocation);
                return TenderOrderModel;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<View_NavItems>> GetNavItemsAllByAsync(long? CompanyId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                var query = "Select * FROM NavItems WHERE CompanyId= @CompanyId;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_NavItems>(query,parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task DeleteTenderOrder()
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        var query = "DELETE FROM SimulationAddhoc";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
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
