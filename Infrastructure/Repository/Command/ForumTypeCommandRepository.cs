using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository.Command
{
    public class ForumTypeCommandRepository : CommandRepository<ForumTypes>, IForumTypeCommandRepository
    {
        public ForumTypeCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<ForumTypes> InsertAsync(ForumTypes forumTypes)
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
