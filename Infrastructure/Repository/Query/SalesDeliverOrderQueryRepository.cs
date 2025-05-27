using Core.Entities;
using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core.EntityModels;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Core.Entities.Views;



namespace Infrastructure.Repository.Query
{
    public class SalesDeliverOrderQueryRepository : DbConnector, ISalesDeliverOrderQueryRepository
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly IPlantQueryRepository _plantQueryRepository;
        public SalesDeliverOrderQueryRepository(IConfiguration configuration, ISalesOrderService salesOrderService, IPlantQueryRepository plantQueryRepository)
            : base(configuration)
        {
            _salesOrderService = salesOrderService;
            _plantQueryRepository = plantQueryRepository;
        }
        public async Task<IReadOnlyList<NavpostedShipment>> GetAllByAsync(NavpostedShipment navpostedShipment)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CustomerId", navpostedShipment.CustomerId);
                parameters.Add("CompanyId", navpostedShipment.CompanyId);
                parameters.Add("DeliveryOrderNo", navpostedShipment.DeliveryOrderNo, DbType.String);
                parameters.Add("StockBalanceMonth", navpostedShipment.StockBalanceMonth, DbType.DateTime);
                var query = "select * from NavpostedShipment where 1=1\r";
                if (navpostedShipment.CustomerId > 0)
                {
                    query += "\r AND CustomerId=@CustomerId\r";
                }
                if (navpostedShipment.CompanyId > 0)
                {
                    query += "\r AND CompanyId=@CompanyId\r";
                }
                if (!string.IsNullOrEmpty(navpostedShipment.DeliveryOrderNo))
                {
                    query += "\r AND DeliveryOrderNo=@DeliveryOrderNo\r";
                }
                if (navpostedShipment.StockBalanceMonth != null)
                {
                    query += "AND (MONTH(StockBalanceMonth) = " + navpostedShipment.StockBalanceMonth.Value.Month + "  and YEAR(StockBalanceMonth) = " + navpostedShipment.StockBalanceMonth.Value.Year + ")\r";
                }
                if (navpostedShipment.FromPostingDate != null)
                {
                    var to = navpostedShipment.FromPostingDate.Value.ToString("yyyy-MM-dd");
                    query += "AND CAST(PostingDate AS Date)>='" + to + "'\r\n";
                }
                if (navpostedShipment.ToPostingDate != null)
                {
                    var to = navpostedShipment.ToPostingDate.Value.ToString("yyyy-MM-dd");
                    query += "AND CAST(PostingDate AS Date)<='" + to + "'\r\n";
                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<NavpostedShipment>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<NavpostedShipment>> GetNavpostedShipmentAsync(long? CompanyId)
        {
            List<NavpostedShipment> ItemBatchInfos = new List<NavpostedShipment>();
            try
            {
                var query = "select  * from NavpostedShipment where  CompanyId= " + CompanyId;

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<NavpostedShipment>(query)).ToList();
                    ItemBatchInfos = result != null ? result : new List<NavpostedShipment>();
                }
                return ItemBatchInfos;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<Navcustomer>> GetNavcustomerAsync(long? CompanyId)
        {
            List<Navcustomer> ItemBatchInfos = new List<Navcustomer>();
            try
            {
                var query = "select  * from Navcustomer where  CompanyId= " + CompanyId;

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<Navcustomer>(query)).ToList();
                    ItemBatchInfos = result != null ? result : new List<Navcustomer>();
                }
                return ItemBatchInfos;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<NavpostedShipment> GetSevrviceNavpostedShipment(NavpostedShipment navpostedShipment)
        {
            if (navpostedShipment.CompanyId > 0)
            {
                var plantData = await _plantQueryRepository.GetByIdAsync(navpostedShipment.CompanyId.GetValueOrDefault(0));
                var resultData = await GetNavpostedShipmentAsync(navpostedShipment.CompanyId);
                var navCustomer = await GetNavcustomerAsync(navpostedShipment.CompanyId);
                var result = await _salesOrderService.GetSalesDeliverOrder(plantData, navpostedShipment, navCustomer, resultData);
                if (result != null)
                {
                    if (result != null && result.Count > 0)
                    {
                        foreach (var simulation in result)
                        {
                            simulation.AddedDate = simulation.ShipmentId > 0 ? (simulation.AddedDate == null ? DateTime.Now : simulation.AddedDate) : DateTime.Now;
                            simulation.AddedByUserId = simulation.ShipmentId > 0 ? (simulation.AddedByUserId > 0 ? simulation.AddedByUserId : navpostedShipment.AddedByUserId) : navpostedShipment.AddedByUserId;
                            simulation.ModifiedByUserId = navpostedShipment.ModifiedByUserId;
                            await InsertOrUpdateSalesDeliverOrder(simulation);
                        }
                    }
                }
            }
            return navpostedShipment;
        }
        public async Task<NavpostedShipment> InsertOrUpdateSalesDeliverOrder(NavpostedShipment navpostedShipment)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ShipmentId", navpostedShipment.ShipmentId);
                    parameters.Add("IsRecived", navpostedShipment.IsRecived == true ? true : false);
                    parameters.Add("ModifiedByUserId", navpostedShipment.ModifiedByUserId);
                    parameters.Add("AddedDate", navpostedShipment.AddedDate, DbType.DateTime);
                    parameters.Add("Company", navpostedShipment.Company, DbType.String);
                    parameters.Add("CompanyId", navpostedShipment.CompanyId);
                    parameters.Add("StockBalanceMonth", navpostedShipment.StockBalanceMonth, DbType.DateTime);
                    parameters.Add("PostingDate", navpostedShipment.PostingDate, DbType.DateTime);
                    parameters.Add("Customer", navpostedShipment.Customer, DbType.String);
                    parameters.Add("CustomerNo", navpostedShipment.CustomerNo, DbType.String);
                    parameters.Add("CustomerId", navpostedShipment.CustomerId);
                    parameters.Add("DeliveryOrderNo", navpostedShipment.DeliveryOrderNo, DbType.String);
                    parameters.Add("DolineNo", navpostedShipment.DolineNo);
                    parameters.Add("ItemNo", navpostedShipment.ItemNo, DbType.String);
                    parameters.Add("Description", navpostedShipment.Description, DbType.String);
                    parameters.Add("DoQty", navpostedShipment.DoQty);
                    parameters.Add("StatusCodeId", navpostedShipment.StatusCodeId);
                    parameters.Add("AddedByUserId", navpostedShipment.AddedByUserId);
                    if (navpostedShipment.ShipmentId > 0)
                    {
                        var query = "UPDATE NavpostedShipment SET CustomerNo=@CustomerNo,Customer=@Customer,CustomerId=@CustomerId,PostingDate=@PostingDate,IsRecived=@IsRecived,ModifiedByUserId=@ModifiedByUserId,AddedDate=@AddedDate\r" +
                            "WHERE ShipmentId = @ShipmentId";
                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO NavpostedShipment(CustomerNo,ModifiedByUserId,IsRecived,AddedDate,AddedByUserId,StatusCodeId,DoQty,Company,CompanyId,StockBalanceMonth,PostingDate,Customer,CustomerId,DeliveryOrderNo,DolineNo,ItemNo,Description)  " +
                               "OUTPUT INSERTED.ShipmentId VALUES " +
                               "(@CustomerNo,@ModifiedByUserId,@IsRecived,@AddedDate,@AddedByUserId,@StatusCodeId,@DoQty,@Company,@CompanyId,@StockBalanceMonth,@PostingDate,@Customer,@CustomerId,@DeliveryOrderNo,@DolineNo,@ItemNo,@Description)";

                        navpostedShipment.ShipmentId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                    }
                    return navpostedShipment;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
