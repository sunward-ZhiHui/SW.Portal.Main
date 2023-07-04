using Core.Entities;
using Core.Repositories.Query;
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
    public class RolePermissionQueryRepository : QueryRepository<ApplicationRole>, IRolePermissionQueryRepository
    {
        public RolePermissionQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ApplicationRole>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM ApplicationRole";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationRole>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Insert(ApplicationRole rolepermission)
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
                            parameters.Add("RoleName", rolepermission.RoleName, DbType.String);
                            parameters.Add("RoleDescription", rolepermission.RoleDescription);
                          
                            parameters.Add("AddedByUserID", rolepermission.AddedByUserID);
                            parameters.Add("AddedDate", rolepermission.AddedDate);
                            parameters.Add("StatusCodeID", rolepermission.StatusCodeID);

                            var query = "INSERT INTO ApplicationRole(RoleName,RoleDescription,AddedByUserID,AddedDate,StatusCodeID) VALUES (@RoleName,@RoleDescription,@AddedByUserID,@AddedDate,@StatusCodeID)";

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
        public async Task<long> Update(ApplicationRole rolepermission)
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
                            parameters.Add("RoleName", rolepermission.RoleName);
                            parameters.Add("RoleDescription", rolepermission.RoleDescription);
                            parameters.Add("RoleID", rolepermission.RoleID, DbType.Int64);

                            var query = " UPDATE ApplicationRole SET RoleName = @RoleName,RoleDescription = @RoleDescription WHERE RoleID = @RoleID";

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


        public async Task<long> Delete(long id)
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
                            parameters.Add("RoleID", id);

                            var query = "DELETE  FROM ApplicationRole WHERE RoleID = @RoleID";


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

        public  async Task<List<ApplicationRole>> GetSelectedRolePermissionListAsync(long RoleId)
        {
            try
            {
                
                        var parameters = new DynamicParameters();
                        parameters.Add("RoleID", RoleId, DbType.Int64);
                        var query = "SELECT * FROM ApplicationRole RP WHERE RP.RoleID =@RoleID";
                        using (var connection = CreateConnection())
                        {
                            return (await connection.QueryAsync<ApplicationRole>(query, parameters)).ToList();
                        }
                       
                    
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}

