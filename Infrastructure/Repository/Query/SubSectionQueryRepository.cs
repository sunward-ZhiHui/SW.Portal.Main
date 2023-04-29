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
using Core.EntityModel;
using Core.Entities.Views;

namespace Infrastructure.Repository.Query
{
    public class SubSectionQueryRepository : QueryRepository<ViewSubSection>, ISubSectionQueryRepository
    {
        public SubSectionQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewSubSection>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_SubSection";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewSubSection>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewSubSection> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_SubSection WHERE SubSectionId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewSubSection>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
