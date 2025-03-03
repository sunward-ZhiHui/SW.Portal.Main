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
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Google.Type;

namespace Infrastructure.Repository.Query
{
    public class DashboardQueryRepository : QueryRepository<EmailScheduler>, IDashboardQueryRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEmailConversationsQueryRepository _emailConversationsQueryRepository;
        public DashboardQueryRepository(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IEmailConversationsQueryRepository emailConversationsQueryRepository)
            : base(configuration)
        {
            _configuration = configuration;
            _hostingEnvironment = webHostEnvironment;
            _emailConversationsQueryRepository = emailConversationsQueryRepository;
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
                                SELECT DISTINCT ECAT.ConversationId
                                FROM EmailConversationParticipant ECAT
                                WHERE ECAT.UserId = @UserId
                            )                         

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
                            INNER JOIN DistinctTopics DT ON DT.ConversationId = TS.ID
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

                        return appointment.ID;
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
                        parameters.Add("IsReminder", userMultiple.IsReminder);


                        var query = @"INSERT INTO UserMultiple (AppointmentID,UserID,AddedByUserID,AddedDate,IsReminder)
                               OUTPUT  INSERTED.UserMultipleID  VALUES (@AppointmentID,@UserID,@AddedByUserID,@AddedDate,@IsReminder)";

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
        public async Task<List<Appointment>> GetPendingAppointmentAsync()
        {
            try
            {
                var currentDate = System.DateTime.Now.Date; 
                var currentDateTime = System.DateTime.Now;  

                var parameters = new DynamicParameters();
                parameters.Add("@CurrentDate", currentDate);
                parameters.Add("@CurrentDateTime", currentDateTime);

                //var query = @"SELECT A.ID, A.AppointmentType, A.Description, A.StartDate, A.EndDate, A.Label, 
                //                       A.Location, A.Recurrence, A.AllDay, A.Caption, A.Status, A.AddedByUserID
                //                FROM Appointment A
                //                WHERE                                     
                //                    CAST(A.StartDate AS DATE) = @CurrentDate 
                //                    AND CAST(A.EndDate AS DATE) = @CurrentDate                                    
                //                    AND A.StartDate >= @CurrentDateTime
                //                ORDER BY A.StartDate";

                var query = @"SELECT A.ID, A.AppointmentType, A.Description, A.StartDate, A.EndDate, A.Label, 
                                   A.Location, A.Recurrence, A.AllDay, A.Caption, A.Status, A.AddedByUserID
                            FROM Appointment A
                            WHERE 
                                A.StartDate BETWEEN @CurrentDateTime AND DATEADD(MINUTE, 10, @CurrentDateTime) 
                            ORDER BY A.StartDate";

                //CAST(GETDATE() AS DATE) BETWEEN CAST(A.StartDate AS DATE) AND CAST(A.EndDate AS DATE)

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
        public async Task<List<long>> GetAppointmentUserMultipleAsync(long id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("id", id);

                var query = @"SELECT UserID FROM UserMultiple WHERE (IsAccepted = 1   OR IsAccepted IS NULL) and (IsReminder = 1   OR IsReminder IS NULL) and AppointmentID = @id";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<long>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Error retrieving user IDs.", exp);
            }
        }

        public async Task<string> GetPendingRemindersAsync()
        {
            var serverToken = _configuration["FcmNotification:ServerKey"];
            var baseurl = _configuration["DocumentsUrl:BaseUrl"];

            //string title = "Title";

            //string bodymsg = "welocme";

            var Result = await GetPendingAppointmentAsync();

            List<string> tokenStringList = new List<string>();

            var hosturls = "Dashboard/1";
            foreach (var item in Result)
            {
                string title = $"Upcoming: {item.Caption}";
                string bodymsg = $"{item.Description}\nStart: {item.StartDate:yyyy-MM-dd HH:mm}";


                var getuserid = await GetAppointmentUserMultipleAsync(item.ID);
                foreach(var items in getuserid)
                {                    
                    var tokens = await _emailConversationsQueryRepository.GetUserTokenListAsync(items);
                    if (tokens.Count > 0)
                    {
                        foreach (var lst in tokens)
                        {
                            await PushNotification(lst.TokenID.ToString(), title, bodymsg, lst.DeviceType == "Mobile" ? "" : hosturls);
                        }
                    }
                }
            }

            return "ok";
        }        
        public async Task<string> PushNotification(string tokens, string titles, string message, string hosturl)
        {
            var baseurl = _configuration["DocumentsUrl:BaseUrl"];
            var projectId = _configuration["FcmNotification:ProjectId"];
            var oauthToken = await GetAccessTokenAsync(_hostingEnvironment);
            var iconUrl = baseurl + "_content/AC.SD.Core/images/SWLogo.png";

            var pushNotificationRequest = new
            {
                message = new
                {
                    token = tokens,
                    notification = new
                    {
                        title = titles,
                        body = message,
                        image = iconUrl
                    },
                    android = new
                    {
                        notification = new
                        {
                            icon = iconUrl,  
                            image = iconUrl
                        }
                    },
                    webpush = new
                    {
                        fcm_options = new
                        {
                            link = hosturl
                        },
                        notification = new
                        {
                            icon = iconUrl  
                        }
                    }
                }
            };

            string url = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", oauthToken);

                try
                {
                    string serializeRequest = JsonConvert.SerializeObject(pushNotificationRequest);
                    var response = await client.PostAsync(url, new StringContent(serializeRequest, Encoding.UTF8, "application/json"));
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Log response content for debugging
                    Console.WriteLine(responseContent);

                    return responseContent; // Return response or analyze as needed
                }
                catch (Exception ex)
                {
                    // Log exceptions for further analysis
                    Console.WriteLine($"Error sending notification: {ex.Message}");
                    return $"Error: {ex.Message}";
                }
            }
        }
        private async Task<string> GetAccessTokenAsync(IWebHostEnvironment env)
        {
            string relativePath = _configuration["FcmNotification:FilePath"];

            string path = Path.Combine(env.ContentRootPath, relativePath);

            GoogleCredential credential = await GoogleCredential.FromFileAsync(path, CancellationToken.None);

            credential = credential.CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }
        public async Task<IReadOnlyList<Appointment>> GetSchedulerListAsync(long UserID)
        {
            try
            {

                var parameters = new DynamicParameters();


                parameters.Add("UserID", UserID);
              
                var query = @"select  A.ID,A.AppointmentType,A.Description,A.StartDate,A.EndDate,A.Label,A.Location,A.Recurrence,A.AllDay,A.Caption,A.Status,A.AddedByUserID From Appointment A
                                left join UserMultiple UM ON UM.AppointmentID =A.ID
                                WHERE UM.UserID = @UserID OR A.AddedByUserID = @UserID
                                GROUP BY A.ID,A.AppointmentType,A.Description,A.StartDate,A.EndDate,A.Label,A.Location,A.Recurrence,A.AllDay,A.Caption,A.Status,A.AddedByUserID
                                ";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Appointment>(query,parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<int> GetUserCreatedSchedulerCountAsync(long UserID)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("UserID", UserID);

                var query = @"SELECT COUNT(A.ID) AS AppointmentCount FROM Appointment A where AddedByUserID = @UserID 
                        AND (CONVERT(DATE, A.StartDate) >= CONVERT(DATE, GETDATE()) OR CONVERT(DATE, A.EndDate) >= CONVERT(DATE, GETDATE()))";

                using (var connection = CreateConnection())
                {
                    return (await connection.QuerySingleOrDefaultAsync<int>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<int> GetInvitationCountAsync(long UserID)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("UserID", UserID);               

                var query = @"SELECT COUNT(A.ID) AS AppointmentCount
                            FROM Appointment A
                            CROSS APPLY (
                                SELECT DISTINCT AppointmentID, IsAccepted 
                                FROM UserMultiple 
                                WHERE UserID = @UserID
                            ) UM
                            WHERE UM.AppointmentID = A.ID
                              AND (CONVERT(DATE, A.StartDate) >= CONVERT(DATE, GETDATE()) 
                                   OR CONVERT(DATE, A.EndDate) >= CONVERT(DATE, GETDATE()))
                              AND (UM.IsAccepted IS NULL OR UM.IsAccepted = 2)";

                using (var connection = CreateConnection())
                {
                    return (await connection.QuerySingleOrDefaultAsync<int>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<int> GetSchedulerCountAsync(long UserID)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("UserID", UserID);

                var query1 = @"SELECT COUNT(A.ID) AS AppointmentCount FROM Appointment A
                                LEFT JOIN UserMultiple UM ON UM.AppointmentID = A.ID
                                WHERE (UM.UserID = @UserID OR A.AddedByUserID = @UserID)
                                  AND (CONVERT(DATE, A.StartDate) >= CONVERT(DATE, GETDATE())  
                                      OR CONVERT(DATE, A.EndDate) >= CONVERT(DATE, GETDATE()))";

                var query = @"SELECT COUNT(A.ID) AS AppointmentCount FROM Appointment A
                                LEFT JOIN UserMultiple UM ON UM.AppointmentID = A.ID
                                WHERE (UM.UserID = @UserID AND UM.IsAccepted = 1)
                                  AND (CONVERT(DATE, A.StartDate) >= CONVERT(DATE, GETDATE())  
                                      OR CONVERT(DATE, A.EndDate) >= CONVERT(DATE, GETDATE()))";

                using (var connection = CreateConnection())
                {
                    return (await connection.QuerySingleOrDefaultAsync<int>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<Appointment>> GetUserListAsync(long Appointmentid)
        {
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("AppointmentID", Appointmentid);
                var query = @"
                           SELECT EP.FirstName as UserName,UM.UserID,UM.IsAccepted ,UM.IsReminder FROM UserMultiple UM
                             Left Join Employee EP ON EP.EmployeeID =UM.UserID
                             Where AppointmentID = @AppointmentID";

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

        public async Task<long> DeleteUsermultipleAsync(long appointmentid)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("id", appointmentid);

                        var query = @"Delete From UserMultiple where AppointmentID = @id";

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

        public async Task<IReadOnlyList<Appointment>> GetUserListNotificationAsync(long ID)
        {
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("AppointmentID", ID);
                var query = @"
                          SELECT UserID FROM UserMultiple Where AppointmentID = @AppointmentID";

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

        public async Task<IReadOnlyList<Appointment>> GetNotificationCaptionAsync(long ID)
        {

            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("ID", ID);
                var query = @"
                          SELECT * FROM Appointment Where ID = @ID";

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

        public async Task<List<Appointment>> GetCreatedUserAsync(long AppointmentID, long userid)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("appointmentid", AppointmentID);
                parameters.Add("userid", userid);
               var query = @"select * From Appointment where ID = @appointmentid And AddedByUserID = @userid and StartDate >= GetDate()";
              

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
        public async Task<List<Appointment>> GetAcceptedUserAsync(long appointmentid, long userid)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("appointmentid", appointmentid);
                parameters.Add("userid", userid);
                var query = @"SELECT IsAccepted from UserMultiple where UserID = @userid AND AppointmentID =@appointmentid";

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
        public async Task<long> UpdateAcceptAsync(long userid, bool accept, long appointmentid)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();


                        parameters.Add("userid", userid);
                        parameters.Add("accept", accept);

                        parameters.Add("appointmentid", appointmentid);

                        var query = @"Update UserMultiple set IsAccepted = @accept where UserID = @userid and AppointmentID = @appointmentid";
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
        public async Task<long> AddAppointmentEmailinsertAsync(Appointment userMultiple)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();


                        parameters.Add("AppointmentID", userMultiple.ID);
                        parameters.Add("ConversationId", userMultiple.ConversationId);
                        parameters.Add("AddedByUserID", userMultiple.AddedByUserID);
                        parameters.Add("AddedDate", userMultiple.AddedDate);


                        var query = @"INSERT INTO AppointmentEmailMultiple (AppointmentID,ConversationId,AddedByUserID,AddedDate)
                               OUTPUT  INSERTED.AppointmentEmailMultipleID  VALUES (@AppointmentID,@ConversationId,@AddedByUserID,@AddedDate)";

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

        public async  Task<IReadOnlyList<Appointment>> GetEmailListAsync(long Appointmentid)
        {
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("AppointmentID", Appointmentid);
                var query = @"SELECT EC.Name AS EmailTopicName,UM.ConversationId FROM AppointmentEmailMultiple UM
                              Left Join EmailConversations Ec ON Ec.ID = UM.ConversationId
                              Where AppointmentID = @AppointmentID";

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

        public async  Task<long> DeleteEmailmultipleAsync(long appointmentid)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("id", appointmentid);

                        var query = @"Delete From AppointmentEmailMultiple where AppointmentID = @id";

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

        public  async Task<long> UpdateRemainder(Appointment appointment)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();


                        parameters.Add("AppointmentID", appointment.AppointmentID);
                        parameters.Add("IsReminder", appointment.IsReminder);
                        parameters.Add("UserID", appointment.UserID);


                        var query = @"Update UserMultiple set IsReminder = @IsReminder where UserID = @UserID and  AppointmentID = @AppointmentID";
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

        public async  Task<IReadOnlyList<Appointment>> GetUserRemainderListAsync(long id, long UserID)
        {

            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("id", id);
                parameters.Add("UserID", UserID);

                var query = @"SELECT IsReminder FROM UserMultiple 
                             
                              Where AppointmentID = @id and UserID = @UserID";

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
    }
}
