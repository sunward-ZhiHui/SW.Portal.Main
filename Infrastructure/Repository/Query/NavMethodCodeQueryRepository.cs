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
        public async Task<IReadOnlyList<NavMethodCodeModel>> GetNavMethodCodeAsync()
        {
            try
            {
                var query = "select t1.*,t2.Description as CompanyName,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser from NavMethodCode t1\r\nLEFT JOIN Plant t2 ON t2.PlantID=t1.CompanyId\r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nJOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<NavMethodCodeModel>(query)).ToList();
                }
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
                    parameters.Add("MethodCodeID", value.MethodCodeID);
                    parameters.Add("CompanyId", value.CompanyId);
                    parameters.Add("NavinpCategoryID", value.NavinpCategoryID);
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
                    parameters.Add("BatchSizeId", value.BatchSizeId);
                    parameters.Add("BatchSizeNo", value.BatchSizeNo);
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
                    if (value.BatchSizeIds != null && value.BatchSizeIds.Count() > 0)
                    {
                        value.BatchSizeIds.ForEach(b =>
                        {

                        });
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
                    return value;

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
                        var query = "DELETE FROM NavmethodCodeBatch WHERE MethodCodeID= @MethodCodeID;";
                        query += "DELETE FROM ProductionForecast WHERE MethodCodeID= @MethodCodeID;";
                        query += "DELETE FROM NavMethodCodeLines WHERE MethodCodeID= @MethodCodeID;";
                        query += "DELETE FROM NavMethodCode WHERE MethodCodeID= @MethodCodeID;";
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
