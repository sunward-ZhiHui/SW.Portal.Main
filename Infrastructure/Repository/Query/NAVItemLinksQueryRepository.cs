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
    public class NAVItemLinksQueryRepository : DbConnector, INAVItemLinksQueryRepository
    {
        private readonly IAuditLogQueryRepository _auditLogQueryRepository;
        public NAVItemLinksQueryRepository(IConfiguration configuration, IAuditLogQueryRepository auditLogQueryRepository)
            : base(configuration)
        {
            _auditLogQueryRepository = auditLogQueryRepository;
        }
        public async Task<IReadOnlyList<NAVItemLinksModel>> GetAllByAsync()
        {
            try
            {
                List<NAVItemLinksModel> aCItemsModels = new List<NAVItemLinksModel>();
                var parameters = new DynamicParameters();
                var query = "select t1.*,t3.UserName as AddedByUser,t4.UserName as ModifiedByUser,t6.CodeValue as StatusCode,t5.No as MalaysiaItemNo,t2.No as SingaporeItemNo from NavitemLinks t1\r\nLEFT JOIN NAVItems t2 ON t1.SgItemId=t2.ItemId \r\nLEFT JOIN ApplicationUser t3 ON t1.AddedByUserID=t3.UserID\r\nLEFT JOIN ApplicationUser t4 ON t1.ModifiedByUserID=t4.UserID\r\nLEFT JOIN NAVItems t5 ON t1.MyItemId=t5.ItemId\r\nLEFT JOIN CodeMaster t6 ON t1.StatusCodeID=t6.CodeID";
                using (var connection = CreateConnection())
                {
                    aCItemsModels = (await connection.QueryAsync<NAVItemLinksModel>(query, parameters)).ToList();
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
        public async Task<NAVItemLinksModel> InsertOrUpdateNavitemLinks(NAVItemLinksModel value)
        {
            try
            {
                var oldData = await _auditLogQueryRepository.GetDataSourceOldData("NavitemLinks", "ItemLinkId", value.ItemLinkId);
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("SgItemId", value.SgItemId);
                    parameters.Add("MyItemId", value.MyItemId);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    string? AuditType = "Added";
                    if (value.ItemLinkId > 0)
                    {

                        AuditType = "Modified";
                    }
                    else
                    {

                    }
                    value.ItemLinkId = await InsertOrUpdate("NavitemLinks", "ItemLinkId", value.ItemLinkId, parameters);

                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = new LowercaseContractResolver(),
                        Formatting = Formatting.Indented
                    };
                    string strJson = JsonConvert.SerializeObject(value, settings);
                    AuditLog auditLog = new AuditLog();
                    auditLog.OldData = oldData;
                    auditLog.NewData = JsonConvert.DeserializeObject(strJson);
                    auditLog.TableName = "NavitemLinks"; auditLog.AuditType = AuditType; auditLog.AuditByUserId = value.ModifiedByUserID; auditLog.PrimaryKeyValue = value.ItemLinkId;
                    await _auditLogQueryRepository.InsertAuditLog(auditLog);
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<NAVItemLinksModel> DeleteNavitemLinks(NAVItemLinksModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ItemLinkId", value.ItemLinkId);
                        var query = "DELETE FROM NavitemLinks WHERE ItemLinkId= @ItemLinkId;";
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
