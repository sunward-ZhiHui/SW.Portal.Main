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
    public class SalesOrderQueryRepository : DbConnector, ISalesOrderQueryRepository
    {
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        public SalesOrderQueryRepository(IConfiguration configuration, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository)
            : base(configuration)
        {
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
        }
        public async Task<IReadOnlyList<SalesOrderModel>> GetAllByAsync()
        {
            try
            {
                var query = "select t1.*,t2.Name as ProfileName,t3.CompanyName as Customer,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser,t6.PlantCode as PurchaseOrderIssue from SalesOrder t1\r\nLEFT JOIN DocumentProfileNoSeries t2 ON t2.ProfileID=t1.ProfileID\r\nLEFT JOIN CompanyListing t3 ON t3.CompanyListingID=t1.CustomerID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\nLEFT JOIN Plant t6 ON t6.PlantID=t1.PurchaseOrderIssueID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<SalesOrderModel>(query)).ToList();
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
        public async Task<SalesOrderModel> InsertOrUpdateSalesOrder(SalesOrderModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProfileId", value.ProfileId);
                        parameters.Add("DateOfOrder", value.DateOfOrder, DbType.DateTime);
                        parameters.Add("Ponumber", value.Ponumber, DbType.String);
                        parameters.Add("PurchaseOrderIssueId", value.PurchaseOrderIssueId);
                        parameters.Add("CustomerId", value.CustomerId);
                        parameters.Add("RequestShipmentDate", value.RequestShipmentDate, DbType.DateTime);
                        parameters.Add("ShipingCodeType", value.ShipingCodeType, DbType.String);
                        parameters.Add("ShipToCodeId", value.ShipingCodeType == "SobyCustomersAddress" ? value.ShipToCodeId : null);
                        parameters.Add("SobyCustomersSalesAddressId", value.ShipingCodeType == "SOByCustomersSalesAddress" ? value.ShipToCodeId : null);
                        parameters.Add("VanDeliveryDate", value.VanDeliveryDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", value.StatusCodeID);
                        parameters.Add("AddedByUserID", value.AddedByUserID);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                        parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                        var ProfileNo = string.Empty;
                        if (value.SalesOrderId > 0)
                        {

                        }
                        else
                        {
                            if (value.ProfileId > 0)
                            {
                                ProfileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(new DocumentNoSeriesModel { ProfileID = value.ProfileId, AddedByUserID = value.AddedByUserID, StatusCodeID = 710, Title = "Sales Order" });
                                value.ProfileName = ProfileNo;
                            }
                            parameters.Add("ProfileName", value.ProfileName);
                        }
                        value.SalesOrderId = await InsertOrUpdate("SalesOrder", "SalesOrderId", value.SalesOrderId, parameters);
                        if (value.SalesOrderId > 0)
                        {

                        }
                        else
                        {

                        }
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
        public async Task<SalesOrderModel> DeleteSalesOrder(SalesOrderModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("SalesOrderId", value.SalesOrderId);
                        var query = "DELETE FROM SalesOrderProduct WHERE SalesOrderId= @SalesOrderId;";
                        query += "DELETE FROM SowithOutBlanketOrder WHERE SalesOrderId= @SalesOrderId";
                        query += "DELETE FROM SalesOrder WHERE SalesOrderId= @SalesOrderId";
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
