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
    public class IctmasterCommandRepository : CommandRepository<Ictmaster>, IIctmasterCommandRepository
    {
        public IctmasterCommandRepository(IConfiguration configuration)
           : base(configuration)
        {

        }
        public async Task<Ictmaster> InsertAsync(Ictmaster ictmaster)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(ictmaster);
                }
                return ictmaster;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
