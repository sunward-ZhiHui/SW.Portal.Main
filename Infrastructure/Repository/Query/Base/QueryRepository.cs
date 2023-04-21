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

    }
}
