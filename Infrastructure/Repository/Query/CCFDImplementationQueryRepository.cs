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
    public class CCFDImplementationQueryRepository : QueryRepository<CCFDImplementation>, ICCDFImplementationQueryRepository
    {
        public CCFDImplementationQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<CCFDImplementation>> GetAllAsync()
        {
            try
            {
                var query = "select  * from CCFDImplementation";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<CCFDImplementation>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    

    }
}
