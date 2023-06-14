using Core.Entities;
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
    public class AppsamplingQueryRepository : QueryRepository<AppSampling>, IAppsamplingQueryRepository
    {
        public AppsamplingQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<AppSampling>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM AppSampling";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AppSampling>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<AppSamplingLine>> GetAllLineAsync()
        {
            try
            {
                // var query = "SELECT * FROM AppSamplingLine";
                var query = "select *, t2.Value as SamplingPurpose from AppSamplingLine t1 LEFT JOIN ApplicationMasterDetail t2 ON t2.ApplicationMasterDetailID = t1.SamplingPurposeID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AppSamplingLine>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        //public async Task<IReadOnlyList<AppSampling>> GetsamplingByStatus(int id)
        //{
        //    try
        //    {
        //        var query = "SELECT * FROM AppSampling WHERE SamplingID =" + "'" + id + "'";
        //        using (var connection = CreateConnection())
        //        {
        //            return (await connection.QueryAsync<AppSampling>(query)).ToList();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw new Exception(exp.Message, exp);
        //    }
        //}
    }
}
