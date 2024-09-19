using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
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
                      
                        parameters.Add("SessionId", timeSheetForQC.SessionId);
                        parameters.Add("AddedByUserID", timeSheetForQC.AddedByUserID);
                        parameters.Add("AddedDate", timeSheetForQC.AddedDate);
                    

                        var query = @"INSERT INTO TimeSheetForQC(ItemName,RefNo,Stage,TestName,QRcode,DetailEntry,Comment,SessionId,AddedByUserID,AddedDate)
                         OUTPUT  INSERTED.QCTimesheetID
                         VALUES (@ItemName,@RefNo,@Stage,@TestName,@QRcode,@DetailEntry,@Comment,@SessionId,@AddedByUserID,@AddedDate)";
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
    }
}
