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
using Newtonsoft.Json;
using static Infrastructure.Repository.Query.ACEntrysQueryRepository;
using static iText.IO.Image.Jpeg2000ImageData;
using static iTextSharp.text.pdf.AcroFields;

namespace Infrastructure.Repository.Query
{
    public class NavMethodCodeQueryRepository : DbConnector, INavMethodCodeQueryRepository
    {
        private readonly IAuditLogQueryRepository _auditLogQueryRepository;
        public NavMethodCodeQueryRepository(IConfiguration configuration, IAuditLogQueryRepository auditLogQueryRepository)
            : base(configuration)
        {
            _auditLogQueryRepository = auditLogQueryRepository;
        }
        public async Task<IReadOnlyList<NAVINPCategoryModel>> GetNAVINPCategorysync()
        {
            try
            {
                var query = "select * from Navinpcategory;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<NAVINPCategoryModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<NavMethodCodeModel>> GetNavMethodCodeAsync()
        {
            try
            {
                List<NavMethodCodeModel> navMethodCodeModels = new List<NavMethodCodeModel>();
                var navmethodCodeBatch = new List<NavmethodCodeBatch>();
                var query = "select t1.*,t2.Description as CompanyName,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser from NavMethodCode t1\r\nLEFT JOIN Plant t2 ON t2.PlantID=t1.CompanyId\r\nLEFT JOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nJOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID;";
                query += "select * from NavmethodCodeBatch;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query);
                    navMethodCodeModels = results.ReadAsync<NavMethodCodeModel>().Result.ToList();
                    navmethodCodeBatch = results.ReadAsync<NavmethodCodeBatch>().Result.ToList();
                }
                if (navMethodCodeModels != null && navMethodCodeModels.Count > 0)
                {
                    navMethodCodeModels.ForEach(s =>
                    {
                        var data = navmethodCodeBatch.Where(w => w.NavMethodCodeId == s.MethodCodeID).ToList();
                        s.BatchSizeIds = new List<long?>();
                        if (data != null && data.Count() > 0)
                        {
                            var BatchUnitSizes = data?.FirstOrDefault()?.BatchUnitSize;
                            if (BatchUnitSizes != null)
                            {
                                var splitData = BatchUnitSizes.ToString().Split(".");
                                var datass = splitData[0] + "." + splitData[1].TrimEnd('0');
                                s.BatchSizeNo = Convert.ToDecimal(datass);
                            }
                            if (s.ProdFrequency != null)
                            {
                                var splitData1 = s.ProdFrequency.ToString().Split(".");
                                var data1 = splitData1[0] + "." + splitData1[1].TrimEnd('0');
                                s.ProdFrequency = Convert.ToDecimal(data1);
                            }
                            if (s.DistReplenishHs != null)
                            {
                                var splitData2 = s.DistReplenishHs.ToString().Split(".");
                                var data2 = splitData2[0] + "." + splitData2[1].TrimEnd('0');
                                s.DistReplenishHs = Convert.ToDecimal(data2);
                            }
                            if (s.DistAcmonth != null)
                            {
                                var splitData2 = s.DistAcmonth.ToString().Split(".");
                                var data2 = splitData2[0] + "." + splitData2[1].TrimEnd('0');
                                s.DistAcmonth = Convert.ToDecimal(data2);
                            }
                            if (s.AdhocReplenishHs != null)
                            {
                                var splitData2 = s.AdhocReplenishHs.ToString().Split(".");
                                var data2 = splitData2[0] + "." + splitData2[1].TrimEnd('0');
                                s.AdhocReplenishHs = Convert.ToDecimal(data2);
                            }
                            if (s.AdhocPlanQty != null)
                            {
                                var splitData2 = s.AdhocPlanQty.ToString().Split(".");
                                var data2 = splitData2[0] + "." + splitData2[1].TrimEnd('0');
                                s.AdhocPlanQty = Convert.ToDecimal(data2);
                            }
                            s.BatchSizeId = data?.FirstOrDefault()?.DefaultBatchSize;
                            s.BatchSizeIds = data.Select(a => a.BatchSize).ToList();
                        }
                    });
                }
                return navMethodCodeModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<NavMethodCodeModel> InsertOrUpdateNavMethodCode(NavMethodCodeModel value)
        {
            try
            {
                var oldData = await _auditLogQueryRepository.GetDataSourceOldData("NavMethodCode", "MethodCodeID", value.MethodCodeID);
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("CompanyId", value.CompanyId);
                    parameters.Add("PlanningCategoryID", value.PlanningCategoryID);
                    parameters.Add("MethodName", value.MethodName, DbType.String);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("MethodDescription", value.MethodDescription, DbType.String);
                    parameters.Add("ProdFrequency", value.ProdFrequency);
                    parameters.Add("DistReplenishHs", value.DistReplenishHs);
                    parameters.Add("DistAcmonth", value.DistAcmonth);
                    parameters.Add("AdhocMonthStandAlone", value.AdhocMonthStandAlone);
                    parameters.Add("AdhocPlanQty", value.AdhocPlanQty);
                    parameters.Add("AdhocReplenishHs", value.AdhocReplenishHs);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    string? AuditType = "Added";
                    var query = string.Empty;
                    if (value.MethodCodeID > 0)
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "UPDATE NavMethodCode SET\r";
                            var names = string.Empty;
                            if (subDynamic.ParameterNames is not null)
                            {
                                foreach (var keyValue in subDynamic.ParameterNames)
                                {
                                    names += keyValue + "=";
                                    names += "@" + keyValue + ",";
                                }
                            }
                            query += names.TrimEnd(',') + "\rwhere MethodCodeID = " + value.MethodCodeID + ";";
                        }
                        AuditType = "Modified";
                    }
                    else
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "INSERT INTO NavMethodCode(\r";
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
                            query += names.TrimEnd(',') + ")\rOUTPUT INSERTED.MethodCodeID VALUES(" + values.TrimEnd(',') + ");";
                        }
                    }
                    if (!string.IsNullOrEmpty(query))
                    {
                        if (value.MethodCodeID > 0)
                        {
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            value.MethodCodeID = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                    }
                    var NavmethodCodeBatchList = await GetNavmethodCodeBatchById(value.MethodCodeID);
                    List<NavmethodCodeBatch> navmethodCodeBatches = new List<NavmethodCodeBatch>();
                    var query1 = string.Empty;
                    query1 += "DELETE  FROM NavmethodCodeBatch WHERE NavMethodCodeId =" + value.MethodCodeID + ";";
                    if (!string.IsNullOrEmpty(query1))
                    {
                        await connection.QuerySingleOrDefaultAsync<long>(query1);
                    }
                    if (value.BatchSizeIds != null && value.BatchSizeIds.Count() > 0)
                    {
                        foreach (var item in value.BatchSizeIds)
                        {
                            NavmethodCodeBatch navmethodCodeBatch = new NavmethodCodeBatch();
                            query1 = string.Empty;
                            var parameters1 = new DynamicParameters();
                            parameters1.Add("NavMethodCodeId", value.MethodCodeID);
                            parameters1.Add("BatchSize", item);
                            parameters1.Add("DefaultBatchSize", value.BatchSizeId);
                            parameters1.Add("BatchUnitSize", value.BatchSizeNo);
                            query1 += "\rINSERT INTO [NavmethodCodeBatch](NavMethodCodeId,BatchSize,DefaultBatchSize,BatchUnitSize) OUTPUT INSERTED.MethodCodeBatchId " +
                               "VALUES (@NavMethodCodeId,@BatchSize,@DefaultBatchSize,@BatchUnitSize);\r\n";
                            navmethodCodeBatch.MethodCodeBatchId = await connection.QuerySingleOrDefaultAsync<long>(query1, parameters1);
                            navmethodCodeBatch.NavMethodCodeId = value.MethodCodeID;
                            navmethodCodeBatch.BatchSize = item;
                            navmethodCodeBatch.DefaultBatchSize = value.BatchSizeId;
                            navmethodCodeBatch.BatchUnitSize = value.BatchSizeNo;
                            navmethodCodeBatches.Add(navmethodCodeBatch);
                        }
                    }
                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = new LowercaseContractResolver(),
                        Formatting = Formatting.Indented
                    };
                    string strJson = JsonConvert.SerializeObject(value, settings);
                    AuditLog auditLog = new AuditLog();
                    auditLog.OldData = oldData;
                    auditLog.NewData = JsonConvert.DeserializeObject(strJson);
                    auditLog.TableName = "NavMethodCode"; auditLog.AuditType = AuditType; auditLog.AuditByUserId = value.ModifiedByUserID; auditLog.PrimaryKeyValue = value.MethodCodeID;
                    await _auditLogQueryRepository.InsertAuditLog(auditLog);
                    if (navmethodCodeBatches != null && navmethodCodeBatches.Count() > 0)
                    {
                        foreach (var item in navmethodCodeBatches)
                        {
                            string NewstrJson = JsonConvert.SerializeObject(item, settings);
                            AuditLog auditLog1 = new AuditLog();
                            auditLog1.NewData = JsonConvert.DeserializeObject(NewstrJson);
                            auditLog1.ParentPrimaryKeyValue = value.MethodCodeID.ToString();
                            auditLog1.TableName = "NavmethodCodeBatch"; auditLog1.AuditType = "Added"; auditLog1.AuditByUserId = value.ModifiedByUserID; auditLog1.PrimaryKeyValue = item.MethodCodeBatchId;
                            await _auditLogQueryRepository.InsertAuditLog(auditLog1);
                        }
                    }
                    if (NavmethodCodeBatchList != null && NavmethodCodeBatchList.Count > 0)
                    {
                        foreach (var item in NavmethodCodeBatchList.ToList())
                        {
                            string oldstrJson = JsonConvert.SerializeObject(item, settings);
                            AuditLog auditLog1 = new AuditLog();
                            auditLog1.OldData = JsonConvert.DeserializeObject(oldstrJson);
                            auditLog1.ParentPrimaryKeyValue = value.MethodCodeID.ToString();
                            auditLog1.TableName = "NavmethodCodeBatch"; auditLog1.AuditType = "Modified"; auditLog1.AuditByUserId = value.ModifiedByUserID; auditLog1.PrimaryKeyValue = item.MethodCodeBatchId;
                            await _auditLogQueryRepository.InsertAuditLog(auditLog1);
                        }
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<IReadOnlyList<NavmethodCodeBatch>> GetNavmethodCodeBatchById(long? MethodCodeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("NavMethodCodeId", MethodCodeId);
                var query = "select * from NavmethodCodeBatch where NavMethodCodeId=@NavMethodCodeId;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<NavmethodCodeBatch>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<NavMethodCodeModel> DeleteNavMethodCode(NavMethodCodeModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("MethodCodeID", value.MethodCodeID);
                        var query = "DELETE FROM NavmethodCodeBatch WHERE NavMethodCodeId= @MethodCodeID;\n";
                        query += "DELETE FROM ProductionForecast WHERE MethodCodeID= @MethodCodeID;\n";
                        query += "DELETE FROM NavMethodCodeLines WHERE MethodCodeID= @MethodCodeID;\n";
                        query += "DELETE FROM NavMethodCode WHERE MethodCodeID= @MethodCodeID;\n";
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
        public async Task<IReadOnlyList<NavMethodCodeLines>> GetNavMethodCodeLinesById(long? MethodCodeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("MethodCodeId", MethodCodeId);
                var query = "select t1.*,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser,\r\nt6.No as ItemName,t6.Description,t6.Description2,t6.PackUOM,t6.BaseUnitofMeasure,t6.PackSize,t6.ItemCategoryCode as CategoryCode,t7.PlantCode as CompanyName\r\nfrom NavMethodCodeLines t1\r\nLEFT JOIN NavMethodCode t2 ON t2.MethodCodeID=t1.MethodCodeID\r\nLEFT JOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\nLEFT JOIN NAVItems t6 ON t6.ItemId=t1.ItemID\r\nLEFT JOIN Plant t7 ON t7.PlantID=t6.CompanyId where t1.MethodCodeId=@MethodCodeId;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<NavMethodCodeLines>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<NavMethodCodeLines> InsertOrUpdateNavMethodCodeLines(NavMethodCodeLines value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("ItemId", value.ItemId);
                    parameters.Add("StatusCodeID", value.StatusCodeId);
                    parameters.Add("MethodCodeId", value.MethodCodeId);
                    parameters.Add("AddedByUserID", value.AddedByUserId);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserId);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    var query = string.Empty;
                    if (value.MethodCodeLineId > 0)
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "UPDATE NavMethodCodeLines SET\r";
                            var names = string.Empty;
                            if (subDynamic.ParameterNames is not null)
                            {
                                foreach (var keyValue in subDynamic.ParameterNames)
                                {
                                    names += keyValue + "=";
                                    names += "@" + keyValue + ",";
                                }
                            }
                            query += names.TrimEnd(',') + "\rwhere MethodCodeLineId = " + value.MethodCodeLineId + ";";
                        }
                    }
                    else
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "INSERT INTO NavMethodCodeLines(\r";
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
                            query += names.TrimEnd(',') + ")\rOUTPUT INSERTED.MethodCodeLineId VALUES(" + values.TrimEnd(',') + ");";
                        }
                    }
                    if (!string.IsNullOrEmpty(query))
                    {
                        if (value.MethodCodeLineId > 0)
                        {
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            value.MethodCodeLineId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<NavMethodCodeLines> DeleteNavMethodCodeLines(NavMethodCodeLines value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("MethodCodeLineId", value.MethodCodeLineId);
                        var query = "DELETE FROM NavMethodCodeLines WHERE MethodCodeLineId= @MethodCodeLineId;";

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
        public async Task<IReadOnlyList<ProductionForecastModel>> GetProductionForecastById(long? MethodCodeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("MethodCodeId", MethodCodeId);
                var query = "select t1.*,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser,\r\nt6.No as ItemNo,t6.Description as Description1,t6.Description2,t7.PlantCode as CompanyName\r\nfrom ProductionForecast t1\r\n" +
                    "LEFT JOIN NavMethodCode t2 ON t2.MethodCodeID=t1.MethodCodeID\r\n" +
                    "LEFT JOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\n" +
                    "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\n" +
                    "LEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\n" +
                    "LEFT JOIN NAVItems t6 ON t6.ItemId=t1.ItemID\r\n LEFT JOIN Plant t7 ON t7.PlantID=t6.CompanyId where t1.MethodCodeId=@MethodCodeId;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionForecastModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ProductionForecastModel> InsertOrUpdateProductionForecast(ProductionForecastModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("ItemId", value.ItemId);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("MethodCodeId", value.MethodCodeID);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedDate", value.ModifiedDate);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ProductionMonth", value.ProductionMonth, DbType.DateTime);
                    parameters.Add("BatchSize", value.BatchSize, DbType.String);
                    parameters.Add("PackQuantity", value.PackQuantity);
                    var query = string.Empty;
                    if (value.ProductionForecastID > 0)
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "UPDATE ProductionForecast SET\r";
                            var names = string.Empty;
                            if (subDynamic.ParameterNames is not null)
                            {
                                foreach (var keyValue in subDynamic.ParameterNames)
                                {
                                    names += keyValue + "=";
                                    names += "@" + keyValue + ",";
                                }
                            }
                            query += names.TrimEnd(',') + "\rwhere ProductionForecastID = " + value.ProductionForecastID + ";";
                        }
                    }
                    else
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "INSERT INTO ProductionForecast(\r";
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
                            query += names.TrimEnd(',') + ")\rOUTPUT INSERTED.ProductionForecastID VALUES(" + values.TrimEnd(',') + ");";
                        }
                    }
                    if (!string.IsNullOrEmpty(query))
                    {
                        if (value.ProductionForecastID > 0)
                        {
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            value.ProductionForecastID = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<ProductionForecastModel> DeleteProductionForecast(ProductionForecastModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProductionForecastID", value.ProductionForecastID);
                        var query = "DELETE FROM ProductionForecast WHERE ProductionForecastID= @ProductionForecastID;";

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
