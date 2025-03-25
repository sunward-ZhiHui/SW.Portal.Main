using Core.Repositories.Query.Base;
using Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

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
        private string GetTableName(Type entityType)
        {
            return entityType.Name;
        }
        public async Task<long> UpdateGenericAsync(T entity, List<string> columnsToUpdate)
        {
            using var connection = CreateConnection();
            try
            {
                string tableName = GetTableName(typeof(T));

                var properties = typeof(T).GetProperties();

                var primaryKeyProperty = properties[0];


                var updateProperties = properties
                    .Where(p => columnsToUpdate.Contains(p.Name) && p != primaryKeyProperty)
                    .ToList();

                if (updateProperties.Count == 0)
                {
                    throw new ArgumentException("No valid columns to update.");
                }

                // Construct the SET clause based on the provided columnsToUpdate
                string setClause = string.Join(", ", updateProperties.Select(p => $"{p.Name} = @{p.Name}"));

                // Construct the SQL UPDATE statement
                string sql = $"UPDATE {tableName} SET {setClause} WHERE {primaryKeyProperty.Name} = @{primaryKeyProperty.Name}";


                return await connection.ExecuteAsync(sql, entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating entity: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<T>> GetListPaggedAsync(int pageNo, int pageSize, string condition, string orderby)
        {
            using var connection = CreateConnection();
            return await connection.GetListPagedAsync<T>(pageNo, pageSize, condition, orderby);
        }
        public async Task<long> InsertQuery(T entity)
        {
            using var _connection = CreateConnection();

            // Get properties that are not identity columns and are not marked with [NotMapped]
            var properties = typeof(T).GetProperties()
                                      .Where(p => p.CanRead && p.GetValue(entity) != null
                                                 && p.GetCustomAttribute<Dapper.NotMappedAttribute>() == null
                                                 && !p.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption.Equals(DatabaseGeneratedOption.Identity) == true)
                                      .ToList();

            // Check if we have valid properties to insert
            if (properties.Count == 0)
            {
                throw new InvalidOperationException("No valid properties found for insertion.");
            }

            // Create the columns and values string for the INSERT statement
            string tableName = typeof(T).Name;
            string columns = string.Join(", ", properties.Select(p => p.Name));
            string values = string.Join(", ", properties.Select(p => "@" + p.Name));

            // Construct the SQL query
            string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values}); SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            try
            {
                // Execute the SQL query and get the inserted ID
                var insertedId = await _connection.ExecuteScalarAsync<long>(sql, entity);
                return insertedId;
            }
            catch (SqlException ex)
            {
                // Log the exception for debugging
                Console.WriteLine(ex.Message);
                throw;
            }
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
