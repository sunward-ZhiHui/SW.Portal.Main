using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.EntityModel;
using Core.Entities.Views;
using Core.Entities;

namespace Infrastructure.Repository.Query
{
    public class NavItemsQueryRepository : QueryRepository<View_NavItems>, INavItemsQueryRepository
    {
        public NavItemsQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_NavItems>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_NavItems";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_NavItems>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<View_NavItems>> GetAsyncList()
        {
            try
            {
                var query = "select  * from NAVItems";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_NavItems>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<View_NavItems>> GetByCompanyAsyncList(long? CompanyId)
        {
            try
            {
                var query = "select  * from NAVItems where CompanyId=" + CompanyId;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_NavItems>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Update(View_NavItems todolist)
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
                            parameters.Add("ItemSerialNo", todolist.ItemSerialNo);
                            parameters.Add("ItemId", todolist.ItemId, DbType.Int64);
                            parameters.Add("ModifiedDate", todolist.ModifiedDate, DbType.DateTime);
                            parameters.Add("ModifiedByUserId", todolist.ModifiedByUserId, DbType.Int64);
                            parameters.Add("UomId", todolist.UomId, DbType.Int64);
                            parameters.Add("SupplyToId", todolist.SupplyToId, DbType.Int64);
                            parameters.Add("PackSizeId", todolist.PackSizeId, DbType.Int64);
                            parameters.Add("CompanyId", todolist.CompanyId, DbType.Int64);
                            var query = " UPDATE Navitems SET ItemSerialNo = @ItemSerialNo,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId,UomId=@UomId,SupplyToId=@SupplyToId,PackSizeId=@PackSizeId,CompanyId=@CompanyId WHERE ItemId = @ItemId";

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
        public async Task<View_NavItems> GetByItemSerialNoExitsAsync(View_NavItems ItemSerialNo)
        {
            try
            {
                var query = "SELECT * FROM view_NavItems WHERE ItemSerialNo =" + "'" + ItemSerialNo.ItemSerialNo + "'";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_NavItems>(query));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_NavItems> GetByItemSerialNoAsync(string ItemSerialNo)
        {
            try
            {
                var query = "SELECT * FROM view_NavItems WHERE  ItemSerialNo =@ItemSerialNo";
                var parameters = new DynamicParameters();
                parameters.Add("ItemSerialNo", ItemSerialNo, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_NavItems>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
