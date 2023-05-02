using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository.Command
{
    public class DesignationCommandRepository : CommandRepository<Designation>, IDesignationCommandRepository
    {
        public DesignationCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<Designation> InsertAsync(Designation applicationRole)
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
