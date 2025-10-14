using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
using Infrastructure.Data;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using NAV;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.EntityModels.SyrupPlanning;
using static Core.EntityModels.SyrupReportDtos;
using static Infrastructure.Repository.Query.ACEntrysQueryRepository;
using static iText.IO.Image.Jpeg2000ImageData;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static iTextSharp.text.pdf.AcroFields;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Repository.Query
{
    public class StockInformationMasterQueryRepository : DbConnector, IStockInformationMasterQueryRepository
    {

        public StockInformationMasterQueryRepository(IConfiguration configuration)
            : base(configuration)
        {
        }
        public async Task<IReadOnlyList<StockInformationMaster>> GetAllByAsync()
        {
            try
            {
                List<StockInformationMaster> aCItemsModels = new List<StockInformationMaster>();
                var parameters = new DynamicParameters();
                var query = @"SELECT *,CONCAT(P.PlantCode ,'-', P.Description)  as CompanyName,CONCAT(C.Name,'-',C.Code) as CustomerName,AMD.Value as PlanningCategoryName,AMD.description as PlanningCategoryDescName from StockInformationMaster SM
                                LEFT JOIN Plant P ON P.PlantID = SM.CompanyID
                                LEFT JOIN NAVCustomer C ON C.CustomerId = SM.CustomerID
                                LEFT JOIN ApplicationMasterDetail AMD ON AMD.ApplicationMasterDetailID = SM.PlanningCategory";
                using (var connection = CreateConnection())
                {
                    aCItemsModels = (await connection.QueryAsync<StockInformationMaster>(query, parameters)).ToList();
                }

                return aCItemsModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<long> InsertOrUpdate(string? TableName, string? PrimareyKeyName, long PrimareyKeyId, DynamicParameters parameters)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var query = string.Empty;
                    if (PrimareyKeyId > 0)
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "UPDATE " + TableName + " SET\r";
                            var names = string.Empty;
                            if (subDynamic.ParameterNames is not null)
                            {
                                foreach (var keyValue in subDynamic.ParameterNames)
                                {
                                    names += keyValue + "=";
                                    names += "@" + keyValue + ",";
                                }
                            }
                            query += names.TrimEnd(',') + "\rwhere " + PrimareyKeyName + " = " + PrimareyKeyId + ";";
                        }
                    }
                    else
                    {
                        if (parameters is DynamicParameters subDynamic)
                        {
                            query += "INSERT INTO " + TableName + "(\r";
                            var values = string.Empty;
                            var names = string.Empty;
                            if (subDynamic.ParameterNames is not null)
                            {
                                foreach (var keyValue in subDynamic.ParameterNames)
                                {
                                    names += keyValue + ",";
                                    values += "@" + keyValue + ",";
                                }
                            }
                            query += names.TrimEnd(',') + ")\rOUTPUT INSERTED." + PrimareyKeyName + " VALUES(" + values.TrimEnd(',') + ");";
                        }
                    }
                    if (!string.IsNullOrEmpty(query))
                    {
                        if (PrimareyKeyId > 0)
                        {
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            PrimareyKeyId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                    }
                }
                return PrimareyKeyId;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<StockInformationMaster> InsertOrUpdateStockInformationMaster(StockInformationMaster value)
        {
            try
            {
                //var oldData = await _auditLogQueryRepository.GetDataSourceOldData("NavitemLinks", "ItemLinkId", value.ItemLinkId);
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("MasterName", value.MasterName);
                    parameters.Add("CompanyID", value.CompanyID);
                    parameters.Add("CustomerID", value.CustomerID);
                    parameters.Add("PlanningCategory", value.PlanningCategory);
                    parameters.Add("BelowMonth", value.BelowMonth);
                    parameters.Add("TopupMonth", value.TopupMonth);
                    parameters.Add("SessionId", value.SessionId);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);

                    value.StockInformationMasterID = await InsertOrUpdate("StockInformationMaster", "StockInformationMasterID", value.StockInformationMasterID, parameters);
                    return value;
                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<StockInformationMaster> DeleteStockInformationMaster(StockInformationMaster value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("StockInformationMasterID", value.StockInformationMasterID);
                        var query = "DELETE FROM StockInformationMaster WHERE StockInformationMasterID= @StockInformationMasterID;";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return value;
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
        public async Task<SyrupPlanning?> SelectSyrupSimplexDataList(long methodCodeID)
        {
            const string sql = @"select DynamicFormDataItemID, DynamicFormDataID,ProfileNo,[13192_MethodCode_UId] as MethodCodeID,[13229_2003_ProductionPlanningProcess] as SyrupSimplexProcessName, [13229_1954_Location] as SyrupSimplexLocation,[13229_IsthereSyrupSimplextoproduce] as IsthereSyrupSimplextoproduce,
                    [13229_1955_1PreparationHour] as SyrupSimplexPreparationHour,
                    ISNULL(NULLIF([13229_1956_SyrupSimplexManpower], ''), 0) as SyrupSimplexManpower,
                    [13229_1957_Level2CleaningHours] as SyrupSimplexLevel2CleaningHours,
                    ISNULL(NULLIF([13229_1958_Level2CleaningManpower], ''), 0) as SyrupSimplexLevel2CleaningManpower, 
                    ISNULL(NULLIF([13229_2009_NoofCampaign], ''), 0) AS SyrupSimplexNoofCampaign,
                    [13229_2004_NextProcessName] as SyrupSimplexNextProcessName
                    from DynamicForm_ProductiontimingNMachineInfosyrup where [13192_MethodCode_UId] = @MethodCodeID";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SyrupPlanning>(sql, new { MethodCodeID = methodCodeID });
        }


        public async Task<SyrupPlanning?> SelectSyruppreparationDataList(long methodCodeID)
        {
            const string sql = @"select DynamicFormDataID,
                    [13229_2004_ProductionPlanningProcess] as SyruppreparationProcessName,
                    [13202_Location] as SyruppreparationLocation,
                    [13203_1PreparationfirstVolumnHour] as SyruppreparationFirstVolumnHour,
                    [13204_1PreparationFirstVolumnManpower] as SyruppreparationFirstVolumnManpower,
                    [13205_2IPQCtest] as SyruppreparationIPQCTest,
                    [13206_3PreparationTopuptoVolumnHour] as SyruppreparationTopupToVolumnHour,
                    [13207_3PreparationTopuptoVolumnManpower] as SyruppreparationTopupToVolumnManpower,
                    [13208_CampaignBatchesNumbers] as SyruppreparationCampaignBatchesNumbers,
                    [13209_Level1CleaningHours] as SyruppreparationLevel1CleaningHours,
                    [13210_Level1Cleaningmanpower] as SyruppreparationLevel1Cleaningmanpower,
                    [13211_Level2Cleaninghours] as SyruppreparationLevel2Cleaninghours,
                    [13212_Level2CleaningManpower] as SyruppreparationLevel2CleaningManpower,
                    [13264_ProductionPlanningProcess] as SyruppreparationNextProcessName
                    from DynamicForm_ProductiontimingNMachineInfosyrup where [13192_MethodCode_UId] = @MethodCodeID";

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SyrupPlanning>(sql, new { MethodCodeID = methodCodeID });
        }
        public async Task<IReadOnlyList<SyrupProcessNameList>> GetSyrupProcessNameList()
        {
            try
            {
                List<SyrupProcessNameList> aCItemsModels = new List<SyrupProcessNameList>();
                var parameters = new DynamicParameters();
                var query = @"SELECT dfs.DynamicFormSectionID AS ID, dfs.SectionName AS ProcessName FROM DynamicFormSection dfs WHERE dfs.DynamicFormID IN (    SELECT t2.DynamicFormID    FROM DynamicForm_ProductiontimingNMachineInfosyrup t1    INNER JOIN DynamicFormData t2 ON t2.DynamicFormDataID = t1.DynamicFormDataID)";
                using (var connection = CreateConnection())
                {
                    aCItemsModels = (await connection.QueryAsync<SyrupProcessNameList>(query, parameters)).ToList();
                }

                return aCItemsModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<SyrupFilling>> GetSyrupFillingList()
        {
            try
            {
                List<SyrupFilling> aCItemsModels = new List<SyrupFilling>();
                var parameters = new DynamicParameters();
                var query = @"select t2.DynamicFormID,t1.DynamicFormDataItemID as ID,t1.ProfileNo,
                                t1.[13213_Primaryfillingmachine] as PrimaryFillingMachine,
                                t1.[13230_TypeofPlanningProcess] as TypeOfPlanningProcess,
                                t1.[13223_1FillingHours] as FillingHours,
                                t1.[13225_1FillingManpower] as FillingManpower,
                                t1.[13218_ChangePackingFillingHours] as ChangePackingFillingHours,
                                t1.ModifiedBy,t1.ModifiedDate
                                from DynamicForm_ProdTimingSyrupPackingGrid t1
                                INNER JOIN DynamicFormData t2 on t2.DynamicFormDataID = t1.DynamicFormDataID
                                where t1.DynamicFormDataGridId = 85012";
                using (var connection = CreateConnection())
                {
                    aCItemsModels = (await connection.QueryAsync<SyrupFilling>(query, parameters)).ToList();
                }

                return aCItemsModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<SyrupOtherProcess>> GetSyrupOtherProcessList()
        {
            try
            {
                List<SyrupOtherProcess> aCItemsModels = new List<SyrupOtherProcess>();
                var parameters = new DynamicParameters();
                var query = @"SELECT 
                            1 as Id,
                            DynamicFormDataItemID AS ID,
                            1 AS SyrupPlanningID,
                            DynamicFormDataID,
                            [13260_OtherJobsInformation] AS OtherJobsInformation,
                            [13260_1980_ProductionPlanningProcess] AS ProcessName,
                            ProfileNo,
                            [13260_1987_ProductionPlanningProcess] AS SyrupOtherProcessNextProcess,
                            [13260_1981_Mustcompletetohavenextprocess] AS MustCompleteForNext,                          
                            [13260_1984_Locationofprocess] AS LocationOfProcess,
                            TRY_CAST([13260_1986_NoofManhoursHours] AS DECIMAL(18,2)) AS ManhoursOrHours,
                            TRY_CAST([13260_1985_Noofmanpower] AS INT) AS NoOfManpower,
                            [13260_1988_Manpowerisfromnextprocess] AS ManpowerFromNextProcess,
                            [13260_1996_Cancarryoutonnonworkday] AS CanCarryoutOnNonWorkday,
                            TRY_CAST([13260_2000_TimeGapHour] AS DECIMAL(18,2)) AS TimeGapHour,
                            1 AS AddedByUserID,
                            '' AS Description,
                            '' AS ModifiedBy,
                            GETDATE() AS ModifiedDate
                        FROM DynamicForm_ProductionTimingSyrupOthers
                        WHERE DynamicFormDataGridId = 85012";
                using (var connection = CreateConnection())
                {
                    aCItemsModels = (await connection.QueryAsync<SyrupOtherProcess>(query, parameters)).ToList();
                }

                return aCItemsModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<SyrupPlanning>> GetSyrupPlannings()
        {
            try
            {
                List<SyrupPlanning> aCItemsModels = new List<SyrupPlanning>();
                var parameters = new DynamicParameters();
                var query = @"select t1.DynamicFormDataItemID,t1.DynamicFormDataID,t1.ProfileNo,t1.[13192_MethodCode] as MethodName,t1.[13192_MethodCode] as MethodCode,t1.[13192_MethodCode_UId] as MethodCodeID,t1.[13193_BatchSizeL] as BatchSizeInLiters,t1.[13262_RestrictiononPlanningday] as RestrictionOnPlanningDay,
                                t1.[13229_IsthereSyrupSimplextoproduce] as IsthereSyrupSimplextoproduce,t1.[13229_2003_ProcessName] as ProcessName,t1.[13202_Location] as SyrupSimplexLocation,
                                t1.[13229_1955_1PreparationHour] as PreparationPerHour,ISNULL(NULLIF(t1.[13229_1956_SyrupSimplexManpower], ''), 0) as SyrupSimplexManpower,t1.[13211_Level2Cleaninghours] as Level2CleaningHours,
                                t1.[13212_Level2CleaningManpower] as Level2CleaningManpower,t1.[13229_2009_NoofCampaign] as NoOfCampaign,t1.[13229_2004_NextProcessName] as NextProcessName,
                                t1.[13229_1954_Location] as SyrupPreparationLocation,t1.[13203_1PreparationfirstVolumnHour] as PreparationFirstVolumePerHour,t1.[13204_1PreparationFirstVolumnManpower] as PreparationFirstVolumeManpower,
                                t1.[13205_2IPQCtest] as IpqcTestRequired,t1.[13206_3PreparationTopuptoVolumnHour] as PreparationTopUpPerHour,t1.[13207_3PreparationTopuptoVolumnManpower] as PreparationTopUpManpower,
                                t1.[13208_CampaignBatchesNumbers] as CampaignBatches,t1.[13209_Level1CleaningHours] as Level1CleaningHours,t1.[13210_Level1Cleaningmanpower] as Level1CleaningManpower,
                                t1.[13229_1957_Level2CleaningHours] as SyrupLevel2CleaningHours,t1.[13229_1958_Level2CleaningManpower] as SyrupLevel2CleaningManpower,t1.[13229_2004_NextProcessName]  as NextProcessNameAfterPreparation
                                from DynamicForm_ProductiontimingNMachineInfosyrup t1";
                using (var connection = CreateConnection())
                {
                    aCItemsModels = (await connection.QueryAsync<SyrupPlanning>(query, parameters)).ToList();
                }

                return aCItemsModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<SyrupPlanning>> GetSyrupPlannings_old()
        {
            try
            {
                List<SyrupPlanning> aCItemsModels = new List<SyrupPlanning>();
                var parameters = new DynamicParameters();
                var query = @"select t3.MethodCodeLineID,t2.MethodName,t2.MethodCodeID,t4.No as itemno,t4.Description,t4.Description2,t4.CategoryID,
                                t1.ProfileNo,t1.[13192_MethodCode] as MethodCode,t1.[13193_BatchSizeL] as BatchSizeInLiters,t1.[13262_RestrictiononPlanningday] as RestrictionOnPlanningDay,
                                t1.[13229_IsthereSyrupSimplextoproduce] as IsthereSyrupSimplextoproduce,t1.[13229_2003_ProcessName] as ProcessName,t1.[13202_Location] as SyrupSimplexLocation,
                                t1.[13229_1955_1PreparationHour] as PreparationPerHour,ISNULL(NULLIF(t1.[13229_1956_SyrupSimplexManpower], ''), 0) as SyrupSimplexManpower,t1.[13211_Level2Cleaninghours] as Level2CleaningHours,
                                t1.[13212_Level2CleaningManpower] as Level2CleaningManpower,t1.[13229_2009_NoofCampaign] as NoOfCampaign,t1.[13229_2004_NextProcessName] as NextProcessName,
                                t1.[13229_1954_Location] as SyrupPreparationLocation,t1.[13203_1PreparationfirstVolumnHour] as PreparationFirstVolumePerHour,t1.[13204_1PreparationFirstVolumnManpower] as PreparationFirstVolumeManpower,
                                t1.[13205_2IPQCtest] as IpqcTestRequired,t1.[13206_3PreparationTopuptoVolumnHour] as PreparationTopUpPerHour,t1.[13207_3PreparationTopuptoVolumnManpower] as PreparationTopUpManpower,
                                t1.[13208_CampaignBatchesNumbers] as CampaignBatches,t1.[13209_Level1CleaningHours] as Level1CleaningHours,t1.[13210_Level1Cleaningmanpower] as Level1CleaningManpower,
                                t1.[13229_1957_Level2CleaningHours] as SyrupLevel2CleaningHours,t1.[13229_1958_Level2CleaningManpower] as SyrupLevel2CleaningManpower,t1.[13229_2004_NextProcessName]  as NextProcessNameAfterPreparation
                                from DynamicForm_ProductiontimingNMachineInfosyrup t1
                                inner join navmethodcode t2 on t2.MethodCodeID = t1.[13192_MethodCode_UId]
                                inner join navmethodcodelines t3 on t3.MethodCodeID = t2.MethodCodeID
                                inner join NAVItems t4 on t4.ItemId = t3.ItemID";
                using (var connection = CreateConnection())
                {
                    aCItemsModels = (await connection.QueryAsync<SyrupPlanning>(query, parameters)).ToList();
                }

                return aCItemsModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task SaveSyrupOtherProcessesAsync(long syrupPlanningId, IEnumerable<SyrupOtherProcess> items)
        {
            if (syrupPlanningId <= 0) throw new ArgumentException("syrupPlanningId must be a valid saved parent id", nameof(syrupPlanningId));

            using var conn = CreateConnection() ?? throw new InvalidOperationException("CreateConnection() returned null.");

            // Ensure connection is open. If it's a SqlConnection we can OpenAsync; otherwise fallback to synchronous Open().
            if (conn is SqlConnection sqlConn)
            {
                await sqlConn.OpenAsync();
            }
            else
            {
                // IDbConnection only exposes Open()
                conn.Open();
            }

            // BeginTransaction() is available on IDbConnection
            using var tran = conn.BeginTransaction();
            try
            {
                // Delete existing child rows in this transaction
                await conn.ExecuteAsync(
                    "DELETE FROM dbo.SyrupOtherProcess WHERE SyrupPlanningID = @SyrupPlanningID",
                    new { SyrupPlanningID = syrupPlanningId },
                    transaction: tran);

                const string insertSql = @"
INSERT INTO dbo.SyrupOtherProcess
    (SyrupPlanningID, DynamicFormDataID,DynamicFormDataItemID, OtherJobsInformation, ProcessName, ProfileNo, SyrupOtherProcessNextProcess,
     MustCompleteForNext, LocationOfProcess, ManhoursOrHours, NoOfManpower, ManpowerFromNextProcess,
     CanCarryoutOnNonWorkday, TimeGapHour, AddedByUserID, Description, ModifiedBy, ModifiedDate)
OUTPUT INSERTED.SyrupOtherProcessID
VALUES
    (@SyrupPlanningID, @DynamicFormDataID,@DynamicFormDataItemID, @OtherJobsInformation, @ProcessName, @ProfileNo, @SyrupOtherProcessNextProcess,
     @MustCompleteForNext, @LocationOfProcess, @ManhoursOrHours, @NoOfManpower, @ManpowerFromNextProcess,
     @CanCarryoutOnNonWorkday, @TimeGapHour, @AddedByUserID, @Description, @ModifiedBy, @ModifiedDate);";

                var minSqlDate = new DateTime(1753, 1, 1);

                foreach (var it in items ?? Enumerable.Empty<SyrupOtherProcess>())
                {
                    // Robust ModifiedDate extraction:
                    DateTime? safeModifiedDate = null;

                    // Try to get property value safely (handles DateTime and DateTime?)
                    var prop = it.GetType().GetProperty("ModifiedDate");
                    if (prop != null)
                    {
                        var val = prop.GetValue(it);
                        if (val is DateTime dt)
                        {
                            if (dt >= minSqlDate) safeModifiedDate = dt;
                        }
                        // if val is null or not a DateTime, safeModifiedDate stays null
                    }

                    var parameters = new
                    {
                        SyrupPlanningID = syrupPlanningId,
                        DynamicFormDataID = (object?)it.DynamicFormDataID ?? DBNull.Value,
                        DynamicFormDataItemID = (object?)it.DynamicFormDataItemID ?? DBNull.Value,
                        OtherJobsInformation = (object?)it.OtherJobsInformation ?? DBNull.Value,
                        ProcessName = (object?)it.ProcessName ?? DBNull.Value,
                        ProfileNo = (object?)it.ProfileNo ?? DBNull.Value,
                        SyrupOtherProcessNextProcess = (object?)it.SyrupOtherProcessNextProcess ?? DBNull.Value,
                        MustCompleteForNext = (object?)it.MustCompleteForNext ?? DBNull.Value,
                        LocationOfProcess = (object?)it.LocationOfProcess ?? DBNull.Value,
                        ManhoursOrHours = (object?)it.ManhoursOrHours ?? DBNull.Value,
                        NoOfManpower = (object?)it.NoOfManpower ?? DBNull.Value,
                        ManpowerFromNextProcess = (object?)it.ManpowerFromNextProcess ?? DBNull.Value,
                        CanCarryoutOnNonWorkday = (object?)it.CanCarryoutOnNonWorkday ?? DBNull.Value,
                        TimeGapHour = (object?)it.TimeGapHour ?? DBNull.Value,
                        AddedByUserID = (object?)it.AddedByUserID ?? DBNull.Value,
                        Description = (object?)it.Description ?? DBNull.Value,
                        ModifiedBy = (object?)it.ModifiedBy ?? DBNull.Value,
                        ModifiedDate = (object?)safeModifiedDate ?? DBNull.Value
                    };

                    // Use the IDbTransaction instance returned by BeginTransaction()
                    var newId = await conn.QuerySingleAsync<long>(insertSql, parameters, tran);
                    it.ID = newId;
                }

                tran.Commit(); // commit is synchronous but safe here
            }
            catch
            {
                try { tran.Rollback(); } catch { /* ignore rollback errors */ }
                throw;
            }
            finally
            {
                // Ensure connection is closed
                if (conn.State != ConnectionState.Closed) conn.Close();
            }
        }
        public async Task<IReadOnlyList<ProcessStepDto>> GetProcessFlowByProfileNoAsync(string profileNo, DateTime productionDay, TimeSpan shiftStart)
        {
            if (string.IsNullOrWhiteSpace(profileNo))
                return Array.Empty<ProcessStepDto>();

            var startDateTime = productionDay.Date + shiftStart;

            const string sql = @"WITH AllProcesses AS
                            (/* 1. Other Process */
                                SELECT 
                                    op.SyrupPlanningID,
                                    10 AS Seq,
                                    'OtherProcess' AS Source,
                                    op.ProcessName AS ProcessName,
									op.LocationOfProcess as Room,
                                    op.SyrupOtherProcessNextProcess AS NextProcessName,
                                    TRY_CONVERT(decimal(18,4), op.ManhoursOrHours) AS DurationHours,
                                    CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), op.ManhoursOrHours), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), op.NoOfManpower) AS Manpower
                                FROM dbo.SyrupOtherProcess op
                                WHERE op.ProfileNo = @ProfileNo

                                UNION ALL

                                /* 2. Syrup Simplex */
                                SELECT
                                    sp.SyrupPlanningID,
                                    20 AS Seq,
                                    'SyrupSimplex' AS Source,
                                    sp.SyrupSimplexProcessName AS ProcessName,
									sp.SyrupSimplexLocation as Room,
                                    sp.SyrupSimplexNextProcessName AS NextProcessName,
                                    TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')) AS DurationHours,
                                    CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sp.SyrupSimplexManpower) AS Manpower
                                FROM dbo.SyrupPlanning sp
                                WHERE sp.ProfileNo = @ProfileNo

                                UNION ALL

                                /* 3. Syrup Simplex - Level2 Cleaning */
                                SELECT
                                    sp.SyrupPlanningID,
                                    21 AS Seq,
                                    'SyrupSimplexLevel2Cleaning' AS Source,
                                    CONCAT(sp.SyrupSimplexProcessName, ' - Level2 Cleaning') AS ProcessName,
									sp.SyruppreparationLocation as Room,
                                    sp.SyrupSimplexNextProcessName AS NextProcessName,
                                    TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours) AS DurationHours,
                                    CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningManpower) AS Manpower
                                FROM dbo.SyrupPlanning sp
                                WHERE sp.ProfileNo = @ProfileNo

                                UNION ALL

                                /* 4. Syrup Preparation - Mixing */
                                SELECT
                                    sp.SyrupPlanningID,
                                    30 AS Seq,
                                    'SyrupPreparation' AS Source,
                                    sp.SyruppreparationProcessName AS ProcessName,
									sp.SyruppreparationLocation as Room,
                                    sp.SyruppreparationNextProcessName AS NextProcessName,
                                    TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')) AS DurationHours,
                                    CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sp.SyruppreparationFirstVolumnManpower) AS Manpower
                                FROM dbo.SyrupPlanning sp
                                WHERE sp.ProfileNo = @ProfileNo

                                /* 5. Syrup Filling — include each process column separately */
                                UNION ALL

                                /* 5a. ProcessName_Primary */
                                SELECT
                                    sf.SyrupPlanningID,
                                    40 AS Seq,
                                    'PrimaryPacking' AS Source,
                                    sf.ProcessName_Primary AS ProcessName,
									null as Room,
                                    sf.NextProcessName_Primary AS NextProcessName,
                                    COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0) AS DurationHours,
                                    CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level1) AS Manpower
                                FROM dbo.SyrupFilling sf
                                JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                WHERE sp.ProfileNo = @ProfileNo AND sf.ProcessName_Primary IS NOT NULL

                                UNION ALL

                                /* 5b. NextProcessName_Primary (Machine Filling) */
                                SELECT
                                    sf.SyrupPlanningID,
                                    41 AS Seq,
                                    'MachineFilling' AS Source,
                                    sf.NextProcessName_Primary AS ProcessName,
									null as Room,
                                    sf.ProcessName_Secondary AS NextProcessName,
                                    COALESCE(sf.FillingHours_Level2, 0) AS DurationHours,
                                    CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level2, 0), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level2) AS Manpower
                                FROM dbo.SyrupFilling sf
                                JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                WHERE sp.ProfileNo = @ProfileNo AND sf.NextProcessName_Primary IS NOT NULL

                                UNION ALL

                                /* 5c. ProcessName_Secondary */
                                SELECT
                                    sf.SyrupPlanningID,
                                    42 AS Seq,
                                    'SecondaryPacking' AS Source,
                                    sf.ProcessName_Secondary AS ProcessName,
									null as Room,
                                    sf.NextProcessName_Secondary AS NextProcessName,
                                    CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                                         ELSE COALESCE(sf.SecondaryPackingHours,0) END AS DurationHours,
                                    CAST(ROUND(ISNULL(
                                        CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                                             ELSE COALESCE(sf.SecondaryPackingHours,0) END, 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sf.SecondaryManpower) AS Manpower
                                FROM dbo.SyrupFilling sf
                                JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                WHERE sp.ProfileNo = @ProfileNo AND sf.ProcessName_Secondary IS NOT NULL

                                UNION ALL

                                /* 5d. NextProcessName_Secondary (if you have one more step after secondary) */
                                SELECT
                                    sf.SyrupPlanningID,
                                    43 AS Seq,
                                    'SecondaryNext' AS Source,
                                    sf.NextProcessName_Secondary AS ProcessName,
									null as Room,
                                    NULL AS NextProcessName,
                                    0 AS DurationHours,
                                    0 AS DurationMinutes,
                                    NULL AS Manpower
                                FROM dbo.SyrupFilling sf
                                JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                WHERE sp.ProfileNo = @ProfileNo AND sf.NextProcessName_Secondary IS NOT NULL
                            ),
                            Ordered AS
                            (
                                SELECT *,
                                       ROW_NUMBER() OVER (ORDER BY Seq) AS rn
                                FROM AllProcesses
                            ),
                            Timeline AS
                            (
                                /* make sure DurationMinutes is treated as 0 when NULL and compute cumulative minutes */
                                SELECT *,
                                       -- ensure DurationMinutes is not null
                                       COALESCE(DurationMinutes, 0) AS DurationMinutesNonNull,
                                       -- cumulative minutes from start to end of each process
                                       SUM(COALESCE(DurationMinutes, 0)) OVER (ORDER BY rn ROWS UNBOUNDED PRECEDING) AS CumMinutes
                                FROM Ordered
                            )
                           SELECT
							
							Timeline.rn           AS TaskId,                                
							Timeline.ProcessName  AS TaskName,                              
							DATEADD(MINUTE, (Timeline.CumMinutes - Timeline.DurationMinutesNonNull), @StartDateTimeParam) AS StartDate,
							DATEADD(MINUTE, Timeline.CumMinutes, @StartDateTimeParam) AS EndDate,
							0 AS Progress,                       
							NULL AS Predecessor,                
							NULL AS ParentId,                   
							Timeline.DurationMinutesNonNull AS DurationMinutes, 
							COALESCE(Timeline.DurationHours, 0) AS DurationHours,
							Timeline.Manpower,
							Timeline.Room,                      
							nxt.ProcessName AS NextProcess_Timeline
						FROM Timeline
						LEFT JOIN Timeline nxt ON nxt.rn = Timeline.rn + 1
						ORDER BY Timeline.rn";

            using var conn = CreateConnection(); // your existing helper that returns IDbConnection

            // Open connection safely (works whether IDbConnection is SqlConnection or other)
            if (conn is SqlConnection sqlConn)
            {
                await sqlConn.OpenAsync();
            }
            else
            {
                conn.Open();
            }

            try
            {
                var rows = await conn.QueryAsync<ProcessStepDto>(sql, new
                {
                    ProfileNo = profileNo,
                    StartDateTimeParam = startDateTime
                });             
                var list = rows.ToList();
                return list;
            }
            finally
            {
                // close connection if not handled by caller
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
        public async Task<IReadOnlyList<ProcessStepDto>> GetProcessFlowByProfileNoAsync_v(string profileNo)
        {
            if (string.IsNullOrWhiteSpace(profileNo))
                return Array.Empty<ProcessStepDto>();

            const string sql = @"
                SELECT Seq, Source, ProcessName, Location, DurationHours, Manpower, NextProcess, Notes
                FROM
                (
                    -- 1. Syrup Simplex from SyrupPlanning
                    SELECT
                        10 AS Seq,
                        'SyrupSimplex' AS Source,
                        sp.SyrupSimplexProcessName AS ProcessName,
                        sp.SyrupSimplexLocation AS Location,
                        TRY_CONVERT(decimal(38,10), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')) AS DurationHours,
                        TRY_CONVERT(decimal(38,10), sp.SyrupSimplexManpower) AS Manpower,
                        sp.SyrupSimplexNextProcessName AS NextProcess,
                        NULL AS Notes
                    FROM dbo.SyrupPlanning sp
                    WHERE sp.ProfileNo = @ProfileNo

                    UNION ALL

                    -- 2. Syrup Preparation from SyrupPlanning
                    SELECT
                        20 AS Seq,
                        'SyrupPreparation' AS Source,
                        sp.SyruppreparationProcessName AS ProcessName,
                        sp.SyruppreparationLocation AS Location,
                        TRY_CONVERT(decimal(38,10), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')) AS DurationHours,
                        TRY_CONVERT(decimal(38,10), sp.SyruppreparationFirstVolumnManpower) AS Manpower,
                        sp.SyruppreparationNextProcessName AS NextProcess,
                        NULL AS Notes
                    FROM dbo.SyrupPlanning sp
                    WHERE sp.ProfileNo = @ProfileNo

                    UNION ALL

                    -- 3. Other Processes (can be many rows)
                    SELECT
                        30 AS Seq,
                        'OtherProcess' AS Source,
                        op.ProcessName AS ProcessName,
                        op.LocationOfProcess AS Location,
                        TRY_CONVERT(decimal(38,10), REPLACE(op.ManhoursOrHours, ',', '')) AS DurationHours,
                        TRY_CONVERT(decimal(38,10), op.NoOfManpower) AS Manpower,
                        op.SyrupOtherProcessNextProcess AS NextProcess,
                        op.OtherJobsInformation AS Notes
                    FROM dbo.SyrupOtherProcess op
                    WHERE op.ProfileNo = @ProfileNo

                    UNION ALL

                    -- 4. Filling Primary (SyrupFilling) -- there may be multiple; include all
                    SELECT
                        40 AS Seq,
                        'FillingPrimary' AS Source,
                        sf.ProcessName_Primary AS ProcessName,
                        NULL AS Location,
                        TRY_CONVERT(decimal(38,10), sf.FillingHours_Level1) AS DurationHours,
                        TRY_CONVERT(decimal(38,10), sf.FillingManpower_Level1) AS Manpower,
                        sf.NextProcessName_Primary AS NextProcess,
                        sf.PlanningType_Primary AS Notes
                    FROM dbo.SyrupFilling sf
                    WHERE sf.ProfileNo = @ProfileNo
                ) t
                ORDER BY Seq;
                ";

            using var conn = CreateConnection(); // your existing helper that returns IDbConnection

            // Open connection safely (works whether IDbConnection is SqlConnection or other)
            if (conn is SqlConnection sqlConn)
            {
                await sqlConn.OpenAsync();
            }
            else
            {
                conn.Open();
            }

            try
            {
                var rows = await conn.QueryAsync<ProcessStepDto>(sql, new { ProfileNo = profileNo });
                // QueryAsync will map column names to DTO properties automatically (case-insensitive)

                // Ensure deterministic ordering (SQL already orders by Seq but reorder in C# as safeguard)
                var list = rows.OrderBy(r => r.Seq).ToList();
                return list;
            }
            finally
            {
                // close connection if not handled by caller
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
        public async Task<SyrupPlanning> InsertOrUpdateSyrupPlanningAsync(SyrupPlanning model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            using var conn = CreateConnection();

            // 1) open connection
            if (conn is SqlConnection sConn)
                await sConn.OpenAsync();
            else
                conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                // 2) find existing parent IDs (if requested)
                var existingIds = new List<long>();
                if (model.DynamicFormDataID > 0 && model.DynamicFormDataItemID > 0)
                {
                    const string findSql = @"
                SELECT SyrupPlanningID
                FROM dbo.SyrupPlanning
                WHERE DynamicFormDataID = @DynamicFormDataID
                  AND DynamicFormDataItemID = @DynamicFormDataItemID;";

                    existingIds = (await conn.QueryAsync<long>(findSql, new
                    {
                        model.DynamicFormDataID,
                        model.DynamicFormDataItemID
                    }, transaction: tran)).ToList();

                    if (existingIds.Count > 0)
                    {
                        // 3) Delete children first (order doesn't matter between these two if both reference SyrupPlanning)
                        const string deleteFillingSql = @"
                    DELETE FROM dbo.SyrupFilling
                    WHERE SyrupPlanningID IN @Ids;";
                        int deletedFilling = await conn.ExecuteAsync(deleteFillingSql, new { Ids = existingIds }, transaction: tran);

                        const string deleteOtherProcessSql = @"
                    DELETE FROM dbo.SyrupOtherProcess
                    WHERE SyrupPlanningID IN @Ids;";
                        int deletedOther = await conn.ExecuteAsync(deleteOtherProcessSql, new { Ids = existingIds }, transaction: tran);

                        // optional: any other child tables delete here, in dependency order

                        // 4) Delete parent rows
                        const string deleteParentSql = @"
                    DELETE FROM dbo.SyrupPlanning
                    WHERE DynamicFormDataID = @DynamicFormDataID
                      AND DynamicFormDataItemID = @DynamicFormDataItemID;";
                        int deletedParents = await conn.ExecuteAsync(deleteParentSql, new
                        {
                            model.DynamicFormDataID,
                            model.DynamicFormDataItemID
                        }, transaction: tran);

                        // (optional) you can log deletedFilling, deletedOther, deletedParents
                    }
                }

                // 5) prepare parameters & upsert (same as previous)
                var minSqlDate = new DateTime(1753, 1, 1);
                DateTime? addedDateSafe = model.AddedDate >= minSqlDate ? model.AddedDate : null;
                DateTime? modifiedDateSafe = model.ModifiedDate >= minSqlDate ? model.ModifiedDate : null;

                var parameters = new DynamicParameters(new
                {
                    model.MethodCodeLineID,
                    model.DynamicFormDataID,
                    model.DynamicFormDataItemID,
                    model.MethodName,
                    model.MethodCodeID,
                    model.ProfileNo,
                    model.MethodCode,
                    model.BatchSizeInLiters,
                    model.RestrictionOnPlanningDay,
                    model.ProcessName,

                    model.IsthereSyrupSimplextoproduce,
                    model.SyrupSimplexProcessName,
                    model.SyrupSimplexLocation,
                    model.SyrupSimplexPreparationHour,
                    model.SyrupSimplexManpower,
                    model.SyrupSimplexLevel2CleaningHours,
                    model.SyrupSimplexLevel2CleaningManpower,
                    model.SyrupSimplexNoofCampaign,
                    model.SyrupSimplexNextProcessName,

                    model.SyruppreparationProcessName,
                    model.SyruppreparationLocation,
                    model.SyruppreparationFirstVolumnHour,
                    model.SyruppreparationFirstVolumnManpower,
                    model.SyruppreparationIPQCTest,
                    model.SyruppreparationTopupToVolumnHour,
                    model.SyruppreparationTopupToVolumnManpower,
                    model.SyruppreparationCampaignBatchesNumbers,
                    model.SyruppreparationLevel1CleaningHours,
                    model.SyruppreparationLevel1Cleaningmanpower,
                    model.SyruppreparationLevel2Cleaninghours,
                    model.SyruppreparationLevel2CleaningManpower,
                    model.SyruppreparationNextProcessName,

                    model.PreparationFirstVolumePerHour,
                    model.PreparationFirstVolumeManpower,
                    model.IpqcTestRequired,
                    model.PreparationTopUpPerHour,
                    model.PreparationTopUpManpower,
                    model.CampaignBatches,
                    model.Level1CleaningHours,
                    model.Level1CleaningManpower,
                    model.NextProcessNameAfterPreparation,

                    model.AddedByUserID,
                    model.ModifiedByUserID,
                    AddedDate = addedDateSafe,
                    ModifiedDate = modifiedDateSafe
                });

                long resultId;

                if (model.Id > 0)
                {
                    parameters.Add("SyrupPlanningID", model.Id);

                    const string updateSql = @"
                UPDATE dbo.SyrupPlanning
                SET 
                    MethodCodeLineID = @MethodCodeLineID,
                    DynamicFormDataID = @DynamicFormDataID,
                    DynamicFormDataItemID = @DynamicFormDataItemID,
                    MethodName = @MethodName,
                    MethodCodeID = @MethodCodeID,
                    ProfileNo = @ProfileNo,
                    MethodCode = @MethodCode,
                    BatchSizeInLiters = @BatchSizeInLiters,
                    RestrictionOnPlanningDay = @RestrictionOnPlanningDay,
                    ProcessName = @ProcessName,
                    IsthereSyrupSimplextoproduce = @IsthereSyrupSimplextoproduce,
                    SyrupSimplexProcessName = @SyrupSimplexProcessName,
                    SyrupSimplexLocation = @SyrupSimplexLocation,
                    SyrupSimplexPreparationHour = @SyrupSimplexPreparationHour,
                    SyrupSimplexManpower = @SyrupSimplexManpower,
                    SyrupSimplexLevel2CleaningHours = @SyrupSimplexLevel2CleaningHours,
                    SyrupSimplexLevel2CleaningManpower = @SyrupSimplexLevel2CleaningManpower,
                    SyrupSimplexNoofCampaign = @SyrupSimplexNoofCampaign,
                    SyrupSimplexNextProcessName = @SyrupSimplexNextProcessName,
                    SyruppreparationProcessName = @SyruppreparationProcessName,
                    SyruppreparationLocation = @SyruppreparationLocation,
                    SyruppreparationFirstVolumnHour = @SyruppreparationFirstVolumnHour,
                    SyruppreparationFirstVolumnManpower = @SyruppreparationFirstVolumnManpower,
                    SyruppreparationIPQCTest = @SyruppreparationIPQCTest,
                    SyruppreparationTopupToVolumnHour = @SyruppreparationTopupToVolumnHour,
                    SyruppreparationTopupToVolumnManpower = @SyruppreparationTopupToVolumnManpower,
                    SyruppreparationCampaignBatchesNumbers = @SyruppreparationCampaignBatchesNumbers,
                    SyruppreparationLevel1CleaningHours = @SyruppreparationLevel1CleaningHours,
                    SyruppreparationLevel1Cleaningmanpower = @SyruppreparationLevel1Cleaningmanpower,
                    SyruppreparationLevel2Cleaninghours = @SyruppreparationLevel2Cleaninghours,
                    SyruppreparationLevel2CleaningManpower = @SyruppreparationLevel2CleaningManpower,
                    SyruppreparationNextProcessName = @SyruppreparationNextProcessName,
                    AddedByUserID = @AddedByUserID,
                    ModifiedByUserID = @ModifiedByUserID,
                    AddedDate = @AddedDate,
                    ModifiedDate = @ModifiedDate
                WHERE SyrupPlanningID = @SyrupPlanningID;

                SELECT SyrupPlanningID
                FROM dbo.SyrupPlanning
                WHERE SyrupPlanningID = @SyrupPlanningID;";

                    resultId = await conn.QuerySingleAsync<long>(updateSql, parameters, transaction: tran);
                }
                else
                {
                    const string insertSql = @"
                INSERT INTO dbo.SyrupPlanning
                (
                    MethodCodeLineID, DynamicFormDataID, DynamicFormDataItemID, MethodName, MethodCodeID, ProfileNo, MethodCode,
                    BatchSizeInLiters, RestrictionOnPlanningDay, ProcessName,
                    IsthereSyrupSimplextoproduce, SyrupSimplexProcessName, SyrupSimplexLocation,
                    SyrupSimplexPreparationHour, SyrupSimplexManpower, SyrupSimplexLevel2CleaningHours,
                    SyrupSimplexLevel2CleaningManpower, SyrupSimplexNoofCampaign, SyrupSimplexNextProcessName,
                    SyruppreparationProcessName, SyruppreparationLocation, SyruppreparationFirstVolumnHour,
                    SyruppreparationFirstVolumnManpower, SyruppreparationIPQCTest, SyruppreparationTopupToVolumnHour,
                    SyruppreparationTopupToVolumnManpower, SyruppreparationCampaignBatchesNumbers, SyruppreparationLevel1CleaningHours,
                    SyruppreparationLevel1Cleaningmanpower, SyruppreparationLevel2Cleaninghours, SyruppreparationLevel2CleaningManpower,
                    SyruppreparationNextProcessName, AddedByUserID, ModifiedByUserID, AddedDate, ModifiedDate
                )
                OUTPUT INSERTED.SyrupPlanningID
                VALUES
                (
                    @MethodCodeLineID, @DynamicFormDataID, @DynamicFormDataItemID, @MethodName, @MethodCodeID, @ProfileNo, @MethodCode,
                    @BatchSizeInLiters, @RestrictionOnPlanningDay, @ProcessName,
                    @IsthereSyrupSimplextoproduce, @SyrupSimplexProcessName, @SyrupSimplexLocation,
                    @SyrupSimplexPreparationHour, @SyrupSimplexManpower, @SyrupSimplexLevel2CleaningHours,
                    @SyrupSimplexLevel2CleaningManpower, @SyrupSimplexNoofCampaign, @SyrupSimplexNextProcessName,
                    @SyruppreparationProcessName, @SyruppreparationLocation, @SyruppreparationFirstVolumnHour,
                    @SyruppreparationFirstVolumnManpower, @SyruppreparationIPQCTest, @SyruppreparationTopupToVolumnHour,
                    @SyruppreparationTopupToVolumnManpower, @SyruppreparationCampaignBatchesNumbers, @SyruppreparationLevel1CleaningHours,
                    @SyruppreparationLevel1Cleaningmanpower, @SyruppreparationLevel2Cleaninghours, @SyruppreparationLevel2CleaningManpower,
                    @SyruppreparationNextProcessName, @AddedByUserID, @ModifiedByUserID, @AddedDate, @ModifiedDate
                );";

                    resultId = await conn.QuerySingleAsync<long>(insertSql, parameters, transaction: tran);
                }

                // 6) commit
                tran.Commit();
                model.Id = resultId;
                return model;
            }
            catch
            {
                try { tran.Rollback(); } catch { /* ignore */ }
                throw;
            }
        }

        public async Task<SyrupPlanning> InsertOrUpdateSyrupPlanningAsync_v(SyrupPlanning model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using var conn = CreateConnection();
            if (conn is SqlConnection sConn) await sConn.OpenAsync();
            else conn.Open();

            var minSqlDate = new DateTime(1753, 1, 1);

            // sanitize SQL-safe dates
            DateTime? addedDateSafe = model.AddedDate >= minSqlDate ? model.AddedDate : null;
            DateTime? modifiedDateSafe = model.ModifiedDate >= minSqlDate ? model.ModifiedDate : null;

            // Build parameters normally — just use nulls
            var parameters = new DynamicParameters(new
            {
                model.MethodCodeLineID,
                model.DynamicFormDataID,
                model.MethodName,
                model.MethodCodeID,
                model.ProfileNo,
                model.MethodCode,
                model.BatchSizeInLiters,
                model.RestrictionOnPlanningDay,
                model.ProcessName,

                // Syrup Simplex
                model.IsthereSyrupSimplextoproduce,
                model.SyrupSimplexProcessName,
                model.SyrupSimplexLocation,
                model.SyrupSimplexPreparationHour,
                model.SyrupSimplexManpower,
                model.SyrupSimplexLevel2CleaningHours,
                model.SyrupSimplexLevel2CleaningManpower,
                model.SyrupSimplexNoofCampaign,
                model.SyrupSimplexNextProcessName,

                // Syrup preparation
                model.SyruppreparationProcessName,
                model.SyruppreparationLocation,
                model.SyruppreparationFirstVolumnHour,
                model.SyruppreparationFirstVolumnManpower,
                model.SyruppreparationIPQCTest,
                model.SyruppreparationTopupToVolumnHour,
                model.SyruppreparationTopupToVolumnManpower,
                model.SyruppreparationCampaignBatchesNumbers,
                model.SyruppreparationLevel1CleaningHours,
                model.SyruppreparationLevel1Cleaningmanpower,
                model.SyruppreparationLevel2Cleaninghours,
                model.SyruppreparationLevel2CleaningManpower,
                model.SyruppreparationNextProcessName,

                // Generic fields
                model.PreparationFirstVolumePerHour,
                model.PreparationFirstVolumeManpower,
                model.IpqcTestRequired,
                model.PreparationTopUpPerHour,
                model.PreparationTopUpManpower,
                model.CampaignBatches,
                model.Level1CleaningHours,
                model.Level1CleaningManpower,
                model.NextProcessNameAfterPreparation,

                // audit
                model.AddedByUserID,
                model.ModifiedByUserID,
                AddedDate = addedDateSafe,
                ModifiedDate = modifiedDateSafe
            });

            long resultId;

            if (model.Id > 0)
            {
                string updateSql = @"
UPDATE dbo.SyrupPlanning
SET 
    MethodCodeLineID = @MethodCodeLineID,
    DynamicFormDataID = @DynamicFormDataID,
    MethodName = @MethodName,
    MethodCodeID = @MethodCodeID,
    ProfileNo = @ProfileNo,
    MethodCode = @MethodCode,
    BatchSizeInLiters = @BatchSizeInLiters,
    RestrictionOnPlanningDay = @RestrictionOnPlanningDay,
    ProcessName = @ProcessName,
    IsthereSyrupSimplextoproduce = @IsthereSyrupSimplextoproduce,
    SyrupSimplexProcessName = @SyrupSimplexProcessName,
    SyrupSimplexLocation = @SyrupSimplexLocation,
    SyrupSimplexPreparationHour = @SyrupSimplexPreparationHour,
    SyrupSimplexManpower = @SyrupSimplexManpower,
    SyrupSimplexLevel2CleaningHours = @SyrupSimplexLevel2CleaningHours,
    SyrupSimplexLevel2CleaningManpower = @SyrupSimplexLevel2CleaningManpower,
    SyrupSimplexNoofCampaign = @SyrupSimplexNoofCampaign,
    SyrupSimplexNextProcessName = @SyrupSimplexNextProcessName,
    SyruppreparationProcessName = @SyruppreparationProcessName,
    SyruppreparationLocation = @SyruppreparationLocation,
    SyruppreparationFirstVolumnHour = @SyruppreparationFirstVolumnHour,
    SyruppreparationFirstVolumnManpower = @SyruppreparationFirstVolumnManpower,
    SyruppreparationIPQCTest = @SyruppreparationIPQCTest,
    SyruppreparationTopupToVolumnHour = @SyruppreparationTopupToVolumnHour,
    SyruppreparationTopupToVolumnManpower = @SyruppreparationTopupToVolumnManpower,
    SyruppreparationCampaignBatchesNumbers = @SyruppreparationCampaignBatchesNumbers,
    SyruppreparationLevel1CleaningHours = @SyruppreparationLevel1CleaningHours,
    SyruppreparationLevel1Cleaningmanpower = @SyruppreparationLevel1Cleaningmanpower,
    SyruppreparationLevel2Cleaninghours = @SyruppreparationLevel2Cleaninghours,
    SyruppreparationLevel2CleaningManpower = @SyruppreparationLevel2CleaningManpower,
    SyruppreparationNextProcessName = @SyruppreparationNextProcessName,
    AddedByUserID = @AddedByUserID,
    ModifiedByUserID = @ModifiedByUserID,
    AddedDate = @AddedDate,
    ModifiedDate = @ModifiedDate
WHERE SyrupPlanningID = @SyrupPlanningID;
SELECT @SyrupPlanningID;";

                parameters.Add("SyrupPlanningID", model.Id);
                resultId = await conn.QuerySingleAsync<long>(updateSql, parameters);
            }
            else
            {
                string insertSql = @"
INSERT INTO dbo.SyrupPlanning
(
    MethodCodeLineID, DynamicFormDataID, MethodName, MethodCodeID, ProfileNo, MethodCode,
    BatchSizeInLiters, RestrictionOnPlanningDay, ProcessName,
    IsthereSyrupSimplextoproduce, SyrupSimplexProcessName, SyrupSimplexLocation,
    SyrupSimplexPreparationHour, SyrupSimplexManpower, SyrupSimplexLevel2CleaningHours,
    SyrupSimplexLevel2CleaningManpower, SyrupSimplexNoofCampaign, SyrupSimplexNextProcessName,
    SyruppreparationProcessName, SyruppreparationLocation, SyruppreparationFirstVolumnHour,
    SyruppreparationFirstVolumnManpower, SyruppreparationIPQCTest, SyruppreparationTopupToVolumnHour,
    SyruppreparationTopupToVolumnManpower, SyruppreparationCampaignBatchesNumbers, SyruppreparationLevel1CleaningHours,
    SyruppreparationLevel1Cleaningmanpower, SyruppreparationLevel2Cleaninghours, SyruppreparationLevel2CleaningManpower,
    SyruppreparationNextProcessName, AddedByUserID, ModifiedByUserID, AddedDate, ModifiedDate
)
OUTPUT INSERTED.SyrupPlanningID
VALUES
(
    @MethodCodeLineID, @DynamicFormDataID, @MethodName, @MethodCodeID, @ProfileNo, @MethodCode,
    @BatchSizeInLiters, @RestrictionOnPlanningDay, @ProcessName,
    @IsthereSyrupSimplextoproduce, @SyrupSimplexProcessName, @SyrupSimplexLocation,
    @SyrupSimplexPreparationHour, @SyrupSimplexManpower, @SyrupSimplexLevel2CleaningHours,
    @SyrupSimplexLevel2CleaningManpower, @SyrupSimplexNoofCampaign, @SyrupSimplexNextProcessName,
    @SyruppreparationProcessName, @SyruppreparationLocation, @SyruppreparationFirstVolumnHour,
    @SyruppreparationFirstVolumnManpower, @SyruppreparationIPQCTest, @SyruppreparationTopupToVolumnHour,
    @SyruppreparationTopupToVolumnManpower, @SyruppreparationCampaignBatchesNumbers, @SyruppreparationLevel1CleaningHours,
    @SyruppreparationLevel1Cleaningmanpower, @SyruppreparationLevel2Cleaninghours, @SyruppreparationLevel2CleaningManpower,
    @SyruppreparationNextProcessName, @AddedByUserID, @ModifiedByUserID, @AddedDate, @ModifiedDate
);";

                resultId = await conn.QuerySingleAsync<long>(insertSql, parameters);
            }

            model.Id = resultId;
            return model;
        }
        public async Task SaveSyrupFillingAsync(long syrupPlanningId, IEnumerable<SyrupFilling> items)
        {
            if (syrupPlanningId <= 0) throw new ArgumentException(nameof(syrupPlanningId));

            using var conn = CreateConnection();
            if (conn is SqlConnection sConn) await sConn.OpenAsync(); else conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                await conn.ExecuteAsync("DELETE FROM dbo.SyrupFilling WHERE SyrupPlanningID = @SyrupPlanningID",
                                        new { SyrupPlanningID = syrupPlanningId }, tran);

                const string insertSql = @"
                        INSERT INTO dbo.SyrupFilling
                        (DynamicFormDataID,DynamicFormDataItemID,SyrupPlanningID, ProfileNo, ProcessName_Primary, NextProcessName_Primary, PlanningType_Primary,
                         PrimaryFillingMachine, TypeOfPlanningProcess, FillingHours_Level1, FillingManpower_Level1,
                         Speed_BottlePerMinute, ChangePackingFillingHours, FillingHours_Level2, FillingManpower_Level2,
                         SecondarySameAsPrimaryTime, SecondaryPackingHours, SecondaryManpower,
                         ProcessName_Secondary, NextProcessName_Secondary, RequireOfflinePacking,
                         AddedByUserID, Description, ModifiedBy, ModifiedDate)
                        OUTPUT INSERTED.SyrupFillingID
                        VALUES
                        (@DynamicFormDataID,@DynamicFormDataItemID,@SyrupPlanningID, @ProfileNo, @ProcessName_Primary, @NextProcessName_Primary, @PlanningType_Primary,
                         @PrimaryFillingMachine, @TypeOfPlanningProcess, @FillingHours_Level1, @FillingManpower_Level1,
                         @Speed_BottlePerMinute, @ChangePackingFillingHours, @FillingHours_Level2, @FillingManpower_Level2,
                         @SecondarySameAsPrimaryTime, @SecondaryPackingHours, @SecondaryManpower,
                         @ProcessName_Secondary, @NextProcessName_Secondary, @RequireOfflinePacking,
                         @AddedByUserID, @Description, @ModifiedBy, @ModifiedDate);";

                var minSqlDate = new DateTime(1753, 1, 1);

                foreach (var it in items ?? Enumerable.Empty<SyrupFilling>())
                {

                    var parameters = new
                    {
                        SyrupPlanningID = syrupPlanningId,
                        DynamicFormDataItemID = it.DynamicFormDataItemID,
                        DynamicFormDataID = it.DynamicFormDataID,
                        ProfileNo = it.ProfileNo,
                        ProcessName_Primary = it.ProcessName_Primary,
                        NextProcessName_Primary = it.NextProcessName_Primary,
                        PlanningType_Primary = it.PlanningType_Primary,
                        PrimaryFillingMachine = it.PrimaryFillingMachine,
                        TypeOfPlanningProcess = it.TypeOfPlanningProcess,
                        FillingHours_Level1 = it.FillingHours_Level1,
                        FillingManpower_Level1 = it.FillingManpower_Level1,
                        Speed_BottlePerMinute = it.Speed_BottlePerMinute,
                        ChangePackingFillingHours = it.ChangePackingFillingHours,
                        FillingHours_Level2 = it.FillingHours_Level2,
                        FillingManpower_Level2 = it.FillingManpower_Level2,
                        SecondarySameAsPrimaryTime = it.SecondarySameAsPrimaryTime,
                        SecondaryPackingHours = it.SecondaryPackingHours,
                        SecondaryManpower = it.SecondaryManpower,
                        ProcessName_Secondary = it.ProcessName_Secondary,
                        NextProcessName_Secondary = it.NextProcessName_Secondary,
                        RequireOfflinePacking = it.RequireOfflinePacking,
                        AddedByUserID = it.AddedByUserID,
                        Description = it.Description,
                        ModifiedBy = it.ModifiedBy,
                        ModifiedDate = it.ModifiedDate
                    };

                    var newId = await conn.QuerySingleAsync<long>(insertSql, parameters, tran);
                    it.ID = newId;
                }

                tran.Commit();
            }
            catch
            {
                try { tran.Rollback(); } catch { /* ignore */ }
                throw;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed) conn.Close();
            }
        }
        public async Task<SyrupPlanning> InsertOrUpdateSyrupPlanningAsync_old(SyrupPlanning model)
        {
            try
            {
                var parameters = new DynamicParameters();

                if (model.Id > 0)
                {
                    parameters.Add("SyrupPlanningID", model.Id);
                }

                // Basic / identification fields
                parameters.Add("DynamicFormDataItemID", model.DynamicFormDataItemID);
                parameters.Add("MethodCodeLineID", model.MethodCodeLineID);
                parameters.Add("DynamicFormDataID", model.DynamicFormDataID);
                parameters.Add("MethodName", model.MethodName);
                parameters.Add("MethodCodeID", model.MethodCodeID);
                parameters.Add("ProfileNo", model.ProfileNo);
                parameters.Add("MethodCode", model.MethodCode);
                parameters.Add("BatchSizeInLiters", model.BatchSizeInLiters);
                parameters.Add("RestrictionOnPlanningDay", model.RestrictionOnPlanningDay);
                parameters.Add("ProcessName", model.ProcessName);

                // Syrup Simplex process
                parameters.Add("IsthereSyrupSimplextoproduce", model.IsthereSyrupSimplextoproduce);
                parameters.Add("SyrupSimplexProcessName", model.SyrupSimplexProcessName);
                parameters.Add("SyrupSimplexLocation", model.SyrupSimplexLocation);
                parameters.Add("SyrupSimplexPreparationHour", model.SyrupSimplexPreparationHour);
                parameters.Add("SyrupSimplexManpower", model.SyrupSimplexManpower);
                parameters.Add("SyrupSimplexLevel2CleaningHours", model.SyrupSimplexLevel2CleaningHours);
                parameters.Add("SyrupSimplexLevel2CleaningManpower", model.SyrupSimplexLevel2CleaningManpower);
                parameters.Add("SyrupSimplexNoofCampaign", model.SyrupSimplexNoofCampaign);
                parameters.Add("SyrupSimplexNextProcessName", model.SyrupSimplexNextProcessName);

                // Syrup preparation (step 3)
                parameters.Add("SyruppreparationProcessName", model.SyruppreparationProcessName);
                parameters.Add("SyruppreparationLocation", model.SyruppreparationLocation);
                parameters.Add("SyruppreparationFirstVolumnHour", model.SyruppreparationFirstVolumnHour);
                parameters.Add("SyruppreparationFirstVolumnManpower", model.SyruppreparationFirstVolumnManpower);
                parameters.Add("SyruppreparationIPQCTest", model.SyruppreparationIPQCTest);
                parameters.Add("SyruppreparationTopupToVolumnHour", model.SyruppreparationTopupToVolumnHour);
                parameters.Add("SyruppreparationTopupToVolumnManpower", model.SyruppreparationTopupToVolumnManpower);
                parameters.Add("SyruppreparationCampaignBatchesNumbers", model.SyruppreparationCampaignBatchesNumbers);
                parameters.Add("SyruppreparationLevel1CleaningHours", model.SyruppreparationLevel1CleaningHours);
                parameters.Add("SyruppreparationLevel1Cleaningmanpower", model.SyruppreparationLevel1Cleaningmanpower);
                parameters.Add("SyruppreparationLevel2Cleaninghours", model.SyruppreparationLevel2Cleaninghours);
                parameters.Add("SyruppreparationLevel2CleaningManpower", model.SyruppreparationLevel2CleaningManpower);
                parameters.Add("SyruppreparationNextProcessName", model.SyruppreparationNextProcessName);

                // Preparation performance & cleaning
                parameters.Add("PreparationPerHour", model.PreparationPerHour);
                parameters.Add("Level2CleaningHours", model.Level2CleaningHours);
                parameters.Add("Level2CleaningManpower", model.Level2CleaningManpower);

                parameters.Add("NoOfCampaign", model.NoOfCampaign);
                parameters.Add("NextProcessName", model.NextProcessName);

                // Additional preparation fields from model          
                parameters.Add("PreparationFirstVolumePerHour", model.PreparationFirstVolumePerHour);
                parameters.Add("PreparationFirstVolumeManpower", model.PreparationFirstVolumeManpower);
                parameters.Add("IpqcTestRequired", model.IpqcTestRequired);
                parameters.Add("PreparationTopUpPerHour", model.PreparationTopUpPerHour);
                parameters.Add("PreparationTopUpManpower", model.PreparationTopUpManpower);
                parameters.Add("CampaignBatches", model.CampaignBatches);

                // Cleaning level 1
                parameters.Add("Level1CleaningHours", model.Level1CleaningHours);
                parameters.Add("Level1CleaningManpower", model.Level1CleaningManpower);

                // Cleaning level 2 (for syrup preparation)
                parameters.Add("SyrupLevel2CleaningHours", model.SyrupLevel2CleaningHours);
                parameters.Add("SyrupLevel2CleaningManpower", model.SyrupLevel2CleaningManpower);

                parameters.Add("NextProcessNameAfterPreparation", model.NextProcessNameAfterPreparation);

                // Metadata / audit fields
                parameters.Add("AddedByUserID", model.AddedByUserID);
                parameters.Add("ModifiedByUserID", model.ModifiedByUserID);
                parameters.Add("AddedDate", model.AddedDate);
                parameters.Add("ModifiedDate", model.ModifiedDate);


                var newId = await InsertOrUpdate("SyrupPlanning", "SyrupPlanningID", model.Id, parameters);

                model.Id = Convert.ToInt64(newId);



                return model;
            }
            catch (Exception exp)
            {
                throw new Exception("InsertOrUpdateSyrupPlanningAsync failed: " + exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<TimingDetailDto>> GetTimingDetailsByProfileNoAsync(string profileNo)
        {
            if (string.IsNullOrWhiteSpace(profileNo)) return Array.Empty<TimingDetailDto>();

            // Reuse union approach but map to TimingDetailDto columns
            const string sql = @"SELECT ProcessName, DurationHours, Manpower, Location, Notes FROM
                    (
                        SELECT sp.SyrupSimplexProcessName AS ProcessName,
                               TRY_CONVERT(decimal(38,10), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')) AS DurationHours,
                               CONVERT(nvarchar(100), TRY_CONVERT(int, sp.SyrupSimplexManpower)) AS Manpower,
                               sp.SyrupSimplexLocation AS Location,
                               NULL AS Notes, 10 AS Seq
                        FROM dbo.SyrupPlanning sp WHERE sp.ProfileNo = @ProfileNo

                        UNION ALL

                        SELECT sp.SyruppreparationProcessName,
                               TRY_CONVERT(decimal(38,10), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')) AS DurationHours,
                               CONVERT(nvarchar(100), TRY_CONVERT(int, sp.SyruppreparationFirstVolumnManpower)) AS Manpower,
                               sp.SyruppreparationLocation,
                               NULL, 20
                        FROM dbo.SyrupPlanning sp WHERE sp.ProfileNo = @ProfileNo

                        UNION ALL

                        SELECT op.ProcessName,
                               TRY_CONVERT(decimal(38,10), REPLACE(op.ManhoursOrHours, ',', '')) AS DurationHours,
                               CONVERT(nvarchar(100), TRY_CONVERT(int, op.NoOfManpower)) AS Manpower,
                               op.LocationOfProcess,
                               op.OtherJobsInformation,
                               30
                        FROM dbo.SyrupOtherProcess op WHERE op.ProfileNo = @ProfileNo

                        UNION ALL

                        SELECT sf.ProcessName_Primary,
                               TRY_CONVERT(decimal(38,10), sf.FillingHours_Level1) AS DurationHours,
                               CONVERT(nvarchar(100), TRY_CONVERT(int, sf.FillingManpower_Level1)) AS Manpower,
                               NULL,
                               sf.PlanningType_Primary,
                               40
                        FROM dbo.SyrupFilling sf WHERE sf.ProfileNo = @ProfileNo
                    ) t
                    ORDER BY Seq";

            using var conn = CreateConnection();
            {
                try
                {
                    var rows = await conn.QueryAsync<TimingDetailDto>(sql, new { ProfileNo = profileNo });
                    return rows.ToList();
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }


        }

        public async Task<IReadOnlyList<MachineInfoDto>> GetMachineInfoByProfileNoAsync(string profileNo)
        {
            // If you have a dedicated machine table, join on that. If not, derive machine info from SyrupFilling
            const string sql = @"SELECT
                            ISNULL(sf.PrimaryFillingMachine, '') AS Machine,
                            'Filling Machine' AS Type,
                            '' AS Capacity,
                            CONCAT(COALESCE(CONVERT(varchar(50), sf.Speed_BottlePerMinute), ''),' bpm') AS Speed,
                            '' AS Notes
                        FROM dbo.SyrupFilling sf
                        WHERE sf.ProfileNo = @ProfileNo
                        GROUP BY sf.PrimaryFillingMachine, sf.Speed_BottlePerMinute";
            using var conn = CreateConnection();
            {
                try
                {
                    var rows = await conn.QueryAsync<MachineInfoDto>(sql, new { ProfileNo = profileNo });
                    return rows.ToList();
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }
            //await OpenConnectionAsync(conn);

        }

        public async Task<IReadOnlyList<ProductItemDto>> GetProductItemsByMethodCodeAsync(int methodCodeId)
        {
            // Use your master join to nav tables. Return the NAV items for the method code
            const string sql = @"SELECT t4.No, t4.Description, t4.Description2, t4.CategoryID
                        FROM DynamicForm_ProductiontimingNMachineInfosyrup t1
                        INNER JOIN navmethodcode t2 ON t2.MethodCodeID = t1.[13192_MethodCode_UId]
                        INNER JOIN navmethodcodelines t3 ON t3.MethodCodeID = t2.MethodCodeID
                        INNER JOIN NAVItems t4 ON t4.ItemId = t3.ItemID
                        WHERE t2.MethodCodeID = @MethodCodeID
                        GROUP BY t4.No, t4.Description, t4.Description2, t4.CategoryID";
            using var conn = CreateConnection();
            {
                try
                {
                    var rows = await conn.QueryAsync<ProductItemDto>(sql, new { MethodCodeID = methodCodeId });
                    return rows.ToList();
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }
            //await OpenConnectionAsync(conn);

        }

        public async Task<SyrupPlanningDto?> GetSyrupPlanningByIdAsync(long syrupPlanningId)
        {
            const string sql = @"SELECT TOP (1)
                        SyrupPlanningID,
                        ProfileNo,
                        MethodCodeID
                    FROM dbo.SyrupPlanning
                    WHERE SyrupPlanningID = @SyrupPlanningID";
            using var conn = CreateConnection();
            {
                try
                {
                    var r = await conn.QuerySingleOrDefaultAsync<SyrupPlanningDto>(sql, new { SyrupPlanningID = syrupPlanningId });
                    return r;
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }
            //await OpenConnectionAsync(conn);

        }

        public async Task<ProductItemDto?> GetMasterByMethodCodeAsync(int methodCodeId)
        {
            const string sql = @"SELECT TOP (1) t4.No, t4.Description, t4.Description2, t4.CategoryID
                        FROM DynamicForm_ProductiontimingNMachineInfosyrup t1
                        INNER JOIN navmethodcode t2 ON t2.MethodCodeID = t1.[13192_MethodCode_UId]
                        INNER JOIN navmethodcodelines t3 ON t3.MethodCodeID = t2.MethodCodeID
                        INNER JOIN NAVItems t4 ON t4.ItemId = t3.ItemID
                        WHERE t2.MethodCodeID = @MethodCodeID";
            using var conn = CreateConnection();
            {
                try
                {
                    var r = await conn.QuerySingleOrDefaultAsync<ProductItemDto>(sql, new { MethodCodeID = methodCodeId });
                    return r;
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }

            }
            //await OpenConnectionAsync(conn);

        }

        public async Task<TimingOverviewDto?> GetTimingOverviewByProfileNoAsync(string profileNo)
        {
            if (string.IsNullOrWhiteSpace(profileNo)) return null;

            const string sql = @"SELECT TOP (1)
                            sp.ProfileNo,
                            -- you can customize these fields: examples below use existing columns or fallbacks
                            COALESCE(
                                NULLIF(sp.MethodCodeID,''), -- if the method stores batch/size etc (example)
                                '') as MethodCode,
                            -- use literal defaults or derive from filling rows / planning rows
                            '10 hrs' AS WorkingHours,
                            '8:00 AM - 6:30 PM (with OT)' AS WorkingHoursNote,
                            '60-85 bpm' AS FillingSpeed,
                            'Depending on bottle size and viscosity' AS FillingSpeedNote,
                            sp.BatchSizeInLiters AS BatchSize,
                            'Standard batch volume' AS BatchSizeNote,
                            '' AS ShortNote,
                            sp.MethodName as MethodCodeName
                        FROM dbo.SyrupPlanning sp
                        WHERE sp.ProfileNo = @ProfileNo";

            using var conn = CreateConnection();
            {
                try
                {
                    var r = await conn.QuerySingleOrDefaultAsync<TimingOverviewDto>(sql, new { ProfileNo = profileNo });
                    return r;
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed) conn.Close();
                }
            }

        }


        public async Task<IReadOnlyList<SyrupResourceData>> GetProductionGanttRowsAsync_old(string profileNo = null)
        {
            const string sql = @"SELECT
                        0 AS Seq,
                        0 AS ParentGroup,
                        CAST(NULL AS int) AS ParentId,
                        -- create a subject label
                        CONCAT('Syrup Simplex - ', COALESCE(sp.SyrupSimplexProcessName, '')) AS Subject,
                        TRY_CONVERT(datetime, sp.SyrupSimplexStartDate) AS StartTime,
                        TRY_CONVERT(datetime, sp.SyrupSimplexEndDate) AS EndTime,
                        0 AS IsAllDay,
                        'Event' AS Type,
                        sp.ProfileNo,
                        sp.SyrupPlanningID
                    FROM dbo.SyrupPlanning sp
                    WHERE (@ProfileNo IS NULL OR sp.ProfileNo = @ProfileNo)

                    UNION ALL

                    -- SyrupOtherProcess rows
                    SELECT
                        1 AS Seq,
                        1 AS ParentGroup,
                        CAST(NULL AS int) AS ParentId,
                        CONCAT('Other - ', op.ProcessName) AS Subject,
                        TRY_CONVERT(datetime, op.StartDate) AS StartTime,
                        TRY_CONVERT(datetime, op.EndDate) AS EndTime,
                        0 AS IsAllDay,
                        'Event' AS Type,
                        op.ProfileNo,
                        op.SyrupPlanningID
                    FROM dbo.SyrupOtherProcess op
                    WHERE (@ProfileNo IS NULL OR op.ProfileNo = @ProfileNo)

                    UNION ALL

                    -- SyrupFilling rows
                    SELECT
                        2 AS Seq,
                        2 AS ParentGroup,
                        CAST(NULL AS int) AS ParentId,
                        CONCAT('Filling - ', sf.ProcessName_Primary) AS Subject,
                        TRY_CONVERT(datetime, sf.StartDate) AS StartTime,
                        TRY_CONVERT(datetime, sf.EndDate) AS EndTime,
                        0 AS IsAllDay,
                        'Event' AS Type,
                        sf.ProfileNo,
                        sf.SyrupPlanningID
                    FROM dbo.SyrupFilling sf
                    WHERE (@ProfileNo IS NULL OR sf.ProfileNo = @ProfileNo)

                    ORDER BY Seq, StartTime;
                    ";

            using var conn = CreateConnection();
            if (conn is SqlConnection sconn) await sconn.OpenAsync();
            else if (conn.State == ConnectionState.Closed) conn.Open();

            try
            {
                // We will read raw rows then construct parent-child grouping in C#
                var rows = (await conn.QueryAsync<dynamic>(sql, new { ProfileNo = profileNo })).ToList();

                var result = new List<SyrupResourceData>();
                int idCounter = 1;

                // We will create top-level project nodes for each distinct ProfileNo (or SyrupPlanning)
                // and then add each event row as a child of that project.
                var grouped = rows
                    .GroupBy(r => (string)r.ProfileNo ?? "NO_PROFILE")
                    .ToList();

                foreach (var g in grouped)
                {
                    // create a top-level project node (if desired)
                    var projectNode = new SyrupResourceData
                    {
                        Id = idCounter++,
                        ParentId = null,
                        Subject = $"Profile: {g.Key}",
                        StartTime = g.Min(x => (DateTime?)x.StartTime) ?? DateTime.Today,
                        EndTime = g.Max(x => (DateTime?)x.EndTime) ?? DateTime.Today.AddHours(1),
                        IsAllDay = false,
                        Type = "Project",
                        ProfileNo = g.Key
                    };
                    result.Add(projectNode);

                    foreach (var r in g)
                    {
                        DateTime start = r.StartTime == null ? projectNode.StartTime : (DateTime)r.StartTime;
                        DateTime end = r.EndTime == null ? start.AddHours(1) : (DateTime)r.EndTime;

                        var ev = new SyrupResourceData
                        {
                            Id = idCounter++,
                            ParentId = projectNode.Id,
                            Subject = (string)r.Subject,
                            StartTime = start,
                            EndTime = end,
                            IsAllDay = r.IsAllDay == 1,
                            Type = (string)r.Type ?? "Event",
                            ProfileNo = r.ProfileNo,
                            SyrupPlanningID = r.SyrupPlanningID
                        };
                        result.Add(ev);
                    }
                }

                return result;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed) conn.Close();
            }
        }
        public async Task<IReadOnlyList<SyrupResourceData>> GetProductionGanttRowsAsync_v(string profileNo, DateTime productionDay, TimeSpan shiftStart)
        {
            // productionDay: the date to schedule (date portion used)
            // shiftStart: time-of-day when scheduling starts (e.g. TimeSpan.FromHours(8) for 08:00)
            // Example call: GetProductionGanttRowsAsync("BMP-206", DateTime.Today, TimeSpan.FromHours(8));

            var startDateTime = productionDay.Date + shiftStart; // will be passed into SQL

            const string sqlTimeline = @"
    WITH AllProcesses AS
    (
        /* 1. Other Process */
        SELECT 
            op.SyrupPlanningID,
            10 AS Seq,
            'OtherProcess' AS Source,
            op.ProcessName AS ProcessName,
            op.SyrupOtherProcessNextProcess AS NextProcessName,
            TRY_CONVERT(decimal(18,4), op.ManhoursOrHours) AS DurationHours,
            CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), op.ManhoursOrHours), 0)*60, 0) AS INT) AS DurationMinutes,
            TRY_CONVERT(decimal(18,4), op.NoOfManpower) AS Manpower
        FROM dbo.SyrupOtherProcess op
        WHERE op.ProfileNo = @ProfileNo

        UNION ALL

        /* 2. Syrup Simplex */
        SELECT
            sp.SyrupPlanningID,
            20 AS Seq,
            'SyrupSimplex' AS Source,
            sp.SyrupSimplexProcessName AS ProcessName,
            sp.SyrupSimplexNextProcessName AS NextProcessName,
            TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')) AS DurationHours,
            CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')), 0)*60, 0) AS INT) AS DurationMinutes,
            TRY_CONVERT(decimal(18,4), sp.SyrupSimplexManpower) AS Manpower
        FROM dbo.SyrupPlanning sp
        WHERE sp.ProfileNo = @ProfileNo

        UNION ALL

        /* 3. Syrup Simplex - Level2 Cleaning */
        SELECT
            sp.SyrupPlanningID,
            21 AS Seq,
            'SyrupSimplexLevel2Cleaning' AS Source,
            CONCAT(sp.SyrupSimplexProcessName, ' - Level2 Cleaning') AS ProcessName,
            sp.SyrupSimplexNextProcessName AS NextProcessName,
            TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours) AS DurationHours,
            CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours), 0)*60, 0) AS INT) AS DurationMinutes,
            TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningManpower) AS Manpower
        FROM dbo.SyrupPlanning sp
        WHERE sp.ProfileNo = @ProfileNo

        UNION ALL

        /* 4. Syrup Preparation - Mixing */
        SELECT
            sp.SyrupPlanningID,
            30 AS Seq,
            'SyrupPreparation' AS Source,
            sp.SyruppreparationProcessName AS ProcessName,
            sp.SyruppreparationNextProcessName AS NextProcessName,
            TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')) AS DurationHours,
            CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')), 0)*60, 0) AS INT) AS DurationMinutes,
            TRY_CONVERT(decimal(18,4), sp.SyruppreparationFirstVolumnManpower) AS Manpower
        FROM dbo.SyrupPlanning sp
        WHERE sp.ProfileNo = @ProfileNo

        /* 5. Syrup Filling — include each process column separately */
        UNION ALL

        /* 5a. ProcessName_Primary */
        SELECT
            sf.SyrupPlanningID,
            40 AS Seq,
            'PrimaryPacking' AS Source,
            sf.ProcessName_Primary AS ProcessName,
            sf.NextProcessName_Primary AS NextProcessName,
            COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0) AS DurationHours,
            CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0), 0)*60, 0) AS INT) AS DurationMinutes,
            TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level1) AS Manpower
        FROM dbo.SyrupFilling sf
        JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
        WHERE sp.ProfileNo = @ProfileNo AND sf.ProcessName_Primary IS NOT NULL

        UNION ALL

        /* 5b. NextProcessName_Primary (Machine Filling) */
        SELECT
            sf.SyrupPlanningID,
            41 AS Seq,
            'MachineFilling' AS Source,
            sf.NextProcessName_Primary AS ProcessName,
            sf.ProcessName_Secondary AS NextProcessName,
            COALESCE(sf.FillingHours_Level2, 0) AS DurationHours,
            CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level2, 0), 0)*60, 0) AS INT) AS DurationMinutes,
            TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level2) AS Manpower
        FROM dbo.SyrupFilling sf
        JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
        WHERE sp.ProfileNo = @ProfileNo AND sf.NextProcessName_Primary IS NOT NULL

        UNION ALL

        /* 5c. ProcessName_Secondary */
        SELECT
            sf.SyrupPlanningID,
            42 AS Seq,
            'SecondaryPacking' AS Source,
            sf.ProcessName_Secondary AS ProcessName,
            sf.NextProcessName_Secondary AS NextProcessName,
            CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                 ELSE COALESCE(sf.SecondaryPackingHours,0) END AS DurationHours,
            CAST(ROUND(ISNULL(
                CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                     ELSE COALESCE(sf.SecondaryPackingHours,0) END, 0)*60, 0) AS INT) AS DurationMinutes,
            TRY_CONVERT(decimal(18,4), sf.SecondaryManpower) AS Manpower
        FROM dbo.SyrupFilling sf
        JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
        WHERE sp.ProfileNo = @ProfileNo AND sf.ProcessName_Secondary IS NOT NULL

        UNION ALL

        /* 5d. NextProcessName_Secondary (if you have one more step after secondary) */
        SELECT
            sf.SyrupPlanningID,
            43 AS Seq,
            'SecondaryNext' AS Source,
            sf.NextProcessName_Secondary AS ProcessName,
            NULL AS NextProcessName,
            0 AS DurationHours,
            0 AS DurationMinutes,
            NULL AS Manpower
        FROM dbo.SyrupFilling sf
        JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
        WHERE sp.ProfileNo = @ProfileNo AND sf.NextProcessName_Secondary IS NOT NULL
    ),
    Ordered AS
    (
        SELECT *, ROW_NUMBER() OVER (ORDER BY Seq) AS rn
        FROM AllProcesses
    ),
    Timeline AS
    (
        SELECT *,
               SUM(DurationMinutes) OVER (ORDER BY rn ROWS UNBOUNDED PRECEDING) AS CumMinutes
        FROM Ordered
    )
    SELECT
        cur.Seq,
        cur.Source,
        cur.ProcessName,
        cur.NextProcessName,
        cur.DurationHours,
        cur.DurationMinutes,
        DATEADD(MINUTE, (cur.CumMinutes - cur.DurationMinutes), @StartDateTimeParam) AS StartTime,
        DATEADD(MINUTE, cur.CumMinutes, @StartDateTimeParam) AS EndTime,
        cur.Manpower,
        nxt.ProcessName AS NextProcess_Timeline
    FROM Timeline cur
    LEFT JOIN Timeline nxt ON nxt.rn = cur.rn + 1
    ORDER BY cur.rn;";

            using var conn = CreateConnection();
            if (conn is SqlConnection sconn) await sconn.OpenAsync();
            else if (conn.State == ConnectionState.Closed) conn.Open();

            try
            {
                var rows = (await conn.QueryAsync(sqlTimeline, new
                {
                    ProfileNo = profileNo,
                    StartDateTimeParam = startDateTime
                })).ToList();

                var results = new List<SyrupResourceData>();
                int idCounter = 1;

                foreach (var r in rows)
                {
                    DateTime? start = r.StartTime;
                    DateTime? end = r.EndTime;
                    decimal durHours = r.DurationHours ?? 0m;
                    int durMinutes = r.DurationMinutes ?? 0;
                    decimal manp = r.Manpower ?? 0m;

                    results.Add(new SyrupResourceData
                    {
                        Id = idCounter++,
                        ChildId = idCounter,           // or set a stable child id if needed
                        ParentId = null,
                        Subject = (r.ProcessName as string) ?? "Process",
                        StartTime = start ?? startDateTime,
                        EndTime = end ?? (start ?? startDateTime).AddMinutes(durMinutes),
                        IsAllDay = false,
                        Type = (r.Source as string) ?? "Process",
                        Location = null,
                        Notes = $"NextProcess(Data)={r.NextProcessName ?? ""}; NextProcess(Timeline)={r.NextProcess_Timeline ?? ""}; Manpower={manp}"
                    });

                }

                return results.OrderBy(r => r.StartTime).ToList();
            }
            finally
            {
                if (conn.State != ConnectionState.Closed) conn.Close();
            }
        }
        public async Task<IReadOnlyList<TaskData>> GetProductionGanttRowsAsync(string profileNo, DateTime productionDay, TimeSpan shiftStart)
        {
            var startDateTime = productionDay.Date + shiftStart;

            const string sqlTimeline = @"
WITH AllProcesses AS
(
    /* 1. Other Process */
    SELECT 
        op.SyrupPlanningID,
        10 AS Seq,
        'OtherProcess' AS Source,
        op.ProcessName AS ProcessName,
        op.SyrupOtherProcessNextProcess AS NextProcessName,
        TRY_CONVERT(decimal(18,4), op.ManhoursOrHours) AS DurationHours,
        CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), op.ManhoursOrHours), 0)*60, 0) AS INT) AS DurationMinutes,
        TRY_CONVERT(decimal(18,4), op.NoOfManpower) AS Manpower
    FROM dbo.SyrupOtherProcess op
    WHERE op.ProfileNo = @ProfileNo

    UNION ALL

    /* 2. Syrup Simplex */
    SELECT
        sp.SyrupPlanningID,
        20 AS Seq,
        'SyrupSimplex' AS Source,
        sp.SyrupSimplexProcessName AS ProcessName,
        sp.SyrupSimplexNextProcessName AS NextProcessName,
        TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')) AS DurationHours,
        CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')), 0)*60, 0) AS INT) AS DurationMinutes,
        TRY_CONVERT(decimal(18,4), sp.SyrupSimplexManpower) AS Manpower
    FROM dbo.SyrupPlanning sp
    WHERE sp.ProfileNo = @ProfileNo

    /* ... (keep other UNION ALL sections same as your current query) ... */
),
Ordered AS
(
    SELECT *, ROW_NUMBER() OVER (ORDER BY Seq) AS rn
    FROM AllProcesses
),
Timeline AS
(
    SELECT *,
           SUM(DurationMinutes) OVER (ORDER BY rn ROWS UNBOUNDED PRECEDING) AS CumMinutes
    FROM Ordered
)
SELECT
    cur.Seq,
    cur.Source,
    cur.ProcessName,
    cur.NextProcessName,
    cur.DurationHours,
    cur.DurationMinutes,
    DATEADD(MINUTE, (cur.CumMinutes - cur.DurationMinutes), @StartDateTimeParam) AS StartTime,
    DATEADD(MINUTE, cur.CumMinutes, @StartDateTimeParam) AS EndTime,
    cur.Manpower,
    nxt.ProcessName AS NextProcess_Timeline
FROM Timeline cur
LEFT JOIN Timeline nxt ON nxt.rn = cur.rn + 1
ORDER BY cur.rn;";

            using var conn = CreateConnection();
            if (conn is SqlConnection sconn) await sconn.OpenAsync();
            else if (conn.State == ConnectionState.Closed) conn.Open();

            try
            {
                var rows = (await conn.QueryAsync(sqlTimeline, new
                {
                    ProfileNo = profileNo,
                    StartDateTimeParam = startDateTime
                })).ToList();

                var tasks = new List<TaskData>();
                int idCounter = 1;

                foreach (var r in rows)
                {
                    DateTime? start = r.StartTime;
                    DateTime? end = r.EndTime;
                    string durationStr = r.DurationHours != null ? r.DurationHours.ToString() : "0";

                    tasks.Add(new TaskData
                    {
                        TaskId = idCounter++,
                        TaskName = (r.ProcessName as string) ?? "Unnamed Process",
                        StartDate = start ?? startDateTime,
                        EndDate = end ?? (start ?? startDateTime).AddMinutes(r.DurationMinutes ?? 0),
                        Duration = durationStr,
                        Progress = 0,
                        Predecessor = string.Empty,
                        Notes = $"NextProcess={r.NextProcessName ?? ""}; NextTimeline={r.NextProcess_Timeline ?? ""}; Manpower={r.Manpower ?? 0}",
                        ParentId = null
                    });
                }

                return tasks;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed) conn.Close();
            }
        }
        public async Task<IReadOnlyList<SyrupResourceData>> GetProductionGanttRowsAsync_old1(string profileNo, DateTime productionDay, TimeSpan shiftStart)
        {
            // productionDay: the date to schedule (date portion used)
            // shiftStart: time-of-day when scheduling starts (e.g. TimeSpan.FromHours(8) for 08:00)
            // Example call: GetProductionGanttRowsAsync("BMP-206", DateTime.Today, TimeSpan.FromHours(8));

            // Build queries to fetch the rows you have; adjust column names to your schema if necessary
            const string sqlPlanning = @"
SELECT SyrupPlanningID, ProfileNo,
       SyrupSimplexProcessName, SyrupSimplexLocation, SyrupSimplexPreparationHour, SyrupSimplexManpower, SyrupSimplexNextProcessName,
       SyruppreparationProcessName, SyruppreparationLocation, SyruppreparationFirstVolumnHour, SyruppreparationFirstVolumnManpower, SyruppreparationNextProcessName,
       -- other fields if needed
       MethodCodeID
FROM dbo.SyrupPlanning
WHERE (@ProfileNo IS NULL OR ProfileNo = @ProfileNo);
";

            const string sqlOther = @"
SELECT ProfileNo, ProcessName, LocationOfProcess, ManhoursOrHours, NoOfManpower, SyrupOtherProcessNextProcess, OtherJobsInformation
FROM dbo.SyrupOtherProcess
WHERE (@ProfileNo IS NULL OR ProfileNo = @ProfileNo)";

            const string sqlFilling = @"
SELECT ProfileNo, ProcessName_Primary, PrimaryFillingMachine, FillingHours_Level1, FillingManpower_Level1,
       FillingHours_Level2, FillingManpower_Level2, NextProcessName_Primary
FROM dbo.SyrupFilling
WHERE (@ProfileNo IS NULL OR ProfileNo = @ProfileNo)
";

            using var conn = CreateConnection();
            if (conn is SqlConnection sconn) await sconn.OpenAsync();
            else if (conn.State == ConnectionState.Closed) conn.Open();

            try
            {
                // 1) read planning row(s) - usually one
                var planning = (await conn.QueryAsync(sqlPlanning, new { ProfileNo = profileNo })).FirstOrDefault();

                // 2) read other process rows
                var otherRows = (await conn.QueryAsync(sqlOther, new { ProfileNo = profileNo })).ToList();

                // 3) read filling rows
                var fillingRows = (await conn.QueryAsync(sqlFilling, new { ProfileNo = profileNo })).ToList();

                // Start scheduling
                var baseDay = productionDay.Date + shiftStart; // e.g. 2025-10-10 08:00
                var nextAvailableByLocation = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
                var results = new List<SyrupResourceData>();
                int idCounter = 1;

                // Helper: parse hours from various possible types (string, decimal, numeric)
                decimal ParseHours(object? value)
                {
                    if (value == null) return 0m;
                    if (value is decimal d) return d;
                    if (value is double db) return Convert.ToDecimal(db);
                    if (value is int i) return i;
                    var s = value.ToString() ?? "";
                    s = s.Replace(",", ".").Trim();
                    if (decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var res))
                        return res;
                    // try remove non-numeric
                    var digits = new string(s.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());
                    if (decimal.TryParse(digits, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res))
                        return res;
                    return 0m;
                }

                // Create a helper to get next available slot for a location
                DateTime GetNextAvailable(string? location)
                {
                    var loc = string.IsNullOrWhiteSpace(location) ? "DEFAULT" : location!;
                    if (!nextAvailableByLocation.TryGetValue(loc, out var dt))
                    {
                        dt = baseDay;
                        nextAvailableByLocation[loc] = dt;
                    }
                    return dt;
                }

                void SetNextAvailable(string? location, DateTime end)
                {
                    var loc = string.IsNullOrWhiteSpace(location) ? "DEFAULT" : location!;
                    nextAvailableByLocation[loc] = end;
                }

                // 1) Schedule Syrup Simplex step from planning (if exists)
                if (planning != null)
                {
                    var simplexName = planning.SyrupSimplexProcessName as string ?? "Syrup Simplex";
                    var loc = planning.SyrupSimplexLocation as string ?? "General";
                    var hours = ParseHours(planning.SyrupSimplexPreparationHour);
                    var start = GetNextAvailable(loc);
                    var end = start.AddMinutes((double)(hours * 60m));
                    results.Add(new SyrupResourceData
                    {
                        Id = idCounter++,
                        ParentId = null,
                        Subject = simplexName + (hours > 0 ? $" - batch" : ""),
                        StartTime = start,
                        EndTime = end,
                        IsAllDay = false,
                        Type = "Simplex",
                        Location = loc,
                        Notes = $"Manpower: {planning.SyrupSimplexManpower}"
                    });
                    SetNextAvailable(loc, end);
                }

                // 2) Schedule Syrup Preparation (first volume) from planning
                if (planning != null)
                {
                    var name = planning.SyruppreparationProcessName as string ?? "Syrup Preparation";
                    var loc = planning.SyruppreparationLocation as string ?? "General";
                    var hours = ParseHours(planning.SyruppreparationFirstVolumnHour);
                    // place in same location or dedicated depending on business logic - we use that location's next slot
                    var start = GetNextAvailable(loc);
                    var end = start.AddMinutes((double)(hours * 60m));
                    results.Add(new SyrupResourceData
                    {
                        Id = idCounter++,
                        ParentId = null,
                        Subject = name + " - First Volume",
                        StartTime = start,
                        EndTime = end,
                        IsAllDay = false,
                        Type = "Preparation",
                        Location = loc,
                        Notes = $"Manpower: {planning.SyruppreparationFirstVolumnManpower}"
                    });
                    SetNextAvailable(loc, end);
                }

                // 3) Schedule SyrupOtherProcess rows (each has ManhoursOrHours)
                foreach (var op in otherRows)
                {
                    var loc = (op.LocationOfProcess as string) ?? "General";
                    var hours = ParseHours(op.ManhoursOrHours);
                    var start = GetNextAvailable(loc);
                    var end = start.AddMinutes((double)(hours * 60m));
                    results.Add(new SyrupResourceData
                    {
                        Id = idCounter++,
                        ParentId = null,
                        Subject = op.ProcessName ?? "Other Job",
                        StartTime = start,
                        EndTime = end,
                        IsAllDay = false,
                        Type = "Other",
                        Location = loc,
                        Notes = op.OtherJobsInformation
                    });
                    SetNextAvailable(loc, end);
                }

                // 4) Schedule SyrupFilling rows (primary) - they might be longer and use different location (e.g., Filling Room)
                foreach (var f in fillingRows)
                {
                    var loc = (f.PrimaryFillingMachine as string) ?? "Filling Room";
                    var hours = ParseHours(f.FillingHours_Level1);
                    var start = GetNextAvailable(loc);
                    var end = start.AddMinutes((double)(hours * 60m));
                    results.Add(new SyrupResourceData
                    {
                        Id = idCounter++,
                        ParentId = null,
                        Subject = (f.ProcessName_Primary as string) ?? "Filling",
                        StartTime = start,
                        EndTime = end,
                        IsAllDay = false,
                        Type = "Filling",
                        Location = loc,
                        Notes = $"Level2 Hrs: {f.FillingHours_Level2}"
                    });
                    SetNextAvailable(loc, end);
                }

                // Optionally sort results by StartTime
                results = results.OrderBy(r => r.Location).ThenBy(r => r.StartTime).ToList();

                return results;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed) conn.Close();
            }
        }


        public async Task<IReadOnlyList<TaskData>> GetProductionGanttAsyncList(string profileNo, DateTime productionDay, TimeSpan shiftStart)
        {
            // productionDay: the date to schedule (date portion used)
            // shiftStart: time-of-day when scheduling starts (e.g. TimeSpan.FromHours(8) for 08:00)
            // Example call: GetProductionGanttRowsAsync("BMP-206", DateTime.Today, TimeSpan.FromHours(8));

            var startDateTime = productionDay.Date + shiftStart; // will be passed into SQL

            const string sqlTimeline = @"WITH AllProcesses AS
                            (/* 1. Other Process */
                                SELECT 
                                    op.SyrupPlanningID,
                                    10 AS Seq,
                                    'OtherProcess' AS Source,
                                    op.ProcessName AS ProcessName,
                                    op.SyrupOtherProcessNextProcess AS NextProcessName,
                                    TRY_CONVERT(decimal(18,4), op.ManhoursOrHours) AS DurationHours,
                                    CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), op.ManhoursOrHours), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), op.NoOfManpower) AS Manpower
                                FROM dbo.SyrupOtherProcess op
                                WHERE op.ProfileNo = @ProfileNo

                                UNION ALL

                                /* 2. Syrup Simplex */
                                SELECT
                                    sp.SyrupPlanningID,
                                    20 AS Seq,
                                    'SyrupSimplex' AS Source,
                                    sp.SyrupSimplexProcessName AS ProcessName,
                                    sp.SyrupSimplexNextProcessName AS NextProcessName,
                                    TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')) AS DurationHours,
                                    CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sp.SyrupSimplexManpower) AS Manpower
                                FROM dbo.SyrupPlanning sp
                                WHERE sp.ProfileNo = @ProfileNo

                                UNION ALL

                                /* 3. Syrup Simplex - Level2 Cleaning */
                                SELECT
                                    sp.SyrupPlanningID,
                                    21 AS Seq,
                                    'SyrupSimplexLevel2Cleaning' AS Source,
                                    CONCAT(sp.SyrupSimplexProcessName, ' - Level2 Cleaning') AS ProcessName,
                                    sp.SyrupSimplexNextProcessName AS NextProcessName,
                                    TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours) AS DurationHours,
                                    CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningManpower) AS Manpower
                                FROM dbo.SyrupPlanning sp
                                WHERE sp.ProfileNo = @ProfileNo

                                UNION ALL

                                /* 4. Syrup Preparation - Mixing */
                                SELECT
                                    sp.SyrupPlanningID,
                                    30 AS Seq,
                                    'SyrupPreparation' AS Source,
                                    sp.SyruppreparationProcessName AS ProcessName,
                                    sp.SyruppreparationNextProcessName AS NextProcessName,
                                    TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')) AS DurationHours,
                                    CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sp.SyruppreparationFirstVolumnManpower) AS Manpower
                                FROM dbo.SyrupPlanning sp
                                WHERE sp.ProfileNo = @ProfileNo

                                /* 5. Syrup Filling — include each process column separately */
                                UNION ALL

                                /* 5a. ProcessName_Primary */
                                SELECT
                                    sf.SyrupPlanningID,
                                    40 AS Seq,
                                    'PrimaryPacking' AS Source,
                                    sf.ProcessName_Primary AS ProcessName,
                                    sf.NextProcessName_Primary AS NextProcessName,
                                    COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0) AS DurationHours,
                                    CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level1) AS Manpower
                                FROM dbo.SyrupFilling sf
                                JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                WHERE sp.ProfileNo = @ProfileNo AND sf.ProcessName_Primary IS NOT NULL

                                UNION ALL

                                /* 5b. NextProcessName_Primary (Machine Filling) */
                                SELECT
                                    sf.SyrupPlanningID,
                                    41 AS Seq,
                                    'MachineFilling' AS Source,
                                    sf.NextProcessName_Primary AS ProcessName,
                                    sf.ProcessName_Secondary AS NextProcessName,
                                    COALESCE(sf.FillingHours_Level2, 0) AS DurationHours,
                                    CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level2, 0), 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level2) AS Manpower
                                FROM dbo.SyrupFilling sf
                                JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                WHERE sp.ProfileNo = @ProfileNo AND sf.NextProcessName_Primary IS NOT NULL

                                UNION ALL

                                /* 5c. ProcessName_Secondary */
                                SELECT
                                    sf.SyrupPlanningID,
                                    42 AS Seq,
                                    'SecondaryPacking' AS Source,
                                    sf.ProcessName_Secondary AS ProcessName,
                                    sf.NextProcessName_Secondary AS NextProcessName,
                                    CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                                         ELSE COALESCE(sf.SecondaryPackingHours,0) END AS DurationHours,
                                    CAST(ROUND(ISNULL(
                                        CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                                             ELSE COALESCE(sf.SecondaryPackingHours,0) END, 0)*60, 0) AS INT) AS DurationMinutes,
                                    TRY_CONVERT(decimal(18,4), sf.SecondaryManpower) AS Manpower
                                FROM dbo.SyrupFilling sf
                                JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                WHERE sp.ProfileNo = @ProfileNo AND sf.ProcessName_Secondary IS NOT NULL

                                UNION ALL

                                /* 5d. NextProcessName_Secondary (if you have one more step after secondary) */
                                SELECT
                                    sf.SyrupPlanningID,
                                    43 AS Seq,
                                    'SecondaryNext' AS Source,
                                    sf.NextProcessName_Secondary AS ProcessName,
                                    NULL AS NextProcessName,
                                    0 AS DurationHours,
                                    0 AS DurationMinutes,
                                    NULL AS Manpower
                                FROM dbo.SyrupFilling sf
                                JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                WHERE sp.ProfileNo = @ProfileNo AND sf.NextProcessName_Secondary IS NOT NULL
                            ),
                            Ordered AS
                            (
                                SELECT *,
                                       ROW_NUMBER() OVER (ORDER BY Seq) AS rn
                                FROM AllProcesses
                            ),
                            Timeline AS
                            (
                                /* make sure DurationMinutes is treated as 0 when NULL and compute cumulative minutes */
                                SELECT *,
                                       -- ensure DurationMinutes is not null
                                       COALESCE(DurationMinutes, 0) AS DurationMinutesNonNull,
                                       -- cumulative minutes from start to end of each process
                                       SUM(COALESCE(DurationMinutes, 0)) OVER (ORDER BY rn ROWS UNBOUNDED PRECEDING) AS CumMinutes
                                FROM Ordered
                            )
                            SELECT
                                -- Map to your TaskData shape expected by the Blazor Gantt
                                Timeline.rn           AS TaskId,                                -- unique id for task (rn)
                                Timeline.ProcessName  AS TaskName,                              -- label shown in grid
                                -- If DurationMinutes is NULL it was forced to 0 via DurationMinutesNonNull above
                                DATEADD(MINUTE, (Timeline.CumMinutes - Timeline.DurationMinutesNonNull), @StartDateTimeParam) AS StartDate,
                                DATEADD(MINUTE, Timeline.CumMinutes, @StartDateTimeParam) AS EndDate,
                                0 AS Progress,                       -- set to 0; change if you have actual progress metric
                                NULL AS Predecessor,                 -- set dependency string if you can map to TaskId(s)
                                NULL AS ParentId,                    -- set parent id if you want a tree structure
                                Timeline.DurationMinutesNonNull AS DurationMinutes, -- optional: duration in minutes (useful to debug)
                                COALESCE(Timeline.DurationHours, 0) AS DurationHours,
                                Timeline.Manpower,
                                nxt.ProcessName AS NextProcess_Timeline
                            FROM Timeline
                            LEFT JOIN Timeline nxt ON nxt.rn = Timeline.rn + 1
                            ORDER BY Timeline.rn";

            using var conn = CreateConnection();
            if (conn is SqlConnection sconn) await sconn.OpenAsync();
            else if (conn.State == ConnectionState.Closed) conn.Open();

            try
            {
                var rows = (await conn.QueryAsync(sqlTimeline, new
                {
                    ProfileNo = profileNo,
                    StartDateTimeParam = startDateTime
                })).ToList();

                var results = new List<TaskData>(rows.Count);

                //foreach (var row in rows)
                //{
                //    int taskId = row.TaskId is null ? 0 : Convert.ToInt32(row.TaskId);
                //    string? taskName = row.TaskName is null ? null : Convert.ToString(row.TaskName);

                //    DateTime? startDate = null;
                //    if (row.StartDate != null)
                //    {
                //        if (DateTime.TryParse(Convert.ToString(row.StartDate), out DateTime sd))
                //            startDate = sd;
                //    }

                //    DateTime? endDate = null;
                //    if (row.EndDate != null)
                //    {
                //        if (DateTime.TryParse(Convert.ToString(row.EndDate), out DateTime ed))
                //            endDate = ed;
                //    }

                //    int progress = row.Progress is null ? 0 : Convert.ToInt32(row.Progress);

                //    string? predecessor = row.Predecessor is null ? null : Convert.ToString(row.Predecessor);

                //    int? parentId = null;
                //    int pid = 0; // declare pid outside
                //    if (row.ParentId != null && int.TryParse(Convert.ToString(row.ParentId), out pid))
                //        parentId = pid;

                //    int durationMinutes = row.DurationMinutes is null ? 0 : Convert.ToInt32(row.DurationMinutes);
                //    decimal durationHours = row.DurationHours is null ? 0m : Convert.ToDecimal(row.DurationHours);
                //    decimal? manpower = row.Manpower is null ? null : (decimal?)Convert.ToDecimal(row.Manpower);
                //    string? nextProcess = row.NextProcess_Timeline is null ? null : Convert.ToString(row.NextProcess_Timeline);

                //    results.Add(new TaskData
                //    {
                //        TaskId = taskId,
                //        TaskName = taskName,
                //        StartDate = startDate,
                //        EndDate = endDate,
                //        Progress = progress,
                //        Predecessor = predecessor,
                //        ParentId = parentId
                //        // Add additional mappings if TaskData has DurationMinutes / Manpower fields
                //    });
                //}

                foreach (var row in rows)
                {
                    int taskId = row.TaskId is null ? 0 : Convert.ToInt32(row.TaskId);
                    string? taskName = row.TaskName is null ? null : Convert.ToString(row.TaskName);

                    DateTime? startDate = null;
                    DateTime? endDate = null;

                    // ---- Start Date ----
                    if (row.StartDate != null)
                    {
                        DateTime sd;
                        if (DateTime.TryParse(Convert.ToString(row.StartDate), out sd))
                            startDate = sd;
                    }

                    // ---- End Date ----
                    if (row.EndDate != null)
                    {
                        DateTime ed;
                        if (DateTime.TryParse(Convert.ToString(row.EndDate), out ed))
                            endDate = ed;
                    }

                    int durationMinutes = row.DurationMinutes is null ? 0 : Convert.ToInt32(row.DurationMinutes);

                    // --- ✅ Compute progress based on real elapsed time ---
                    int progress = 0;
                    DateTime now = DateTime.Now; // or DateTime.UtcNow if all times are UTC

                    if (startDate.HasValue)
                    {
                        if (durationMinutes <= 0)
                        {
                            progress = (now >= startDate.Value) ? 100 : 0;
                        }
                        else if (now <= startDate.Value)
                        {
                            progress = 0; // not started yet
                        }
                        else if (endDate.HasValue && now >= endDate.Value)
                        {
                            progress = 100; // fully completed
                        }
                        else
                        {
                            // ongoing task
                            var elapsedMinutes = (now - startDate.Value).TotalMinutes;
                            double fraction = elapsedMinutes / durationMinutes;
                            progress = (int)Math.Round(Math.Min(100, fraction * 100));
                        }
                    }

                    string? predecessor = row.Predecessor is null ? null : Convert.ToString(row.Predecessor);

                    int? parentId = null;
                    int pid = 0;
                    if (row.ParentId != null && int.TryParse(Convert.ToString(row.ParentId), out pid))
                        parentId = pid;

                    decimal durationHours = row.DurationHours is null ? 0m : Convert.ToDecimal(row.DurationHours);
                    decimal? manpower = row.Manpower is null ? null : (decimal?)Convert.ToDecimal(row.Manpower);
                    string? nextProcess = row.NextProcess_Timeline is null ? null : Convert.ToString(row.NextProcess_Timeline);

                    results.Add(new TaskData
                    {
                        TaskId = taskId,
                        TaskName = taskName,
                        StartDate = startDate,
                        EndDate = endDate,
                        Progress = progress, // ✅ Now correctly 0–100 scale
                        Predecessor = predecessor,
                        ParentId = parentId
                    });
                }


                return results.AsReadOnly();
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
    

    }
}
