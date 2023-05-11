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
    public class ApplicationUserRoleCommandRepository : CommandRepository<ApplicationUserRole>, IApplicationUserRoleCommandRepository
    {
        public ApplicationUserRoleCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<ApplicationUserRole> InsertAsync(ApplicationUserRole applicationUserRole)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(applicationUserRole);
                }
                return applicationUserRole;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
