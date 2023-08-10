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
    public class FileProfileTypeDocumentQueryRepository : QueryRepository<view_FileProfileTypeDocument>, IFileProfileTypeDocumentQueryRepository
    {
        public FileProfileTypeDocumentQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<view_FileProfileTypeDocument>> GetAllFileProfileDocumentAsync()
        {
            try
            {
                var query = "select  * from view_FileProfileTypeDocument";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<view_FileProfileTypeDocument>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
