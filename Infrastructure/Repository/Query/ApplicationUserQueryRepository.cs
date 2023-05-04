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
using System.Security.Cryptography;
using Application.Queries;
using Azure.Core;
using System.Data.Common;
using Application.Common;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Infrastructure.Repository.Query
{
    public class ApplicationUserQueryRepository : QueryRepository<ApplicationUser>, IApplicationUserQueryRepository
    {
        public ApplicationUserQueryRepository(IConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<ApplicationUser> Auth(string LoginID, string Password)
        {

            try
            {
                var query = "SELECT * FROM ApplicationUser WHERE LoginID = @LoginID and LoginPassword = @LoginPassword";
                var parameters = new DynamicParameters();
                parameters.Add("LoginID", LoginID);
                var password = EncryptDecryptPassword.Encrypt(Password);
                parameters.Add("LoginPassword", password);

                using (var connection = CreateConnection())
                {
                    var user = await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters);

                    return user;                   
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }      

        public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM ApplicationRole";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationUser>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ApplicationUser> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM ApplicationRole WHERE roleId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ApplicationUser> GetByUsers(string name)
        {
            try
            {
                var query = "SELECT * FROM ApplicationUser WHERE LoginID = @LoginID";
                var parameters = new DynamicParameters();
                parameters.Add("LoginID", name, DbType.String);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
