using Core.Repositories.Command.Base;
using Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Command.Base
{
    public class CommandRepository<T> : DbConnector, ICommandRepository<T> where T : class
    {
        public CommandRepository(IConfiguration configuration)
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
