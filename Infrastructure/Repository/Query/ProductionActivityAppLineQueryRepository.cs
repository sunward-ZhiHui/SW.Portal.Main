using Core.Entities;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class ProductionActivityAppLineQueryRepository : QueryRepository<ProductionActivityAppLine>, IProductionActivityQueryRepository
    {
        public ProductionActivityAppLineQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ProductionActivityAppLine>> GetAllAsync()
        {
            try
            {
             
                var query = "select  AMC.Value as Process,AMD.Value as Result,AMC1.Value as Category,AMC2.Value AS Action from ProductionActivityAppLine as PAAL inner Join ApplicationMasterChild as AMC ON AMC.ApplicationMasterChildID = PAAL.ManufacturingProcessChildID \r\ninner Join ApplicationMasterChild as AMC1 ON AMC1.ApplicationMasterChildID = PAAL.ProdActivityCategoryChildID inner Join ApplicationMasterChild as AMC2 ON AMC2.ApplicationMasterChildID = PAAL.ProdActivityActionChildD inner join ApplicationMasterDetail as AMD ON AMD.ApplicationMasterDetailID = PAAL.ProdActivityResultID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionActivityAppLine>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    
    }
}
