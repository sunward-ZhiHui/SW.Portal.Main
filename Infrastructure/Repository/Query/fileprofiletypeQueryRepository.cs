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
    public class fileprofiletypeQueryRepository : QueryRepository<fileprofiletype>, IfileprofileQueryRepository
    {
        public fileprofiletypeQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<fileprofiletype>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM Fileprofiletype";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<fileprofiletype>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        
    }
}
