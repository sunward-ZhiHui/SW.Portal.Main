using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
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

        public async Task<IReadOnlyList<ProductionActivityApp>> GetAllListAsync()
        {
            try
            {
                var parameters = new DynamicParameters();
                // parameters.Add("Uid", Uid);
               var query = "SELECT * FROM ProductionActivityApp";
 
                using (var connection = CreateConnection())
                {
                    var List = await connection.QueryAsync<ProductionActivityApp>(query);
                    return List.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Insert(ProductionActivityModel PPAlist)
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

                            var lists = await GetAllListAsync();
                            if (lists != null)
                            {
                                var Exsist = lists.Where(f => f.CompanyID == PPAlist.CompanyID && f.LocationID == PPAlist.LocationID && f.ProdOrderNo == PPAlist.ProdOrderNo);

                                if (Exsist != null)
                                {
                                    var id = Exsist.ToList();
                                    var Appid = id[0].ProductionActivityAppID;

                                    var parameter = new DynamicParameters();
                                    parameter.Add("Appid", Appid);
                                    parameter.Add("ManufacturingProcessChildID", PPAlist.ManufacturingProcessChildID);
                                    parameter.Add("ProdActivityCategoryChildID", PPAlist.ProdActivityCategoryChildID);
                                    parameter.Add("ProdActivityActionChildD", PPAlist.ProdActivityActionChildD);
                                    parameter.Add("PAApplineComment", PPAlist.PAApplineComment);
                                    parameter.Add("NavprodOrderLineId", PPAlist.NavprodOrderLineId);
                                    parameter.Add("applineAddedByUserID", PPAlist.applineAddedByUserID);
                                    parameter.Add("applineAddedDate", PPAlist.applineAddedDate);
                                    parameter.Add("applineSessionId", PPAlist.applineSessionId);
                                    parameter.Add("applineStatusCodeID", PPAlist.applineStatusCodeID);

                                    var applinequery = "INSERT INTO ProductionActivityAppLine(ProductionActivityAppID,ManufacturingProcessChildID,ProdActivityCategoryChildID,ProdActivityActionChildD,Comment,NavprodOrderLineId,AddedByUserID,AddedDate,SessionId,StatusCodeID) " +
                                                                                     "VALUES (@Appid,@ManufacturingProcessChildID,@ProdActivityCategoryChildID,@ProdActivityActionChildD,@PAApplineComment,@NavprodOrderLineId,@applineAddedByUserID,@applineAddedDate,@applineSessionId,@applineStatusCodeID)";

                                    var rowsAffected = await connection.ExecuteAsync(applinequery, parameter, transaction);

                                    //transaction.Commit();

                                    //return rowsAffected;
                                }

                            }
                            else
                            {
                                var parameters = new DynamicParameters();

                                parameters.Add("CompanyID", PPAlist.CompanyID);
                                parameters.Add("ProdOrderNo", PPAlist.ProdOrderNo);
                                parameters.Add("SessionId", PPAlist.SessionId);
                                parameters.Add("AddedByUserID", PPAlist.AddedByUserID);
                                parameters.Add("AddedDate", PPAlist.AddedDate);
                                parameters.Add("LocationID", PPAlist.LocationID);
                                parameters.Add("StatusCodeID", PPAlist.StatusCodeID);

                                var query = @"INSERT INTO ProductionActivityApp(SessionId,AddedByUserID,AddedDate,StatusCodeID,LocationID,CompanyID,ProdOrderNo) 
				                       OUTPUT INSERTED.ProductionActivityAppID 
				                       VALUES (@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID,@LocationID,@CompanyID,@ProdOrderNo)";


                                var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters, transaction);

                                PPAlist.ProductionActivityAppID = insertedId;

                                var parameter = new DynamicParameters();
                                parameter.Add("ProductionActivityAppID", PPAlist.ProductionActivityAppID);
                                parameter.Add("ManufacturingProcessChildID", PPAlist.ManufacturingProcessChildID);
                                parameter.Add("ProdActivityCategoryChildID", PPAlist.ProdActivityCategoryChildID);
                                parameter.Add("ProdActivityActionChildD", PPAlist.ProdActivityActionChildD);
                                parameter.Add("PAApplineComment", PPAlist.PAApplineComment);
                                parameter.Add("NavprodOrderLineId", PPAlist.NavprodOrderLineId);
                                parameter.Add("applineAddedByUserID", PPAlist.applineAddedByUserID);
                                parameter.Add("applineAddedDate", PPAlist.applineAddedDate);
                                parameter.Add("applineSessionId", PPAlist.applineSessionId);
                                parameter.Add("applineStatusCodeID", PPAlist.applineStatusCodeID);

                                var applinequery = "INSERT INTO ProductionActivityAppLine(ProductionActivityAppID,ManufacturingProcessChildID,ProdActivityCategoryChildID,ProdActivityActionChildD,Comment,NavprodOrderLineId,AddedByUserID,AddedDate,SessionId,StatusCodeID) " +
                                                                                 "VALUES (@ProductionActivityAppID,@ManufacturingProcessChildID,@ProdActivityCategoryChildID,@ProdActivityActionChildD,@PAApplineComment,@NavprodOrderLineId,@applineAddedByUserID,@applineAddedDate,@applineSessionId,@applineStatusCodeID)";

                                var rowsAffected = await connection.ExecuteAsync(applinequery, parameter, transaction);


                                //transaction.Commit();
                                //return PPAlist.ProductionActivityAppID;

                               

                            }
                            transaction.Commit();

                            return PPAlist.ProductionActivityAppID;
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


   


