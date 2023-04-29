using Core.Entities;
using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class CodeMasterQueryRepository : QueryRepository<CodeMaster>, ICodeMasterQueryRepository
    {
        public CodeMasterQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<CodeMaster>> GetCodeMasterByStatus(string name)
        {
            try
            {
                var query = "SELECT * FROM Codemaster WHERE CodeType =" + "'" + name + "'";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<CodeMaster>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
