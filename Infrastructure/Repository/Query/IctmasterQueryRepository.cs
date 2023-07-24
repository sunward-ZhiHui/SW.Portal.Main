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
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class IctmasterQueryRepository : QueryRepository<ViewIctmaster>, IIctmasterQueryRepository
    {
        public IctmasterQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

      

        public  async Task<ViewIctmaster> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_Ictmaster WHERE IctmasterId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewIctmaster>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
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

        //public async Task<IReadOnlyList<Ictmaster>> GetIctmasterByMasterType(int mastertype)
        //{
        //    try
        //    {
        //        var query = "SELECT * FROM Ictmaster WHERE MasterType =" + "'" + mastertype + "'";
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryAsync<Ictmaster>(query)).ToList();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw new Exception(exp.Message, exp);
        //    }
        //}

        
         async  Task<IReadOnlyList<ViewIctmaster>> IIctmasterQueryRepository.GetBySiteAsync(int MasterType)
        {
            try
            {
                var query = "select  * from view_Ictmaster where MasterType = @MasterType";
                var parameters = new DynamicParameters();
                parameters.Add("MasterType", MasterType);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewIctmaster>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        async  Task <IReadOnlyList<ViewIctmaster>> IIctmasterQueryRepository.GetAllAsync()
        {
            try
            {
                var query = "select  * from view_Ictmaster";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewIctmaster>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
   
}
