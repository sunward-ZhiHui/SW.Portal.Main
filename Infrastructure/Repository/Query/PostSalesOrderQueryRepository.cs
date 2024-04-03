using Core.Entities;
using Core.Entities.Views;
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
    public class PostSalesOrderQueryRepository : QueryRepository<PostSalesOrder>, IPostSalesOrderQueryRepository
    {
        public PostSalesOrderQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<PostSalesOrder>> GetAllAsync()
        {
            try
            {
                var query = "select  * from PostSalesOrder";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<PostSalesOrder>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<PostSalesOrder> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM PostSalesOrder  WHERE PostSalesOrderID = @PostSalesOrderId";
                var parameters = new DynamicParameters();
                parameters.Add("PostSalesOrderId", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<PostSalesOrder>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<string> InsertRawMatItemList(IEnumerable<RawMatItemList> rawMatItemList)
        {
            var successCount = 0;

            foreach (var rawMatItem in rawMatItemList)
            {
                var validation = await GetRawMatItemValidatation(rawMatItem.CompanyId, rawMatItem.ItemNo, rawMatItem.Type);

                if (validation == null)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ItemNo", rawMatItem.ItemNo);
                    parameters.Add("Description", rawMatItem.Description);
                    parameters.Add("Description2", rawMatItem.Description2);
                    parameters.Add("Inventory", rawMatItem.Inventory);
                    parameters.Add("InternalRef", rawMatItem.InternalRef);
                    parameters.Add("ItemRegistration", rawMatItem.ItemRegistration);
                    parameters.Add("BatchNos", rawMatItem.BatchNos);
                    parameters.Add("PSOItemNo", rawMatItem.PSOItemNo);
                    parameters.Add("ProductionRecipeNo", rawMatItem.ProductionRecipeNo);
                    parameters.Add("SafetyLeadTime", rawMatItem.SafetyLeadTime);
                    parameters.Add("ProductionBOMNo", rawMatItem.ProductionBOMNo);
                    parameters.Add("RoutingNo", rawMatItem.RoutingNo);
                    parameters.Add("BaseUnitofMeasure", rawMatItem.BaseUnitofMeasure);
                    parameters.Add("StandardCost", rawMatItem.StandardCost);
                    parameters.Add("UnitCost", rawMatItem.UnitCost);
                    parameters.Add("LastDirectCost", rawMatItem.LastDirectCost);
                    parameters.Add("ItemCategoryCode", rawMatItem.ItemCategoryCode);
                    parameters.Add("ProductGroupCode", rawMatItem.ProductGroupCode);
                    parameters.Add("CompanyId", rawMatItem.CompanyId);
                    parameters.Add("Type", rawMatItem.Type);

                    var query = "INSERT INTO RawMatItemList (ItemNo, Description, Description2,Inventory,InternalRef,ItemRegistration,BatchNos,PSOItemNo,ProductionRecipeNo,SafetyLeadTime,ProductionBOMNo,RoutingNo,BaseUnitofMeasure,StandardCost,UnitCost,LastDirectCost,ItemCategoryCode,ProductGroupCode,CompanyId,Type) VALUES (@ItemNo,@Description, @Description2,@Inventory,@InternalRef,@ItemRegistration,@BatchNos,@PSOItemNo,@ProductionRecipeNo,@SafetyLeadTime,@ProductionBOMNo,@RoutingNo,@BaseUnitofMeasure,@StandardCost,@UnitCost,@LastDirectCost,@ItemCategoryCode,@ProductGroupCode,@CompanyId,@Type)";

                    using (var connection = CreateConnection())
                    {
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);

                        if (rowsAffected > 0)
                            successCount++;
                    }
                }
            }

            if (successCount > 0)
                return $"Successfully added {successCount} items.";
            else
                return "All items already added or failed to insert.";
        }

        public async Task<RawMatItemList> GetRawMatItemValidatation(long? companyid, string? itemno,string? type)
        {
            try
            {              
                var parameters = new DynamicParameters();
                parameters.Add("companyid", companyid);
                parameters.Add("itemno", itemno);
                parameters.Add("type", type);

                var query = "SELECT * FROM RawMatItemList WHERE CompanyId = @companyid AND Type = @type AND ItemNo = @itemno";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<RawMatItemList>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


    }
}

