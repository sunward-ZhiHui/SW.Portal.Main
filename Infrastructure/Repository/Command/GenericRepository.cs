using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using Core.Repositories.Command.Base;

namespace Infrastructure.Repository.Command
{
    public class GenericRepository<T> : DbConnector, IGenericRepository<T> where T : class
    {
        public GenericRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<T> AddAsync(T entity)
        {
            //try
            //{
            using (var connection = CreateConnection())
            {
                await connection.InsertAsync(entity);
            }
            return entity;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
        }

        public async Task UpdateAsync(T entity)
        {
            using (var connection = CreateConnection())
            {
                await connection.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(T entity)
        {
            using (var connection = CreateConnection())
            {
                await connection.DeleteAsync(entity);
            }
        }
    }
}
