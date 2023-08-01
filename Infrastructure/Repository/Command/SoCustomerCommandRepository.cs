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
    internal class SoCustomerCommandRepository : CommandRepository<SoCustomer>, ISoCustomerCommandRepository
    {
        public SoCustomerCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<SoCustomer> InsertAsync(SoCustomer soCustomer)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(soCustomer);
                }
                return soCustomer;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}

