using Core.Repositories.Command.Base;
using Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Base;
using System.Reflection;
using System.Data.Entity.Core.Objects.DataClasses;

namespace Infrastructure.Repository.Command.Base
{
    public class CommandRepository<T> : DbConnector, ICommandRepository<T> where T : class
    {
        public CommandRepository(IConfiguration configuration)
           : base(configuration)
        {

        }

        public async Task<int?> AddAsync(T entity)
        {
            int Id = 0;
            //try
            //{
            using (var connection = CreateConnection())
            {
                var id=await connection.InsertAsync(entity);
                Id = id.Value;
            }
            return Id;
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
