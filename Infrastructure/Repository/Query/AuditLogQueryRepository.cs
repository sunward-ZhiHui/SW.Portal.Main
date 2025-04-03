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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;
using static iTextSharp.text.pdf.AcroFields;
using System.Dynamic;
using System.Data.SqlClient;
using Newtonsoft.Json;
using static Infrastructure.Repository.Query.ACEntrysQueryRepository;

namespace Infrastructure.Repository.Query
{
    public class AuditLogQueryRepository : DbConnector, IAuditLogQueryRepository
    {
        public AuditLogQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<AuditLog>> GetAuditLog(AuditLog auditLog)
        {
            try
            {
                var result = await GetAuditLogTableData(auditLog.TableName, true);
                List<AuditLog> auditLogs = new List<AuditLog>();
                var parameters = new DynamicParameters();
                parameters.Add("TableName", auditLog.TableName, DbType.String);
                parameters.Add("PrimaryKeyValue", auditLog.PrimaryKeyValue);

                var query = "select t1.* ,t2.UserName as AuditByUser from AuditLog t1 \r\nLEFT JOIN ApplicationUser t2 ON t2.UserID=t1.AuditByUserId where t1.TableName=@TableName AND t1.PrimaryKeyValue=@PrimaryKeyValue\r";
                query += "\rAND ((t1.OldValue!=null OR t1.OldValue!='' ) OR  (t1.NewValue!=null OR t1.NewValue!='' )) AND t1.OldValue!=t1.NewValue\r";
                if (!string.IsNullOrEmpty(auditLog.ColumnName))
                {
                    var split = auditLog.ColumnName.Split(",").ToList();
                    query += "\rAND t1.ColumnName IN(" + string.Join(",", split.Select(x => string.Format("'{0}'", x)).ToList()) + ");";
                }
                using (var connection = CreateConnection())
                {
                    auditLogs = (await connection.QueryAsync<AuditLog>(query, parameters)).ToList();
                }
                if (auditLogs != null && auditLogs.Count > 0)
                {
                    auditLogs.ForEach(s =>
                    {
                        s.OldValueName = s.OldValue;
                        s.NewValueName = s.NewValue;
                        s.ReferencedColumnName = result.Foreign_Key_Table_Schema.Where(w => w.REFERENCED_TABLE_NAME == s.ForeignKeyName && w.FK_COLUMN_NAME.ToLower() == s.ColumnName.ToLower()).FirstOrDefault()?.REFERENCED_COLUMN_NAME;
                        s.ColumnNames = s.ColumnName;
                        if (!string.IsNullOrEmpty(s.ColumnName))
                        {
                            string? mystring = s.ColumnName;
                            string? isId = mystring.Substring(mystring.Length - 2);
                            if (isId.ToLower() == "id")
                            {
                                s.ColumnNames = mystring.Remove(mystring.Length - 2);
                            }
                            var exits = result.Table_Schema.Where(w => w.COLUMN_NAME.ToLower() == s.ColumnName.ToLower()).FirstOrDefault();
                            s.DataType = exits?.DATA_TYPE.ToLower();
                        }
                    });
                }
                auditLogs = auditLogs == null ? new List<AuditLog>() : auditLogs;
                auditLogs = await ReferenceauditLogs(auditLogs);
                return auditLogs;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<AuditLog>> ReferenceauditLogs(List<AuditLog> auditLog)
        {
            List<string?> ForeignKeyNames = new List<string?>();
            ForeignKeyNames = auditLog.Where(w => w.ForeignKeyName != "").Select(s => s.ForeignKeyName).Distinct().ToList();
            if (ForeignKeyNames != null && ForeignKeyNames.Count > 0)
            {
                foreach (var foreignKey in ForeignKeyNames)
                {
                    var listData = auditLog.Where(w => w.ForeignKeyName == foreignKey).ToList();
                    List<string?> ids = new List<string?>();
                    ids.AddRange(listData.Where(w => w.NewValue != null && w.NewValue != "").Select(s => s.NewValue).ToList());
                    ids.AddRange(listData.Where(w => w.OldValue != null && w.OldValue != "").Select(s => s.OldValue).ToList());
                    ids = ids.Distinct().ToList();
                    var tableList = await GetAuditLogTableData(foreignKey, false);
                    List<string> dataTypes = new List<string>() { "nvarchar", "varchar" };
                    var schemData = tableList.Table_Schema.Where(w => dataTypes.Contains(w.DATA_TYPE.ToLower())).Select(s => s.COLUMN_NAME).Distinct().ToList();
                    if (schemData.Count > 0)
                    {
                        var schemDatas = schemData.Take(3).ToList();
                        string? concatStr = listData.FirstOrDefault()?.ReferencedColumnName;
                        if (schemDatas.Count() > 0)
                        {
                            concatStr = string.Empty;
                            int counts = schemDatas.Count(); int i = 1;
                            schemDatas.ForEach(q =>
                            {
                                var last = schemDatas.ToList().Last();
                                concatStr += q;
                                if (i == counts)
                                {

                                }
                                else
                                {
                                    concatStr += ",'|',";
                                }
                                i++;
                            });
                        }
                        if (ids.Count() > 0)
                        {
                            var querys = "Select " + listData.FirstOrDefault()?.ReferencedColumnName + " as UniqueId,CONCAT(" + concatStr + ") as ResultName from " + foreignKey + " WHERE " + listData.FirstOrDefault()?.ReferencedColumnName + " IN (" + string.Join(',', ids) + ")";
                            var resultData = await ExecuteQueryAsync(querys);
                            if (listData.Count() > 0)
                            {
                                listData.ForEach(q =>
                                {
                                    var indexOf = auditLog.IndexOf(q);
                                    if (!string.IsNullOrEmpty(q.OldValue))
                                    {
                                        if (q.DataType == "bigint")
                                        {
                                            auditLog[indexOf].OldValueName = resultData.Cast<dynamic>().Where(s => s.UniqueId == Convert.ToInt64(q.OldValue)).FirstOrDefault()?.ResultName;
                                        }
                                        else if (q.DataType == "int")
                                        {
                                            auditLog[indexOf].OldValueName = resultData.Cast<dynamic>().Where(s => s.UniqueId == Convert.ToInt64(q.OldValue)).FirstOrDefault()?.ResultName;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(q.NewValue))
                                    {
                                        if (q.DataType == "bigint")
                                        {
                                            auditLog[indexOf].NewValueName = resultData.Cast<dynamic>().Where(s => s.UniqueId == Convert.ToInt64(q.NewValue)).FirstOrDefault()?.ResultName;
                                        }
                                        else if (q.DataType == "int")
                                        {
                                            auditLog[indexOf].NewValueName = resultData.Cast<dynamic>().Where(s => s.UniqueId == Convert.ToInt64(q.NewValue)).FirstOrDefault()?.ResultName;
                                        }

                                    }
                                });
                            }
                        }
                    }
                }
            }
            return auditLog;
        }
        public class AuditLogTableDataAll
        {
            public List<Table_Schema> Table_Schema { get; set; } = new List<Table_Schema>();
            public List<Foreign_Key_Table_Schema> Foreign_Key_Table_Schema { get; set; } = new List<Foreign_Key_Table_Schema>();
        }
        public async Task<AuditLogTableDataAll> GetAuditLogTableData(string? TableName, bool? isForeignTable)
        {
            try
            {
                AuditLogTableDataAll AuditLogTableDataAll = new AuditLogTableDataAll();
                var parameters1 = new DynamicParameters();
                parameters1.Add("TableName", TableName, DbType.String);
                var query = ";WITH DetailInfo\r\nAS(\r\nSELECT \r\n    o.name TABLE_NAME,\r\n    c.name COLUMN_NAME,\r\n    t.Name DATA_TYPE,\r\n    c.max_length CHARACTER_MAXIMUM_LENGTH,\r\n    c.is_nullable IsNullabled,\r\n    ISNULL(i.is_primary_key, 0) 'IsPrimaryKey',\r\n    ISNULL(i.is_unique_constraint, 0) 'IsUniqueKey',\r\n    ISNULL(i.name, 0) 'IsIndexName',\r\n    ISNULL(i.type_desc, 0) 'IsIndexType',\r\n    ISNULL(i.is_disabled, 0) 'IsIndexDisabled'\r\nFROM  sys.objects o\r\nINNER JOIN sys.columns c ON o.object_id = c.object_id\r\nINNER JOIN sys.types t ON c.user_type_id = t.user_type_id\r\nLEFT OUTER JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id\r\nLEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id\r\n)\r\nSELECT * FROM DetailInfo\r\nWHERE TABLE_NAME = @TableName;";
                if (isForeignTable == true)
                {
                    query += "SELECT \r\n    KCU1.CONSTRAINT_NAME AS FK_CONSTRAINT_NAME \r\n    ,KCU1.TABLE_NAME AS FK_TABLE_NAME \r\n    ,KCU1.COLUMN_NAME AS FK_COLUMN_NAME \r\n    ,KCU2.CONSTRAINT_NAME AS REFERENCED_CONSTRAINT_NAME \r\n    ,KCU2.TABLE_NAME AS REFERENCED_TABLE_NAME \r\n    ,KCU2.COLUMN_NAME AS REFERENCED_COLUMN_NAME \r\nFROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS RC \r\n\r\nINNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU1 \r\n    ON KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG  \r\n    AND KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA \r\n    AND KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME \r\n\r\nINNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU2 \r\n    ON KCU2.CONSTRAINT_CATALOG = RC.UNIQUE_CONSTRAINT_CATALOG  \r\n    AND KCU2.CONSTRAINT_SCHEMA = RC.UNIQUE_CONSTRAINT_SCHEMA \r\n    AND KCU2.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME \r\n    AND KCU2.ORDINAL_POSITION = KCU1.ORDINAL_POSITION \r\n\twhere KCU1.TABLE_NAME=@TableName;";
                }
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters1);
                    AuditLogTableDataAll.Table_Schema = results.Read<Table_Schema>().ToList();
                    if (isForeignTable == true)
                    {
                        AuditLogTableDataAll.Foreign_Key_Table_Schema = results.Read<Foreign_Key_Table_Schema>().ToList();
                    }
                }
                return AuditLogTableDataAll;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<List<object>> ExecuteQueryAsync(object sql)
        {
            List<object> result = new List<object>();
            using (var connection = CreateConnection())
            {
                using (SqlCommand command = new SqlCommand((string)sql, (SqlConnection)connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string name = string.Empty; object value = string.Empty;
                            IDictionary<string, object> objectDataList = new ExpandoObject();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                name = reader.GetName(i); value = reader.GetValue(i);
                                objectDataList[name] = value;
                            }
                            result.Add(objectDataList);
                        }
                    }
                    connection.Close();
                }

            }

            return result;
        }
        public async Task<AuditLog> InsertAuditParentLog(AuditLog auditLog)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var result = await GetAuditLogTableData(auditLog.TableName, true);
                        var res = result.Table_Schema;
                        var res1 = result.Foreign_Key_Table_Schema;
                        if (res != null && res.Count > 0)
                        {

                            foreach (var s in res)
                            {
                                bool? IsModified = false;
                                if (auditLog.NewValue != auditLog.OldValue)
                                {
                                    IsModified = true;
                                }
                                var parameters = new DynamicParameters();
                                parameters.Add("AuditType", auditLog.AuditType, DbType.String);
                                parameters.Add("TableName", auditLog.TableName, DbType.String);
                                parameters.Add("IsPrimaryKey", s.IsPrimaryKey);
                                parameters.Add("PrimaryKeyName", s.IsPrimaryKey == true ? s.COLUMN_NAME : string.Empty);
                                parameters.Add("PrimaryKeyValue", auditLog.PrimaryKeyValue);
                                parameters.Add("IsModified", IsModified);
                                parameters.Add("ColumnName", s.COLUMN_NAME, DbType.String);
                                parameters.Add("AuditDate", DateTime.Now);
                                parameters.Add("AuditByUserId", auditLog.AuditByUserId);
                                parameters.Add("NewValue", auditLog.NewValue, DbType.String);
                                parameters.Add("OldValue", auditLog.NewValue, DbType.String);
                                bool? IsForeignKey = false; string? ForeignKeyName = string.Empty;
                                if (!string.IsNullOrEmpty(s.COLUMN_NAME))
                                {
                                    var Foreign_Keys = res1.Where(w => w.FK_COLUMN_NAME != null && w.FK_COLUMN_NAME.ToLower() == s.COLUMN_NAME.ToLower()).FirstOrDefault();
                                    if (Foreign_Keys != null)
                                    {
                                        IsForeignKey = true;
                                        ForeignKeyName = Foreign_Keys?.REFERENCED_TABLE_NAME;
                                    }
                                }
                                parameters.Add("IsForeignKey", IsForeignKey);
                                parameters.Add("ForeignKeyName", ForeignKeyName, DbType.String);
                                var Aquery = "INSERT INTO AuditLog(PrimaryKeyValue,OldValue,NewValue,AuditType,TableName,IsPrimaryKey,PrimaryKeyName,IsModified," +
                         "ColumnName,AuditDate,AuditByUserId,IsForeignKey,ForeignKeyName) OUTPUT INSERTED.AuditLogId VALUES " +
                            "(@PrimaryKeyValue,@OldValue,@NewValue,@AuditType,@TableName,@IsPrimaryKey,@PrimaryKeyName,@IsModified,@ColumnName,@AuditDate,@AuditByUserId,@IsForeignKey,@ForeignKeyName)";
                                var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(Aquery, parameters);

                            }
                        }
                        return auditLog;
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
        public async Task<AuditLog> InsertAuditLog(AuditLog auditLog)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var result = await GetAuditLogTableData(auditLog.TableName, true);
                        var res = result.Table_Schema;
                        var res1 = result.Foreign_Key_Table_Schema;
                        if (res != null && res.Count > 0)
                        {
                            foreach (var s in res)
                            {
                                string? newValue = string.Empty;
                                if (auditLog.NewData != null)
                                {
                                    var Names = auditLog.NewData.ContainsKey(s.COLUMN_NAME.ToLower());
                                    
                                    if (Names == true)
                                    {
                                        var value = auditLog.NewData[s.COLUMN_NAME.ToLower()];
                                        if (value != null)
                                        {
                                            newValue = (string?)value;
                                        }
                                    }
                                }
                                bool? IsModified = false;
                                string? OldValue = string.Empty;
                                if (auditLog.OldData != null)
                                {
                                    var oldName = auditLog.OldData.ContainsKey(s.COLUMN_NAME.ToLower());

                                    if (oldName == true)
                                    {
                                        var value = auditLog.OldData[s.COLUMN_NAME.ToLower()];
                                        if (value != null)
                                        {
                                            OldValue = (string?)value;
                                        }
                                    }
                                }
                                if (newValue != OldValue)
                                {
                                    IsModified = true;
                                }
                                var parameters = new DynamicParameters();
                                parameters.Add("AuditType", auditLog.AuditType, DbType.String);
                                parameters.Add("TableName", auditLog.TableName, DbType.String);
                                parameters.Add("IsPrimaryKey", s.IsPrimaryKey);
                                parameters.Add("PrimaryKeyName", s.IsPrimaryKey == true ? s.COLUMN_NAME : string.Empty);
                                parameters.Add("PrimaryKeyValue", auditLog.PrimaryKeyValue);
                                parameters.Add("IsModified", IsModified);
                                parameters.Add("ColumnName", s.COLUMN_NAME, DbType.String);
                                parameters.Add("AuditDate", DateTime.Now);
                                parameters.Add("AuditByUserId", auditLog.AuditByUserId);
                                parameters.Add("NewValue", newValue, DbType.String);
                                parameters.Add("OldValue", OldValue, DbType.String);
                                parameters.Add("ParentPrimaryKeyValue", auditLog.ParentPrimaryKeyValue, DbType.String);
                                bool? IsForeignKey = false; string? ForeignKeyName = string.Empty;
                                if (!string.IsNullOrEmpty(s.COLUMN_NAME))
                                {
                                    var Foreign_Keys = res1.Where(w => w.FK_COLUMN_NAME != null && w.FK_COLUMN_NAME.ToLower() == s.COLUMN_NAME.ToLower()).FirstOrDefault();
                                    if (Foreign_Keys != null)
                                    {
                                        IsForeignKey = true;
                                        ForeignKeyName = Foreign_Keys?.REFERENCED_TABLE_NAME;
                                    }
                                }
                                parameters.Add("IsForeignKey", IsForeignKey);
                                parameters.Add("ForeignKeyName", ForeignKeyName, DbType.String);
                                var Aquery = "INSERT INTO AuditLog(ParentPrimaryKeyValue,PrimaryKeyValue,OldValue,NewValue,AuditType,TableName,IsPrimaryKey,PrimaryKeyName,IsModified," +
                         "ColumnName,AuditDate,AuditByUserId,IsForeignKey,ForeignKeyName) OUTPUT INSERTED.AuditLogId VALUES " +
                            "(@ParentPrimaryKeyValue,@PrimaryKeyValue,@OldValue,@NewValue,@AuditType,@TableName,@IsPrimaryKey,@PrimaryKeyName,@IsModified,@ColumnName,@AuditDate,@AuditByUserId,@IsForeignKey,@ForeignKeyName)";
                                var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(Aquery, parameters);
                            }
                        }
                        return auditLog;
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
        public async Task<object?> GetDataSourceOldData(string? tableName, string? PrimaryKeyName, long? Id)
        {
            try
            {
                object? objects = null;
                var query = "select t1.* from " + tableName + " t1 where " + PrimaryKeyName + "=" + Id + "";
                using (var connection = CreateConnection())
                {
                    var res = await ExecuteQueryAsync(query);
                    if (res != null && res.Count > 0)
                    {
                        var results = res.FirstOrDefault();
                        var settings = new JsonSerializerSettings
                        {
                            ContractResolver = new LowercaseContractResolver(),
                            Formatting = Formatting.Indented
                        };
                        string strJson = JsonConvert.SerializeObject(results, settings);
                        objects = JsonConvert.DeserializeObject(strJson);
                    }
                    return objects;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }

}
