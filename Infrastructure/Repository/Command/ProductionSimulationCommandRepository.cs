using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Command
{
    public class ProductionSimulationCommandRepository :DbConnector, IProductionSimulationQueryRepository
    {
        public ProductionSimulationCommandRepository(IConfiguration configuration) : base(configuration)
        {
        }

      

        public  async Task<long> Delete(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                  

                    var query = "DELETE  FROM ProductionSimulation WHERE ProductionSimulationId = @id";
                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

       
      
        public async Task<IReadOnlyList<ProductionSimulation>> GetProductionSimulationListAsync()
        {
            try
            {
                var query = "select  * from ProductionSimulation";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionSimulation>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Update(ProductionSimulation company)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    var parameters = new DynamicParameters();
                    parameters.Add("ProductionSimulationId", company.ProductionSimulationId);
                    parameters.Add("Quantity", company.Quantity);
                    parameters.Add("PerQuantity", company.PerQuantity);
                    parameters.Add("StartingDate", company.StartingDate);
                   

                    var query = " UPDATE ProductionSimulation SET Quantity = @Quantity,PerQuantity=@PerQuantity,StartingDate =@StartingDate WHERE ProductionSimulationId = @ProductionSimulationId";

                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
    }
    
}
