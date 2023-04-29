using Core.Repositories.Query.Base;
using Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query.Base
{
    public class QueryRepository<T> : DbConnector, IQueryRepository<T> where T : class
    {
        public QueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<T> GetAsync(int id)
        {
            using var connection = CreateConnection();
            return await connection.GetAsync<T>(id);
        }
        public async Task<IEnumerable<T>> GetListAsync()
        {
            using var connection = CreateConnection();
            return await connection.GetListAsync<T>();
        }
        public async Task<IEnumerable<T>> GetListAsync(string query)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(query);
        }

        public async Task<IEnumerable<T>> GetListPaggedAsync(int pageNo, int pageSize, string condition, string orderby)
        {
            using var connection = CreateConnection();
            return await connection.GetListPagedAsync<T>(pageNo, pageSize, condition, orderby);
        }
        public void Add(T entity)
        {
            using var connection = CreateConnection();
            //string sql = "INSERT INTO " + typeof(T).Name + "s VALUES (@Property1, @Property2, ...)";
            //connection.Execute(sql, entity);

            string tableName = typeof(T).Name + "s";
            string columns = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name));
            string values = string.Join(", ", typeof(T).GetProperties().Select(p => "@" + p.Name));
            string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
            connection.Execute(sql, entity);
        }

    }
}
