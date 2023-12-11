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

namespace Infrastructure.Repository.Query
{
    public class QuotationHistoryQueryRepository : QueryRepository<QuotationHistory>, IQuotationHistoryQueryRepository
    {
        public QuotationHistoryQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<QuotationHistory>> GetAllByAsync()
        {
            try
            {
                var query = "select t1.QuotationHistoryID,t1.CompanyID,t1.SWReferenceNo,t1.Date,t1.CustomerID,t1.CustomerRefNo,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.SessionID,t2.PlantCode,t2.Description as CompanyDescription,\r\nt3.CodeValue as StatusCode,t4.UserName as AddedBy,t5.UserName as ModifiedBy,\r\nt6.SoCustomerID,t6.ShipCode,t6.CustomerName,t6.AssignToRep,t6.Address1,t6.Address2,t6.City,t6.StateCode,t6.PostCode,t6.Channel,t6.Type\r\nfrom QuotationHistory t1\r\nJOIN Plant t2 ON t2.PlantID=t1.CompanyID\r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\nJOIN SoCustomer t6 ON t6.SoCustomerID=t1.CustomerID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<QuotationHistory>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<QuotationHistory> GetQuotationHistoryBySession(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.QuotationHistoryID,t1.CompanyID,t1.SWReferenceNo,t1.Date,t1.CustomerID,t1.CustomerRefNo,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.SessionID,t2.PlantCode,t2.Description as CompanyDescription,\r\nt3.CodeValue as StatusCode,t4.UserName as AddedBy,t5.UserName as ModifiedBy,\r\nt6.SoCustomerID,t6.ShipCode,t6.CustomerName,t6.AssignToRep,t6.Address1,t6.Address2,t6.City,t6.StateCode,t6.PostCode,t6.Channel,t6.Type\r\nfrom QuotationHistory t1\r\nJOIN Plant t2 ON t2.PlantID=t1.CompanyID\r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\nJOIN SoCustomer t6 ON t6.SoCustomerID=t1.CustomerID WHERE t1.SessionId=@SessionId";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<QuotationHistory>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<QuotationHistory> InsertOrUpdateQuotationHistory(QuotationHistory value)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("QuotationHistoryId", value.QuotationHistoryId);
                        parameters.Add("CompanyId", value.CompanyId);
                        parameters.Add("SwreferenceNo", value.SwreferenceNo, DbType.String);
                        parameters.Add("Date", value.Date, DbType.DateTime);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("CustomerId", value.CustomerId);
                        parameters.Add("CustomerRefNo", value.CustomerRefNo, DbType.String);
                        parameters.Add("AddedByUserID", value.AddedByUserId);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserId);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", value.StatusCodeId);
                        if (value.QuotationHistoryId > 0)
                        {
                            var query = " UPDATE QuotationHistory SET CompanyId = @CompanyId,SwreferenceNo =@SwreferenceNo,Date=@Date,CustomerId=@CustomerId,CustomerRefNo=@CustomerRefNo,\n\r" +
                                "ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,SessionId=@SessionId\n\r" +
                                "WHERE QuotationHistoryId = @QuotationHistoryId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            var query = "INSERT INTO QuotationHistory(CompanyId,SwreferenceNo,Date,CustomerId,CustomerRefNo,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,SessionId)  " +
                                "OUTPUT INSERTED.QuotationHistoryId VALUES " +
                                "(@CompanyId,@SwreferenceNo,@Date,@CustomerId,@CustomerRefNo,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@SessionId)";

                            value.QuotationHistoryId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
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
                throw new NotImplementedException();
            }
        }

        public async Task<IReadOnlyList<QuotationHistoryLine>> GetAllByLineAsync(long? quotationHistoryId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("QutationHistoryId", quotationHistoryId);
                var query = "\r\nselect t1.*,\r\nt3.CodeValue as StatusCode,t4.UserName as AddedBy,t5.UserName as ModifiedBy,\r\nt6.Value as Packing,t7.Value as OfferCurrency,t8.Value as OfferUOM,t9.Value as ShippingTerms,\r\nt10.Code as ProductCode,t10.Description as ProductDescription,t10.Description2 as ProductDescription2,t11.Value as ProductUom\r\nfrom QuotationHistoryLine t1\r\nJOIN QuotationHistory t2 ON t2.QuotationHistoryID=t1.QutationHistoryID\r\nLEFT JOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID \r\nLEFT JOIN ApplicationMasterDetail t6 ON t6.ApplicationMasterDetailID=t1.PackingID \r\nLEFT JOIN ApplicationMasterDetail t7 ON t7.ApplicationMasterDetailID=t1.OfferCurrencyID\r\nLEFT JOIN ApplicationMasterDetail t8 ON t8.ApplicationMasterDetailID=t1.OfferUOMID \r\nLEFT JOIN ApplicationMasterDetail t9 ON t9.ApplicationMasterDetailID=t1.ShippingTermsID\r\nLEFT JOIN GenericCodes t10 ON t10.GenericCodeId=t1.ProductID\r\nLEFT JOIN ApplicationMasterDetail t11 ON t10.UOM=t11.ApplicationMasterDetailID WHERE t1.QutationHistoryId=@QutationHistoryId;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<QuotationHistoryLine>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<QuotationHistoryLine> GetQuotationHistoryByLineSession(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "\r\nselect t1.*,\r\nt3.CodeValue as StatusCode,t4.UserName as AddedBy,t5.UserName as ModifiedBy,\r\nt6.Value as Packing,t7.Value as OfferCurrency,t8.Value as OfferUOM,t9.Value as ShippingTerms,\r\nt10.Code as ProductCode,t10.Description as ProductDescription,t10.Description2 as ProductDescription2,t11.Value as ProductUom\r\nfrom QuotationHistoryLine t1\r\nJOIN QuotationHistory t2 ON t2.QuotationHistoryID=t1.QutationHistoryID\r\nLEFT JOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID \r\nLEFT JOIN ApplicationMasterDetail t6 ON t6.ApplicationMasterDetailID=t1.PackingID \r\nLEFT JOIN ApplicationMasterDetail t7 ON t7.ApplicationMasterDetailID=t1.OfferCurrencyID\r\nLEFT JOIN ApplicationMasterDetail t8 ON t8.ApplicationMasterDetailID=t1.OfferUOMID \r\nLEFT JOIN ApplicationMasterDetail t9 ON t9.ApplicationMasterDetailID=t1.ShippingTermsID\r\nLEFT JOIN GenericCodes t10 ON t10.GenericCodeId=t1.ProductID\r\nLEFT JOIN ApplicationMasterDetail t11 ON t10.UOM=t11.ApplicationMasterDetailID WHERE t1.SessionId=@SessionId";
                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<QuotationHistoryLine>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<QuotationHistoryLine> InsertOrUpdateQuotationHistoryLine(QuotationHistoryLine value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("QutationHistoryId", value.QutationHistoryId);
                        parameters.Add("QuotationHistoryLineId", value.QuotationHistoryLineId);
                        parameters.Add("ProductId", value.ProductId);
                        parameters.Add("Source", value.Source, DbType.String);
                        parameters.Add("Quantity", value.Quantity);
                        parameters.Add("Uomid", value.Uomid);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("PackingId", value.PackingId);
                        parameters.Add("OfferCurrencyId", value.OfferCurrencyId);
                        parameters.Add("OfferPrice", value.OfferPrice);
                        parameters.Add("OfferUomid", value.OfferUomid);
                        parameters.Add("ShippingTermsId", value.ShippingTermsId);
                        parameters.Add("Focqty", value.Focqty);
                        parameters.Add("OfferCurrencyId", value.OfferCurrencyId);
                        parameters.Add("AddedByUserID", value.AddedByUserId);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserId);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", value.StatusCodeId);
                        parameters.Add("IsTenderExceed", value.IsTenderExceed);
                        parameters.Add("IsAwarded", value.IsAwarded);
                        parameters.Add("Remarks", value.Remarks, DbType.String);
                        if (value.QuotationHistoryLineId > 0)
                        {
                            var query = " UPDATE QuotationHistoryLine SET QutationHistoryId=@QutationHistoryId, ProductId = @ProductId,Source =@Source,Quantity=@Quantity,Uomid=@Uomid,PackingId=@PackingId,OfferCurrencyId=@OfferCurrencyId,\n\r" +
                                "OfferPrice=@OfferPrice,OfferUomid=@OfferUomid,Focqty=@Focqty,ShippingTermsId=@ShippingTermsId,IsTenderExceed=@IsTenderExceed,IsAwarded=@IsAwarded,Remarks=@Remarks,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,SessionId=@SessionId\n\r" +
                                "WHERE QuotationHistoryLineId = @QuotationHistoryLineId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            var query = "INSERT INTO QuotationHistoryLine(QutationHistoryId,IsTenderExceed,IsAwarded,Remarks,ProductId,Source,Quantity,Uomid,PackingId,OfferCurrencyId,OfferPrice,OfferUomid,Focqty,ShippingTermsId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,SessionId)  " +
                                "OUTPUT INSERTED.QuotationHistoryLineId VALUES " +
                                "(@QutationHistoryId,@IsTenderExceed,@IsAwarded,@Remarks,@ProductId,@Source,@Quantity,@Uomid,@PackingId,@OfferCurrencyId,@OfferPrice,@OfferUomid,@Focqty,@ShippingTermsId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@SessionId)";

                            value.QuotationHistoryLineId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
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
                throw new NotImplementedException();
            }
        }

        public async Task<IReadOnlyList<GenericCodes>> GetAllByGenericCodesAsync()
        {
            try
            {
                var query = "select t1.*,t2.Value as UomName from genericcodes t1 LEFT JOIN ApplicationMasterDetail t2 ON t2.ApplicationMasterDetailID=t1.UOM";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<GenericCodes>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<QuotationHistory> DeleteQuotationHistory(QuotationHistory value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("QuotationHistoryId", value.QuotationHistoryId);
                        var query = "DELETE  FROM QuotationHistoryLine WHERE QutationHistoryID = @QuotationHistoryId;";
                        query += "DELETE  FROM QuotationHistory WHERE QuotationHistoryID = @QuotationHistoryId;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
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
        public async Task<QuotationHistoryLine> DeleteQuotationHistoryLine(QuotationHistoryLine value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("QuotationHistoryLineId", value.QuotationHistoryLineId);
                        var query = "DELETE  FROM QuotationHistoryLine WHERE QuotationHistoryLineId = @QuotationHistoryLineId;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
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
