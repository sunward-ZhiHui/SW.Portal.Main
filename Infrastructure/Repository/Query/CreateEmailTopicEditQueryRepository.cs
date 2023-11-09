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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class CreateEmailTopicEditQueryRepository : QueryRepository<EmailTopics>, ICreateEmailTopicEditQueryRepository
    {
        public CreateEmailTopicEditQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<Documents>> GetAllAsync(Guid? sessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", sessionId);
                var query = "select  * from Documents where SessionId = @SessionId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Documents>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Update(EmailTopics emailTopics)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("SessionId", emailTopics.SessionId);
                            parameters.Add("FileData", emailTopics.FileData);

                            var query = " UPDATE EmailTopics SET FileData = @FileData WHERE SessionId = @SessionId";
                            var rowsAffected = await connection.ExecuteAsync(query, parameters);

                            return rowsAffected;
                        }
                        catch (Exception exp)
                        {                            
                            throw new Exception(exp.Message, exp);
                        }                    
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


    }
}
