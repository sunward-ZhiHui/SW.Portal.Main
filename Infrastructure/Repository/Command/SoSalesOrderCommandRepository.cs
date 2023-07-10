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


        public async Task<ViewSoSalesOrder> InsertAsync(ViewSoSalesOrder soSalesOrder)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.InsertAsync(soSalesOrder);
                }
                return soSalesOrder;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ViewSoSalesOrder> UpdateAsync(ViewSoSalesOrder soSalesOrder)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.UpdateAsync(soSalesOrder);
                }
                return soSalesOrder;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ViewSoSalesOrder> DeleteAsync(ViewSoSalesOrder soSalesOrder)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.DeleteAsync(soSalesOrder);
                }
                return soSalesOrder;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }

}
