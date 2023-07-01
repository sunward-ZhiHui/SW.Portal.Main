using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
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
    public class ApplicationPermissionQueryRepository : QueryRepository<ApplicationPermission>, IApplicationPermissionQueryRepository
    {
        public ApplicationPermissionQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ApplicationPermission>> GetAllAsync()
        {
            try
            {
                //var query = "SELECT * FROM ApplicationPermission";
                // var query = @"Select* from view_UserPermission where  IsDisplay = 1 and IsNewPortal = 1 and IsCmsApp = 1  and(IsMobile is null or IsMobile = 0) ORDER BY PermissionOrder";
                var query = @"select * from applicationpermission where IsDisplay = 1 and PermissionID > =60000";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationPermission>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

       
    }
}
