using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class ProductionActivityAppLineReportQueryRepository : QueryRepository<view_ProductionActivityAppLineReport>, IProductionActivityAppLineReportQueryRepository
    {
        public ProductionActivityAppLineReportQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<view_ProductionActivityAppLineReport>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_ProductionActivityAppLineReport";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<view_ProductionActivityAppLineReport>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<view_ProductionActivityAppLineReport>> GetAllFilterAsync(long? CompanyId, DateTime? FromDate, DateTime? ToDate)
        {
            try
            {

                var query = "select * from view_ProductionActivityAppLineReport where CompanyId= @CompanyId and (Date>=@FromDate and Date<=@ToDate)";
                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("FromDate", FromDate);
                parameters.Add("ToDate", ToDate);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<view_ProductionActivityAppLineReport>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
                
            }
        }

        public async  Task<List<Documents>> GetDocumentListAsync(Guid sessionId)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("SessionId", sessionId, DbType.Guid);

                var query = "select * from Documents where SessionId = @SessionId AND IsLatest = 1 AND FilePath is not null";
             
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Documents>(query, parameters)).ToList();
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
        //public async Task<IReadOnlyList<view_ProductionActivityAppLineReport>> GetAllFilterAsync()
        //{
        //    try
        //    {

        //        var query = "select * from view_ProductionActivityAppLineReport where Companyid= @Companyid and (Date>=@FromDate and Date<=@ToDate)";
        //        var parameters = new DynamicParameters();
        //        parameters.Add("CompanyId", CompanyId, DbType.Int64);
        //        parameters.Add("FromDate", FromDate, DbType.Date);
        //        parameters.Add("ToDate", ToDate, DbType.Date);
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryAsync<view_ProductionActivityAppLineReport>(query, parameters)).ToList();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw new Exception(exp.Message, exp);
        //    }
        //}
    
}

