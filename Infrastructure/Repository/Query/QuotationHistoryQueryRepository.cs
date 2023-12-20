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
using Newtonsoft.Json.Linq;

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
        public async Task<IReadOnlyList<GenericCodes>> GetQuotationHistoryLineProducts(long? id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("QutationHistoryId", id);
                var query = "select t1.*,t2.Value as UomName from genericcodes t1 LEFT JOIN ApplicationMasterDetail t2 ON t2.ApplicationMasterDetailID=t1.UOM\n\r" +
                    "WHERE t1.GenericCodeId in (select t3.ProductID from QuotationHistoryLine t3 WHERE t3.QutationHistoryId=@QutationHistoryId group by t3.ProductID )";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<GenericCodes>(query,parameters)).ToList();
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
        public async Task<IReadOnlyList<QuotationAward>> GetAllByQuotationAwardAsync(long? id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("QuotationHistoryId", id);
                var query = "select t1.*, t3.CodeValue as StatusCode,t4.UserName as AddedBy,t5.UserName as ModifiedBy,\r\nt6.Code as GenericCode,t6.Description as GenericCodeDescription,t7.Value as TypeOfOrder\r\nfrom QuotationAward t1 \r\nJOIN QuotationHistory t2 ON t2.QuotationHistoryID=t1.QuotationHistoryId \r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID \r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID \r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID \r\nLEFT JOIN GenericCodes t6 ON t6.GenericCodeId=t1.ItemID\r\nLEFT JOIN ApplicationMasterDetail t7 ON t7.ApplicationMasterDetailID=t1.TypeOfOrderID WHERE t1.QuotationHistoryId=@QuotationHistoryId\r\n\r\n";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<QuotationAward>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<QuotationAward> GetQuotationAwardSession(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.*, t3.CodeValue as StatusCode,t4.UserName as AddedBy,t5.UserName as ModifiedBy,\r\nt6.Code as GenericCode,t6.Description as GenericCodeDescription,t7.Value as TypeOfOrder\r\nfrom QuotationAward t1 \r\nJOIN QuotationHistory t2 ON t2.QuotationHistoryID=t1.QuotationHistoryId \r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID \r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID \r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID \r\nLEFT JOIN GenericCodes t6 ON t6.GenericCodeId=t1.ItemID\r\nLEFT JOIN ApplicationMasterDetail t7 ON t7.ApplicationMasterDetailID=t1.TypeOfOrderID WHERE t1.SessionId=@SessionId\r\n\r\n";
                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<QuotationAward>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<QuotationAward> DeleteQuotationAward(QuotationAward value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("QuotationAwardId", value.QuotationAwardId);
                        var query = "DELETE  FROM QuotationAwardLine WHERE QuotatonAwardID = @QuotationAwardId;";
                        query += "DELETE  FROM QuotationAward WHERE QuotationAwardID = @QuotationAwardId;";
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
        public async Task<QuotationAward> InsertOrUpdateQuotationAward(QuotationAward value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("QuotationAwardId", value.QuotationAwardId);
                        parameters.Add("QuotationHistoryId", value.QuotationHistoryId);
                        parameters.Add("TypeOfOrderId", value.TypeOfOrderId);
                        parameters.Add("ItemId", value.ItemId);
                        parameters.Add("TotalQty", value.TotalQty);
                        parameters.Add("IsCommitment", value.IsCommitment);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("CommittedQty", value.CommittedQty);
                        parameters.Add("CommittedOn", value.CommittedOn, DbType.DateTime);
                        parameters.Add("IsFollowDate", value.IsFollowDate);
                        parameters.Add("Price", value.Price);
                        parameters.Add("ReasonForDifferentQty", value.ReasonForDifferentQty,DbType.String);
                        parameters.Add("ReasonForDifferentPrice", value.ReasonForDifferentPrice,DbType.String);
                        parameters.Add("IsDifferentQty", value.IsDifferentQty);
                        parameters.Add("AddedByUserID", value.AddedByUserId);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserId);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", value.StatusCodeId);
                        parameters.Add("IsDifferentPrice", value.IsDifferentPrice);
                        if (value.QuotationAwardId > 0)
                        {
                            var query = " UPDATE QuotationAward SET QuotationHistoryId=@QuotationHistoryId, TypeOfOrderId = @TypeOfOrderId,ItemId =@ItemId,TotalQty=@TotalQty,IsCommitment=@IsCommitment,CommittedQty=@CommittedQty,CommittedOn=@CommittedOn,\n\r" +
                                "IsFollowDate=@IsFollowDate,Price=@Price,ReasonForDifferentPrice=@ReasonForDifferentPrice,ReasonForDifferentQty=@ReasonForDifferentQty,IsDifferentQty=@IsDifferentQty,IsDifferentPrice=@IsDifferentPrice,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,SessionId=@SessionId\n\r" +
                                "WHERE QuotationAwardId = @QuotationAwardId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            var query = "INSERT INTO QuotationAward(QuotationHistoryId,ReasonForDifferentPrice,IsDifferentQty,IsDifferentPrice,TypeOfOrderId,ItemId,TotalQty,IsCommitment,CommittedQty,CommittedOn,IsFollowDate,Price,ReasonForDifferentQty,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,SessionId)  " +
                                "OUTPUT INSERTED.QuotationAwardId VALUES " +
                                "(@QuotationHistoryId,@ReasonForDifferentPrice,@IsDifferentQty,@IsDifferentPrice,@TypeOfOrderId,@ItemId,@TotalQty,@IsCommitment,@CommittedQty,@CommittedOn,@IsFollowDate,@Price,@ReasonForDifferentQty,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@SessionId)";

                            value.QuotationAwardId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
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
        public async Task<IReadOnlyList<QuotationAwardLine>> GetAllByQuotationAwardLineAsync(long? id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("QuotatonAwardId", id);
                var query = "select t1.*, t3.CodeValue as StatusCode,t4.UserName as AddedBy,t5.UserName as ModifiedBy\r\nfrom QuotationAwardLine t1 \r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID \r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID \r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID WHERE t1.QuotatonAwardId=@QuotatonAwardId\r\n\r\n";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<QuotationAwardLine>(query,parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<QuotationAwardLine> GetQuotationAwardLineSession(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.*, t3.CodeValue as StatusCode,t4.UserName as AddedBy,t5.UserName as ModifiedBy\r\nfrom QuotationAwardLine t1 \r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID \r\nLEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID \r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID WHERE t1.SessionId=@SessionId\r\n\r\n";
                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<QuotationAwardLine>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<QuotationAwardLine> InsertOrUpdateQuotationAwardLine(QuotationAwardLine value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("QuotationAwardLineId", value.QuotationAwardLineId);
                        parameters.Add("QuotatonAwardId", value.QuotatonAwardId);
                        parameters.Add("SessionId", value.SessionId);
                        parameters.Add("QtyPerShipment", value.QtyPerShipment);
                        parameters.Add("ShipmentDate", value.ShipmentDate, DbType.DateTime);
                        parameters.Add("AddedByUserID", value.AddedByUserId);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserId);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", value.StatusCodeId);
                        if (value.QuotationAwardLineId > 0)
                        {
                            var query = " UPDATE QuotationAwardLine SET QuotatonAwardId=@QuotatonAwardId, QtyPerShipment = @QtyPerShipment,ShipmentDate =@ShipmentDate,\n\r" +
                                "ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,SessionId=@SessionId\n\r" +
                                "WHERE QuotationAwardLineId = @QuotationAwardLineId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            var query = "INSERT INTO QuotationAwardLine(QuotatonAwardId,QtyPerShipment,ShipmentDate,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,SessionId)  " +
                                "OUTPUT INSERTED.QuotationAwardLineId VALUES " +
                                "(@QuotatonAwardId,@QtyPerShipment,@ShipmentDate,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@SessionId)";

                            value.QuotationAwardLineId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
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
        public async Task<QuotationAwardLine> DeleteQuotationAwardLine(QuotationAwardLine value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("QuotationAwardLineId", value.QuotationAwardLineId);
                        var query = "DELETE  FROM QuotationAwardLine WHERE QuotationAwardLineId = @QuotationAwardLineId;";
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
