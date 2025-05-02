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
    public class ACItemsQueryRepository : DbConnector, IACItemsQueryRepository
    {
        private readonly IAuditLogQueryRepository _auditLogQueryRepository;
        public ACItemsQueryRepository(IConfiguration configuration, IAuditLogQueryRepository auditLogQueryRepository)
            : base(configuration)
        {
            _auditLogQueryRepository = auditLogQueryRepository;
        }
        public async Task<IReadOnlyList<ACItemsModel>> GetAllByAsync(ACItemsModel aCItemsModel)
        {
            try
            {
                var categoryList = new List<string>
                {
                    "CAP",
                    "CREAM",
                    "DD",
                    "SYRUP",
                    "TABLET",
                    "VET",
                    "POWDER",
                    "INJ"
                };
                List<ACItemsModel> aCItemsModels = new List<ACItemsModel>();
                var navItemCList = new List<ACItemsModel>();
                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", aCItemsModel.CompanyId);
                parameters.Add("CustomerId", aCItemsModel.CustomerId);
                var query = "select t1.*,t2.PlantCode as CompanyName,t3.UserName as AddedByUser,t4.UserName as ModifiedByUser,t5.Name as CustomerNmae from ACItems t1\r\nLEFT JOIN Plant t2 ON t1.CompanyId=t2.PlantID \r\nLEFT JOIN ApplicationUser t3 ON t1.AddedByUserID=t3.UserID\r\n" +
                    "LEFT JOIN ApplicationUser t4 ON t1.ModifiedByUserID=t4.UserID\r\nLEFT JOIN Navcustomer t5 ON t1.CustomerId=t5.CustomerId\r\nwhere t1.CompanyId=@CompanyId AND t1.StatusCodeID=1";
                if (aCItemsModel.CustomerId > 0)
                {
                    query += "\r AND t1.CustomerId=@CustomerId;";
                }
                using (var connection = CreateConnection())
                {
                    aCItemsModels = (await connection.QueryAsync<ACItemsModel>(query, parameters)).ToList();

                    if (aCItemsModels != null && aCItemsModels.Count > 0)
                    {
                        var acItemIDs = aCItemsModels.Select(c => c.DistACID).ToList();

                        var query1 = "select t1.*,t2.PackSize,t2.Packuom,t3.Code as GenericCode,t2.No as SWItemNo,t2.Description as SWItemDesc,t2.Description2 as SWItemDesc2,\r\nt2.ItemCategoryCode as CategoryCode,t2.ExpirationCalculation as ShelfLife,t2.InternalRef as InternalRefNo from NavItemCitemList t1\r\n" +
                            "LEFT JOIN NAVItems t2 ON t2.ItemId=t1.NavItemId\r\n" +
                            "LEFT JOIN GenericCodes t3 ON t3.GenericCodeId=t2.GenericCodeId where t2.CompanyId=@CompanyId AND t1.NavItemCustomerItemId in(" + string.Join(',', acItemIDs) + ");";
                        navItemCList = (await connection.QueryAsync<ACItemsModel>(query1, parameters)).ToList();
                    }
                }
                if (aCItemsModels != null && aCItemsModels.Count > 0)
                {
                    aCItemsModels.ForEach(a =>
                    {
                        var navCItem = navItemCList.Where(w => w.NavItemId > 0).FirstOrDefault(c => c.NavItemCustomerItemId == a.DistACID);
                        if (navCItem != null)
                        {
                            var navItem = navCItem;
                            var groupName = navItem != null ? navItem.CategoryId.GetValueOrDefault(0) : 0;
                            a.PackSize = navItem != null && navItem.PackSize != null ? navItem.PackSize.ToString() : string.Empty;
                            a.Packuom = navItem != null ? navItem.Packuom : string.Empty;
                            a.GenericCode = navItem != null ? navItem.GenericCode : string.Empty;
                            a.SWItemNo = navItem != null ? navItem.SWItemNo : "Not Mapped";
                            a.ItemIds = navItem != null ? navItemCList.Where(n => n.NavItemCustomerItemId == navCItem.NavItemCustomerItemId).Where(w => w.NavItemId > 0).Select(n => n.NavItemId).ToList() : new List<long?>();
                            a.CategoryCode = navItem != null ? navItem.CategoryCode : "";
                            //a.ItemGroup = groupName > 0 ? categoryList[int.Parse(groupName.ToString()) - 1].ToString() : "";
                            a.ShelfLife = navItem != null ? navItem.ShelfLife : "";
                            a.InternalRefNo = navItem != null ? navItem.InternalRefNo : "";
                            a.SWItemDesc = navItem != null ? navItem.SWItemDesc : "";
                            a.SWItemDesc2 = navItem != null ? navItem.SWItemDesc2 : "";
                        }
                        else
                        {
                            a.SWItemNo = "Not Mapped";
                        }
                    });
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
        public async Task<ACItemsModel> InsertOrUpdateAcitems(ACItemsModel value)
        {
            try
            {
                var oldData = await _auditLogQueryRepository.GetDataSourceOldData("Acitems", "DistACID", value.DistACID);
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("DistName", value.DistName, DbType.String);
                    parameters.Add("ItemNo", value.ItemNo, DbType.String);
                    parameters.Add("ItemGroup", value.ItemGroup, DbType.String);
                    parameters.Add("Steriod", value.Steriod, DbType.String);
                    parameters.Add("ShelfLife", value.ShelfLife, DbType.String);
                    parameters.Add("Quota", value.Quota, DbType.String);
                    parameters.Add("Status", value.Status, DbType.String);
                    parameters.Add("ItemDesc", value.ItemDesc, DbType.String);
                    parameters.Add("PackSize", value.PackSize, DbType.String);
                    parameters.Add("ACQty", value.ACQty);
                    parameters.Add("ACMonth", value.ACMonth, DbType.DateTime);
                    parameters.Add("CompanyId", value.CompanyId);
                    parameters.Add("CustomerId", value.CustomerId);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    string? AuditType = "Added";
                    if (value.DistACID > 0)
                    {

                        AuditType = "Modified";
                    }
                    else
                    {

                    }
                    value.DistACID = await InsertOrUpdate("Acitems", "DistACID", value.DistACID, parameters);
                    var query2 = string.Empty;
                    query2 += "Delete  FROM NavItemCitemList WHERE NavItemCustomerItemId =" + value.DistACID + ";";
                    await connection.QuerySingleOrDefaultAsync<long>(query2, parameters);
                    if (value.ItemIds != null && value.ItemIds.ToList().Count() > 0)
                    {
                        foreach (var sw in value.ItemIds)
                        {
                            if (sw.GetValueOrDefault(0) > 0)
                            {
                                var query1 = string.Empty;
                                var parameters1 = new DynamicParameters();
                                parameters1.Add("NavItemCustomerItemId", value.DistACID);
                                parameters1.Add("NavItemId", sw);
                                query1 += "\rINSERT INTO [NavItemCitemList](NavItemCustomerItemId,NavItemId) OUTPUT INSERTED.NavItemCitemId " +
                                   "VALUES (@NavItemCustomerItemId,@NavItemId);\r\n";
                                await connection.QuerySingleOrDefaultAsync<long>(query1, parameters1);
                            }
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
                    auditLog.TableName = "Acitems"; auditLog.AuditType = AuditType; auditLog.AuditByUserId = value.ModifiedByUserID; auditLog.PrimaryKeyValue = value.DistACID;
                    await _auditLogQueryRepository.InsertAuditLog(auditLog);
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<ACItemsModel> DeleteACItems(ACItemsModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DistACID", value.DistACID);
                        var query = "DELETE FROM DistStockBalance WHERE DistItemId= @DistACID;";
                        query += "DELETE FROM NavItemCitemList WHERE NavItemCustomerItemId= @DistACID;";
                        query += "DELETE FROM Acitems WHERE DistACID= @DistACID";
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
