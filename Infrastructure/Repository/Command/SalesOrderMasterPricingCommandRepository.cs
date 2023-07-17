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
    public class SalesOrderMasterPricingCommandRepository : CommandRepository<SalesOrderMasterPricing>, ISalesOrderMasterPricingCommandRepository
    {
        public SalesOrderMasterPricingCommandRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }

}
