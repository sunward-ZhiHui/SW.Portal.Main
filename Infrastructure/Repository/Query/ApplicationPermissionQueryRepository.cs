using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
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
                var query = "Select  * from view_UserPermission where IsDisplay=1 and IsNewPortal =1 and IsCmsApp =1  and (IsMobile is null or IsMobile=0) ORDER BY PermissionOrder";
                // var query = @"Select* from view_UserPermission where  IsDisplay = 1 and IsNewPortal = 1 and IsCmsApp = 1  and(IsMobile is null or IsMobile = 0) ORDER BY PermissionOrder";
              //  var query = @"select * from applicationpermission where IsDisplay = 1 and PermissionID > =60000";
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

        public  async Task<long> Insert(ApplicationRolePermission applicationrolepermission)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("RoleID", applicationrolepermission.RoleID);
                        parameters.Add("PermissionIDs", applicationrolepermission.PermissionIDs);
                        connection.Open();
                        var result = connection.QueryFirstOrDefault<long>("sp_Ins_RolePermission", parameters, commandType: CommandType.StoredProcedure);
                        return result;
                    }
                    catch (Exception exp)
                    {                        
                        throw new Exception(exp.Message, exp);
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
