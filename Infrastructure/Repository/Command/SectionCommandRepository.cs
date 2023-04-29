using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository.Command
{
    public class SectionCommandRepository : CommandRepository<Section>, ISectionCommandRepository
    {
        public SectionCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<Section> InsertAsync(Section applicationRole)
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
