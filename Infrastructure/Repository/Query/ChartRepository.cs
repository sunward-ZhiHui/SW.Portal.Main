using Core.Repositories.Query;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class ChartRepository : DbConnector, IChartRepository
    {
        public ChartRepository(IConfiguration configuration)
            : base(configuration) // ✅ Calls DbConnector
        {
        }

        public async Task<List<dynamic>> GetDynamicDataAsync(string tableName, string xField, string yField)
        {
            using var connection = CreateConnection(); // ✅ Same as EmailConversationsQueryRepository

            // ⚠️ Always validate tableName/columns to prevent SQL injection
            string sql = $"SELECT [{xField}] AS X, [{yField}] AS Y FROM [{tableName}]";

            var result = await connection.QueryAsync(sql);
            return result.ToList();
        }

        //public async Task<List<dynamic>> GetDynamicDataAsync(string tableName, string xField, string yField, string lineField = null)
        //{
        //    using var connection = CreateConnection();

        //    string sql;
        //    if (!string.IsNullOrEmpty(lineField))
        //        sql = $"SELECT [{xField}] AS X, [{yField}] AS Y, [{lineField}] AS Z FROM [{tableName}]";
        //    else
        //        sql = $"SELECT [{xField}] AS X, [{yField}] AS Y FROM [{tableName}]";

        //    var result = await connection.QueryAsync(sql);
        //    return result.ToList();
        //}


        public async Task<List<string>> GetTableListAsync()
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
            return (await connection.QueryAsync<string>(sql)).ToList();
        }

        public async Task<List<string>> GetColumnListAsync(string tableName)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@table";
            return (await connection.QueryAsync<string>(sql, new { table = tableName })).ToList();
        }
        public async Task<List<string>> GetNumericColumnListAsync(string tableName)
        {
            using var connection = CreateConnection();

            const string sql = @" SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table
          AND DATA_TYPE IN ('int','bigint','smallint','tinyint','decimal','numeric','float','real')";

            return (await connection.QueryAsync<string>(sql, new { table = tableName })).ToList();
        }
        public async Task<List<dynamic>> GetDynamicRawTableDataAsync(string tableName)
        {
            using var connection = CreateConnection();
            var sql = $"SELECT * FROM [{tableName}]";
            return (await connection.QueryAsync(sql)).ToList();
        }


    }
}
