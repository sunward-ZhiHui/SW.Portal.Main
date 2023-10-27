using Core.Entities;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
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

        public async Task<long> Insert(ProductionActivityAppLine PPALinelist)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            
                            var parameters = new DynamicParameters();
                            parameters.Add("Comment", PPALinelist.Comment);
                            parameters.Add("ActionDropdown", PPALinelist.ActionDropdown);
                            parameters.Add("ManufacturingProcessID", PPALinelist.ManufacturingProcessID);
                            parameters.Add("ProdActivityCategoryID", PPALinelist.ProdActivityCategoryID);
                            parameters.Add("NavprodOrderLineID", PPALinelist.NavprodOrderLineID);
                            // parameters.Add("Description", PPAlist.Description);
                           // parameters.Add("TopicID", PPALinelist.TopicID);
                            //parameters.Add("ICTMasterID", PPAlist.ICTMasterID);
                            //parameters.Add("SiteName", PPAlist.SiteName);
                            //parameters.Add("ZoneName", PPAlist.ZoneName);
                           //parameters.Add("Companyid", PPALinelist.com);
                            // parameters.Add("DeropdownName", PPAlist.DeropdownName);
                            //parameters.Add("NavprodOrderLineId", PPAlist.NavprodOrderLineId);
                            //parameters.Add("BatchNo", PPAlist.BatchNo);
                            //parameters.Add("prodOrderNo", PPALinelist.prod);

                            parameters.Add("SessionId", PPALinelist.SessionId);
                            parameters.Add("AddedByUserID", PPALinelist.AddedByUserID);
                            parameters.Add("AddedDate", PPALinelist.AddedDate);
                            parameters.Add("LocationID", PPALinelist.LocationID);
                            parameters.Add("StatusCodeID", PPALinelist.StatusCodeID);

                            var query = "INSERT INTO ProductionActivityAppLine(Comment,NavprodOrderLineID,ProdActivityCategoryID,ManufacturingProcessID,ActionDropdown,SessionId,AddedByUserID,AddedDate,StatusCodeID,LocationID) VALUES (@ProdActivityCategoryID,@ManufacturingProcessID,@Comment,@ActionDropdown,@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID,@LocationID,@NavprodOrderLineID)";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }


                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        
        }
    }
}
