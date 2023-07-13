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

namespace Infrastructure.Repository.Query
{
    public class ApplicationMasterDetailQueryRepository : QueryRepository<View_ApplicationMasterDetail>, IApplicationMasterDetailQueryRepository
    {
        public ApplicationMasterDetailQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_ApplicationMasterDetail>> GetApplicationMasterByCode(long? Id)
        {
            try
            {
                var query = "select * from view_ApplicationMasterDetail WHERE ApplicationMasterCodeID =" + "'" + Id + "'";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_ApplicationMasterDetail>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_ApplicationMasterDetail> GetByIdAsync(long? Id)
        {
            try
            {
                var query = "SELECT * FROM view_ApplicationMasterDetail WHERE ApplicationMasterDetailID = @ApplicationMasterDetailID";
                var parameters = new DynamicParameters();
                parameters.Add("ApplicationMasterDetailID", Id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_ApplicationMasterDetail>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
