using Core.Entities;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class ProductionActivityAppQueryRepository : QueryRepository<ProductionActivityApp>, IProductionActivityAppQueryRepository
    {
        public ProductionActivityAppQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ProductionActivityApp>> GetAllAsync(long? CompanyId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", CompanyId);
                var query = " select i.ICTMasterID,s.Name as SiteName , z.Name as ZoneName, i.Name,i.Companyid, CONCAT(i.Name ,+'|'+i.Description+'|', s.Name, Z.Name ) as DeropdownName,  i.Name,i.Description,i.siteid,i.locationid,i.zoneid,i.areaid from ICTMaster i left join ICTMaster z on z.ICTMasterID = i.ParentICTID left join ICTMaster s on s.ICTMasterID = z.ParentICTID where i.companyid=@CompanyID and i.MasterType = 572 ";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionActivityApp>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    
            public async Task<IReadOnlyList<ProductionActivityApp>> GetAllAsyncPO(long? CompanyId)
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyID", CompanyId);
                    var query = "select nav.RePlanRefNo, nav.NavprodOrderLineId,nav.ProdOrderNo,nav.Description,nav.BatchNo,nav.RePlanRefNo, nav.CompanyID , CONCAT(nav.RePlanRefNo,+'||'+p.BatchNo, +'||'+nav.Description ) as prodOrderNo from NavprodOrderLine nav\r\nleft join ProductionSimulation p on p.ProdOrderNo = nav.RePlanRefNo where Nav.companyid=@CompanyID";

                    using (var connection = CreateConnection())
                    {
                        return (await connection.QueryAsync<ProductionActivityApp>(query, parameters)).ToList();
                   }
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
            }
        }

    } 
  

   


