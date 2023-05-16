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

namespace Infrastructure.Repository.Query
{
    public class DocumentsQueryRepository : QueryRepository<Documents>, IDocumentsQueryRepository
    {
        public DocumentsQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<Documents> GetByIdAsync(Guid? SessionId)
        {
            try
            {
                var query = "SELECT * FROM Documents WHERE SessionId = @SessionId";
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
    }
}
