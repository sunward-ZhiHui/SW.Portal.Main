using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;

namespace Infrastructure.Repository.Query
{
    public class DistStockBalanceQueryRepository : DbConnector, IDistStockBalanceQueryRepository
    {
        public DistStockBalanceQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public  async Task<IReadOnlyList<DistStockBalance>> GetAllDistStockBalanceAsync(DistStockBalance value)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("WeekNumberOfMonth", value.WeekNumberOfMonth);
                parameters.Add("CompanyID", value.CompanyID);
                parameters.Add("Month", value.Month);
                parameters.Add("Year", value.Year);
                var query = @"Select Distinct(NIC.NavItemCustomerItemId), AI.DistName as Dist,AI.ItemDesc as DistDescription,NI.No as NavItem,NI.Description as NavDescription, 
                    NI.Description2 As NavDescription2,NI.InternalRef as InternalRef,NI.ItemCategoryCode as Category,NI.PackUOM as PackUOM,
                    AI.PackSize as PackSize ,AI.ItemNo as NavItemNo,NI.PurchaseUOM as UOM,DSB.StockBalMonth,DSB.Quantity,DSB.StockBalWeek,AI.CompanyId as NavCompany From AcItems AI 
                    inner Join DistStockBalance DSB on DSB.DistItemId =AI.DistACID
                    inner Join NavItemCItemList NIC on NIC.NavItemCustomerItemId = ai.DistACID 
                    inner Join NAVItems NI on NI.ItemId = NIC.NavItemId
                    Where DSB.StockBalWeek =@WeekNumberOfMonth AND AI.CompanyId = @CompanyID and AI.StatusCodeID = 1 and Month(DSB.StockBalMonth) = @Month and Year(DSB.StockBalMonth)=@Year";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DistStockBalance>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async  Task<IReadOnlyList<NavitemStockBalance>> GetAllNavItemStockBalanceAsync(NavitemStockBalance value)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("WeekNumberOfMonth", value.WeekNumberOfMonth);
                parameters.Add("CompanyID", value.CompanyID);
                parameters.Add("Month", value.Month);
                parameters.Add("Year", value.Year);
                var query = @"Select NI.No as ItemNo,NI.Description as ItemDescription,NI.Description2 as ItemDescription2,NI.InternalRef as InternalRef,
                            NI.ItemCategoryCode as Category,NI.PurchaseUOM as UOM,NI.PackSize as PackSize,NI.PackUOM as PackUOM,NIB.Quantity,NIB.StockBalMonth, NI.StatusCodeID as NavStatusCodeID,NIB.StockBalWeek From NavItemStockBalance NIB 
                            LEFT Join NavItems NI on NI.ItemID = NIB.ItemID where NI.StatusCodeID = 1 And NIB.StockBalWeek = @WeekNumberOfMonth and NI.CompanyId = @CompanyID and Month(NIB.StockBalMonth) = @Month and Year(NIB.StockBalMonth)=@Year";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<NavitemStockBalance>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

       
    }
}
