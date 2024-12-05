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
using static iText.IO.Image.Jpeg2000ImageData;
using static iTextSharp.text.pdf.AcroFields;
using Infrastructure.Service;

namespace Infrastructure.Repository.Query
{
    public class JobScheduleQueryRepository : DbConnector, IJobScheduleQueryRepository
    {
        private readonly IPlantQueryRepository _plantQueryRepository;
        private readonly INavItemsQueryRepository _queryRepository;
        private readonly ISalesOrderService _salesOrderService;
        public JobScheduleQueryRepository(IConfiguration configuration, IPlantQueryRepository plantQueryRepository, INavItemsQueryRepository queryRepository, ISalesOrderService salesOrderService)
            : base(configuration)
        {
            _plantQueryRepository = plantQueryRepository;
            _queryRepository = queryRepository;
            _salesOrderService = salesOrderService;
        }
        public async Task<IReadOnlyList<JobSchedule>> GetAllByAsync()
        {

            try
            {
                List<JobSchedule> schedules = new List<JobSchedule>(); List<JobScheduleWeekly> jobScheduleWeekly = new List<JobScheduleWeekly>();
                var query = "select t1.*,t2.CodeValue as Frequency,t3.PlantCode as CompanyCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser,t6.DisplayName as JobScheduleFunUnique  from JobSchedule t1 JOIN\r\nCodeMaster t2 ON t1.FrequencyID=t2.CodeID JOIN\r\nPlant t3 ON t1.CompanyID=t3.PlantID JOIN \r\nApplicationUser t4 ON t1.AddedByUserID=t4.UserID JOIN \r\nApplicationUser t5 ON t1.ModifiedByUserID=t5.UserID LEFT JOIN JobScheduleFun t6 ON t6.JobScheduleFunUniqueID=t1.JobScheduleFunUniqueID;\n\r";
                query += "select * from JobScheduleWeekly;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query);
                    schedules = results.Read<JobSchedule>().ToList();
                    jobScheduleWeekly = results.Read<JobScheduleWeekly>().ToList();
                    if (schedules != null && schedules.Count > 0)
                    {
                        schedules.ForEach(s =>
                        {
                            s.NoticeWeeklyIds = jobScheduleWeekly != null ? jobScheduleWeekly.Where(c => c.JobScheduleId == s.JobScheduleId && c.CustomType == "Weekly").Select(c => c.WeeklyId).ToList() : new List<int?>();
                            s.DaysOfWeekIds = jobScheduleWeekly != null ? jobScheduleWeekly.Where(c => c.JobScheduleId == s.JobScheduleId && c.CustomType == "Yearly").Select(c => c.WeeklyId).ToList() : new List<int?>();
                        });
                    }
                }
                return schedules;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<JobScheduleFun>> GetJobScheduleFunAsync()
        {
            try
            {
                var query = "SELECT * FROM JobScheduleFun;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<JobScheduleFun>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> DeleteJobSchedule(JobSchedule jobSchedule)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("JobScheduleId", jobSchedule.JobScheduleId);
                        var query = string.Empty;
                        query += "Delete from JobScheduleWeekly where JobScheduleId= @JobScheduleId;";
                        query += "DELETE  FROM JobSchedule WHERE JobScheduleId = @JobScheduleId;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return rowsAffected;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<JobSchedule> InsertOrUpdateJobSchedule(JobSchedule jobSchedule)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("JobScheduleId", jobSchedule.JobScheduleId);
                        parameters.Add("SessionId", jobSchedule.SessionId, DbType.Guid);
                        parameters.Add("AddedByUserID", jobSchedule.AddedByUserId);
                        parameters.Add("ModifiedByUserID", jobSchedule.ModifiedByUserId);
                        parameters.Add("AddedDate", jobSchedule.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", jobSchedule.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", jobSchedule.StatusCodeId);
                        parameters.Add("FrequencyId", jobSchedule.FrequencyId);
                        parameters.Add("StartDate", jobSchedule.StartDate, DbType.DateTime);
                        parameters.Add("StartTime", jobSchedule.StartTime, DbType.Time);
                        parameters.Add("EndDate", jobSchedule.EndDate, DbType.DateTime);
                        parameters.Add("MonthlyDay", jobSchedule.MonthlyDay);
                        parameters.Add("DaysOfWeek", jobSchedule.DaysOfWeek == true ? true : null);
                        parameters.Add("CompanyId", jobSchedule.CompanyId);
                        parameters.Add("JobScheduleFunUniqueId", jobSchedule.JobScheduleFunUniqueId);
                        if (jobSchedule.JobScheduleId > 0)
                        {
                            var query = " UPDATE JobSchedule SET JobScheduleFunUniqueId=@JobScheduleFunUniqueId,StartTime=@StartTime,FrequencyId=@FrequencyId,StartDate = @StartDate,EndDate =@EndDate,CompanyId=@CompanyId," +
                                "SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,DaysOfWeek=@DaysOfWeek," +
                                "MonthlyDay=@MonthlyDay " +
                                "WHERE JobScheduleId = @JobScheduleId";
                            await connection.ExecuteAsync(query, parameters);
                        }
                        else
                        {
                            var query = "INSERT INTO JobSchedule(JobScheduleFunUniqueId,StartTime,FrequencyId,StartDate,EndDate,SessionId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,DaysOfWeek,MonthlyDay,CompanyId)  " +
                                "OUTPUT INSERTED.JobScheduleId VALUES " +
                                "(@JobScheduleFunUniqueId,@StartTime,@FrequencyId,@StartDate,@EndDate,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@DaysOfWeek,@MonthlyDay,@CompanyId)";

                            jobSchedule.JobScheduleId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        if (jobSchedule.JobScheduleId > 0)
                        {
                            var querys = string.Empty;
                            querys += "Delete from JobScheduleWeekly where JobScheduleId=" + jobSchedule.JobScheduleId + ";\r\n";
                            if (jobSchedule.FrequencyId == 2132)
                            {
                                if (jobSchedule.NoticeWeeklyIds != null && jobSchedule.NoticeWeeklyIds.Count() > 0)
                                {
                                    jobSchedule.NoticeWeeklyIds.ToList().ForEach(b =>
                                    {
                                        var CustomType = "Weekly";
                                        querys += "INSERT INTO JobScheduleWeekly(JobScheduleId,WeeklyId,CustomType) VALUES " +
                                            "(" + jobSchedule.JobScheduleId + "," + b + ",'" + CustomType + "');\n\r";
                                    });
                                }
                            }
                            if (jobSchedule.DaysOfWeek == true && jobSchedule.FrequencyId == 2134)
                            {
                                if (jobSchedule.DaysOfWeekIds != null && jobSchedule.DaysOfWeekIds.Count() > 0)
                                {
                                    jobSchedule.DaysOfWeekIds.ToList().ForEach(b =>
                                    {
                                        var CustomType = "Yearly";
                                        querys += "INSERT INTO JobScheduleWeekly(JobScheduleId,WeeklyId,CustomType) VALUES " +
                                            "(" + jobSchedule.JobScheduleId + "," + b + ",'" + CustomType + "');\n\r";
                                    });
                                }
                            }
                            var rowsAffected = await connection.ExecuteAsync(querys);
                        }
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
        public async Task<IReadOnlyList<JobSchedule>> GetJobScheduleAsync()
        {

            try
            {

                List<JobSchedule> schedulesList = new List<JobSchedule>();
                List<JobSchedule> schedules = new List<JobSchedule>(); List<JobScheduleWeekly> jobScheduleWeekly = new List<JobScheduleWeekly>();
                var query = "select t1.*,t2.CodeValue as Frequency,t3.PlantCode as CompanyCode,t4.DisplayName as JobScheduleFunUnique from JobSchedule t1 JOIN\r\nCodeMaster t2 ON t1.FrequencyID=t2.CodeID JOIN\r\nPlant t3 ON t1.CompanyID=t3.PlantID LEFT JOIN JobScheduleFun t4 ON t4.JobScheduleFunUniqueID=t1.JobScheduleFunUniqueID;\n\r";
                query += "select t1.*,t2.CodeValue as Weekly from JobScheduleWeekly t1 JOIN CodeMaster t2 ON t1.WeeklyID=t2.CodeID;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query);
                    schedules = results.Read<JobSchedule>().ToList();
                    jobScheduleWeekly = results.Read<JobScheduleWeekly>().ToList();
                    if (schedules != null && schedules.Count > 0)
                    {
                        schedules.ForEach(s =>
                        {
                            s.NoticeWeeklyIds = jobScheduleWeekly != null ? jobScheduleWeekly.Where(c => c.JobScheduleId == s.JobScheduleId && c.CustomType == "Weekly").Select(c => c.WeeklyId).ToList() : new List<int?>();
                            s.DaysOfWeekIds = jobScheduleWeekly != null ? jobScheduleWeekly.Where(c => c.JobScheduleId == s.JobScheduleId && c.CustomType == "Yearly").Select(c => c.WeeklyId).ToList() : new List<int?>();
                        });
                        schedules = schedules.Where(w => ((DateTime.Now.Date >= w.StartDate?.Date && DateTime.Now.Date <= w.EndDate?.Date) || (DateTime.Now.Date >= w.StartDate?.Date && DateTime.Now.Date <= w.EndDate?.Date)) || w.EndDate == null).ToList();
                        int i = 1;
                        schedules = schedules.Where(w => w.StartDate == null || w.StartDate.Value.Date <= DateTime.Now.Date).ToList();
                        schedules.ForEach(s =>
                        {
                            DateTime now = DateTime.Now;
                            DateTime time = DateTime.Now;
                            if (s.StartTime != null)
                            {
                                time = DateTime.Today.Add((TimeSpan)s.StartTime);
                            }
                            string currentTime = now.ToString("hh:mm");
                            string startTime = time.ToString("hh:mm");
                            if (s.Frequency != null && s.Frequency == "Monthly" && s.MonthlyDay != null)
                            {
                                DateTime dateTime = DateTime.Now;
                                var date = new DateTime(dateTime.Year, dateTime.Month, s.MonthlyDay.Value);
                                if (date.Date == dateTime.Date)
                                {
                                    if (s.EndDate == null || date.Date <= s.EndDate.Value.Date)
                                    {
                                        if (s.StartTime != null && currentTime == startTime)
                                        {
                                            schedulesList.Add(s);
                                        }
                                    }
                                }
                            }
                            else if (s.Frequency != null && s.Frequency == "Weekly")
                            {
                                DateTime dateTime = DateTime.Now;
                                var weekList = jobScheduleWeekly.Where(w => w.CustomType == "Weekly" && w.Weekly == dateTime.DayOfWeek.ToString()).Select(s => s.Weekly).ToList();
                                if (weekList.Count > 0)
                                {
                                    if (s.EndDate == null || dateTime.Date <= s.EndDate.Value.Date)
                                    {
                                        if (s.StartTime != null && currentTime == startTime)
                                        {
                                            schedulesList.Add(s);
                                        }
                                    }
                                }
                            }
                            else if (s.Frequency != null && s.Frequency == "Yearly")
                            {
                                DateTime dateTime = DateTime.Now;
                                if (dateTime.Date == s.StartDate.Value.Date)
                                {
                                    if (s.EndDate == null || dateTime.Date <= s.EndDate.Value.Date)
                                    {
                                        if (s.StartTime != null && currentTime == startTime)
                                        {
                                            schedulesList.Add(s);
                                        }
                                    }
                                }
                                else
                                {
                                    DateTime addYear = s.StartDate.Value.AddYears(1);
                                    if (dateTime.Date == addYear.Date)
                                    {
                                        if (s.DaysOfWeek == true)
                                        {
                                            DateTime dateTimes = DateTime.Now;
                                            var weekList = jobScheduleWeekly.Where(w => w.CustomType == "Yearly" && w.Weekly == dateTimes.DayOfWeek.ToString()).Select(s => s.Weekly).ToList();
                                            if (weekList.Count > 0)
                                            {
                                                if (s.EndDate == null || dateTimes.Date <= s.EndDate.Value.Date)
                                                {
                                                    if (s.StartTime != null && currentTime == startTime)
                                                    {
                                                        schedulesList.Add(s);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (s.EndDate == null || dateTime.Date <= s.EndDate.Value.Date)
                                            {
                                                if (s.StartTime != null && currentTime == startTime)
                                                {
                                                    schedulesList.Add(s);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (s.StartTime != null && currentTime == startTime)
                                {
                                    schedulesList.Add(s);
                                }
                            }
                            i++;
                        });
                    }
                }
                return schedulesList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<List<ViewPlants>> GetPlatDatas()
        {
            List<ViewPlants> viewPlants = new List<ViewPlants>();
            var plantData = await _plantQueryRepository.GetAllByNavCompanyAsync();
            List<string> NavCompanyName = new List<string>() { "NAV_JB", "NAV_SG" };
            if (plantData != null && plantData.Count() > 0)
            {
                viewPlants = plantData.Where(w => w.NavCompanyName != null && w.NavCompanyName != "" && NavCompanyName.Contains(w.NavCompanyName)).ToList();
            }
            return viewPlants;
        }
        public async Task<string> GetJobScheduleNavFuctionAsync(string JobType)
        {
            var plantDatas = await GetPlatDatas();
            if (plantDatas != null && plantDatas.Count() > 0)
            {
                if (JobType == "NavItems")
                {
                    foreach (var item in plantDatas)
                    {
                        await _queryRepository.GetNavItemServicesList(item.PlantID, 1);
                    }
                }
                else if (JobType == "FinishedProdOrder")
                {
                    foreach (var item in plantDatas)
                    {
                        await _queryRepository.GetFinishedProdOrderLineList(item.PlantID);
                    }
                }
                else if (JobType == "ItemBatchInfo")
                {
                    foreach (var item in plantDatas)
                    {
                        await _queryRepository.GetNavItemBatchInfo(item.PlantID);
                    }
                }
                else if (JobType == "NavprodOrder")
                {
                    foreach (var item in plantDatas)
                    {
                        await _queryRepository.GetNavprodOrderLineList(item.PlantID);
                    }
                }
                else if (JobType == "NavVendor")
                {
                    foreach (var item in plantDatas)
                    {
                        await _queryRepository.GetNavVendorList(item.PlantID);
                    }
                }
                else if (JobType == "RawMatPurch")
                {
                    foreach (var item in plantDatas)
                    {
                        await _queryRepository.GetRawMatPurchList(item.PlantID);
                    }
                }
                else if (JobType == "RawMatItem")
                {
                    List<string> Types = new List<string>() { "RawMatItem", "PackagingItem", "ProcessItem" };
                    if (plantDatas != null && plantDatas.Count() > 0)
                    {
                        foreach (var item in plantDatas)
                        {
                            await _salesOrderService.RawMatItemAsync(item.NavCompanyName, item.PlantID, "RawMatItem");
                            await _salesOrderService.PackagingItemAsync(item.NavCompanyName, item.PlantID, "PackagingItem");
                            await _salesOrderService.ProcessItemAsync(item.NavCompanyName, item.PlantID, "ProcessItem");
                        }
                    }
                }
                else
                {

                }
            }
            return string.Empty;
        }
    }
}
