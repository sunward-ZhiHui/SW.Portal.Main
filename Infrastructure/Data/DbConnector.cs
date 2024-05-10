 using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;


namespace Infrastructure.Data
{
    public class DbConnector
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;
        //private readonly string? _connectionPGString;

        public DbConnector(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
           // _connectionPGString = _configuration.GetConnectionString("DefaultConnection");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);


        //public IDbConnection CreatePGConnection()
        //    => new NpgsqlConnection(_connectionPGString);
    }
}
