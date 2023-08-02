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
    public class ReportFileUploadsQueryRepository : QueryRepository<ReportDocuments>, IReportFileUploadsQueryRepository
    {
        public ReportFileUploadsQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<ReportDocuments>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM ReportDocuments";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ReportDocuments>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
    

    
 
