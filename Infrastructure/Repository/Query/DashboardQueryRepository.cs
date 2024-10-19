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
using static IdentityModel.OidcConstants;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

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
        public async Task<List<Appointment>> GetAppointments(long userId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserId", userId);
            //    var queryS = @"SELECT *,0 as Accepted,NEWID() as SessionId,'Appointment' as StatusType from Appointment WHERE AddedByUserID  = @UserId
            //                    UNION ALL                                    
            //                        SELECT ID,0 as AppointmentType,
            //                        --DueDate as StartDate,
	           //                         CASE 
            //                                WHEN CAST(DueDate AS TIME) = '00:00:00' THEN DATEADD(HOUR, 9, DueDate) -- Add 9 hours if time is 00:00:00
            //                                ELSE DueDate 
            //                            END AS StartDate,
	           //                           CASE 
            //                                WHEN CAST(DueDate AS TIME) = '00:00:00' THEN DATEADD(HOUR, 9, DATEADD(MINUTE, 15, DATEADD(DAY, NoOfDays, DueDate))) -- Add 9 hours to EndDate if time is 00:00:00, and add 15 minutes if same time as StartDate
            //                                WHEN CAST(DueDate AS TIME) = CAST(DueDate AS TIME) THEN DATEADD(MINUTE, 15, DATEADD(DAY, NoOfDays, DueDate)) -- Add 15 minutes to EndDate if StartDate and EndDate have the same time
            //                                ELSE DATEADD(DAY, NoOfDays, DueDate) 
            //                            END AS EndDate,
            //                        --DueDate + NoOfDays AS EndDate,
            //                        Name AS Caption, 1 as Label, 
            //                        3 as Status, 0 as AllDay, null as  Recurrence, null as Location, null as Description,
            //                        AddedByUserID,AddedDate,1 as Accepted,TS.SessionId,'EmailDueDate' as StatusType
            //                        from EmailConversations TS where TS.DueDate IS NOT NULL AND TS.AddedByUserID  = @UserId
            //                    UNION ALL
            //                        SELECT TNH.ID,0 as AppointmentType,TNH.DueDate as StartDate,
            //                        --TNH.DueDate as EndDate,
            //                        CASE WHEN TNH.DueDate = TNH.DueDate THEN DATEADD(minute, 15, TNH.DueDate) ELSE TNH.DueDate END AS EndDate,CONCAT(EC.Name,'-',ET.TopicName) as Caption,4 as Label,
            //                        4 as Status, 0 as AllDay, null as  Recurrence, null as Location, CONCAT(TNH.Description,'-',TD.Notes) as Description,
            //                        TNH.AddedByUserID,TNH.AddedDate,1 as Accepted,TNH.SessionId,'TodoDueDate' as StatusType FROM ToDoNotesHistory TNH
            //                        INNER JOIN EmailConversations EC ON EC.ID = TNH.TopicId
            //                        INNER JOIN EmailTopics ET ON ET.ID = EC.TopicId
            //                        INNER JOIN ToDoNotes TD ON TD.ID = TNH.NotesId
								    //LEFT JOIN TodoNotesUsers TNU ON TNU.NotesHistoryID = TNH.ID
            //                        LEFT JOIN ApplicationUser AP ON AP.UserID = TNU.UserID
            //                        WHERE TNH.AddedByUserID = @UserId
            //                            AND TNH.TopicId IS NOT NULL
            //                            AND TNH.TopicId > 0         
            //                            AND TNH.DueDate IS NOT NULL
            //                            AND TNH.Status = 'Open'";

                var query = @"WITH DistinctTopics AS (
                                SELECT DISTINCT ECAT.TopicId
                                FROM EmailConversationParticipant ECAT
                                WHERE ECAT.UserId = @UserId
                            )

                            -- First SELECT statement
                            SELECT 
                                A.ID,
                                0 AS AppointmentType, 
                                A.StartDate,    
                                A.EndDate,      
                                A.Caption AS Caption,      
                                A.Label,
                                A.Status,
                                --A.AllDay,
                                1 AS AllDay,
                                A.Recurrence,
                                A.Location,
                                A.Description,
                                A.AddedByUserID,
                                A.AddedDate,
                                0 AS Accepted,
                                NEWID() AS SessionId,
                                'Appointment' AS StatusType,
                                2 AS ResourceId,
	                            '' as UserTag,
	                            '' as OtherTag
                            FROM Appointment A
                            WHERE A.AddedByUserID = @UserId

                            UNION ALL

                            -- Second SELECT statement
                            SELECT 
                                TS.ID,
                                0 AS AppointmentType,
                                CASE 
                                    WHEN CAST(TS.DueDate AS TIME) = '00:00:00' THEN DATEADD(HOUR, 9, TS.DueDate)
                                    ELSE TS.DueDate 
                                END AS StartDate,
                                CASE 
                                    WHEN CAST(TS.DueDate AS TIME) = '00:00:00' THEN DATEADD(HOUR, 9, DATEADD(MINUTE, 15, DATEADD(DAY, TS.NoOfDays, TS.DueDate)))
                                    WHEN CAST(TS.DueDate AS TIME) = CAST(TS.DueDate AS TIME) THEN DATEADD(MINUTE, 15, DATEADD(DAY, TS.NoOfDays, TS.DueDate))
                                    ELSE DATEADD(DAY, TS.NoOfDays, TS.DueDate) 
                                END AS EndDate,
                                TS.Name AS Caption,
                                1 AS Label,
                                3 AS Status,
                                1 AS AllDay,
                                NULL AS Recurrence,
                                NULL AS Location,
                                NULL AS Description,
                                TS.AddedByUserID,
                                TS.AddedDate,
                                1 AS Accepted,
                                TS.SessionId,
                                'EmailDueDate' AS StatusType,
                                0 AS ResourceId,
	                            ECTUT.UserTag as UserTag,
	                            EAC.Name as OtherTag
                            FROM EmailConversations TS
                            INNER JOIN DistinctTopics DT ON DT.TopicId = TS.TopicID
                            LEFT JOIN EmailTopicUserTags ECTUT ON ECTUT.TopicId = TS.TopicID AND ECTUT.AddedByUserID = @UserId
                            LEFT JOIN EmailActivityCatgorys EAC ON EAC.TopicId = TS.TopicID	
                            WHERE TS.DueDate IS NOT NULL

                            UNION ALL

                            -- Third SELECT statement
                            SELECT 
                                TNH.ID,
                                0 AS AppointmentType,
                                TNH.DueDate AS StartDate,
                                CASE 
                                    WHEN TNH.DueDate = TNH.DueDate THEN DATEADD(MINUTE, 15, TNH.DueDate) 
                                    ELSE TNH.DueDate 
                                END AS EndDate,
                                CONCAT(EC.Name, '-', ET.TopicName) AS Caption,
                                4 AS Label,
                                4 AS Status,
                                1 AS AllDay,
                                NULL AS Recurrence,
                                NULL AS Location,
                                CONCAT(TNH.Description, '-', TD.Notes) AS Description,
                                TNH.AddedByUserID,
                                TNH.AddedDate,
                                1 AS Accepted,
                                TNH.SessionId,
                                'TodoDueDate' AS StatusType,
                                1 AS ResourceId,
	                            ECTUT.UserTag as UserTag,
	                            EAC.Name as OtherTag
                            FROM ToDoNotesHistory TNH
                            INNER JOIN EmailConversations EC ON EC.ID = TNH.TopicId
                            INNER JOIN EmailTopics ET ON ET.ID = EC.TopicId
                            LEFT JOIN EmailTopicUserTags ECTUT ON ECTUT.TopicId = ET.ID AND ECTUT.AddedByUserID = @UserId
                            LEFT JOIN EmailActivityCatgorys EAC ON EAC.TopicId = ET.ID	
                            INNER JOIN ToDoNotes TD ON TD.ID = TNH.NotesId
                            LEFT JOIN TodoNotesUsers TNU ON TNU.NotesHistoryID = TNH.ID
                            LEFT JOIN ApplicationUser AP ON AP.UserID = TNU.UserID
                            WHERE TNH.AddedByUserID = @UserId
                                AND TNH.TopicId IS NOT NULL
                                AND TNH.TopicId > 0         
                                AND TNH.DueDate IS NOT NULL
                                AND TNH.Status = 'Open'";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Appointment>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> AddAppointmentAsync(Appointment appointment)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                       
                        parameters.Add("AppointmentType", appointment.AppointmentType);
                        parameters.Add("StartDate", appointment.StartDate);
                        parameters.Add("EndDate", appointment.EndDate);
                        parameters.Add("Caption", appointment.Caption);
                        parameters.Add("Label", appointment.Label);
                        parameters.Add("Status", appointment.Status);
                        parameters.Add("AllDay", appointment.AllDay);
                        parameters.Add("Recurrence", appointment.Recurrence);
                        parameters.Add("AddedByUserID", appointment.AddedByUserID);
                        parameters.Add("AddedDate", appointment.AddedDate);
                        parameters.Add("Location", appointment.Location);
                        parameters.Add("Description", appointment.Description);
                      
                        var query = @"INSERT INTO Appointment (AppointmentType,StartDate,EndDate,Caption,Label,Status,AllDay,Recurrence,AddedByUserID,AddedDate,Location,Description)
                               OUTPUT  INSERTED.ID  VALUES (@AppointmentType,@StartDate,@EndDate,@Caption,@Label,@Status,@AllDay,@Recurrence,@AddedByUserID,@AddedDate,@Location,@Description)";

                        var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);
                        var id = insertedId;

                        return id;
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
            };
        }
        

        public async Task<long> UpdateAppointmentAsync(Appointment appointment)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();

                       
                        parameters.Add("StartDate", appointment.StartDate);
                        parameters.Add("EndDate", appointment.EndDate);
                        parameters.Add("Caption", appointment.Caption);
                        parameters.Add("Label", appointment.Label);
                        parameters.Add("Status", appointment.Status);
                        parameters.Add("AllDay", appointment.AllDay);                       
                        parameters.Add("Location", appointment.Location);
                        parameters.Add("Description", appointment.Description);
                        parameters.Add("ID", appointment.ID);


                        var query = @"UPDATE Appointment SET StartDate = @StartDate,EndDate = @EndDate,Caption = @Caption,Label = @Label,Status = @Status,AllDay = @AllDay,Location = @Location,Description = @Description WHERE ID = @ID";

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
            };
        }


        public async Task<long> DeleteAppointmentAsync(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("id", id);

                        var query = @"DELETE FROM Appointment WHERE ID = @id";

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
            };
        }
        public async Task<IReadOnlyList<EmailScheduler>> GetAllEmailSchedulerTodoAsync(long UserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);
                var query = @"SELECT BackgroundCss='dx-purple-color',TextCss='text-white', ET.ID,ET.TopicName as Name, ET.TopicName as Type,ET.TopicName as Caption, ET.AddedDate as StartDate, DATEADD(HOUR, 2, ET.StartDate) as EndDate,ET.ID as LabelId,ET.ID as StatusId,ET.ID as Status,ET.ID as Label from EmailTopics ET
                                WHERE ET.AddedByUserID  = @UserId
	                                UNION ALL
                                SELECT BackgroundCss = 'dx-green-color', TextCss='text-white',ET.ID,ET.TopicName as Name, ET.TopicName as Type,TNH.Description as Caption, TNH.DueDate as StartDate, DATEADD(HOUR, 2, TNH.DueDate) as EndDate,ET.ID as LabelId,ET.ID as StatusId,ET.ID as Status,ET.ID as Label from EmailTopics ET
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

        public  async Task<IReadOnlyList<DynamicForm>> GetApprovalListAsync()
        {
            try
            {
                var query = @"	select ID,Name,ScreenID,t1.PlantCode as CompanyName from DynamicForm DF
							INNER JOIN Plant t1 on t1.PlantID =DF.CompanyID
							where IsApproval=1 AND (IsDeleted is null or IsDeleted=0)";


                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicForm>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<DynamicFormData>> GetDynamicDataAsync(long dynamicID)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("id", dynamicID);

                var query = @"select Distinct  DFD.DynamicFormDataID ,DPS.Name,DFD.ProfileNo from DynamicFormData DFD
                            INNER JOIN DocumentProfileNoSeries DPS On DPS.ProfileID = DFD.ProfileID
							inner Join DynamicFormApproved DFA on DFA.DynamicFormDataID =DFD.DynamicFormDataID
                            where DynamicFormID =@id";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormData>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
            
        }

        public  async Task<IReadOnlyList<DynamicFormApproved>> GetDynamicApprovedStatusAsync(long DataFormID)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("id", DataFormID);

                var query = @"select(CASE
                            WHEN DFA.IsApproved IS Null THEN 'Pending'
                            WHEN DFA.IsApproved  = 1 THEN 'Approved'
                            WHEN DFA.IsApproved  = 0 THEN 'Rejected'
                            ELSE 'Completed'
                        END) as ApprovedStatus, DFA.DynamicFormApprovedID,DFA.ApprovedDescription,AU.UserName as ApprovedByUser,DFA.IsApproved from DynamicFormApproved DFA
                            Inner Join ApplicationUser AU on AU.UserID = DFA.UserID
                            where DynamicFormDataID = @id";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormApproved>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> AddAppointmentinsertAsync(Appointment userMultiple)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();

                    
                        parameters.Add("AppointmentID", userMultiple.ID);
                        parameters.Add("UserID", userMultiple.UserID);
                        parameters.Add("AddedByUserID", userMultiple.AddedByUserID);
                        parameters.Add("AddedDate", userMultiple.AddedDate);
                      

                        var query = @"INSERT INTO UserMultiple (AppointmentID,UserID,AddedByUserID,AddedDate)
                               OUTPUT  INSERTED.UserMultipleID  VALUES (@AppointmentID,@UserID,@AddedByUserID,@AddedDate)";

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
            };
        }

        public async Task<IReadOnlyList<Appointment>> GetSchedulerListAsync()
        {
            try
            {
                var query = @"
                             select * from Appointment";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Appointment>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
