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
using Application.Response;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Application.Queries;
using Microsoft.AspNetCore.Http;
using DevExpress.Data.Filtering.Helpers;
using IdentityModel.Client;

namespace Infrastructure.Repository.Query
{
    public class DashboardQueryRepository : QueryRepository<EmailScheduler>, IDashboardQueryRepository
    {
        public DashboardQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<EmailScheduler>> GetAllEmailSchedulerAsync()
        {
            try
            {
                var query = @"select ET.TopicName as Type,ET.TopicName as Caption, ET.AddedDate as StartDate, DATEADD(HOUR, 2, ET.StartDate) as EndDate,ET.ID as LabelId,ET.ID as StatusId,ET.ID as Status,ET.ID as Label from EmailTopics ET";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailScheduler>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<GenderRatio>> GetGenderRatioAsync()
        {
            try
            {
                var query = @"SELECT Gender AS region, COUNT(*) AS val
                                FROM View_Employee
                                GROUP BY Gender;";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<GenderRatio>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public  async Task<IReadOnlyList<GeneralDashboard>> GetEmployeeCountAsync()
        {
            try
            {
                var query = @"select Count(*) as HeadCount from View_Employee where StatusName!='Resign' or StatusName is null";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<GeneralDashboard>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<EmailTopics>> GetEailDashboard()
        {
            try
            {
                var query = @"select CC,TopicFrom from EmailTopics";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailTopics>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        
    }
}
