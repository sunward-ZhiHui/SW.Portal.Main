using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Command;
using Core.Repositories.Query.Base;
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
    public class SalesOrderMasterPricingLineCommandRepository : CommandRepository<SalesOrderMasterPricingLine>,ISalesOrderMasterPricingLineCommandRepository
    {

        public SalesOrderMasterPricingLineCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<SalesOrderMasterPricingLine> InsertAsync(SalesOrderMasterPricingLine designation)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(designation);
                }
                return designation;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<SalesOrderMasterPricingLine> UpdateAsync(SalesOrderMasterPricingLine designation)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.UpdateAsync(designation);
                }
                return designation;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<SalesOrderMasterPricingLine> DeleteAsync(SalesOrderMasterPricingLine designation)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.DeleteAsync(designation);
                }
                return designation;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }

}   

