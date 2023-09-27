using Azure.Core;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class ReportFileUploadsQueryRepository : QueryRepository<ReportDocuments>, IReportFileUploadsQueryRepository
    {
        public ReportFileUploadsQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<ReportDocuments>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM ReportDocuments";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ReportDocuments>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Insert(ReportDocuments reportDocuments)
        {

            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("Name", reportDocuments.Name);
                            parameters.Add("Description", reportDocuments.Description);
                            parameters.Add("SessionId", reportDocuments.SessionId);
                            parameters.Add("FileName", reportDocuments.FileName);
                            //parameters.Add("AddedDate", reportDocuments.AddedDate);
                            //parameters.Add("Iscompleted", reportDocuments.Iscompleted);
                            //parameters.Add("StatusCodeID", todolist.StatusCodeID);

                            var query = "INSERT INTO ReportDocuments(Name,Description,FileName,SessionId) VALUES (@Name,@Description,@FileName,@SessionId)";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }


                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<long> Update(ReportDocuments reportDocuments)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("Name", reportDocuments.Name);
                            parameters.Add("Description", reportDocuments.Description);
                            parameters.Add("SessionId", reportDocuments.SessionId);
                            parameters.Add("FileName", reportDocuments.FileName);
                            parameters.Add("ReportDocumentID", reportDocuments.ReportDocumentID);

                            var query = " UPDATE ReportDocuments SET FileName = @FileName,Name=@Name,Description = @Description,SessionId =@SessionId WHERE ReportDocumentID = @ReportDocumentID";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Delete(long ReportDocumentID)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ReportDocumentID", ReportDocumentID);
                           // parameters.Add("FileName", reportName);

                            var query = "DELETE  FROM ReportDocuments WHERE ReportDocumentID = @ReportDocumentID";


                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }




        public async Task<Guid?> InsertNew(ReportDocuments reportDocuments)
        {

            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("Name", reportDocuments.Name);
                            parameters.Add("Description", reportDocuments.Description);
                            parameters.Add("SessionId", reportDocuments.SessionId);

                            //parameters.Add("AddedDate", reportDocuments.AddedDate);
                            //parameters.Add("Iscompleted", reportDocuments.Iscompleted);
                            //parameters.Add("StatusCodeID", todolist.StatusCodeID);

                            var query = "INSERT INTO ReportDocuments(Name,Description,SessionId)" + "OUTPUT INSERTED.SessionID VALUES" + "(@Name,@Description,@SessionId)";

                            reportDocuments.SessionId = await connection.QuerySingleOrDefaultAsync<Guid?>(query, parameters, transaction);
                            transaction.Commit();
                            return reportDocuments.SessionId;
                        }


                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }

        public async Task<ReportDocuments> InsertCreateReportDocumentBySession(ReportDocuments value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("Name", value.Name);
                            parameters.Add("Description", value.Description);
                            parameters.Add("SessionId", value.SessionId);
                            parameters.Add("FileName", value.FileName);
                            //var query = "INSERT INTO [ReportDocuments](Name,Description,SessionId,FileName) " +
                            //    "OUTPUT INSERTED.ReportDocumentID VALUES " +
                            //   "(@Name,@Description,@SessionId,@FileName)";


                            var query = "Update [ReportDocuments]Set FileName = @FileName Where SessionID = @SessionID";

                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            transaction.Commit();
                            return value;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
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

    
 
