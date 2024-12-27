using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using NAV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class QCTimeSheetQueryRepository : DbConnector, IQCTimesheetQueryRepository
    {
        public QCTimeSheetQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<TimeSheetForQC>> GetAllAsync()
        {
            try
            {
              
                var query = @"select AU.UserName as AddedBy,DC.DocumentID,DC.FilePath,DC.IsNewPath,DC.SessionID as DocumentSessionId,DC.IsLocked,DC.LockedByUserId,
                            DC.UniqueSessionID,DC.ContentType,DC.FileName,QC.ItemName,QC.RefNo,QC.Stage,QC.AddedDate,QC.SpecificTestName,
                            QC.QRcode,QC.TestName,QC.SessionID,QC.QCTimesheetID,QC.Comment,AE.EmailTopicSessionId,AE.SessionId as ActivitySessionID
                            From TimeSheetForQC QC
                            Left Join Documents DC on DC.SessionID = QC.SessionID
                            Left Join ApplicationUser AU on AU.UserID = QC.AddedByUserID 
                            Left Join ActivityEmailTopics AE On AE.ActivityMasterId = QC.QCTimesheetID And AE.ActivityType ='TimeSheetForQC'
                            and AE.DocumentSessionId IS Not Null

                            ";

                using (var connection = CreateConnection())
                {
                   return(await connection.QueryAsync<TimeSheetForQC>(query)).ToList();

              
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async  Task<IReadOnlyList<TimeSheetForQC>> GetAllQCTimeSheetAsync(long QCTimeSheetID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("QCTimeSheetID", QCTimeSheetID);
                var query = @"select SessionID From TimeSheetForQC Where QCTimesheetID = @QCTimeSheetID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<TimeSheetForQC>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Insert(TimeSheetForQC timeSheetForQC)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ItemName", timeSheetForQC.ItemName);
                        parameters.Add("RefNo", timeSheetForQC.RefNo);
                        parameters.Add("Stage", timeSheetForQC.Stage);
                        parameters.Add("TestName", timeSheetForQC.TestName);
                        parameters.Add("QRcode", timeSheetForQC.QRcode);
                        parameters.Add("DetailEntry", timeSheetForQC.DetailEntry);
                        parameters.Add("Comment", timeSheetForQC.Comment);
                        parameters.Add("SpecificTestName", timeSheetForQC.SpecificTestName);
                        parameters.Add("SessionId", timeSheetForQC.SessionId);
                        parameters.Add("AddedByUserID", timeSheetForQC.AddedByUserID);
                        parameters.Add("AddedDate", timeSheetForQC.AddedDate);
                        parameters.Add("Action", timeSheetForQC.Action);
                        parameters.Add("MachineAction", timeSheetForQC.MachineAction);
                        parameters.Add("MachineName", timeSheetForQC.MachineName);


                        var query = @"INSERT INTO TimeSheetForQC(MachineAction,MachineName,ItemName,RefNo,Stage,TestName,QRcode,DetailEntry,Comment,SessionId,AddedByUserID,AddedDate,SpecificTestName,Action)
                         OUTPUT  INSERTED.QCTimesheetID
                         VALUES (@MachineAction,@MachineName,@ItemName,@RefNo,@Stage,@TestName,@QRcode,@DetailEntry,@Comment,@SessionId,@AddedByUserID,@AddedDate,@SpecificTestName,@Action)";
                        var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);
                        var id  = insertedId;

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
            }
        }

        public async Task<long> Update(TimeSheetForQC timeSheetForQC)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                       
                        parameters.Add("Comment", timeSheetForQC.Comment);

                        parameters.Add("QCTimesheetID", timeSheetForQC.QCTimesheetID);
                        parameters.Add("ModifiedByUserID", timeSheetForQC.ModifiedByUserID);
                        parameters.Add("ModifiedDate", timeSheetForQC.ModifiedDate);


                        var query = @"Update  TimeSheetForQC  Set Comment = @Comment, ModifiedByUserID = @ModifiedByUserID,ModifiedDate  = @ModifiedDate
                                         where QCTimesheetID = @QCTimesheetID";

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
            }
        }

        public async Task<long> UpdateStatus(long ID, long StatusID,long ModifiedByUserID)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var Date = DateTime.Now;
                        var parameters = new DynamicParameters();

                        parameters.Add("ID", ID);
                        parameters.Add("ModifiedByUserID", ModifiedByUserID);
                        parameters.Add("ModifiedDate", Date);
                        parameters.Add("StatusID", StatusID);
                       

                        var query = @"Update  TimeSheetForQC  Set ActivityStatusId = @StatusID, ModifiedByUserID = @ModifiedByUserID,ModifiedDate  = @ModifiedDate
                                         where QCTimesheetID = @ID";

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
            }
        }
        public async Task<IReadOnlyList<TimeSheetForQC>> GetMultipleQueryAsync(long? QCTimesheetID)
        {

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("QCTimeSheetID", QCTimesheetID);
                var query = @"select ActivityEmailTopicID,ActivityType,EmailTopicSessionId,ActivityMasterId,SessionId from ActivityEmailTopics where documentsessionid is not null AND ActivityType='TimeSheetForQC' AND ActivityMasterId = @QCTimesheetID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<TimeSheetForQC>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
            
        }

        public async Task<IReadOnlyList<view_QCAssignmentRM>> GetAllListByQRAsync(string Date, string Company)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Date", Date);
                parameters.Add("Company", Company);
                var query = @"select * from view_QCAssignmentRM where Date =@Date and Company =@Company";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<view_QCAssignmentRM>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }

        public async  Task<IReadOnlyList<view_QCAssignmentRM>> GetAllQCListByQRAsync(string ItemName, string QCRefNo, string TestName)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ItemName", ItemName);
                parameters.Add("QCRefNo", QCRefNo);
                parameters.Add("TestName", TestName);
                var query = @"select * from view_QCAssignmentRM where Description =@ItemName and QCReferenceNo =@QCRefNo and Test =@TestName";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<view_QCAssignmentRM>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
