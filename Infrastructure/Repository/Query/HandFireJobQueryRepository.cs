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
using Core.Entities.Views;
using Core.EntityModels;
using IdentityModel.Client;
using NAV;
using Application.Queries;
using Infrastructure.Data;

namespace Infrastructure.Repository.Query
{
    public class HandFireJobQueryRepository : DbConnector, IHandFireJobQueryRepository
    {
        public HandFireJobQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<JobSchedule> InsertHandFireJob(string? Type)
        {
            try
            {
                JobSchedule jobSchedule = new JobSchedule();
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Type", Type);
                        parameters.Add("SchedulerDateTime", DateTime.Now, DbType.DateTime);
                        var query = "INSERT INTO HandFireJob(Type,SchedulerDateTime)  " +
                            "OUTPUT INSERTED.SchedulerID VALUES " +
                            "(@Type,@SchedulerDateTime)";
                        jobSchedule.JobScheduleId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        return jobSchedule;
                    }


                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }

                }


            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }
    }
}
