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
    public class RolePermissionQueryRepository : QueryRepository<RolePermission>, IRolePermissionQueryRepository
    {
        public RolePermissionQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<RolePermission>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM RolePermission";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<RolePermission>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Insert(RolePermission rolepermission)
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
                            parameters.Add("Name", rolepermission.Name, DbType.String);
                            parameters.Add("Description", rolepermission.Description);
                            parameters.Add("SessionId", rolepermission.SessionId);
                            parameters.Add("AddedByUserID", rolepermission.AddedByUserID);
                            parameters.Add("AddedDate", rolepermission.AddedDate);
                            parameters.Add("StatusCodeID", rolepermission.StatusCodeID);

                            var query = "INSERT INTO RolePermission(Name,Description,SessionId,AddedByUserID,AddedDate,StatusCodeID) VALUES (@Name,@Description,@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID)";

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
        public async Task<long> Update(RolePermission rolepermission)
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
                            parameters.Add("Name", rolepermission.Name);
                            parameters.Add("Description", rolepermission.Description);
                            parameters.Add("RolePermissionID", rolepermission.RolePermissionID, DbType.Int64);

                            var query = " UPDATE RolePermission SET Name = @Name,Description = @Description WHERE RolePermissionID = @RolePermissionID";

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
                            parameters.Add("RolePermissionID", id);

                            var query = "DELETE  FROM RolePermission WHERE RolePermissionID = @RolePermissionID";


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
    }
}

