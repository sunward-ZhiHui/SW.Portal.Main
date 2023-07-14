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

namespace Infrastructure.Repository.Query
{
    public class ApplicationMasterParentQueryRepository : QueryRepository<ApplicationMasterParent>, IApplicationMasterParentQueryRepository
    {
        public ApplicationMasterParentQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ApplicationMasterParent>> GetAllAsync()
        {
            try
            {
                var query = "select * from ApplicationMasterParent";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterParent>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ApplicationMasterParent>> GetAllByParentAsync()
        {
            try
            {
                var query = "select * from ApplicationMasterParent where ParentID is null";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterParent>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
