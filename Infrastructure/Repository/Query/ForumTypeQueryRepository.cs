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

namespace Infrastructure.Repository.Query
{
    public class ForumTypeQueryRepository : QueryRepository<ForumTypes>, IForumTypeQueryRepository
    {
        public ForumTypeQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ForumTypes>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM ForumTypes";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ForumTypes>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ForumTypes>> GetAllTypeUserAsync()
        {
            try
            {
                // var query = "Select AU.UserName,FT.* From ForumTypes FT inner join ApplicationUser AU on AU.UserID = FT.AddedByUserID";
                var query = "select RowIndex = ROW_NUMBER() OVER(ORDER BY FT.ID DESC), AU.UserName as AddedBy,A.UserName as ModifiedBy,FT.* From ForumTypes FT left join ApplicationUser AU on AU.UserID = FT.AddedByUserID left join ApplicationUser A on A.UserID = FT.ModifiedByUserID";
               /// Select AU.UserName,A.UserName,FC.* From ForumCategorys FC inner join ApplicationUser AU on AU.UserID = FC.AddedByUserID inner join ApplicationUser A on A.UserID = FC.ModifiedByUserID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ForumTypes>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ForumTypes> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM ForumTypes WHERE ID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("ID", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ForumTypes>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ForumTypes> GetCustomerByEmail(string name)
        {
            try
            {
                var query = "SELECT * FROM ForumTypes WHERE Name = @Name";
                var parameters = new DynamicParameters();
                parameters.Add("Name", name, DbType.String);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ForumTypes>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
