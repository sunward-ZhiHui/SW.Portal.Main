using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository.Command
{
    public class ApplicationUserCommandRepository : CommandRepository<ApplicationUser>, IApplicationUserCommandRepository
    {
        public ApplicationUserCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<ApplicationUser> InsertAsync(ApplicationUser applicationUser)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(applicationUser);
                }
                return applicationUser;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
