using Core.Entities;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class NewEmailUploadQueryRepository : QueryRepository<Documents>, INewEmailUploadQueryRepository
    {
        public NewEmailUploadQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<Documents>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM Documents";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Documents>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    

        public async Task<long> Insert(Documents Documents)
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
                          
                            parameters.Add("UploadDate", Documents.UploadDate);
                            parameters.Add("SessionId", Documents.SessionId);
                            parameters.Add("FileName", Documents.FileName);
                            parameters.Add("FilePath", Documents.FilePath);
                            parameters.Add("ContentType", Documents.ContentType);
                            parameters.Add("FileSize", Documents.FileSize);
                            parameters.Add("AddedByUserId", Documents.AddedByUserId);
                            parameters.Add("AddedDate", Documents.AddedDate);
                            parameters.Add("IsLatest", Documents.IsLatest);
                            var query = "INSERT INTO Documents(UploadDate,FileName,SessionId,FilePath,ContentType,FileSize,AddedByUserId,AddedDate,IsLatest) VALUES (@UploadDate,@FileName,@SessionId,@FilePath,@ContentType,@FileSize,@AddedByUserId,@AddedDate,@IsLatest)";

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
    }
}
