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
        public async Task<QuotationHistory> InsertOrUpdateDocumentDmsShare(QuotationHistory value)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                       /* var parameters = new DynamicParameters();
                        parameters.Add("QuotationHistoryId", value.QuotationHistoryId);
                        parameters.Add("CompanyId", value.CompanyId);
                        parameters.Add("SwreferenceNo", value.SwreferenceNo,DbType.String);
                        parameters.Add("Date", value.Date,DbType.DateTime);
                        parameters.Add("CustomerId", value.CustomerId);
                        parameters.Add("ExpiryDate", value.ExpiryDate, DbType.DateTime);
                        parameters.Add("AddedByUserID", documentDmsShare.AddedByUserID);
                        parameters.Add("ModifiedByUserID", documentDmsShare.ModifiedByUserID);
                        parameters.Add("AddedDate", documentDmsShare.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", documentDmsShare.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", documentDmsShare.StatusCodeID);
                        parameters.Add("IsDeleted", documentDmsShare.IsDeleted);
                        parameters.Add("Description", documentDmsShare.Description, DbType.String);
                        if (documentDmsShare.DocumentDmsShareId > 0)
                        {
                            var query = " UPDATE QuotationHistory SET DocSessionId = @DocSessionId,SessionId =@SessionId,DocumentId=@DocumentId,IsExpiry=@IsExpiry,ExpiryDate=@ExpiryDate,\n\r" +
                                "ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,IsDeleted=@IsDeleted,Description=@Description\n\r" +
                                "WHERE DocumentDmsShareId = @DocumentDmsShareId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            var query = "INSERT INTO QuotationHistory(DocSessionId,SessionId,DocumentId,IsExpiry,ExpiryDate,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsDeleted,Description)  " +
                                "OUTPUT INSERTED.DocumentDmsShareId VALUES " +
                                "(@DocSessionId,@SessionId,@DocumentId,@IsExpiry,@ExpiryDate,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsDeleted,@Description)";

                            documentDmsShare.DocumentDmsShareId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }*/

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
    }
    
}
