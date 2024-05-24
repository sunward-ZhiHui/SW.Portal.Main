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
                var query = @"Select AI.DistName as Dist,AI.ItemDesc as DistDescription,NI.No as NavItem,NI.Description as NavDescription, 
                    NI.Description2 As NavDescription2,NI.InternalRef as InternalRef,NI.ItemCategoryCode as Category,NI.PackUOM as PackUOM, +
                    AI.PackSize as PackSize ,AI.ItemNo as NavItemNo,NI.PurchaseUOM as UOM,DSB.StockBalMonth,DSB.Quantity,DSB.StockBalWeek,NI.CompanyId as NavCompany From DistStockBalance DSB
                    Inner Join AcItems AI on AI.DistACID = DSB.DistItemId
                    Inner Join NavItemCItemList NIC on NIC.NavItemId = AI.DistACID
                    Inner Join NAVItems NI on NI.ItemId = NIC.NavItemId
                    Where DSB.StockBalWeek = @WeekNumberOfMonth AND NI.CompanyId = @CompanyID";

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
    }
}
