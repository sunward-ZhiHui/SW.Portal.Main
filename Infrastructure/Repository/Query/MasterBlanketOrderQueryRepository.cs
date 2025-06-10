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

namespace Infrastructure.Repository.Query
{
    public class MasterBlanketOrderQueryRepository : DbConnector, IMasterBlanketOrderQueryRepository
    {
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        public MasterBlanketOrderQueryRepository(IConfiguration configuration, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository)
            : base(configuration)
        {
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
        }
        public async Task<IReadOnlyList<MasterBlanketOrderModel>> GetAllByAsync()
        {
            try
            {
                var query = "select t1.*,t2.PlantCode as CompanyName,t3.UserName as AddedByUser,t4.UserName as ModifiedByUser,t5.CodeValue as StatusCode,t6.CompanyName as CustomerName from MasterBlanketOrder t1\r\nLEFT JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\nLEFT JOIN ApplicationUser t3 ON t1.AddedByUserID=t3.UserID\r\nLEFT JOIN ApplicationUser t4 ON t1.ModifiedByUserID=t4.UserID\r\nLEFT JOIN CodeMaster t5 ON t1.StatusCodeID=t5.CodeID\r\nLEFT JOIN CompanyListing t6 ON t1.CustomerID=t6.CompanyListingID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<MasterBlanketOrderModel>(query)).ToList();
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
        public async Task<MasterBlanketOrderModel> InsertOrUpdateMasterBlanketOrder(MasterBlanketOrderModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("CompanyID", value.CompanyID);
                        parameters.Add("FromPeriod", value.FromPeriod, DbType.DateTime);
                        parameters.Add("ToPeriod", value.ToPeriod, DbType.DateTime);
                        parameters.Add("CustomerID", value.CustomerID);
                        parameters.Add("IsRequireVersionInformation", value.IsRequireVersionInformation);
                        parameters.Add("CustomerName", value.CustomerName, DbType.String);
                        parameters.Add("CompanyName", value.CompanyName);
                        parameters.Add("VersionSessionId", value.VersionSessionId);
                        parameters.Add("StatusCodeID", value.StatusCodeID);
                        parameters.Add("AddedByUserID", value.AddedByUserID);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                        parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);

                        value.MasterBlanketOrderId = await InsertOrUpdate("MasterBlanketOrder", "MasterBlanketOrderId", value.MasterBlanketOrderId, parameters);

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
        public async Task<MasterBlanketOrderModel> DeleteMasterBlanketOrder(MasterBlanketOrderModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("MasterBlanketOrderId", value.MasterBlanketOrderId);
                        var query = "DELETE FROM MasterBlanketOrderLine WHERE MasterBlanketOrderId= @MasterBlanketOrderId;";
                        query += "DELETE FROM MasterBlanketOrder WHERE MasterBlanketOrderId= @MasterBlanketOrderId";
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
