using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository.Command
{
    public class LevelMasterCommandRepository : CommandRepository<LevelMaster>, ILevelMasterCommandRepository
    {
        public LevelMasterCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<LevelMaster> InsertAsync(LevelMaster applicationRole)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(applicationRole);
                }
                return applicationRole;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
