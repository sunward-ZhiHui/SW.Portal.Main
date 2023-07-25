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
using IdentityModel.Client;
using Application.Common.Helper;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace Infrastructure.Repository.Query
{
    public class DocumentsQueryRepository : QueryRepository<Documents>, IDocumentsQueryRepository
    {
        public DocumentsQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<byte[]> GetByteFromUrl(string url)
        {
            var webClient = new WebClient();
            {
                byte[] bytesData = await webClient.DownloadDataTaskAsync(new Uri(url));
                return bytesData;
            }
        }
        public async Task<Documents> GetBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var query = "SELECT * FROM Documents WHERE IsLatest= 1 and  SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<Documents>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Documents> GetByIdAsync(long? DocumentId)
        {
            try
            {
                var query = "SELECT * FROM Documents WHERE IsLatest= 1 and DocumentId = @DocumentId";
                var parameters = new DynamicParameters();
                parameters.Add("DocumentId", DocumentId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<Documents>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Delete(long? DocumentId)
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
                            parameters.Add("DocumentID", DocumentId);

                            var query = "DELETE  FROM Documents WHERE DocumentID = @DocumentId";

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
