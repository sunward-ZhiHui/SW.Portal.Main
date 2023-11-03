using Core.Entities;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using NAV;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repository.Query
{
    public class ProductionActivityAppLineQueryRepository : QueryRepository<ProductionActivityAppLine>, IProductionActivityQueryRepository
    {
        public ProductionActivityAppLineQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ProductionActivityAppLine>> GetAllAsync(long? companyid, string? prododerno, long? locationid)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", companyid);
                parameters.Add("ProdOrderNo",prododerno);
                parameters.Add("LocationID", locationid);
                var query = @"select AMC.Value as Process,AMD.Value as Result,AMC1.Value as Category,AMC2.Value AS Action from ProductionActivityAppLine t1 
 JOIN ProductionActivityApp t2 ON t1.ProductionActivityAppId=t2.ProductionActivityAppId
left Join ApplicationMasterChild as AMC ON AMC.ApplicationMasterChildID = t1.ManufacturingProcessChildID 
left Join ApplicationMasterChild as AMC1 ON AMC1.ApplicationMasterChildID = t1.ProdActivityCategoryChildID 
left Join ApplicationMasterChild as AMC2 ON AMC2.ApplicationMasterChildID = t1.ProdActivityActionChildD 
left join ApplicationMasterDetail as AMD ON AMD.ApplicationMasterDetailID = t1.ProdActivityResultID
WHERE (t2.CompanyId=@CompanyID AND t2.ProdOrderNo= @ProdOrderNo or t2.LocationID = @LocationID)";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionActivityAppLine>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }

        public async Task<IReadOnlyList<ProductionActivityApp>> GetAlllocAsync(long? location)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("LoctionId",location);
                var query = @"select * from ProductionActivityApp  where LocationID =@LoctionId";

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

        public async Task<IReadOnlyList<ProductionActivityApp>> GetAllprodAsync(string? NAVProdOrder)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProdOrderNo", NAVProdOrder);
                var query = @"select * from ProductionActivityApp  where ProdOrderNo =@ProdOrderNo";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionActivityApp>(query,parameters)).ToList();
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
