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

namespace Infrastructure.Repository.Query
{
    public class ACEntrysQueryRepository : DbConnector, IACEntrysQueryRepository
    {
        private readonly IAuditLogQueryRepository _auditLogQueryRepository;
        public ACEntrysQueryRepository(IConfiguration configuration, IAuditLogQueryRepository auditLogQueryRepository)
            : base(configuration)
        {
            _auditLogQueryRepository = auditLogQueryRepository;
        }
        public async Task<IReadOnlyList<ACEntryModel>> GetAllByAsync()
        {
            try
            {
                var query = "select t1.*,t2.Description as CompanyName,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser,t6.Name as CustomerName from Acentry t1\r\nJOIN Plant t2 ON t2.PlantID=t1.CompanyId\r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nJOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\nLEFT JOIN Navcustomer t6 ON t6.CustomerId=t1.CustomerId";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ACEntryModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        
        public async Task<IReadOnlyList<ACEntryLinesModel>> GetACEntryLinesByAsync(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ACEntryId", Id);
                var query = "select \r\n(select TOP(1)tt2.ItemNo  from NavItemCitemList tt1 LEFT JOIN Acitems  tt2 ON tt2.DistACID=tt1.NavItemCustomerItemId Where tt1.NavItemId=t1.ItemId order by tt1.NavItemCItemId asc ) as AcItemNo,\r\nt1.*,t8.Code as GenericCode,t3.CodeValue as StatusCode,t7.PlantCode as CompanyName,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser,t6.No as ItemName,t6.Description,t6.Description2,t6.BaseUnitofMeasure as BUOM,t6.ItemCategoryCode as ItemCategory,t6.PackSize,t6.PackUOM,t6.VendorNo from ACEntryLines t1\r\nJOIN ACEntry t2 ON t2.ACEntryId=t1.ACEntryId\r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nJOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\nJOIN NAVItems t6 ON t6.ItemId=t1.ItemId\r\nJOIN Plant t7 ON t7.PlantID=t6.CompanyId\r\nLEFT JOIN GenericCodes t8 ON t8.GenericCodeId=t6.GenericCodeId where t1.ACEntryId=@ACEntryId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ACEntryLinesModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<NavCustomerModel>> GetNavCustomerAsync()
        {
            try
            {
                var query = "select * from NavCustomer";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<NavCustomerModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ACEntryModel> DeleteACEntry(ACEntryModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ACEntryId", value.ACEntryId);
                        var query = "DELETE FROM ACEntryLines WHERE ACEntryId= @ACEntryId;";
                        query += "DELETE FROM Acentry WHERE ACEntryId= @ACEntryId";
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
        public async Task<ACEntryModel> InsertOrUpdateAcentry(ACEntryModel value)
        {
            try
            {
                var oldData = await _auditLogQueryRepository.GetDataSourceOldData("Acentry", "ACEntryId", value.ACEntryId);
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyId", value.CompanyId);
                    parameters.Add("CustomerId", value.CustomerId);
                    parameters.Add("Version", value.Version, DbType.String);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("FromDate", value.FromDate, DbType.DateTime);
                    parameters.Add("ToDate", value.ToDate);
                    parameters.Add("ACEntryId", value.ACEntryId);
                    parameters.Add("Remark", value.Remark, DbType.String);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    parameters.Add("SessionId", value.SessionId, DbType.Guid);
                    string? AuditType = "Added";
                    if (value.ACEntryId > 0)
                    {
                        var query = "UPDATE Acentry SET AddedByUserID=@AddedByUserID,ModifiedByUserID=@ModifiedByUserID,AddedDate=@AddedDate,ModifiedDate=@ModifiedDate,SessionId=@SessionId,Remark=@Remark,CompanyId=@CompanyId,CustomerId=@CustomerId,Version=@Version,StatusCodeID = @StatusCodeID,FromDate =@FromDate,ToDate =@ToDate\r" +
                            "WHERE ACEntryId = @ACEntryId";
                        AuditType = "Modified";
                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO Acentry(Remark,CompanyId,CustomerId,Version,StatusCodeID,FromDate,ToDate,SessionId,AddedByUserID," +
                         "ModifiedByUserID,AddedDate,ModifiedDate) OUTPUT INSERTED.ACEntryId VALUES " +
                            "(@Remark,@CompanyId,@CustomerId,@Version,@StatusCodeID,@FromDate,@ToDate,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        value.ACEntryId = rowsAffected;
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
                    auditLog.TableName = "Acentry"; auditLog.AuditType = AuditType; auditLog.AuditByUserId = value.ModifiedByUserID; auditLog.PrimaryKeyValue = value.ACEntryId;
                    await _auditLogQueryRepository.InsertAuditLog(auditLog);
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower(); // Convert property names to lowercase
            }
        }
        public async Task<ACEntryLinesModel> InsertOrUpdateAcentryLine(ACEntryLinesModel value)
        {
            try
            {
                var oldData = await _auditLogQueryRepository.GetDataSourceOldData("ACEntryLines", "ACEntryLineId", value.ACEntryLineId);
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ItemId", value.ItemId);
                    parameters.Add("Quantity", value.Quantity);
                    parameters.Add("ACEntryId", value.ACEntryId);
                    parameters.Add("ACEntryLineId", value.ACEntryLineId);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    parameters.Add("SessionId", value.SessionId, DbType.Guid);
                    string? AuditType = "Added";
                    if (value.ACEntryLineId > 0)
                    {
                        var query = "UPDATE ACEntryLines SET StatusCodeID=@StatusCodeID,ACEntryId=@ACEntryId,AddedByUserID=@AddedByUserID,ModifiedByUserID=@ModifiedByUserID,AddedDate=@AddedDate," +
                            "ModifiedDate=@ModifiedDate,ItemId=@ItemId,Quantity =@Quantity\r" +
                            "WHERE ACEntryLineId = @ACEntryLineId";
                        AuditType = "Modified";
                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO ACEntryLines(StatusCodeID,ACEntryId,ItemId,Quantity,AddedByUserID," +
                         "ModifiedByUserID,AddedDate,ModifiedDate) OUTPUT INSERTED.ACEntryLineId VALUES " +
                            "(@StatusCodeID,@ACEntryId,@ItemId,@Quantity,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        value.ACEntryLineId = rowsAffected;
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
                    auditLog.TableName = "ACEntryLines"; auditLog.AuditType = AuditType; auditLog.AuditByUserId = value.ModifiedByUserID; auditLog.PrimaryKeyValue = value.ACEntryLineId;
                    await _auditLogQueryRepository.InsertAuditLog(auditLog);
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }

        public async Task<ACEntryLinesModel> DeleteACEntryLine(ACEntryLinesModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ACEntryLineId", value.ACEntryLineId);
                        var query = "DELETE FROM ACEntryLines WHERE ACEntryLineId= @ACEntryLineId;";
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
        public async Task<ACEntryModel> getAcentry(ACEntryModel acentry)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("ACEntryId", acentry.ACEntryCopyId);
                var query = "select * from Acentry where ACEntryId=@ACEntryId;";
                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<ACEntryModel>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ACEntryModel> CopyAcentry(ACEntryModel aCEntryModel)
        {
            var result = await getAcentry(aCEntryModel);
            if (result != null)
            {
                aCEntryModel.FromDate = aCEntryModel.FromDate.Value.AddDays(1);
                aCEntryModel.ToDate = aCEntryModel.ToDate.Value.AddMonths(1);
                var resultData = await InsertOrUpdateAcentry(aCEntryModel);
                if (resultData != null)
                {
                    var linesData = await GetACEntryLinesByAsync(aCEntryModel.ACEntryCopyId);
                    if (linesData != null && linesData.Count() > 0)
                    {
                        var lineData = linesData.ToList();
                        foreach (var line in lineData)
                        {
                            line.AddedByUserID = aCEntryModel.AddedByUserID;
                            line.ModifiedByUserID = aCEntryModel.ModifiedByUserID;
                            line.ModifiedDate = aCEntryModel.ModifiedDate;
                            line.AddedDate = aCEntryModel.AddedDate;
                            line.ACEntryLineId = 0;
                            line.ACEntryId = resultData.ACEntryId;
                            await InsertOrUpdateAcentryLine(line);
                        }
                    }
                }
            }
            return aCEntryModel;
        }
    }

}
