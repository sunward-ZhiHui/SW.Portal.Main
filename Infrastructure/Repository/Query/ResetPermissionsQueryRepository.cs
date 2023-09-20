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
using Core.EntityModels;
using Core.Entities;

namespace Infrastructure.Repository.Query
{
    public class ResetPermissionsQueryRepository : QueryRepository<DocumentUserRoleModel>, IResetPermissionsQueryRepository
    {
        public ResetPermissionsQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<UserGroupUser>> GetUserGroupUsers(List<long?> userIds)
        {
            try
            {
                userIds = userIds != null && userIds.Count > 0 ? userIds : new List<long?>() { -1 };
                var query = "select t1.*,t2.Name as UserGroupName  from UserGroupUser t1\r\nJOIN UserGroup t2 ON t1.UserGroupID=t2.UserGroupID WHERE  t1.UserID in(" + string.Join(',', userIds) + ")";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<UserGroupUser>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
