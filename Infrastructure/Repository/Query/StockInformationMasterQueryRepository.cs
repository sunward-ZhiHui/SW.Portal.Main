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
using IdentityModel.Client;
using NAV;
using Application.Queries;
using Infrastructure.Data;
using static iText.IO.Image.Jpeg2000ImageData;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Dynamic;
using System.Data.SqlClient;
using Newtonsoft.Json.Serialization;
using static Infrastructure.Repository.Query.ACEntrysQueryRepository;
using static iTextSharp.text.pdf.AcroFields;

namespace Infrastructure.Repository.Query
{
    public class StockInformationMasterQueryRepository : DbConnector, IStockInformationMasterQueryRepository
    {
        
        public StockInformationMasterQueryRepository(IConfiguration configuration)
            : base(configuration)
        {            
        }
        public async Task<IReadOnlyList<StockInformationMaster>> GetAllByAsync()
        {
            try
            {
                List<StockInformationMaster> aCItemsModels = new List<StockInformationMaster>();
                var parameters = new DynamicParameters();
                var query = @"SELECT *,CONCAT(P.PlantCode ,'-', P.Description)  as CompanyName,CONCAT(C.Name,'-',C.Code) as CustomerName,AMD.Value as PlanningCategoryName,AMD.description as PlanningCategoryDescName from StockInformationMaster SM
                                LEFT JOIN Plant P ON P.PlantID = SM.CompanyID
                                LEFT JOIN NAVCustomer C ON C.CustomerId = SM.CustomerID
                                LEFT JOIN ApplicationMasterDetail AMD ON AMD.ApplicationMasterDetailID = SM.PlanningCategory";
                using (var connection = CreateConnection())
                {
                    aCItemsModels = (await connection.QueryAsync<StockInformationMaster>(query, parameters)).ToList();
                }

                return aCItemsModels;
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
        public async Task<StockInformationMaster> InsertOrUpdateStockInformationMaster(StockInformationMaster value)
        {
            try
            {
                //var oldData = await _auditLogQueryRepository.GetDataSourceOldData("NavitemLinks", "ItemLinkId", value.ItemLinkId);
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("MasterName", value.MasterName);
                    parameters.Add("CompanyID", value.CompanyID);
                    parameters.Add("CustomerID", value.CustomerID);
                    parameters.Add("PlanningCategory", value.PlanningCategory);
                    parameters.Add("BelowMonth", value.BelowMonth);
                    parameters.Add("TopupMonth", value.TopupMonth);
                    parameters.Add("SessionId", value.SessionId);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);

                    value.StockInformationMasterID = await InsertOrUpdate("StockInformationMaster", "StockInformationMasterID", value.StockInformationMasterID, parameters);
                    return value;
                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<StockInformationMaster> DeleteStockInformationMaster(StockInformationMaster value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("StockInformationMasterID", value.StockInformationMasterID);
                        var query = "DELETE FROM StockInformationMaster WHERE StockInformationMasterID= @StockInformationMasterID;";
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
    }
}
