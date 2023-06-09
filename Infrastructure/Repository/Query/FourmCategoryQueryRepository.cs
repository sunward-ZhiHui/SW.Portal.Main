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
    public class ForumCategoryQueryRepository : QueryRepository<ForumCategorys>, IForumCategoryQueryRepository
    {
        public ForumCategoryQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ForumCategorys>> GetAllCategoryUserAsync()
        {
            try
            {
              // var query = "select RowIndex = ROW_NUMBER() OVER(ORDER BY FC.ID DESC), AU.UserName,FC.* From ForumCategorys FC inner join ApplicationUser AU on AU.UserID = FC.AddedByUserID";
               //var query = "Select AU.UserName,FC.* From ForumCategorys FC inner join ApplicationUser AU on AU.UserID = FC.AddedByUserID";
               var query = "select RowIndex = ROW_NUMBER() OVER(ORDER BY FC.ID DESC),AU.UserName as AddedBy,A.UserName as ModifiedBy,FC.* From ForumCategorys FC left join ApplicationUser AU on AU.UserID = FC.AddedByUserID left join ApplicationUser A on  A.UserID = FC.ModifiedByUserID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ForumCategorys>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ForumCategorys> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM ForumCategorys WHERE ID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("ID", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ForumCategorys>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
