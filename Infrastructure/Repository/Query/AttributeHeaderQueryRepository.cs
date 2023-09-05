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
    public class AttributeHeaderQueryRepository : QueryRepository<AttributeHeader>, IAttributeQueryRepository
    {
        public AttributeHeaderQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public Task<long> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<AttributeHeader>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM AttributeHeader";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AttributeHeader>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<AttributeHeader> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM AttributeHeader WHERE AttrubuteID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<AttributeHeader>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public Task<long> Insert(AttributeHeader attributeHeader)
        {
            throw new NotImplementedException();
        }

        public Task<long> UpdateAsync(AttributeHeader attributeHeader)
        {
            throw new NotImplementedException();
        }
    }
}
