using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class EmailActivityCatgorysQueryRepository : QueryRepository<EmailActivityCatgorys>, IEmailActivityCatgorysQueryRepository
    {
        public EmailActivityCatgorysQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<EmailActivityCatgorys>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM EmailActivityCatgorys";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailActivityCatgorys>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Insert(EmailActivityCatgorys emailActivityCatgorys)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("Name", emailActivityCatgorys.Name);
                            parameters.Add("ID", emailActivityCatgorys.ID);
                           

                            var query = "INSERT INTO EmailActivityCatgorys(Name,ID) VALUES (@Name,@ID)";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }


                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            };
        }
    }
}
