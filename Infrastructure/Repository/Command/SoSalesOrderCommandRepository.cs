using Core.Entities;
using Core.Entities.Views;
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
    public class SoSalesOrderCommandRepository : CommandRepository<SoSalesOrder>,ISoSalesOrderCommandRepository
    {
        public SoSalesOrderCommandRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }

}
