using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Command;
using Core.Repositories.Command.Base;
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
    public class SoSalesOrderLineCommandRepository : CommandRepository<SoSalesOrderLine>, ISoSalesOrderLineCommandRepository
    {
        public SoSalesOrderLineCommandRepository(IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<ViewSoSalesOrderLine> InsertAsync(ViewSoSalesOrderLine soSalesOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(soSalesOrderLine);
                }
                return soSalesOrderLine;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewSoSalesOrderLine> UpdateAsync(ViewSoSalesOrderLine soSalesOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.UpdateAsync(soSalesOrderLine);
                }
                return soSalesOrderLine;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ViewSoSalesOrderLine> DeleteAsync(ViewSoSalesOrderLine soSalesOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.DeleteAsync(soSalesOrderLine);
                }
                return soSalesOrderLine;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }

}

