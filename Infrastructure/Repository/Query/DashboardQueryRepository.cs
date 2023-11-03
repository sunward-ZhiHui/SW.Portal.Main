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
using System.Security.Cryptography;

namespace Infrastructure.Repository.Query
{
    public class DashboardQueryRepository : QueryRepository<EmailScheduler>, IDashboardQueryRepository
    {
        public DashboardQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        
       

        public async Task<List<GenderRatio>> GetGenderRatioAsync()
        {
            try
            {
                //var query = @"SELECT Gender AS region, COUNT(*) AS val
                //                FROM View_Employee
                //                GROUP BY Gender;";


                var query = @"SELECT Gender AS region, COUNT(*) AS val
                                FROM View_Employee where Gender Is NOT NULL
                                GROUP BY Gender";
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

        public  async Task<IReadOnlyList<EmailRatio>> GetEmailRatioAsync(long id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("id", id);
                var query = @"declare @UserId int = @id

                     SELECT SUM(FN.NotificationCount) AS ToCount,SUM(FN.NotificationCount) AS CCCount,SUM(FN.NotificationCount) AS AllCount FROM EmailConversations EC
			         INNER JOIN EmailConversationParticipant ECP ON ECP.ConversationId = EC.ID AND ECP.UserId = @UserId
			         Cross APPLY(SELECT DISTINCT ReplyId = CASE WHEN ECC.ReplyId >0 THEN ECC.ReplyId ELSE ECC.ID END
				     FROM EmailConversations ECC 
				     WHERE --ECC.TopicID=TS.ID
				        --AND 
				        EXISTS(SELECT * FROM EmailConversationAssignTo TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = @UserId or TP.AddedByUserID = @UserId))
				        )K
				         OUTER APPLY
				        (
					        SELECT									
						        COUNT(*) AS NotificationCount
					        FROM  EmailConversations ECC
					        INNER JOIN EmailNotifications EN ON ECC.ID=EN.ConversationId and EN.IsRead = 0
					        WHERE EN.TopicId=EC.TopicID AND EN.UserId=ECP.UserId AND EC.ID=ECC.ReplyId
				        ) FN 
			        WHERE EC.ID=K.ReplyId ";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailRatio>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }


        public async Task<IReadOnlyList<EmailScheduler>> GetAllEmailSchedulerTodoAsync(long UserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);
                var query = @"select ET.TopicName as Type,ET.TopicName as Caption, ET.AddedDate as StartDate, DATEADD(HOUR, 2, ET.StartDate) as EndDate,ET.ID as LabelId,ET.ID as StatusId,ET.ID as Status,ET.ID as Label from EmailTopics ET
                                UNION ALL
                                select ET.TopicName as Type,TNH.Description as Caption, TNH.DueDate as StartDate, DATEADD(HOUR, 2, TNH.DueDate) as EndDate,ET.ID as LabelId,ET.ID as StatusId,ET.ID as Status,ET.ID as Label from EmailTopics ET
                                inner join EmailConversations EC on EC.TopicID = ET.ID
                                inner join ToDoNotesHistory TNH ON TNH.TopicId = EC.ID WHERE TNH.AddedByUserID  = @UserId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailScheduler>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
