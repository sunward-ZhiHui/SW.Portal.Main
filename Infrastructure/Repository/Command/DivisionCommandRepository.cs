using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository.Command
{
    public class DivisionCommandRepository : CommandRepository<Division>, IDivisionCommandRepository
    {
        public DivisionCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<Division> InsertAsync(Division applicationRole)
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
