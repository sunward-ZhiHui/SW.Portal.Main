using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Command;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Infrastructure.Repository.Query;
using Infrastructure.Service;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Infrastructure.Repository.Command
{
    public class ProductionSimulationCommandRepository : DbConnector, IProductionSimulationQueryRepository
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly IPlantQueryRepository _plantQueryRepository;
        public ProductionSimulationCommandRepository(IConfiguration configuration, ISalesOrderService salesOrderService, IPlantQueryRepository plantQueryRepository) : base(configuration)
        {
            _salesOrderService = salesOrderService;
            _plantQueryRepository = plantQueryRepository;
        }



        public async Task<long> Delete(long id)
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



        public async Task<IReadOnlyList<ProductionSimulation>> GetProductionSimulationListAsync(long? CompanyId, bool? IsReresh, long? UserId)
        {
            try
            {
                if (IsReresh == true)
                {
                    await IsRereshSyncLoadData(CompanyId, UserId);
                }
                DateTime now = DateTime.Now;
                DateTime firstDay = new DateTime(now.Year, now.Month, 1);
                DateTime lastDay = firstDay.AddMonths(7).AddDays(-1);
                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("firstDay", firstDay, DbType.DateTime);
                parameters.Add("lastDay", lastDay, DbType.DateTime);
                var query = "select t1.*,t2.Description as ItemName from ProductionSimulation t1\r\nLEFT JOIN NAVItems t2 ON t1.ItemID=t2.ItemId where t1.CompanyId=@CompanyId AND (t1.IsBMRTicket=0 OR t1.IsBMRTicket is null)" +
                    "AND (CAST(t1.StartingDate AS Date) >=@firstDay AND CAST(t1.StartingDate AS Date) <=@lastDay);";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionSimulation>(query, parameters)).ToList();
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
        public async Task<long?> DeleteExistingProductionSimulation(ViewPlants plantData, long? UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("CompanyId", plantData.PlantID);
                        var query = "DELETE FROM ProductionSimulation WHERE CompanyId= @CompanyId AND (IsBMRTicket=0 OR IsBMRTicket is null);";
                       
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return UserId;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task IsRereshSyncLoadData(long? CompanyId, long? UserId)
        {
            var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
            if (plantData != null)
            {
                await DeleteExistingProductionSimulation(plantData, UserId);
                await InsertOrUpdateProdPlanningLine(plantData, UserId);
                await InsertOrUpdateGetProdOrderOutputLine(plantData, UserId);
                await InsertOrUpdateProdGroupLine(plantData, UserId);
                await InsertOrUpdateProdOrderNotStartLine(plantData, UserId);
            }
        }
        public async Task<List<Navitems>> GetNavItemItemNosAsync(long? CompanyId)
        {
            List<Navitems> ItemBatchInfos = new List<Navitems>();
            try
            {
                var query = "select  * from Navitems where  CompanyId= " + CompanyId;

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<Navitems>(query)).ToList();
                    ItemBatchInfos = result != null ? result : new List<Navitems>();
                }
                return ItemBatchInfos;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task InsertOrUpdateProdPlanningLine(ViewPlants plantData, long? UserId)
        {
            var navitems = await GetNavItemItemNosAsync(plantData?.PlantID);
            var result = await _salesOrderService.GetProdPlanningLine(plantData?.NavCompanyName, plantData?.PlantID, navitems);
            if (result != null)
            {
                if (result.ProductionSimulations != null && result.ProductionSimulations.Count > 0)
                {
                    foreach (var simulation in result.ProductionSimulations)
                    {
                        await InsertOrUpdateProductionSimulation(simulation, UserId);
                    }
                }
                if (result.productionSimulationHistories != null && result.productionSimulationHistories.Count > 0)
                {
                    foreach (var histories in result.productionSimulationHistories)
                    {
                        await InsertOrUpdateProductionSimulationHistory(histories, UserId);
                    }
                }
            }
        }
        public async Task InsertOrUpdateGetProdOrderOutputLine(ViewPlants plantData, long? UserId)
        {
            var navitems = await GetNavItemItemNosAsync(plantData?.PlantID);
            var result = await _salesOrderService.GetProdOrderOutputLine(plantData?.NavCompanyName, plantData?.PlantID, navitems);
            if (result != null)
            {
                if (result.ProductionSimulations != null && result.ProductionSimulations.Count > 0)
                {
                    foreach (var simulation in result.ProductionSimulations)
                    {
                        await InsertOrUpdateProductionSimulation(simulation, UserId);
                    }
                }
                if (result.productionSimulationHistories != null && result.productionSimulationHistories.Count > 0)
                {
                    foreach (var histories in result.productionSimulationHistories)
                    {
                        await InsertOrUpdateProductionSimulationHistory(histories, UserId);
                    }
                }
            }
        }
        private async Task<long> InsertOrUpdate(string? TableName, string? PrimareyKeyName, long PrimareyKeyId, DynamicParameters parameters)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var query = string.Empty;
                    if (PrimareyKeyId > 0)
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "UPDATE " + TableName + " SET\r";
                            var names = string.Empty;
                            if (subDynamic.ParameterNames is not null)
                            {
                                foreach (var keyValue in subDynamic.ParameterNames)
                                {
                                    names += keyValue + "=";
                                    names += "@" + keyValue + ",";
                                }
                            }
                            query += names.TrimEnd(',') + "\rwhere " + PrimareyKeyName + " = " + PrimareyKeyId + ";";
                        }
                    }
                    else
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "INSERT INTO " + TableName + "(\r";
                            var values = string.Empty;
                            var names = string.Empty;
                            if (subDynamic.ParameterNames is not null)
                            {
                                foreach (var keyValue in subDynamic.ParameterNames)
                                {
                                    names += keyValue + ",";
                                    values += "@" + keyValue + ",";
                                }
                            }
                            query += names.TrimEnd(',') + ")\rOUTPUT INSERTED." + PrimareyKeyName + " VALUES(" + values.TrimEnd(',') + ");";
                        }
                    }
                    if (!string.IsNullOrEmpty(query))
                    {
                        if (PrimareyKeyId > 0)
                        {
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            PrimareyKeyId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                    }
                }
                return PrimareyKeyId;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertOrUpdateProductionSimulation(ProductionSimulation value, long? UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("AddedByUserId", UserId);
                    parameters.Add("StatusCodeId", value.StatusCodeId);
                    parameters.Add("CompanyId", value.CompanyId);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("BatchNo", value.BatchNo, DbType.String);
                    parameters.Add("Description", value.Description, DbType.String);
                    parameters.Add("ItemNo", value.ItemNo, DbType.String);
                    parameters.Add("PackSize", value.PackSize, DbType.String);
                    parameters.Add("PerQtyUom", value.PerQtyUom, DbType.String);
                    parameters.Add("PerQuantity", value.PerQuantity);
                    parameters.Add("ProdOrderNo", value.ProdOrderNo, DbType.String);
                    parameters.Add("Quantity", value.Quantity);
                    parameters.Add("PlannedQty", value.PlannedQty);
                    parameters.Add("StartingDate", value.StartingDate, DbType.DateTime);
                    parameters.Add("Uom", value.Uom, DbType.String);
                    parameters.Add("IsOutput", value.IsOutput);
                    parameters.Add("ItemId", value.ItemId);
                    parameters.Add("ProcessDate", value.ProcessDate, DbType.DateTime);
                    parameters.Add("RePlanRefNo", value.RePlanRefNo, DbType.String);
                    parameters.Add("IsBmrticket", value.IsBmrticket);
                    value.ProductionSimulationId = await InsertOrUpdate("ProductionSimulation", "ProductionSimulationId", value.ProductionSimulationId, parameters);
                    return value.ProductionSimulationId;
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertOrUpdateProductionSimulationHistory(ProductionSimulationHistory value, long? UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("AddedByUserId", UserId);
                    parameters.Add("StatusCodeId", value.StatusCodeId);
                    parameters.Add("CompanyId", value.CompanyId);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("BatchNo", value.BatchNo, DbType.String);
                    parameters.Add("Description", value.Description, DbType.String);
                    parameters.Add("ItemNo", value.ItemNo, DbType.String);
                    parameters.Add("PackSize", value.PackSize, DbType.String);
                    parameters.Add("PerQtyUom", value.PerQtyUom, DbType.String);
                    parameters.Add("PerQuantity", value.PerQuantity);
                    parameters.Add("ProdOrderNo", value.ProdOrderNo, DbType.String);
                    parameters.Add("Quantity", value.Quantity);
                    parameters.Add("PlannedQty", value.PlannedQty);
                    parameters.Add("StartingDate", value.StartingDate, DbType.DateTime);
                    parameters.Add("Uom", value.Uom, DbType.String);
                    parameters.Add("IsOutput", value.IsOutput);
                    parameters.Add("ItemId", value.ItemId);
                    value.ProductionSimulationId = await InsertOrUpdate("ProductionSimulationHistory", "ProductionSimulationHistoryId", value.ProductionSimulationHistoryId, parameters);
                    return value.ProductionSimulationHistoryId;
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> DeleteGroupPlaningTicket(long? CompanyId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyId", CompanyId);


                    var query = "DELETE  FROM GroupPlaningTicket WHERE CompanyId = @CompanyId";
                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task InsertOrUpdateProdGroupLine(ViewPlants plantData, long? UserId)
        {
            await DeleteGroupPlaningTicket(plantData?.PlantID);
            var result = await _salesOrderService.GetProdGroupLine(plantData?.NavCompanyName, plantData?.PlantID);
            if (result != null && result.Count > 0)
            {
                foreach (var simulation in result)
                {
                    await InsertOrUpdateGroupPlaningTicket(simulation, UserId);
                }
            }

        }
        public async Task<long> InsertOrUpdateGroupPlaningTicket(GroupPlaningTicket value, long? UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("BatchName", value.BatchName, DbType.String);
                    parameters.Add("BatchSize", value.BatchSize, DbType.String);
                    parameters.Add("CompanyId", value.CompanyId);
                    parameters.Add("Description", value.Description, DbType.String);
                    parameters.Add("Description1", value.Description1, DbType.String);
                    parameters.Add("ItemNo", value.ItemNo, DbType.String);
                    parameters.Add("NoOfTicket", value.NoOfTicket);
                    parameters.Add("ProductGroupCode", value.ProductGroupCode, DbType.String);
                    parameters.Add("Quantity", value.Quantity);
                    parameters.Add("StartDate", value.StartDate, DbType.DateTime);
                    parameters.Add("Uom", value.Uom, DbType.String);
                    parameters.Add("TotalQuantity", value.TotalQuantity);
                    value.GroupPlanningId = await InsertOrUpdate("GroupPlaningTicket", "GroupPlanningId", value.GroupPlanningId, parameters);
                    return value.GroupPlanningId;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> DeleteProdOrderNotStart(long? CompanyId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyId", CompanyId);


                    var query = "DELETE  FROM ProdOrderNotStart WHERE CompanyId = @CompanyId";
                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task InsertOrUpdateProdOrderNotStartLine(ViewPlants plantData, long? UserId)
        {
            await DeleteProdOrderNotStart(plantData?.PlantID);
            var result = await _salesOrderService.GetProdNotStart(plantData?.NavCompanyName, plantData?.PlantID);
            if (result != null && result.Count > 0)
            {
                foreach (var simulation in result)
                {
                    await InsertOrUpdateProdOrderNotStart(simulation, UserId);
                }
            }

        }
        public async Task<long> InsertOrUpdateProdOrderNotStart(ProdOrderNotStart value, long? UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyId", value.CompanyId);
                    parameters.Add("ItemNo", value.ItemNo, DbType.String);
                    parameters.Add("PackSize", value.PackSize, DbType.String);
                    parameters.Add("Quantity", value.Quantity);
                    parameters.Add("StartDate", value.StartDate, DbType.DateTime);
                    parameters.Add("ProdOrderNo", value.ProdOrderNo, DbType.String);
                    value.ProdNotStartId = await InsertOrUpdate("ProdOrderNotStart", "ProdNotStartId", value.ProdNotStartId, parameters);
                    return value.ProdNotStartId;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }

}
