using Core.Entities;
using Core.Repositories.Command;
using Dapper;
using Infrastructure.Repository.Command.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Command
{
    public class FourmCategoryCommandRepository : CommandRepository<ForumCategorys>, IFourmCategoryCommandRepository
    {
        public FourmCategoryCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<ForumCategorys> InsertAsync(ForumCategorys forumTypes)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(forumTypes);
                }
                return forumTypes;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}

