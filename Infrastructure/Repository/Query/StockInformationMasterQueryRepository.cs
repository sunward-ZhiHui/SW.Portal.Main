using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using DevExpress.Xpo.DB;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Bibliography;
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
using System.Globalization;
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
        public async Task<SyrupPlanning?> SelectSyrupSimplexDataList(long? DynamicFormDataID)
        {
            const string sql = @"select DynamicFormDataItemID, DynamicFormDataID,ProfileNo,[13192_MethodCode_UId] as MethodCodeID,[13229_2003_ProductionPlanningProcess] as SyrupSimplexProcessName, [13229_1954_Location] as SyrupSimplexLocation,[13229_IsthereSyrupSimplextoproduce] as IsthereSyrupSimplextoproduce,
                    [13229_1955_1PreparationHour] as SyrupSimplexPreparationHour,
                    ISNULL(NULLIF([13229_1956_SyrupSimplexManpower], ''), 0) as SyrupSimplexManpower,
                    [13229_1957_Level2CleaningHours] as SyrupSimplexLevel2CleaningHours,
                    ISNULL(NULLIF([13229_1958_Level2CleaningManpower], ''), 0) as SyrupSimplexLevel2CleaningManpower, 
                    ISNULL(NULLIF([13229_2009_NoofCampaign], ''), 0) AS SyrupSimplexNoofCampaign,
                    [13229_2004_NextProcessName] as SyrupSimplexNextProcessName
                    from DynamicForm_ProductiontimingNMachineInfosyrup where DynamicFormDataID = @DynamicFormDataID";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SyrupPlanning>(sql, new { DynamicFormDataID = DynamicFormDataID });
        }


        public async Task<SyrupPlanning?> SelectSyruppreparationDataList(long? DynamicFormDataID)
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
                    from DynamicForm_ProductiontimingNMachineInfosyrup where DynamicFormDataID = @DynamicFormDataID";

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SyrupPlanning>(sql, new { DynamicFormDataID = DynamicFormDataID });
        }
        public async Task<IReadOnlyList<SyrupProcessNameList>> GetSyrupProcessNameList(long? DynamicFormDataID)
        {
            try
            {
                List<SyrupProcessNameList> aCItemsModels = new List<SyrupProcessNameList>();
                //var parameters = new DynamicParameters();

                //var query = @"SELECT dfs.DynamicFormSectionID AS ID, dfs.SectionName AS ProcessName FROM DynamicFormSection dfs WHERE dfs.DynamicFormID IN (    SELECT t2.DynamicFormID    FROM DynamicForm_ProductiontimingNMachineInfosyrup t1    INNER JOIN DynamicFormData t2 ON t2.DynamicFormDataID = t1.DynamicFormDataID)";
                var query = @"SELECT DISTINCT 
                                    t4.DynamicFormSectionID,
                                    t4.SectionName AS ProcessName
                                FROM DynamicForm_ProductiontimingNMachineInfosyrup t1
                                INNER JOIN DynamicFormData t2 
                                    ON t2.DynamicFormDataID = t1.DynamicFormDataID 
                                INNER JOIN DynamicFormSection t4 
                                    ON t4.DynamicFormID = t2.DynamicFormID
                                INNER JOIN DynamicFormSectionAttribute t7 
                                    ON t7.DynamicFormSectionID = t4.DynamicFormSectionID
                                INNER JOIN DynamicForm_ProdTimingSyrupPackingGrid t5 
                                    ON t5.DynamicFormDataGridId = t1.DynamicFormDataID
                                WHERE 
                                    t1.DynamicFormDataID = @DynamicFormDataID
                                    AND (
                                        t4.SectionName <> 'Other Process'
                                        OR EXISTS (
                                            SELECT 1 
                                            FROM DynamicForm_ProductionTimingSyrupOthers t6
                                            WHERE t6.DynamicFormDataGridId = t1.DynamicFormDataID
                                        )
                                    )";
                using (var connection = CreateConnection())
                {
                    aCItemsModels = (await connection.QueryAsync<SyrupProcessNameList>(query, new { DynamicFormDataID })).ToList();
                }

                return aCItemsModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<SyrupFilling>> GetSyrupFillingList(long? DynamicFormDataID)
        {
            try
            {
                var query = @"SELECT
                        1 AS SyrupPlanningID,
                        t2.DynamicFormID AS DynamicFormID,
                        t1.DynamicFormDataID AS DynamicFormDataID,
                        t1.DynamicFormDataItemID AS DynamicFormDataItemID,
                        t1.ProfileNo AS ProfileNo,

                        -- Primary Packing
                        t1.[13265_ProductionPlanningProcess] AS ProcessName_Primary,
                        t1.[13266_ProductionPlanningProcess] AS NextProcessName_Primary,
                        t1.[13230_TypeofPlanningProcess] AS PlanningType_Primary,

                        -- Machine Filling / Primary Filling
                        t1.[13213_Primaryfillingmachine] AS PrimaryFillingMachine,
                        t1.[13219_Level1hours] AS FillingHours_Level1,
                        t1.[13220_Level1manpower] AS FillingManpower_Level1,
                        t1.[13224_1FillingSpeedbottleminutes] AS Speed_BottlePerMinute,
                        t1.[13218_ChangePackingFillingHours] AS ChangePackingFillingHours,
                        t1.[13221_Level2hours] AS FillingHours_Level2,
                        t1.[13222_Level2Manpower] AS FillingManpower_Level2,
                        -- Secondary Packing
                        CAST(CASE WHEN LOWER(LTRIM(RTRIM(t1.[13256_SecondaryPackingTimeisthesameasPrimarypackingtime]))) = 'yes' THEN 1    ELSE 0  END AS bit) AS SecondarySameAsPrimaryTime,

                       TRY_CAST(NULLIF(NULLIF(LTRIM(RTRIM(REPLACE(t1.[13256_1944_SecondaryPackingHours], ',', ''))),'-'),'') AS decimal(18,4)) AS SecondaryPackingHours,
                        TRY_CAST(NULLIF(NULLIF(LTRIM(RTRIM(REPLACE(t1.[13256_1945_NoofManpower], ',', ''))),'-'),'') AS int) AS SecondaryManpower,
                        t1.[13267_ProductionPlanningProcess] AS ProcessName_Secondary,
                        t1.[13268_ProductionPlanningProcess] AS NextProcessName_Secondary,
                        t1.[13270_Syruprequireofflinepacking] AS RequireOfflinePacking,

                        -- Generic short-named fields (map to common properties)
                        t1.[13230_TypeofPlanningProcess] AS TypeOfPlanningProcess,
                        t1.[13223_1FillingHours] AS FillingHours,
                        t1.[13225_1FillingManpower] AS FillingManpower,

                        -- metadata
                        1 AS AddedByUserID,
                        '' AS Description,
                        t1.ModifiedBy AS ModifiedBy,
                        t1.ModifiedDate AS ModifiedDate

                    FROM DynamicForm_ProdTimingSyrupPackingGrid t1
                    INNER JOIN DynamicFormData t2 on t2.DynamicFormDataID = t1.DynamicFormDataID
                    WHERE t1.DynamicFormDataGridId = @DynamicFormDataID";

                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormDataID", DynamicFormDataID);

                using (var connection = CreateConnection())
                {
                    var list = (await connection.QueryAsync<SyrupFilling>(query, parameters)).ToList();
                    return list;
                }
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
                var query = @"SELECT    ISNULL(TRY_CAST(NULLIF(t1.DynamicFormDataItemID, '') AS BIGINT), 0) AS DynamicFormDataItemID,
    ISNULL(TRY_CAST(NULLIF(t1.DynamicFormDataID, '') AS BIGINT), 0) AS DynamicFormDataID,
    t1.ProfileNo,

    -- Method code/name/id (keep as strings except UId which might be numeric)
    t1.[13192_MethodCode] AS MethodName,
    t1.[13192_MethodCode] AS MethodCode,
    ISNULL(TRY_CAST(NULLIF(t1.[13192_MethodCode_UId], '') AS BIGINT), 0) AS MethodCodeID,

    -- Batch size (decimal)
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13193_BatchSizeL], '-', ''), '') AS DECIMAL(18,2)), 0) AS BatchSizeInLiters,

    t1.[13262_RestrictiononPlanningday] AS RestrictionOnPlanningDay,
    t1.[13229_IsthereSyrupSimplextoproduce] AS IsthereSyrupSimplextoproduce,
    t1.[13229_2003_ProcessName] AS ProcessName,
    t1.[13202_Location] AS SyrupSimplexLocation,

    -- Preparation per hour (decimal) — remove hyphen or empty and try cast
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13229_1955_1PreparationHour], '-', ''), '') AS DECIMAL(18,2)), 0) AS PreparationPerHour,

    -- SyrupSimplexManpower (int) — handle empty/non-numeric
    ISNULL(TRY_CAST(NULLIF(t1.[13229_1956_SyrupSimplexManpower], '') AS INT), 0) AS SyrupSimplexManpower,

    -- Level2 cleaning hours & manpower for the top-level section
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13211_Level2Cleaninghours], '-', ''), '') AS DECIMAL(18,2)), 0) AS Level2CleaningHours,
    ISNULL(TRY_CAST(NULLIF(t1.[13212_Level2CleaningManpower], '') AS INT), 0) AS Level2CleaningManpower,

    t1.[13229_2009_NoofCampaign] AS NoOfCampaign,
    t1.[13229_2004_NextProcessName] AS NextProcessName,

    t1.[13229_1954_Location] AS SyrupPreparationLocation,

    -- Preparation first volume per hour (decimal) and manpower (int)
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13203_1PreparationfirstVolumnHour], '-', ''), '') AS DECIMAL(18,2)), 0) AS PreparationFirstVolumePerHour,
    ISNULL(TRY_CAST(NULLIF(t1.[13204_1PreparationFirstVolumnManpower], '') AS INT), 0) AS PreparationFirstVolumeManpower,

    -- IPQC test -> boolean mapping (common values: '1','0','Yes','No','Y','N')
    CASE
        WHEN LOWER(NULLIF(t1.[13205_2IPQCtest], '')) IN ('1','true','t','yes','y') THEN 1
        ELSE 0
    END AS IpqcTestRequired,

    -- Preparation top up per hour / manpower
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13206_3PreparationTopuptoVolumnHour], '-', ''), '') AS DECIMAL(18,2)), 0) AS PreparationTopUpPerHour,
    ISNULL(TRY_CAST(NULLIF(t1.[13207_3PreparationTopuptoVolumnManpower], '') AS INT), 0) AS PreparationTopUpManpower,

    t1.[13208_CampaignBatchesNumbers] AS CampaignBatches,

    -- Level1 cleaning hours & manpower (decimal / int)
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13209_Level1CleaningHours], '-', ''), '') AS DECIMAL(18,2)), 0) AS Level1CleaningHours,
    ISNULL(TRY_CAST(NULLIF(t1.[13210_Level1Cleaningmanpower], '') AS INT), 0) AS Level1CleaningManpower,

    -- The problematic column: SyrupLevel2CleaningHours -> safe cast (DECIMAL)
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13229_1957_Level2CleaningHours], '-', ''), '') AS DECIMAL(18,2)), 0) AS SyrupLevel2CleaningHours,

    -- SyrupLevel2CleaningManpower (int)
    ISNULL(TRY_CAST(NULLIF(t1.[13229_1958_Level2CleaningManpower], '') AS INT), 0) AS SyrupLevel2CleaningManpower,

    -- Next process after preparation
    t1.[13229_2004_NextProcessName] AS NextProcessNameAfterPreparation

FROM DynamicForm_ProductiontimingNMachineInfosyrup t1";
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
                var query = @"SELECT    ISNULL(TRY_CAST(NULLIF(t1.DynamicFormDataItemID, '') AS BIGINT), 0) AS DynamicFormDataItemID,
    ISNULL(TRY_CAST(NULLIF(t1.DynamicFormDataID, '') AS BIGINT), 0) AS DynamicFormDataID,
    t1.ProfileNo,

    -- Method code/name/id (keep as strings except UId which might be numeric)
    t1.[13192_MethodCode] AS MethodName,
    t1.[13192_MethodCode] AS MethodCode,
    ISNULL(TRY_CAST(NULLIF(t1.[13192_MethodCode_UId], '') AS BIGINT), 0) AS MethodCodeID,

    -- Batch size (decimal)
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13193_BatchSizeL], '-', ''), '') AS DECIMAL(18,2)), 0) AS BatchSizeInLiters,

    t1.[13262_RestrictiononPlanningday] AS RestrictionOnPlanningDay,
    t1.[13229_IsthereSyrupSimplextoproduce] AS IsthereSyrupSimplextoproduce,
    t1.[13229_2003_ProcessName] AS ProcessName,
    t1.[13202_Location] AS SyrupSimplexLocation,

    -- Preparation per hour (decimal) — remove hyphen or empty and try cast
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13229_1955_1PreparationHour], '-', ''), '') AS DECIMAL(18,2)), 0) AS PreparationPerHour,

    -- SyrupSimplexManpower (int) — handle empty/non-numeric
    ISNULL(TRY_CAST(NULLIF(t1.[13229_1956_SyrupSimplexManpower], '') AS INT), 0) AS SyrupSimplexManpower,

    -- Level2 cleaning hours & manpower for the top-level section
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13211_Level2Cleaninghours], '-', ''), '') AS DECIMAL(18,2)), 0) AS Level2CleaningHours,
    ISNULL(TRY_CAST(NULLIF(t1.[13212_Level2CleaningManpower], '') AS INT), 0) AS Level2CleaningManpower,

    t1.[13229_2009_NoofCampaign] AS NoOfCampaign,
    t1.[13229_2004_NextProcessName] AS NextProcessName,

    t1.[13229_1954_Location] AS SyrupPreparationLocation,

    -- Preparation first volume per hour (decimal) and manpower (int)
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13203_1PreparationfirstVolumnHour], '-', ''), '') AS DECIMAL(18,2)), 0) AS PreparationFirstVolumePerHour,
    ISNULL(TRY_CAST(NULLIF(t1.[13204_1PreparationFirstVolumnManpower], '') AS INT), 0) AS PreparationFirstVolumeManpower,

    -- IPQC test -> boolean mapping (common values: '1','0','Yes','No','Y','N')
    CASE
        WHEN LOWER(NULLIF(t1.[13205_2IPQCtest], '')) IN ('1','true','t','yes','y') THEN 1
        ELSE 0
    END AS IpqcTestRequired,

    -- Preparation top up per hour / manpower
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13206_3PreparationTopuptoVolumnHour], '-', ''), '') AS DECIMAL(18,2)), 0) AS PreparationTopUpPerHour,
    ISNULL(TRY_CAST(NULLIF(t1.[13207_3PreparationTopuptoVolumnManpower], '') AS INT), 0) AS PreparationTopUpManpower,

    t1.[13208_CampaignBatchesNumbers] AS CampaignBatches,

    -- Level1 cleaning hours & manpower (decimal / int)
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13209_Level1CleaningHours], '-', ''), '') AS DECIMAL(18,2)), 0) AS Level1CleaningHours,
    ISNULL(TRY_CAST(NULLIF(t1.[13210_Level1Cleaningmanpower], '') AS INT), 0) AS Level1CleaningManpower,

    -- The problematic column: SyrupLevel2CleaningHours -> safe cast (DECIMAL)
    ISNULL(TRY_CAST(NULLIF(REPLACE(t1.[13229_1957_Level2CleaningHours], '-', ''), '') AS DECIMAL(18,2)), 0) AS SyrupLevel2CleaningHours,

    -- SyrupLevel2CleaningManpower (int)
    ISNULL(TRY_CAST(NULLIF(t1.[13229_1958_Level2CleaningManpower], '') AS INT), 0) AS SyrupLevel2CleaningManpower,

    -- Next process after preparation
    t1.[13229_2004_NextProcessName] AS NextProcessNameAfterPreparation

FROM DynamicForm_ProductiontimingNMachineInfosyrup t1";
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
        public async Task<IReadOnlyList<ProcessStepDto>> GetProcessFlowByProfileNoAsync(long DynamicFormDataID, DateTime productionDay, TimeSpan shiftStart, int? weekOfMonth = null, int? month = null, int? year = null)
        {
            if (DynamicFormDataID == 0)
                return Array.Empty<ProcessStepDto>();

            var startDateTime = productionDay.Date + shiftStart;

            const string sql = @"WITH AllProcesses AS(
                    /* 1. Other Process */
                    SELECT 
                        op.SyrupPlanningID,
                        10 AS Seq,
                        'OtherProcess' AS Source,
                        op.ProcessName AS ProcessName,
                        op.LocationOfProcess AS Room,
                        op.SyrupOtherProcessNextProcess AS NextProcessName,
                        TRY_CONVERT(decimal(18,4), op.ManhoursOrHours) AS DurationHours,
                        CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), op.ManhoursOrHours), 0)*60, 0) AS INT) AS DurationMinutes,
                        TRY_CONVERT(decimal(18,4), op.NoOfManpower) AS Manpower
                    FROM dbo.SyrupOtherProcess op
                    INNER JOIN dbo.SyrupPlanning sp on sp.SyrupPlanningID = op.SyrupPlanningID
                    WHERE op.DynamicFormDataID = @DynamicFormDataID
                      AND ISNULL(LTRIM(RTRIM(op.ProcessName)), '') <> ''  
	                  AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
                      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
                      AND (@YearParam IS NULL OR sp.Year = @YearParam)


                    UNION ALL

                    /* 2. Syrup Simplex */
                    SELECT
                        sp.SyrupPlanningID,
                        20 AS Seq,
                        'SyrupSimplex' AS Source,
                        sp.SyrupSimplexProcessName AS ProcessName,
                        sp.SyrupSimplexLocation AS Room,
                        sp.SyrupSimplexNextProcessName AS NextProcessName,
                        TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')) AS DurationHours,
                        CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')), 0)*60, 0) AS INT) AS DurationMinutes,
                        TRY_CONVERT(decimal(18,4), sp.SyrupSimplexManpower) AS Manpower
                    FROM dbo.SyrupPlanning sp
                    WHERE sp.DynamicFormDataID = @DynamicFormDataID
                      AND ISNULL(LTRIM(RTRIM(sp.SyrupSimplexProcessName)), '') <> ''
					    AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
                        AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
                        AND (@YearParam IS NULL OR sp.Year = @YearParam)


                    UNION ALL

                    /* 3. Syrup Simplex - Level2 Cleaning */
                    SELECT
                        sp.SyrupPlanningID,
                        21 AS Seq,
                        'SyrupSimplexLevel2Cleaning' AS Source,
                        CONCAT(sp.SyrupSimplexProcessName, ' - Level2 Cleaning') AS ProcessName,
                        sp.SyruppreparationLocation AS Room,
                        sp.SyrupSimplexNextProcessName AS NextProcessName,
                        TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours) AS DurationHours,
                        CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours), 0)*60, 0) AS INT) AS DurationMinutes,
                        TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningManpower) AS Manpower
                    FROM dbo.SyrupPlanning sp
                    WHERE sp.DynamicFormDataID = @DynamicFormDataID
                      AND ISNULL(LTRIM(RTRIM(sp.SyrupSimplexProcessName)), '') <> ''
					  AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
                      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
                      AND (@YearParam IS NULL OR sp.Year = @YearParam)

                    UNION ALL

                    /* 4. Syrup Preparation - Mixing */
                    SELECT
                        sp.SyrupPlanningID,
                        30 AS Seq,
                        'SyrupPreparation' AS Source,
                        sp.SyruppreparationProcessName AS ProcessName,
                        sp.SyruppreparationLocation AS Room,
                        sp.SyruppreparationNextProcessName AS NextProcessName,
                        TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')) AS DurationHours,
                        CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')), 0)*60, 0) AS INT) AS DurationMinutes,
                        TRY_CONVERT(decimal(18,4), sp.SyruppreparationFirstVolumnManpower) AS Manpower
                    FROM dbo.SyrupPlanning sp
                    WHERE sp.DynamicFormDataID = @DynamicFormDataID
                      AND ISNULL(LTRIM(RTRIM(sp.SyruppreparationProcessName)), '') <> ''
					  AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
                      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
                      AND (@YearParam IS NULL OR sp.Year = @YearParam)

                    /* 5. Syrup Filling — include each process column separately */
                    UNION ALL

                    /* 5a. ProcessName_Primary */
                    SELECT
                        sf.SyrupPlanningID,
                        40 AS Seq,
                        'PrimaryPacking' AS Source,
                        sf.ProcessName_Primary AS ProcessName,
                        NULL AS Room,
                        sf.NextProcessName_Primary AS NextProcessName,
                        COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0) AS DurationHours,
                        CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0), 0)*60, 0) AS INT) AS DurationMinutes,
                        TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level1) AS Manpower
                    FROM dbo.SyrupFilling sf
                    JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                    WHERE sp.DynamicFormDataID = @DynamicFormDataID
                      AND ISNULL(LTRIM(RTRIM(sf.ProcessName_Primary)), '') <> ''
					  AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
                      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
                      AND (@YearParam IS NULL OR sp.Year = @YearParam)

                    UNION ALL

                    /* 5b. NextProcessName_Primary (Machine Filling) */
                    SELECT
                        sf.SyrupPlanningID,
                        41 AS Seq,
                        'MachineFilling' AS Source,
                        sf.NextProcessName_Primary AS ProcessName,
                        NULL AS Room,
                        sf.ProcessName_Secondary AS NextProcessName,
                        COALESCE(sf.FillingHours_Level2, 0) AS DurationHours,
                        CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level2, 0), 0)*60, 0) AS INT) AS DurationMinutes,
                        TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level2) AS Manpower
                    FROM dbo.SyrupFilling sf
                    JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                    WHERE sp.DynamicFormDataID = @DynamicFormDataID
                      AND ISNULL(LTRIM(RTRIM(sf.NextProcessName_Primary)), '') <> ''
					  AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
                      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
                      AND (@YearParam IS NULL OR sp.Year = @YearParam)

                    UNION ALL

                    /* 5c. ProcessName_Secondary */
                    SELECT
                        sf.SyrupPlanningID,
                        42 AS Seq,
                        'SecondaryPacking' AS Source,
                        sf.ProcessName_Secondary AS ProcessName,
                        NULL AS Room,
                        sf.NextProcessName_Secondary AS NextProcessName,
                        CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                             ELSE COALESCE(sf.SecondaryPackingHours,0) END AS DurationHours,
                        CAST(ROUND(ISNULL(
                            CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                                 ELSE COALESCE(sf.SecondaryPackingHours,0) END, 0)*60, 0) AS INT) AS DurationMinutes,
                        TRY_CONVERT(decimal(18,4), sf.SecondaryManpower) AS Manpower
                    FROM dbo.SyrupFilling sf
                    JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                    WHERE sp.DynamicFormDataID = @DynamicFormDataID
                      AND ISNULL(LTRIM(RTRIM(sf.ProcessName_Secondary)), '') <> ''
					  AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
                      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
                      AND (@YearParam IS NULL OR sp.Year = @YearParam)

                    UNION ALL

                    /* 5d. NextProcessName_Secondary (if you have one more step after secondary) */
                    SELECT
                        sf.SyrupPlanningID,
                        43 AS Seq,
                        'SecondaryNext' AS Source,
                        sf.NextProcessName_Secondary AS ProcessName,
                        NULL AS Room,
                        NULL AS NextProcessName,
                        0 AS DurationHours,
                        0 AS DurationMinutes,
                        NULL AS Manpower
                    FROM dbo.SyrupFilling sf
                    JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                    WHERE sp.DynamicFormDataID = @DynamicFormDataID
                      AND ISNULL(LTRIM(RTRIM(sf.NextProcessName_Secondary)), '') <> ''
					  AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
                      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
                      AND (@YearParam IS NULL OR sp.Year = @YearParam)
                ),
                Ordered AS
                (
                    SELECT *,
                           ROW_NUMBER() OVER (ORDER BY Seq) AS rn
                    FROM AllProcesses
                ),
                Timeline AS
                (
                    -- make sure DurationMinutes is treated as 0 when NULL and compute cumulative minutes
                    SELECT *,
                           COALESCE(DurationMinutes, 0) AS DurationMinutesNonNull,
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
                WHERE EXISTS (SELECT 1 FROM Timeline)
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
                    DynamicFormDataID = DynamicFormDataID,
                    StartDateTimeParam = startDateTime,
                    WeekOfMonthParam = weekOfMonth,
                    MonthParam = month,
                    YearParam = year
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
        public async Task<bool> CheckSyrupOtherProcessExists(long DynamicFormDataID)
        {
            try
            {
                var query = @"SELECT COUNT(1) FROM DynamicForm_ProductionTimingSyrupOthers WHERE DynamicFormDataGridId = @DynamicFormDataID";

                var parameters = new DynamicParameters();
                parameters.Add("@DynamicFormDataID", DynamicFormDataID, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    var count = await connection.ExecuteScalarAsync<int>(query, parameters);
                    return count > 0;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<(List<GanttTaskDto> Tasks, List<GanttSegmentDto> Segments)> GetGanttTasksAndSegmentsByProfileNoAsync(string profileNo, DateTime productionDay, TimeSpan shiftStart)
        {
            if (string.IsNullOrWhiteSpace(profileNo))
                return (new List<GanttTaskDto>(), new List<GanttSegmentDto>());

            var startDateTime = productionDay.Date + shiftStart;

            // SQL returns two resultsets: first Tasks, then Segments
            const string sql = @"
                        WITH AllProcesses AS
                        (
                            /* 1. Other Process */
                            SELECT 
                                sp.MethodCodeID,
                                sp.MethodName,
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
                            LEFT JOIN dbo.SyrupPlanning sp ON sp.SyrupPlanningID = op.SyrupPlanningID
                            WHERE op.ProfileNo = @ProfileNo AND ISNULL(LTRIM(RTRIM(op.ProcessName)), '') <> ''

                            UNION ALL

                            /* 2. Syrup Simplex */
                            SELECT
                                sp.MethodCodeID,
                                sp.MethodName,
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
                            WHERE sp.ProfileNo = @ProfileNo AND ISNULL(LTRIM(RTRIM(sp.SyrupSimplexProcessName)), '') <> ''

                            UNION ALL

                            /* 3. Syrup Simplex - Level2 Cleaning */
                            SELECT
                                sp.MethodCodeID,
                                sp.MethodName,
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
                            WHERE sp.ProfileNo = @ProfileNo AND ISNULL(LTRIM(RTRIM(sp.SyrupSimplexProcessName)), '') <> ''

                            UNION ALL

                            /* 4. Syrup Preparation - Mixing */
                            SELECT
                                sp.MethodCodeID,
                                sp.MethodName,
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
                            WHERE sp.ProfileNo = @ProfileNo AND ISNULL(LTRIM(RTRIM(sp.SyruppreparationProcessName)), '') <> ''

                            UNION ALL

                            /* 5a. PrimaryPacking */
                            SELECT
                                sp.MethodCodeID,
                                sp.MethodName,
                                sf.SyrupPlanningID,
                                40 AS Seq,
                                'PrimaryPacking' AS Source,
                                sf.ProcessName_Primary AS ProcessName,
                                NULL as Room,
                                sf.NextProcessName_Primary AS NextProcessName,
                                COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0) AS DurationHours,
                                CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0), 0)*60, 0) AS INT) AS DurationMinutes,
                                TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level1) AS Manpower
                            FROM dbo.SyrupFilling sf
                            JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                            WHERE sp.ProfileNo = @ProfileNo AND ISNULL(LTRIM(RTRIM(sf.ProcessName_Primary)), '') <> ''

                            UNION ALL

                            /* 5b. MachineFilling */
                            SELECT
                                sp.MethodCodeID,
                                sp.MethodName,
                                sf.SyrupPlanningID,
                                41 AS Seq,
                                'MachineFilling' AS Source,
                                sf.NextProcessName_Primary AS ProcessName,
                                NULL as Room,
                                sf.ProcessName_Secondary AS NextProcessName,
                                COALESCE(sf.FillingHours_Level2, 0) AS DurationHours,
                                CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level2, 0), 0)*60, 0) AS INT) AS DurationMinutes,
                                TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level2) AS Manpower
                            FROM dbo.SyrupFilling sf
                            JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                            WHERE sp.ProfileNo = @ProfileNo AND ISNULL(LTRIM(RTRIM(sf.NextProcessName_Primary)), '') <> ''

                            UNION ALL

                            /* 5c. SecondaryPacking */
                            SELECT
                                sp.MethodCodeID,
                                sp.MethodName,
                                sf.SyrupPlanningID,
                                42 AS Seq,
                                'SecondaryPacking' AS Source,
                                sf.ProcessName_Secondary AS ProcessName,
                                NULL as Room,
                                sf.NextProcessName_Secondary AS NextProcessName,
                                CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                                     ELSE COALESCE(sf.SecondaryPackingHours,0) END AS DurationHours,
                                CAST(ROUND(ISNULL(
                                    CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                                         ELSE COALESCE(sf.SecondaryPackingHours,0) END, 0)*60, 0) AS INT) AS DurationMinutes,
                                TRY_CONVERT(decimal(18,4), sf.SecondaryManpower) AS Manpower
                            FROM dbo.SyrupFilling sf
                            JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                            WHERE sp.ProfileNo = @ProfileNo AND ISNULL(LTRIM(RTRIM(sf.ProcessName_Secondary)), '') <> ''

                        ),
                        -- ordered rows within each plan
                        OrderedPerPlan AS
                        (
                            SELECT *,
                                   ROW_NUMBER() OVER (PARTITION BY MethodCodeID, SyrupPlanningID ORDER BY Seq, ProcessName) AS rn_within_plan,
                                   COALESCE(DurationMinutes, 0) AS DurationMinutesNonNull
                            FROM AllProcesses
                        ),
                        TimelinePerPlan AS
                        (
                            SELECT *,
                                   SUM(DurationMinutesNonNull) OVER (PARTITION BY MethodCodeID, SyrupPlanningID ORDER BY rn_within_plan
                                                                    ROWS UNBOUNDED PRECEDING) AS CumMinutesPerPlan
                            FROM OrderedPerPlan
                        ),
                        -- Methods list and ranks
                        MethodList AS
                        (
                            SELECT DISTINCT MethodCodeID, MethodName,
                                   ROW_NUMBER() OVER (ORDER BY MethodCodeID) AS MethodRank
                            FROM AllProcesses
                        ),
                        -- Plans list and ranks within method
                        PlanList AS
                        (
                            SELECT DISTINCT a.MethodCodeID, a.MethodName, a.SyrupPlanningID,
                                   ROW_NUMBER() OVER (PARTITION BY a.MethodCodeID ORDER BY a.SyrupPlanningID) AS PlanRank
                            FROM AllProcesses a
                        ),
                        -- Compose parents and children with deterministic integer IDs
                        MethodRows AS
                        (
                            SELECT
                                (ml.MethodRank * 1000000) AS TaskId,
                                ml.MethodName AS TaskName,
                                NULL AS StartDate,
                                NULL AS EndDate,
                                0 AS Progress,
                                CAST(NULL AS INT) AS Predecessor,
                                CAST(NULL AS INT) AS ParentId,
                                0 AS DurationMinutes,
                                0.0 AS DurationHours,
                                NULL AS Manpower,
                                NULL AS Room,
                                NULL AS NextProcessName,
                                NULL AS rn_within_plan,
                                ml.MethodCodeID,
                                ml.MethodName AS MethodName2,
                                ml.MethodRank,
                                NULL AS SyrupPlanningID
                            FROM MethodList ml
                        ),
                        PlanRows AS
                        (
                            SELECT
                                (pl.PlanRank + (ml.MethodRank * 100000)) AS TaskId,  -- MethodRank*100000 + PlanRank
                                CONCAT('SyrupPlanning ', pl.SyrupPlanningID) AS TaskName,
                                DATEADD(MINUTE, MIN(tp.CumMinutesPerPlan - tp.DurationMinutesNonNull), @StartDateTimeParam) AS StartDate,
                                DATEADD(MINUTE, MAX(tp.CumMinutesPerPlan), @StartDateTimeParam) AS EndDate,
                                0 AS Progress,
                                NULL AS Predecessor,
                                (ml.MethodRank * 100000) AS ParentId, -- reference method's TaskId
                                SUM(tp.DurationMinutesNonNull) AS DurationMinutes,
                                SUM(tp.DurationHours) AS DurationHours,
                                MAX(tp.Manpower) AS Manpower,
                                MAX(tp.Room) AS Room,
                                NULL AS NextProcessName,
                                NULL AS rn_within_plan,
                                ml.MethodCodeID,
                                ml.MethodName AS MethodName2,
                                ml.MethodRank,
                                pl.SyrupPlanningID
                            FROM PlanList pl
                            INNER JOIN MethodList ml ON ml.MethodCodeID = pl.MethodCodeID
                            LEFT JOIN TimelinePerPlan tp ON tp.SyrupPlanningID = pl.SyrupPlanningID AND tp.MethodCodeID = pl.MethodCodeID
                            GROUP BY ml.MethodRank, ml.MethodCodeID, ml.MethodName, pl.PlanRank, pl.SyrupPlanningID
                        ),
                        ChildRows AS
                        (
                            SELECT
                                ((ml.MethodRank * 100000) + pl.PlanRank) * 1000 + tp.rn_within_plan AS TaskId, -- PlanTaskId*1000 + child index
                                tp.ProcessName AS TaskName,
                                DATEADD(MINUTE, (tp.CumMinutesPerPlan - tp.DurationMinutesNonNull), @StartDateTimeParam) AS StartDate,
                                DATEADD(MINUTE, tp.CumMinutesPerPlan, @StartDateTimeParam) AS EndDate,
                                0 AS Progress,
                                NULL AS Predecessor,
                                ((ml.MethodRank * 100000) + pl.PlanRank) AS ParentId, -- PlanTaskId
                                tp.DurationMinutesNonNull AS DurationMinutes,
                                COALESCE(tp.DurationHours, 0) AS DurationHours,
                                tp.Manpower,
                                tp.Room,
                                tp.NextProcessName,
                                tp.rn_within_plan,
                                ml.MethodCodeID,
                                ml.MethodName AS MethodName2,
                                ml.MethodRank,
                                tp.SyrupPlanningID
                            FROM TimelinePerPlan tp
                            INNER JOIN PlanList pl ON pl.SyrupPlanningID = tp.SyrupPlanningID AND pl.MethodCodeID = tp.MethodCodeID
                            INNER JOIN MethodList ml ON ml.MethodCodeID = tp.MethodCodeID
                        )
                        -- Output two resultsets:
                        -- 1) Tasks (Methods, Plans, Children)
                        SELECT TaskId, TaskName, StartDate, EndDate,
                               0 AS Progress,
                               CAST(NULL AS VARCHAR(200)) AS Predecessor,
                               ParentId,
                               CAST(DurationMinutes AS VARCHAR(50)) AS Duration,
                               CAST(ROUND(ISNULL(DurationHours,0),2) AS VARCHAR(50)) AS DurationHours,
                               CAST(Progress AS INT) AS ProgressInt,
                               SyrupPlanningID,
                               MethodCodeID,
                               MethodName2 AS MethodName
                        FROM MethodRows

                        UNION ALL

                        SELECT TaskId, TaskName, StartDate, EndDate,
                               Progress, CAST(NULL AS VARCHAR(200)) AS Predecessor, ParentId,
                               CAST(DurationMinutes AS VARCHAR(50)) AS Duration, CAST(DurationHours AS VARCHAR(50)) AS DurationHours,
                               0 AS ProgressInt, SyrupPlanningID, MethodCodeID, MethodName2
                        FROM PlanRows

                        UNION ALL

                        SELECT TaskId, TaskName, StartDate, EndDate,
                               Progress, CAST(NULL AS VARCHAR(200)) AS Predecessor, ParentId,
                               CAST(DurationMinutes AS VARCHAR(50)) AS Duration, CAST(DurationHours AS VARCHAR(50)) AS DurationHours,
                               0 AS ProgressInt, SyrupPlanningID, MethodCodeID, MethodName2
                        FROM ChildRows
                        ORDER BY MethodCodeID, ParentId, TaskId;

                        -- 2) Segments: for now create one segment per child equal to child StartDate/EndDate
                        SELECT ROW_NUMBER() OVER (ORDER BY ((ml.MethodRank * 100000) + pl.PlanRank) * 1000 + tp.rn_within_plan) AS id,
                               ((ml.MethodRank * 100000) + pl.PlanRank) * 1000 + tp.rn_within_plan AS TaskId,
                               DATEADD(MINUTE, (tp.CumMinutesPerPlan - tp.DurationMinutesNonNull), @StartDateTimeParam) AS StartDate,
                               DATEADD(MINUTE, tp.CumMinutesPerPlan, @StartDateTimeParam) AS EndDate,
                               CAST(tp.DurationMinutesNonNull AS VARCHAR(50)) AS Duration
                        FROM TimelinePerPlan tp
                        INNER JOIN PlanList pl ON pl.SyrupPlanningID = tp.SyrupPlanningID AND pl.MethodCodeID = tp.MethodCodeID
                        INNER JOIN MethodList ml ON ml.MethodCodeID = tp.MethodCodeID
                        ORDER BY TaskId";

            using var conn = CreateConnection();
            if (conn is SqlConnection sqlConn)
                await sqlConn.OpenAsync();
            else
                conn.Open();

            try
            {
                using var multi = await conn.QueryMultipleAsync(sql, new
                {
                    ProfileNo = profileNo,
                    StartDateTimeParam = startDateTime
                });

                // Read Tasks (first resultset)
                var tasksRaw = (await multi.ReadAsync()).ToList();

                // Map raw objects to strongly typed DTOs
                var tasks = new List<GanttTaskDto>();
                foreach (var r in tasksRaw)
                {
                    // dynamic mapping
                    var dict = (IDictionary<string, object?>)r;
                    tasks.Add(new GanttTaskDto
                    {
                        TaskId = Convert.ToInt32(dict["TaskId"]),
                        TaskName = dict["TaskName"]?.ToString() ?? string.Empty,
                        StartDate = dict["StartDate"] as DateTime?,
                        EndDate = dict["EndDate"] as DateTime?,
                        Duration = dict["Duration"]?.ToString(),
                        Progress = dict.ContainsKey("ProgressInt") && dict["ProgressInt"] != null ? Convert.ToInt32(dict["ProgressInt"]) : 0,
                        ParentId = dict["ParentId"] != null ? (int?)Convert.ToInt32(dict["ParentId"]) : null,
                        Predecessor = dict["Predecessor"]?.ToString(),
                        SyrupPlanningID = dict.ContainsKey("SyrupPlanningID") && dict["SyrupPlanningID"] != null ? Convert.ToInt32(dict["SyrupPlanningID"]) : (int?)null,
                        MethodCodeID = dict.ContainsKey("MethodCodeID") && dict["MethodCodeID"] != null ? Convert.ToInt32(dict["MethodCodeID"]) : (int?)null,
                        MethodName = dict["MethodName"]?.ToString()
                    });
                }

                // Read Segments (second resultset)
                var segmentsRaw = (await multi.ReadAsync()).ToList();
                var segments = new List<GanttSegmentDto>();
                foreach (var r in segmentsRaw)
                {
                    var dict = (IDictionary<string, object?>)r;
                    segments.Add(new GanttSegmentDto
                    {
                        id = Convert.ToInt32(dict["id"]),
                        TaskId = Convert.ToInt32(dict["TaskId"]),
                        StartDate = Convert.ToDateTime(dict["StartDate"]),
                        EndDate = dict["EndDate"] as DateTime?,
                        Duration = dict["Duration"]?.ToString()
                    });
                }

                return (tasks, segments);
            }
            finally
            {
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
        private int GetWeekOfMonth(DateTime selectedDate)
        {
            //DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            //int firstDayOffset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7; // Monday = 0
            //int weekOfMonth = ((selectedDate.Day + firstDayOffset - 1) / 7) + 1;
            //return weekOfMonth;

            var cultureInfo = CultureInfo.CurrentCulture;
            var calendar = cultureInfo.Calendar;

            return calendar.GetWeekOfYear(selectedDate,cultureInfo.DateTimeFormat.CalendarWeekRule,cultureInfo.DateTimeFormat.FirstDayOfWeek);
        }
        public async Task<SyrupPlanning> InsertOrUpdateSyrupPlanningAsync(SyrupPlanning model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            using var conn = CreateConnection();
            if (conn is SqlConnection sConn)
                await sConn.OpenAsync();
            else
                conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                DateTime planningDate = model.AddedDate != default ? model.AddedDate : DateTime.Now;

                int weekOfMonth = GetWeekOfMonth(planningDate);              
                int month = planningDate.Month;
                int year = planningDate.Year;


                var existingIds = new List<long>();

                //var existingWeekIds = new List<long>();
                //if (model.DynamicFormDataID > 0 && model.DynamicFormDataItemID > 0)
                //{

                //    const string checkWeekSql = @"SELECT SyrupPlanningID FROM dbo.SyrupPlanning WHERE DynamicFormDataID = @DynamicFormDataID
                //  AND DynamicFormDataItemID = @DynamicFormDataItemID
                //  AND WeekOfMonth = @WeekOfMonth
                //  AND [Month] = @Month
                //  AND [Year] = @Year";

                //    existingWeekIds = (await conn.QueryAsync<long>(checkWeekSql, new
                //    {
                //        model.DynamicFormDataID,
                //        model.DynamicFormDataItemID,
                //        WeekOfMonth = weekOfMonth,
                //        Month = month,
                //        Year = year
                //    }, transaction: tran)).ToList();                   

                //    if (existingWeekIds.Count > 0)
                //    {
                //        const string deleteFillingSql = @"DELETE FROM dbo.SyrupFilling WHERE SyrupPlanningID IN @Ids";
                //        int deletedFilling = await conn.ExecuteAsync(deleteFillingSql, new { Ids = existingWeekIds }, transaction: tran);

                //        const string deleteOtherProcessSql = @"DELETE FROM dbo.SyrupOtherProcess WHERE SyrupPlanningID IN @Ids";
                //        int deletedOther = await conn.ExecuteAsync(deleteOtherProcessSql, new { Ids = existingWeekIds }, transaction: tran);

                //        const string deleteParentSql = @"DELETE FROM dbo.SyrupPlanning WHERE DynamicFormDataID = @DynamicFormDataID AND DynamicFormDataItemID = @DynamicFormDataItemID;";
                //        int deletedParents = await conn.ExecuteAsync(deleteParentSql, new
                //        {
                //            model.DynamicFormDataID,
                //            model.DynamicFormDataItemID
                //        }, transaction: tran);
                //    }
                //}

                var existingWeekIds = new List<long>();
                if (model.DynamicFormDataID > 0 && model.DynamicFormDataItemID > 0)
                {
                    const string checkWeekSql = @"SELECT SyrupPlanningID FROM dbo.SyrupPlanning WHERE DynamicFormDataID = @DynamicFormDataID
          AND DynamicFormDataItemID = @DynamicFormDataItemID AND WeekOfMonth = @WeekOfMonth AND [Month] = @Month AND [Year] = @Year;";

                    existingWeekIds = (await conn.QueryAsync<long>(checkWeekSql, new
                    {
                        model.DynamicFormDataID,
                        model.DynamicFormDataItemID,
                        WeekOfMonth = weekOfMonth,
                        Month = month,
                        Year = year
                    }, transaction: tran)).ToList();

                    if (existingWeekIds.Count > 0)
                    {
                        
                        const string countChildrenSql = @"SELECT (SELECT COUNT(*) FROM dbo.SyrupFilling sf WHERE sf.SyrupPlanningID IN @Ids) AS FillingCount,
                                                          (SELECT COUNT(*) FROM dbo.SyrupOtherProcess sop WHERE sop.SyrupPlanningID IN @Ids) AS OtherProcessCount";
                        var counts = await conn.QuerySingleAsync(countChildrenSql, new { Ids = existingWeekIds }, transaction: tran);
                                              
                        const string deleteFillingSql = @"DELETE sf FROM dbo.SyrupFilling sf
                                                            JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                                            WHERE sp.DynamicFormDataID = @DynamicFormDataID
                                                              AND sp.DynamicFormDataItemID = @DynamicFormDataItemID
                                                              AND sp.WeekOfMonth = @WeekOfMonth AND sp.[Month] = @Month AND sp.[Year] = @Year";

                        int deletedFilling = await conn.ExecuteAsync(deleteFillingSql, new
                        {
                            model.DynamicFormDataID,
                            model.DynamicFormDataItemID,
                            WeekOfMonth = weekOfMonth,
                            Month = month,
                            Year = year
                        }, transaction: tran);

                        const string deleteOtherProcessSql = @" DELETE sop FROM dbo.SyrupOtherProcess sop
                                                                JOIN dbo.SyrupPlanning sp ON sop.SyrupPlanningID = sp.SyrupPlanningID
                                                                WHERE sp.DynamicFormDataID = @DynamicFormDataID
                                                                    AND sp.DynamicFormDataItemID = @DynamicFormDataItemID
                                                                    AND sp.WeekOfMonth = @WeekOfMonth AND sp.[Month] = @Month AND sp.[Year] = @Year";

                        int deletedOther = await conn.ExecuteAsync(deleteOtherProcessSql, new
                        {
                            model.DynamicFormDataID,
                            model.DynamicFormDataItemID,
                            WeekOfMonth = weekOfMonth,
                            Month = month,
                            Year = year
                        }, transaction: tran);

                        // Double-check no children remain for those parents
                        const string remainingChildrenCheck = @" SELECT TOP 1 1 FROM dbo.SyrupOtherProcess sop
                                                                JOIN dbo.SyrupPlanning sp ON sop.SyrupPlanningID = sp.SyrupPlanningID
                                                                WHERE sp.DynamicFormDataID = @DynamicFormDataID
                                                                  AND sp.DynamicFormDataItemID = @DynamicFormDataItemID
                                                                  AND sp.WeekOfMonth = @WeekOfMonth AND sp.[Month] = @Month AND sp.[Year] = @Year";

                        var stillExists = await conn.QueryFirstOrDefaultAsync<int?>(remainingChildrenCheck, new
                        {
                            model.DynamicFormDataID,
                            model.DynamicFormDataItemID,
                            WeekOfMonth = weekOfMonth,
                            Month = month,
                            Year = year
                        }, transaction: tran);

                        if (stillExists.HasValue)
                        {                            
                            throw new InvalidOperationException("Child rows in SyrupOtherProcess still exist after delete attempt. Aborting parent delete.");
                        }

                        const string deleteParentSql = @" DELETE FROM dbo.SyrupPlanning WHERE DynamicFormDataID = @DynamicFormDataID AND DynamicFormDataItemID = @DynamicFormDataItemID AND WeekOfMonth = @WeekOfMonth AND [Month] = @Month AND [Year] = @Year";

                        int deletedParents = await conn.ExecuteAsync(deleteParentSql, new
                        {
                            model.DynamicFormDataID,
                            model.DynamicFormDataItemID,
                            WeekOfMonth = weekOfMonth,
                            Month = month,
                            Year = year
                        }, transaction: tran);
                       
                    }
                }


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
                    ModifiedDate = modifiedDateSafe,
                    WeekOfMonth = weekOfMonth,
                    Month = month,
                    Year = year
                });

                long resultId;

                if (model.Id > 0)
                {
                    parameters.Add("SyrupPlanningID", model.Id);

                    const string updateSql = @"UPDATE dbo.SyrupPlanning
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
                    ModifiedDate = @ModifiedDate,
                    WeekOfMonth = @WeekOfMonth,
                    [Month] = @Month,
                    [Year] = @Year
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
                    SyruppreparationNextProcessName, AddedByUserID, ModifiedByUserID, AddedDate, ModifiedDate,WeekOfMonth, [Month], [Year]
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
                    @SyruppreparationNextProcessName, @AddedByUserID, @ModifiedByUserID, @AddedDate, @ModifiedDate,@WeekOfMonth, @Month, @Year
                );";

                    resultId = await conn.QuerySingleAsync<long>(insertSql, parameters, transaction: tran);
                }

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
        
        private static string FormatDuration(TimeSpan ts)
        {
            if (ts.TotalMinutes <= 0) return "0m";
            int days = ts.Days, hours = ts.Hours, minutes = ts.Minutes;
            var parts = new List<string>(3);
            if (days > 0) parts.Add($"{days}d");
            if (hours > 0) parts.Add($"{hours}h");
            if (minutes > 0) parts.Add($"{minutes}m");
            return string.Join(" ", parts);
        }
        private static (DateTime LStart, DateTime LEnd)? GetLunchWindow(DateTime when)
        {
            var day = when.Date;
            bool isFriday = when.DayOfWeek == DayOfWeek.Friday;

            var lunchStart = day.AddHours(12).AddMinutes(30); 
            var lunchEnd = isFriday
                ? day.AddHours(14).AddMinutes(30)   // Friday: 12:30–14:30
                : day.AddHours(13).AddMinutes(30);  // Other days: 12:30–13:30

            return (lunchStart, lunchEnd);
        }
       
        private static (List<SegmentModel> Segments, int NextId) BuildSegmentsForTask(int taskId, DateTime start, DateTime end, int nextSegId)
        {
            var segments = new List<SegmentModel>();
            int currentId = nextSegId;

            if (end <= start) return (segments, currentId);

            DateTime cursor = start;

            while (cursor < end)
            {
                var endOfDay = cursor.Date.AddDays(1);
                var sliceEnd = end < endOfDay ? end : endOfDay;

                var (lunchStart, lunchEnd) = GetLunchWindow(cursor).Value;

                // Before lunch
                var preStart = cursor;
                var preEnd = sliceEnd <= lunchStart ? sliceEnd : lunchStart;
                if (preEnd > preStart)
                    Add(preStart, preEnd);

                // After lunch
                var postStart = sliceEnd <= lunchEnd ? sliceEnd : lunchEnd;
                postStart = postStart < lunchEnd ? lunchEnd : postStart; // ensure after lunch
                if (sliceEnd > postStart)
                    Add(postStart, sliceEnd);

                cursor = sliceEnd;
            }

            return (segments, currentId);

            void Add(DateTime s, DateTime e)
            {
                var span = e - s;
                if (span.TotalMinutes <= 0) return;

                var hours = Math.Round(span.TotalHours, 2);
                segments.Add(new SegmentModel
                {
                    id = currentId++,
                    TaskId = taskId,
                    StartDate = s,
                    EndDate = e,
                    Duration = hours.ToString("0.##", CultureInfo.InvariantCulture)
                });
            }
        }
        public List<SegmentModel> FlattenSegments(IEnumerable<TaskData> tasks) => tasks?.SelectMany(t => t.Segments ?? Enumerable.Empty<SegmentModel>()).ToList() ?? new List<SegmentModel>();

        public async Task<IReadOnlyList<TaskData>> GetProductionGanttAsyncList(string profileNo,DateTime productionDay,TimeSpan shiftStart,long? DynamicFormDataID = null,int? SelectedWeekOfMonth = null,int? SelectedMonth = null,int? SelectedYear = null)
        {
            var startDateTime = productionDay.Date + shiftStart;
            const string procName = "dbo.sp_GetProductionGantt";

            using var conn = CreateConnection();
            if (conn is SqlConnection sconn) await sconn.OpenAsync();
            else if (conn.State == ConnectionState.Closed) conn.Open();

            try
            {
                var rows = (await conn.QueryAsync(
                    procName,
                    new
                    {
                        ProfileNo = profileNo,
                        StartDateTimeParam = startDateTime,
                        DynamicFormDataID,
                        WeekOfMonthParam = SelectedWeekOfMonth,
                        MonthParam = SelectedMonth,
                        YearParam = SelectedYear
                    },
                    commandType: CommandType.StoredProcedure
                )).ToList();

                var results = new List<TaskData>(rows.Count);
                int nextSegmentId = 1;

                foreach (var row in rows)
                {
                    long taskId = row.TaskId is null ? 0L : Convert.ToInt64(row.TaskId);
                    string? taskName = Convert.ToString(row.TaskName);

                    DateTime sd, ed;
                    DateTime? startDate = DateTime.TryParse(Convert.ToString(row.StartDate), out sd) ? sd : (DateTime?)null;
                    DateTime? endDate = DateTime.TryParse(Convert.ToString(row.EndDate), out ed) ? ed : (DateTime?)null;

                    int durationMinutesFromSql = row.DurationMinutes is null ? 0 : Convert.ToInt32(row.DurationMinutes);
                    int durationMinutesComputed = 0;

                    if (startDate.HasValue && endDate.HasValue)
                    {
                        var diff = endDate.Value - startDate.Value;
                        if (diff.TotalMinutes > 0)
                            durationMinutesComputed = (int)Math.Round(diff.TotalMinutes);
                    }

                    int durationMinutesFinal = durationMinutesComputed > 0 ? durationMinutesComputed : durationMinutesFromSql;
                    string durationText = FormatDuration(TimeSpan.FromMinutes(durationMinutesFinal));
                    decimal? durationHours = Math.Round(durationMinutesFinal / 60m, 2);

                    var task = new TaskData
                    {
                        TaskId = (int)taskId,
                        TaskName = taskName,
                        StartDate = startDate,
                        EndDate = endDate,
                        Duration = durationText,
                        Room = row.Room is null ? null : Convert.ToString(row.Room),
                        Predecessor = taskId.ToString(),
                        DurationHours = durationHours
                    };

                    // ✅ build segments (no ref!)
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        var segResult = BuildSegmentsForTask(task.TaskId, startDate.Value, endDate.Value, nextSegmentId);
                        task.Segments.AddRange(segResult.Segments);
                        nextSegmentId = segResult.NextId;
                    }

                    results.Add(task);
                }

                return results.AsReadOnly();
            }
            finally
            {
                if (conn.State != ConnectionState.Closed) conn.Close();
            }
        }
        public async Task<IReadOnlyList<TaskData>> GetProductionGanttAsyncList_vvkk1(string profileNo, DateTime productionDay, TimeSpan shiftStart,    long? DynamicFormDataID = null, int? SelectedWeekOfMonth = null,    int? SelectedMonth = null, int? SelectedYear = null)
        {
            var startDateTime = productionDay.Date + shiftStart;

            const string sqlTimeline = @"
WITH AllProcesses AS
(
    /* 1. Other Process */
    SELECT 
        sp.MethodCodeID,
        sp.MethodName,
        op.SyrupPlanningID,
        10 AS Seq,
        'OtherProcess' AS Source,
        op.ProcessName AS ProcessName,
        op.LocationOfProcess AS Room,
        op.SyrupOtherProcessNextProcess AS NextProcessName,
        TRY_CONVERT(decimal(18,4), op.ManhoursOrHours) AS DurationHours,
        CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), op.ManhoursOrHours), 0) * 60, 0) AS INT) AS DurationMinutes,
        TRY_CONVERT(decimal(18,4), op.NoOfManpower) AS Manpower
    FROM dbo.SyrupOtherProcess op
    INNER JOIN dbo.SyrupPlanning sp on sp.SyrupPlanningID = op.SyrupPlanningID
    WHERE ISNULL(LTRIM(RTRIM(op.ProcessName)), '') <> ''
      AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
      AND (@YearParam IS NULL OR sp.Year = @YearParam)
      AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

    UNION ALL

    /* 2. Syrup Simplex */
    SELECT
        sp.MethodCodeID,
        sp.MethodName,
        sp.SyrupPlanningID,
        20 AS Seq,
        'SyrupSimplex' AS Source,
        sp.SyrupSimplexProcessName AS ProcessName,
        sp.SyrupSimplexLocation AS Room,
        sp.SyrupSimplexNextProcessName AS NextProcessName,
        TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')) AS DurationHours,
        CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')), 0) * 60, 0) AS INT) AS DurationMinutes,
        TRY_CONVERT(decimal(18,4), sp.SyrupSimplexManpower) AS Manpower
    FROM dbo.SyrupPlanning sp
    WHERE ISNULL(LTRIM(RTRIM(sp.SyrupSimplexProcessName)), '') <> ''
      AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
      AND (@YearParam IS NULL OR sp.Year = @YearParam)
      AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

    UNION ALL

    /* 3. Syrup Simplex - Level2 Cleaning */
    SELECT
        sp.MethodCodeID,
        sp.MethodName,
        sp.SyrupPlanningID,
        21 AS Seq,
        'SyrupSimplexLevel2Cleaning' AS Source,
        CONCAT(sp.SyrupSimplexProcessName, ' - Level2 Cleaning') AS ProcessName,
        sp.SyruppreparationLocation AS Room,
        sp.SyrupSimplexNextProcessName AS NextProcessName,
        TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours) AS DurationHours,
        CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours), 0) * 60, 0) AS INT) AS DurationMinutes,
        TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningManpower) AS Manpower
    FROM dbo.SyrupPlanning sp
    WHERE ISNULL(LTRIM(RTRIM(sp.SyrupSimplexProcessName)), '') <> ''
      AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
      AND (@YearParam IS NULL OR sp.Year = @YearParam)
      AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

    UNION ALL

    /* 4. Syrup Preparation - Mixing */
    SELECT
        sp.MethodCodeID,
        sp.MethodName,
        sp.SyrupPlanningID,
        30 AS Seq,
        'SyrupPreparation' AS Source,
        sp.SyruppreparationProcessName AS ProcessName,
        sp.SyruppreparationLocation AS Room,
        sp.SyruppreparationNextProcessName AS NextProcessName,
        TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')) AS DurationHours,
        CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')), 0) * 60, 0) AS INT) AS DurationMinutes,
        TRY_CONVERT(decimal(18,4), sp.SyruppreparationFirstVolumnManpower) AS Manpower
    FROM dbo.SyrupPlanning sp
    WHERE ISNULL(LTRIM(RTRIM(sp.SyruppreparationProcessName)), '') <> ''
      AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
      AND (@YearParam IS NULL OR sp.Year = @YearParam)
      AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

    /* 5. Syrup Filling — include each process column separately */
    UNION ALL

    /* 5a. ProcessName_Primary */
    SELECT
        sp.MethodCodeID,
        sp.MethodName,
        sf.SyrupPlanningID,
        40 AS Seq,
        'PrimaryPacking' AS Source,
        sf.ProcessName_Primary AS ProcessName,
        NULL AS Room,
        sf.NextProcessName_Primary AS NextProcessName,
        COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0) AS DurationHours,
        CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0), 0) * 60, 0) AS INT) AS DurationMinutes,
        TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level1) AS Manpower
    FROM dbo.SyrupFilling sf
    LEFT JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
    WHERE ISNULL(LTRIM(RTRIM(sf.ProcessName_Primary)), '') <> ''
      AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
      AND (@YearParam IS NULL OR sp.Year = @YearParam)
      AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

    UNION ALL

    /* 5b. NextProcessName_Primary (Machine Filling) */
    SELECT
        sp.MethodCodeID,
        sp.MethodName,
        sf.SyrupPlanningID,
        41 AS Seq,
        'MachineFilling' AS Source,
        sf.NextProcessName_Primary AS ProcessName,
        NULL AS Room,
        sf.ProcessName_Secondary AS NextProcessName,
        COALESCE(sf.FillingHours_Level2, 0) AS DurationHours,
        CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level2, 0), 0) * 60, 0) AS INT) AS DurationMinutes,
        TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level2) AS Manpower
    FROM dbo.SyrupFilling sf
    LEFT JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
    WHERE ISNULL(LTRIM(RTRIM(sf.NextProcessName_Primary)), '') <> ''
      AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
      AND (@YearParam IS NULL OR sp.Year = @YearParam)
      AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

    UNION ALL

    /* 5c. ProcessName_Secondary */
    SELECT
        sp.MethodCodeID,
        sp.MethodName,
        sf.SyrupPlanningID,
        42 AS Seq,
        'SecondaryPacking' AS Source,
        sf.ProcessName_Secondary AS ProcessName,
        NULL AS Room,
        sf.NextProcessName_Secondary AS NextProcessName,
        CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
             ELSE COALESCE(sf.SecondaryPackingHours,0) END AS DurationHours,
        CAST(ROUND(ISNULL(
            CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                 ELSE COALESCE(sf.SecondaryPackingHours,0) END, 0) * 60, 0) AS INT) AS DurationMinutes,
        TRY_CONVERT(decimal(18,4), sf.SecondaryManpower) AS Manpower
    FROM dbo.SyrupFilling sf
    LEFT JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
    WHERE ISNULL(LTRIM(RTRIM(sf.ProcessName_Secondary)), '') <> ''
      AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
      AND (@YearParam IS NULL OR sp.Year = @YearParam)
      AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

    UNION ALL

    /* 5d. NextProcessName_Secondary (if present) */
    SELECT
        sp.MethodCodeID,
        sp.MethodName,
        sf.SyrupPlanningID,
        43 AS Seq,
        'SecondaryNext' AS Source,
        sf.NextProcessName_Secondary AS ProcessName,
        NULL AS Room,
        NULL AS NextProcessName,
        0 AS DurationHours,
        0 AS DurationMinutes,
        NULL AS Manpower
    FROM dbo.SyrupFilling sf
    LEFT JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
    WHERE ISNULL(LTRIM(RTRIM(sf.NextProcessName_Secondary)), '') <> ''
      AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
      AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
      AND (@YearParam IS NULL OR sp.Year = @YearParam)
      AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)
),
OrderedPerPlan AS
(
    SELECT *,
           ROW_NUMBER() OVER (PARTITION BY MethodCodeID, SyrupPlanningID ORDER BY Seq, ProcessName) AS rn_within_plan,
           COALESCE(DurationMinutes, 0) AS DurationMinutesNonNull
    FROM AllProcesses
    WHERE SyrupPlanningID IS NOT NULL
),
TimelinePerPlan AS
(
    SELECT *,
           SUM(DurationMinutesNonNull) OVER (PARTITION BY MethodCodeID, SyrupPlanningID ORDER BY rn_within_plan
                                            ROWS UNBOUNDED PRECEDING) AS CumMinutesPerPlan
    FROM OrderedPerPlan
),
-- CHILD rows (numeric IDs) — compute Predecessor
ChildRows AS
(
    SELECT
        CAST((COALESCE(SyrupPlanningID, 0) * 1000000) + rn_within_plan AS BIGINT) AS TaskId, -- unique numeric child id
        ProcessName AS TaskName,
        DATEADD(MINUTE, (CumMinutesPerPlan - DurationMinutesNonNull), @StartDateTimeParam) AS StartDate,
        DATEADD(MINUTE, CumMinutesPerPlan, @StartDateTimeParam) AS EndDate,
        0 AS Progress,

        /* Predecessor logic:
           - If this row is MachineFilling (Seq = 41) -> depend on PrimaryPacking (Seq = 40) for same SyrupPlanning (if present)
           - If this row is SecondaryPacking (Seq = 42) -> depend on PrimaryPacking (Seq = 40) if present
           - Otherwise default to previous sibling
        */
        CASE
            WHEN Seq = 41 THEN
                /* MachineFilling -> PrimaryPacking if present, else previous sibling */
                CAST(
                    (COALESCE(SyrupPlanningID,0) * 1000000)
                    +
                    ISNULL(
                        (SELECT MIN(rn_within_plan) FROM OrderedPerPlan op2
                         WHERE op2.SyrupPlanningID = TimelinePerPlan.SyrupPlanningID AND op2.Seq = 40),
                        rn_within_plan - 1
                    )
                    AS VARCHAR(50)
                )
            WHEN Seq = 42 THEN
                /* SecondaryPacking -> PrimaryPacking if present, else previous sibling */
                CAST(
                    (COALESCE(SyrupPlanningID,0) * 1000000)
                    +
                    ISNULL(
                        (SELECT MIN(rn_within_plan) FROM OrderedPerPlan op2
                         WHERE op2.SyrupPlanningID = TimelinePerPlan.SyrupPlanningID AND op2.Seq = 40),
                        rn_within_plan - 1
                    )
                    AS VARCHAR(50)
                )
            WHEN rn_within_plan > 1 THEN
                /* default: previous sibling */
                CAST((COALESCE(SyrupPlanningID,0) * 1000000) + (rn_within_plan - 1) AS VARCHAR(50))
            ELSE NULL
        END AS Predecessor,

        CAST(MethodCodeID AS BIGINT) AS ParentId, -- <- child points directly to Method
        DurationMinutesNonNull AS DurationMinutes,
        COALESCE(DurationHours, 0) AS DurationHours,
        Manpower,
        Room,
        NextProcessName,
        SyrupPlanningID,
        MethodCodeID,
        MethodName,
        MethodCodeID AS SortMethod,
        SyrupPlanningID AS SortPlanning,
        rn_within_plan AS SortChild,
        Seq
    FROM TimelinePerPlan
),
-- METHOD top-level rows (numeric IDs)
MethodRows AS
(
    SELECT
        CAST(MethodCodeID AS BIGINT) AS TaskId, -- numeric top-level id = MethodCodeID
        MethodName AS TaskName,
        @StartDateTimeParam AS StartDate,
        @StartDateTimeParam AS EndDate,
        0 AS Progress,
        NULL AS Predecessor,
        NULL AS ParentId, -- top level has no parent
        SUM(COALESCE(DurationMinutes,0)) AS DurationMinutes,
        SUM(COALESCE(DurationHours,0)) AS DurationHours,
        NULL AS Manpower,
        NULL AS Room,
        NULL AS NextProcessName,
        NULL AS SyrupPlanningID,
        MethodCodeID,
        MethodName,
        MethodCodeID AS SortMethod,
        NULL AS SortPlanning,
        0 AS SortChild
    FROM AllProcesses
    GROUP BY MethodCodeID, MethodName
)

-- final union: methods + children
SELECT
    TaskId, TaskName, StartDate, EndDate, Progress, Predecessor, ParentId,
    DurationMinutes, DurationHours, Manpower, Room, NextProcessName, SyrupPlanningID,
    MethodCodeID, MethodName,
    SortMethod, SortPlanning, SortChild
FROM MethodRows

UNION ALL

SELECT
    TaskId, TaskName, StartDate, EndDate, Progress, Predecessor, ParentId,
    DurationMinutes, DurationHours, Manpower, Room, NextProcessName, SyrupPlanningID,
    MethodCodeID, MethodName,
    SortMethod, SortPlanning, SortChild
FROM ChildRows

ORDER BY SortMethod, SortPlanning, SortChild, TaskId;
";

            using var conn = CreateConnection();
            if (conn is SqlConnection sconn) await sconn.OpenAsync();
            else if (conn.State == ConnectionState.Closed) conn.Open();

            try
            {
                var rows = (await conn.QueryAsync(sqlTimeline, new
                {
                    ProfileNo = profileNo,
                    StartDateTimeParam = startDateTime,
                    DynamicFormDataID = DynamicFormDataID,
                    WeekOfMonthParam = SelectedWeekOfMonth,
                    MonthParam = SelectedMonth,
                    YearParam = SelectedYear
                })).ToList();

                var results = new List<TaskData>(rows.Count);

                foreach (var row in rows)
                {
                    // use long for TaskId because we built large IDs
                    long taskId = row.TaskId is null ? 0L : Convert.ToInt64(row.TaskId);
                    string? taskName = row.TaskName is null ? null : Convert.ToString(row.TaskName);

                    DateTime? startDate = null;
                    DateTime? endDate = null;

                    DateTime sd;
                    if (DateTime.TryParse(Convert.ToString(row.StartDate), out sd))
                        startDate = sd;

                    DateTime ed;
                    if (DateTime.TryParse(Convert.ToString(row.EndDate), out ed))
                        endDate = ed;

                   


                    int durationMinutes = row.DurationMinutes is null ? 0 : Convert.ToInt32(row.DurationMinutes);

                    int progress = 0;
                    DateTime now = DateTime.Now;

                    if (startDate.HasValue)
                    {
                        if (durationMinutes <= 0)
                        {
                            progress = (now >= startDate.Value) ? 100 : 0;
                        }
                        else if (now <= startDate.Value)
                        {
                            progress = 0;
                        }
                        else if (endDate.HasValue && now >= endDate.Value)
                        {
                            progress = 100;
                        }
                        else
                        {
                            var elapsedMinutes = (now - startDate.Value).TotalMinutes;
                            double fraction = elapsedMinutes / durationMinutes;
                            progress = (int)Math.Round(Math.Min(100, fraction * 100));
                        }
                    }

                    // predecessor is a string from SQL (or null)
                    string? predecessor = row.Predecessor is null ? null : Convert.ToString(row.Predecessor);

                    // ParentId -> may be null
                    long? parentId = null;
                    long pid;
                    if (row.ParentId != null)
                    {
                       
                        if (long.TryParse(Convert.ToString(row.ParentId), out pid))
                            parentId = pid;
                    }

                    decimal durationHours = row.DurationHours is null ? 0m : Convert.ToDecimal(row.DurationHours);
                    decimal? manpower = row.Manpower is null ? null : (decimal?)Convert.ToDecimal(row.Manpower);
                    string? nextProcess = row.NextProcessName is null ? null : Convert.ToString(row.NextProcessName);

                    results.Add(new TaskData
                    {
                        TaskId = (int)taskId,
                        TaskName = taskName,
                        StartDate = startDate,
                        EndDate = endDate,
                        Room = row.Room is null ? null : Convert.ToString(row.Room),
                        Progress = progress,
                        Predecessor = taskId.ToString(),
                        ParentId = (int?)parentId
                    });
                }

                return results.AsReadOnly();
            }
            finally
            {
                if (conn.State != ConnectionState.Closed) conn.Close();
            }
        }

        public async Task<IReadOnlyList<TaskData>> GetProductionGanttAsyncList_vvkk(string profileNo, DateTime productionDay, TimeSpan shiftStart, long? DynamicFormDataID = null, int? SelectedWeekOfMonth = null, int? SelectedMonth = null, int? SelectedYear = null)
        {
            var startDateTime = productionDay.Date + shiftStart;

            const string sqlTimeline = @"WITH AllProcesses AS
                                    (
                                        /* 1. Other Process */
                                        SELECT 
                                            sp.MethodCodeID,
                                            sp.MethodName,
                                            op.SyrupPlanningID,
                                            10 AS Seq,
                                            'OtherProcess' AS Source,
                                            op.ProcessName AS ProcessName,
                                            op.LocationOfProcess AS Room,
                                            op.SyrupOtherProcessNextProcess AS NextProcessName,
                                            TRY_CONVERT(decimal(18,4), op.ManhoursOrHours) AS DurationHours,
                                            CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), op.ManhoursOrHours), 0) * 60, 0) AS INT) AS DurationMinutes,
                                            TRY_CONVERT(decimal(18,4), op.NoOfManpower) AS Manpower
                                        FROM dbo.SyrupOtherProcess op                                       
										INNER JOIN dbo.SyrupPlanning sp on sp.SyrupPlanningID = op.SyrupPlanningID
                                        WHERE ISNULL(LTRIM(RTRIM(op.ProcessName)), '') <> ''
										 AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
  AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
  AND (@YearParam IS NULL OR sp.Year = @YearParam)
										  AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)
                                        UNION ALL

                                        /* 2. Syrup Simplex */
                                        SELECT
                                            sp.MethodCodeID,
                                            sp.MethodName,
                                            sp.SyrupPlanningID,
                                            20 AS Seq,
                                            'SyrupSimplex' AS Source,
                                            sp.SyrupSimplexProcessName AS ProcessName,
                                            sp.SyrupSimplexLocation AS Room,
                                            sp.SyrupSimplexNextProcessName AS NextProcessName,
                                            TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')) AS DurationHours,
                                            CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyrupSimplexPreparationHour, ',', '')), 0) * 60, 0) AS INT) AS DurationMinutes,
                                            TRY_CONVERT(decimal(18,4), sp.SyrupSimplexManpower) AS Manpower
                                        FROM dbo.SyrupPlanning sp
                                        WHERE ISNULL(LTRIM(RTRIM(sp.SyrupSimplexProcessName)), '') <> ''
										AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
  AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
  AND (@YearParam IS NULL OR sp.Year = @YearParam)
  AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

                                        UNION ALL

                                        /* 3. Syrup Simplex - Level2 Cleaning */
                                        SELECT
                                            sp.MethodCodeID,
                                            sp.MethodName,
                                            sp.SyrupPlanningID,
                                            21 AS Seq,
                                            'SyrupSimplexLevel2Cleaning' AS Source,
                                            CONCAT(sp.SyrupSimplexProcessName, ' - Level2 Cleaning') AS ProcessName,
                                            sp.SyruppreparationLocation AS Room,
                                            sp.SyrupSimplexNextProcessName AS NextProcessName,
                                            TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours) AS DurationHours,
                                            CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningHours), 0) * 60, 0) AS INT) AS DurationMinutes,
                                            TRY_CONVERT(decimal(18,4), sp.SyrupSimplexLevel2CleaningManpower) AS Manpower
                                        FROM dbo.SyrupPlanning sp
                                        WHERE ISNULL(LTRIM(RTRIM(sp.SyrupSimplexProcessName)), '') <> ''
										AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
  AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
  AND (@YearParam IS NULL OR sp.Year = @YearParam)
  AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

                                        UNION ALL

                                        /* 4. Syrup Preparation - Mixing */
                                        SELECT
                                            sp.MethodCodeID,
                                            sp.MethodName,
                                            sp.SyrupPlanningID,
                                            30 AS Seq,
                                            'SyrupPreparation' AS Source,
                                            sp.SyruppreparationProcessName AS ProcessName,
                                            sp.SyruppreparationLocation AS Room,
                                            sp.SyruppreparationNextProcessName AS NextProcessName,
                                            TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')) AS DurationHours,
                                            CAST(ROUND(ISNULL(TRY_CONVERT(decimal(18,4), REPLACE(sp.SyruppreparationFirstVolumnHour, ',', '')), 0) * 60, 0) AS INT) AS DurationMinutes,
                                            TRY_CONVERT(decimal(18,4), sp.SyruppreparationFirstVolumnManpower) AS Manpower
                                        FROM dbo.SyrupPlanning sp
                                        WHERE ISNULL(LTRIM(RTRIM(sp.SyruppreparationProcessName)), '') <> ''
										AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
  AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
  AND (@YearParam IS NULL OR sp.Year = @YearParam)
  AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

                                        /* 5. Syrup Filling — include each process column separately */
                                        UNION ALL

                                        /* 5a. ProcessName_Primary */
                                        SELECT
                                            sp.MethodCodeID,
                                            sp.MethodName,
                                            sf.SyrupPlanningID,
                                            40 AS Seq,
                                            'PrimaryPacking' AS Source,
                                            sf.ProcessName_Primary AS ProcessName,
                                            NULL AS Room,
                                            sf.NextProcessName_Primary AS NextProcessName,
                                            COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0) AS DurationHours,
                                            CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level1, sf.ChangePackingFillingHours, 0), 0) * 60, 0) AS INT) AS DurationMinutes,
                                            TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level1) AS Manpower
                                        FROM dbo.SyrupFilling sf
                                        LEFT JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                        WHERE ISNULL(LTRIM(RTRIM(sf.ProcessName_Primary)), '') <> ''
										AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
  AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
  AND (@YearParam IS NULL OR sp.Year = @YearParam)
  AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

                                        UNION ALL

                                        /* 5b. NextProcessName_Primary (Machine Filling) */
                                        SELECT
                                            sp.MethodCodeID,
                                            sp.MethodName,
                                            sf.SyrupPlanningID,
                                            41 AS Seq,
                                            'MachineFilling' AS Source,
                                            sf.NextProcessName_Primary AS ProcessName,
                                            NULL AS Room,
                                            sf.ProcessName_Secondary AS NextProcessName,
                                            COALESCE(sf.FillingHours_Level2, 0) AS DurationHours,
                                            CAST(ROUND(ISNULL(COALESCE(sf.FillingHours_Level2, 0), 0) * 60, 0) AS INT) AS DurationMinutes,
                                            TRY_CONVERT(decimal(18,4), sf.FillingManpower_Level2) AS Manpower
                                        FROM dbo.SyrupFilling sf
                                        LEFT JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                        WHERE ISNULL(LTRIM(RTRIM(sf.NextProcessName_Primary)), '') <> ''
										AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
  AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
  AND (@YearParam IS NULL OR sp.Year = @YearParam)
  AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

                                        UNION ALL

                                        /* 5c. ProcessName_Secondary */
                                        SELECT
                                            sp.MethodCodeID,
                                            sp.MethodName,
                                            sf.SyrupPlanningID,
                                            42 AS Seq,
                                            'SecondaryPacking' AS Source,
                                            sf.ProcessName_Secondary AS ProcessName,
                                            NULL AS Room,
                                            sf.NextProcessName_Secondary AS NextProcessName,
                                            CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                                                 ELSE COALESCE(sf.SecondaryPackingHours,0) END AS DurationHours,
                                            CAST(ROUND(ISNULL(
                                                CASE WHEN ISNULL(sf.SecondarySameAsPrimaryTime,0) = 1 THEN COALESCE(sf.FillingHours_Level1,0)
                                                     ELSE COALESCE(sf.SecondaryPackingHours,0) END, 0) * 60, 0) AS INT) AS DurationMinutes,
                                            TRY_CONVERT(decimal(18,4), sf.SecondaryManpower) AS Manpower
                                        FROM dbo.SyrupFilling sf
                                        LEFT JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                        WHERE ISNULL(LTRIM(RTRIM(sf.ProcessName_Secondary)), '') <> ''
										AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
  AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
  AND (@YearParam IS NULL OR sp.Year = @YearParam)
  AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

                                        UNION ALL

                                        /* 5d. NextProcessName_Secondary (if present) */
                                        SELECT
                                            sp.MethodCodeID,
                                            sp.MethodName,
                                            sf.SyrupPlanningID,
                                            43 AS Seq,
                                            'SecondaryNext' AS Source,
                                            sf.NextProcessName_Secondary AS ProcessName,
                                            NULL AS Room,
                                            NULL AS NextProcessName,
                                            0 AS DurationHours,
                                            0 AS DurationMinutes,
                                            NULL AS Manpower
                                        FROM dbo.SyrupFilling sf
                                        LEFT JOIN dbo.SyrupPlanning sp ON sf.SyrupPlanningID = sp.SyrupPlanningID
                                        WHERE ISNULL(LTRIM(RTRIM(sf.NextProcessName_Secondary)), '') <> ''
										AND (@WeekOfMonthParam IS NULL OR sp.WeekOfMonth = @WeekOfMonthParam)
  AND (@MonthParam IS NULL OR sp.Month = @MonthParam)
  AND (@YearParam IS NULL OR sp.Year = @YearParam)
  AND (@DynamicFormDataID IS NULL OR sp.DynamicFormDataID = @DynamicFormDataID)

                                    ),
                                    OrderedPerPlan AS
                                    (
                                        -- number child rows within each Method + SyrupPlanning in Seq order
                                        SELECT *,
                                               ROW_NUMBER() OVER (PARTITION BY MethodCodeID, SyrupPlanningID ORDER BY Seq, ProcessName) AS rn_within_plan,
                                               COALESCE(DurationMinutes, 0) AS DurationMinutesNonNull
                                        FROM AllProcesses
                                        WHERE SyrupPlanningID IS NOT NULL
                                    ),
                                    TimelinePerPlan AS
                                    (
                                        -- compute cumulative minutes per planning so each planning's timeline starts at @StartDateTimeParam
                                        SELECT *,
                                               SUM(DurationMinutesNonNull) OVER (PARTITION BY MethodCodeID, SyrupPlanningID ORDER BY rn_within_plan
                                                                                ROWS UNBOUNDED PRECEDING) AS CumMinutesPerPlan
                                        FROM OrderedPerPlan
                                    ),
                                    -- CHILD rows (numeric IDs) — now link directly to Method (ParentId = MethodCodeID)
                                    ChildRows AS
                                    (
                                        SELECT
                                            CAST((COALESCE(SyrupPlanningID, 0) * 1000000) + rn_within_plan AS BIGINT) AS TaskId, -- unique numeric child id
                                            ProcessName AS TaskName,
                                            DATEADD(MINUTE, (CumMinutesPerPlan - DurationMinutesNonNull), @StartDateTimeParam) AS StartDate,
                                            DATEADD(MINUTE, CumMinutesPerPlan, @StartDateTimeParam) AS EndDate,
                                            0 AS Progress,
                                            NULL AS Predecessor,
                                            CAST(MethodCodeID AS BIGINT) AS ParentId, -- <- child points directly to Method
                                            DurationMinutesNonNull AS DurationMinutes,
                                            COALESCE(DurationHours, 0) AS DurationHours,
                                            Manpower,
                                            Room,
                                            NextProcessName,
                                            SyrupPlanningID,
                                            MethodCodeID,
                                            MethodName,
                                            MethodCodeID AS SortMethod,
                                            SyrupPlanningID AS SortPlanning,
                                            rn_within_plan AS SortChild
                                        FROM TimelinePerPlan
                                    ),
                                    -- METHOD top-level rows (numeric IDs)
                                    MethodRows AS
                                    (
                                        SELECT
                                            CAST(MethodCodeID AS BIGINT) AS TaskId, -- numeric top-level id = MethodCodeID
                                            MethodName AS TaskName,
                                            -- placeholder start/end (can be computed from children if you prefer)
                                            @StartDateTimeParam AS StartDate,
                                            @StartDateTimeParam AS EndDate,
                                            0 AS Progress,
                                            NULL AS Predecessor,
                                            NULL AS ParentId, -- top level has no parent
                                            SUM(COALESCE(DurationMinutes,0)) AS DurationMinutes,
                                            SUM(COALESCE(DurationHours,0)) AS DurationHours,
                                            NULL AS Manpower,
                                            NULL AS Room,
                                            NULL AS NextProcessName,
                                            NULL AS SyrupPlanningID,
                                            MethodCodeID,
                                            MethodName,
                                            MethodCodeID AS SortMethod,
                                            NULL AS SortPlanning,
                                            0 AS SortChild
                                        FROM AllProcesses
                                        GROUP BY MethodCodeID, MethodName
                                    )

                                    -- final union: methods + children (NO SyrupPlanning parent rows)
                                    SELECT
                                        TaskId, TaskName, StartDate, EndDate, Progress, Predecessor, ParentId,
                                        DurationMinutes, DurationHours, Manpower, Room, NextProcessName, SyrupPlanningID,
                                        MethodCodeID, MethodName,
                                        SortMethod, SortPlanning, SortChild
                                    FROM MethodRows

                                    UNION ALL

                                    SELECT
                                        TaskId, TaskName, StartDate, EndDate, Progress, Predecessor, ParentId,
                                        DurationMinutes, DurationHours, Manpower, Room, NextProcessName, SyrupPlanningID,
                                        MethodCodeID, MethodName,
                                        SortMethod, SortPlanning, SortChild
                                    FROM ChildRows

                                    ORDER BY SortMethod, SortPlanning, SortChild, TaskId";

            using var conn = CreateConnection();
            if (conn is SqlConnection sconn) await sconn.OpenAsync();
            else if (conn.State == ConnectionState.Closed) conn.Open();

            try
            {
                var rows = (await conn.QueryAsync(sqlTimeline, new
                {
                    ProfileNo = profileNo,
                    StartDateTimeParam = startDateTime,
                    DynamicFormDataID = DynamicFormDataID,
                    WeekOfMonthParam = SelectedWeekOfMonth,
                    MonthParam = SelectedMonth,
                    YearParam = SelectedYear
                })).ToList();

                var results = new List<TaskData>(rows.Count);

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
                        Room = row.Room is null ? null : Convert.ToString(row.Room),
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

        private const string InsertSyrupPlanningSql = @"
INSERT INTO SyrupPlanning
(
    DynamicFormDataItemID,
    MethodCodeLineID,
    DynamicFormDataID,
    MethodName,
    MethodCodeID,
    ProfileNo,
    MethodCode,
    BatchSizeInLiters,
    RestrictionOnPlanningDay,
    ProcessName,
    IsthereSyrupSimplextoproduce,
    SyrupSimplexProcessName,
    SyrupSimplexLocation,
    SyrupSimplexPreparationHour,
    SyrupSimplexManpower,
    SyrupSimplexLevel2CleaningHours,
    SyrupSimplexLevel2CleaningManpower,
    SyrupSimplexNoofCampaign,
    SyrupSimplexNextProcessName,


    SyruppreparationFirstVolumnHour,
    SyruppreparationFirstVolumnManpower,
    SyruppreparationIPQCTest,
    SyruppreparationTopupToVolumnHour,
    SyruppreparationTopupToVolumnManpower,
    SyruppreparationCampaignBatchesNumbers,
    SyruppreparationLevel1CleaningHours,
    SyruppreparationLevel1Cleaningmanpower,
    SyruppreparationLevel2Cleaninghours,
    SyruppreparationLevel2CleaningManpower,
    SyruppreparationNextProcessName,
    PreparationPerHour,
    Level2CleaningHours,
    Level2CleaningManpower,
    NoOfCampaign,
    NextProcessName,
    PreparationFirstVolumePerHour,
    PreparationFirstVolumeManpower,
    IpqcTestRequired,
    PreparationTopUpPerHour,
    PreparationTopUpManpower,
    CampaignBatches,
    Level1CleaningHours,
    Level1CleaningManpower,
    SyrupLevel2CleaningHours,
    SyrupLevel2CleaningManpower,
    NextProcessNameAfterPreparation,
    AddedByUserID,
    ModifiedByUserID,
    AddedDate,
    ModifiedDate,
    WeekOfMonth,
    Month,
    Year
)
OUTPUT INSERTED.SyrupPlanningID
VALUES
(
    @DynamicFormDataItemID,
    @MethodCodeLineID,
    @DynamicFormDataID,
    @MethodName,
    @MethodCodeID,
    @ProfileNo,
    @MethodCode,
    @BatchSizeInLiters,
    @RestrictionOnPlanningDay,
    @ProcessName,
    @IsthereSyrupSimplextoproduce,
    @SyrupSimplexProcessName,
    @SyrupSimplexLocation,
    @SyrupSimplexPreparationHour,
    @SyrupSimplexManpower,
    @SyrupSimplexLevel2CleaningHours,
    @SyrupSimplexLevel2CleaningManpower,
    @SyrupSimplexNoofCampaign,
    @SyrupSimplexNextProcessName,
  
  
    @SyruppreparationFirstVolumnHour,
    @SyruppreparationFirstVolumnManpower,
    @SyruppreparationIPQCTest,
    @SyruppreparationTopupToVolumnHour,
    @SyruppreparationTopupToVolumnManpower,
    @SyruppreparationCampaignBatchesNumbers,
    @SyruppreparationLevel1CleaningHours,
    @SyruppreparationLevel1Cleaningmanpower,
    @SyruppreparationLevel2Cleaninghours,
    @SyruppreparationLevel2CleaningManpower,
    @SyruppreparationNextProcessName,
    @PreparationPerHour,
    @Level2CleaningHours,
    @Level2CleaningManpower,
    @NoOfCampaign,
    @NextProcessName,
    @PreparationFirstVolumePerHour,
    @PreparationFirstVolumeManpower,
    @IpqcTestRequired,
    @PreparationTopUpPerHour,
    @PreparationTopUpManpower,
    @CampaignBatches,
    @Level1CleaningHours,
    @Level1CleaningManpower,
    @SyrupLevel2CleaningHours,
    @SyrupLevel2CleaningManpower,
    @NextProcessNameAfterPreparation,
    @AddedByUserID,
    @ModifiedByUserID,
    @AddedDate,
    @ModifiedDate,
    @WeekOfMonth,
    @Month,
    @Year
)";

        // child selects (fetch packing & other rows for a given DynamicFormDataID)
        const string packingSelectPerId = @"SELECT 
    t1.DynamicFormDataID AS DynamicFormDataID,
    t1.DynamicFormDataItemID AS DynamicFormDataItemID,
    t1.ProfileNo AS ProfileNo,
    t1.[13265_ProductionPlanningProcess] AS ProcessName_Primary,
    t1.[13266_ProductionPlanningProcess] AS NextProcessName_Primary,
    t1.[13230_TypeofPlanningProcess] AS PlanningType_Primary,
    t1.[13213_Primaryfillingmachine] AS PrimaryFillingMachine,
    TRY_CAST(NULLIF(NULLIF(LTRIM(RTRIM(REPLACE(t1.[13219_Level1hours], ',', ''))),'-'),'') AS decimal(18,4)) AS FillingHours_Level1,
    TRY_CAST(NULLIF(NULLIF(LTRIM(RTRIM(REPLACE(t1.[13220_Level1manpower], ',', ''))),'-'),'') AS int) AS FillingManpower_Level1,
    TRY_CAST(NULLIF(NULLIF(LTRIM(RTRIM(REPLACE(t1.[13224_1FillingSpeedbottleminutes], ',', ''))),'-'),'') AS decimal(18,4)) AS Speed_BottlePerMinute,
    TRY_CAST(NULLIF(NULLIF(LTRIM(RTRIM(REPLACE(t1.[13218_ChangePackingFillingHours], ',', ''))),'-'),'') AS decimal(18,4)) AS ChangePackingFillingHours,
    TRY_CAST(NULLIF(NULLIF(LTRIM(RTRIM(REPLACE(t1.[13221_Level2hours], ',', ''))),'-'),'') AS decimal(18,4)) AS FillingHours_Level2,
    TRY_CAST(NULLIF(NULLIF(LTRIM(RTRIM(REPLACE(t1.[13222_Level2Manpower], ',', ''))),'-'),'') AS int) AS FillingManpower_Level2,
    CAST(CASE WHEN LOWER(LTRIM(RTRIM(t1.[13256_SecondaryPackingTimeisthesameasPrimarypackingtime]))) = 'yes' THEN 1 ELSE 0 END AS bit) AS SecondarySameAsPrimaryTime,
    TRY_CAST(NULLIF(NULLIF(LTRIM(RTRIM(REPLACE(t1.[13256_1944_SecondaryPackingHours], ',', ''))),'-'),'') AS decimal(18,4)) AS SecondaryPackingHours,
    TRY_CAST(NULLIF(NULLIF(LTRIM(RTRIM(REPLACE(t1.[13256_1945_NoofManpower], ',', ''))),'-'),'') AS int) AS SecondaryManpower,
    t1.[13267_ProductionPlanningProcess] AS ProcessName_Secondary,
    t1.[13268_ProductionPlanningProcess] AS NextProcessName_Secondary,
    t1.[13270_Syruprequireofflinepacking] AS RequireOfflinePacking,
    1 AS AddedByUserID,
    '' AS Description,
    t1.ModifiedBy AS ModifiedBy,
    t1.ModifiedDate AS ModifiedDate
FROM DynamicForm_ProdTimingSyrupPackingGrid t1
INNER JOIN DynamicFormData t2 on t2.DynamicFormDataID = t1.DynamicFormDataID
WHERE t1.DynamicFormDataGridId = @Id";

        const string otherSelectPerId = @"SELECT 
    DynamicFormDataItemID AS DynamicFormDataItemID,
    DynamicFormDataID,
    [13260_OtherJobsInformation] AS OtherJobsInformation,
    [13260_1980_ProductionPlanningProcess] AS ProcessName,
    ProfileNo,
    [13260_1987_ProductionPlanningProcess] AS SyrupOtherProcessNextProcess,
    [13260_1981_Mustcompletetohavenextprocess] AS MustCompleteForNext,                          
    [13260_1984_Locationofprocess] AS LocationOfProcess,
    TRY_CAST(NULLIF([13260_1986_NoofManhoursHours], '') AS DECIMAL(18,2)) AS ManhoursOrHours,
    TRY_CAST(NULLIF([13260_1985_Noofmanpower], '') AS INT) AS NoOfManpower,
    [13260_1988_Manpowerisfromnextprocess] AS ManpowerFromNextProcess,
    [13260_1996_Cancarryoutonnonworkday] AS CanCarryoutOnNonWorkday,
    TRY_CAST(NULLIF([13260_2000_TimeGapHour], '') AS DECIMAL(18,2)) AS TimeGapHour,
    1 AS AddedByUserID,
    '' AS Description,
    '' AS ModifiedBy,
    GETDATE() AS ModifiedDate
FROM DynamicForm_ProductionTimingSyrupOthers
WHERE DynamicFormDataGridId = @Id;
";

        // child insert SQLs
        const string insertFillingSql = @"INSERT INTO SyrupFilling
(
    SyrupPlanningID,
    ProcessName_Primary,
    DynamicFormDataID,
    DynamicFormDataItemID,
    ProfileNo,
    PlanningType_Primary,
    ModifiedBy,
    ModifiedDate,
    PrimaryFillingMachine,
    FillingHours_Level1,
    FillingManpower_Level1,
    Speed_BottlePerMinute,
    ChangePackingFillingHours,
    FillingHours_Level2,
    FillingManpower_Level2,
    SecondarySameAsPrimaryTime,
    SecondaryPackingHours,
    SecondaryManpower,
    ProcessName_Secondary,
    NextProcessName_Secondary,
    RequireOfflinePacking,
    AddedByUserID,
    Description
)
VALUES
(
    @SyrupPlanningID,
    @ProcessName_Primary,
    @DynamicFormDataID,
    @DynamicFormDataItemID,
    @ProfileNo,
    @PlanningType_Primary,
    @ModifiedBy,
    @ModifiedDate,
    @PrimaryFillingMachine,
    @FillingHours_Level1,
    @FillingManpower_Level1,
    @Speed_BottlePerMinute,
    @ChangePackingFillingHours,
    @FillingHours_Level2,
    @FillingManpower_Level2,
    @SecondarySameAsPrimaryTime,
    @SecondaryPackingHours,
    @SecondaryManpower,
    @ProcessName_Secondary,
    @NextProcessName_Secondary,
    @RequireOfflinePacking,
    @AddedByUserID,
    @Description
)";

        const string insertOtherSql = @"
INSERT INTO SyrupOtherProcess
( 
    DynamicFormDataItemID,
    SyrupPlanningID,
    DynamicFormDataID,
    OtherJobsInformation,
    ProcessName,
    ProfileNo,
    SyrupOtherProcessNextProcess,
    MustCompleteForNext,
    LocationOfProcess,
    ManhoursOrHours,
    NoOfManpower,
    ManpowerFromNextProcess,
    CanCarryoutOnNonWorkday,
    TimeGapHour,
    AddedByUserID,
    Description,
    ModifiedBy,
    ModifiedDate
)
VALUES
(
    @DynamicFormDataItemID,
    @SyrupPlanningID,
    @DynamicFormDataID,
    @OtherJobsInformation,
    @ProcessName,
    @ProfileNo,
    @SyrupOtherProcessNextProcess,
    @MustCompleteForNext,
    @LocationOfProcess,
    @ManhoursOrHours,
    @NoOfManpower,
    @ManpowerFromNextProcess,
    @CanCarryoutOnNonWorkday,
    @TimeGapHour,
    @AddedByUserID,
    @Description,
    @ModifiedBy,
    @ModifiedDate
)";
        // DTOs for intermediate selects (thin)
        public class PackingDto
        {
            public long DynamicFormID { get; set; }
            public long DynamicFormDataID { get; set; }
            public long DynamicFormDataItemID { get; set; }
            public string? ProfileNo { get; set; }
            public string? ProcessName_Primary { get; set; }
            public string? NextProcessName_Primary { get; set; }
            public string? PlanningType_Primary { get; set; }
            public string? PrimaryFillingMachine { get; set; }
            public decimal? FillingHours_Level1 { get; set; }
            public int? FillingManpower_Level1 { get; set; }
            public decimal? Speed_BottlePerMinute { get; set; }
            public decimal? ChangePackingFillingHours { get; set; }
            public decimal? FillingHours_Level2 { get; set; }
            public int? FillingManpower_Level2 { get; set; }
            public bool SecondarySameAsPrimaryTime { get; set; }
            public decimal? SecondaryPackingHours { get; set; }
            public int? SecondaryManpower { get; set; }
            public string? ProcessName_Secondary { get; set; }
            public string? NextProcessName_Secondary { get; set; }
            public string? RequireOfflinePacking { get; set; }
            public int AddedByUserID { get; set; }
            public string Description { get; set; } = "";
            public string? ModifiedBy { get; set; }
            public DateTime? ModifiedDate { get; set; }
        }

        public class OtherDto
        {
            public long DynamicFormDataItemID { get; set; }
            public long DynamicFormDataID { get; set; }
            public string? OtherJobsInformation { get; set; }
            public string? ProcessName { get; set; }
            public string? ProfileNo { get; set; }
            public string? SyrupOtherProcessNextProcess { get; set; }
            public string? MustCompleteForNext { get; set; }
            public string? LocationOfProcess { get; set; }
            public decimal? ManhoursOrHours { get; set; }
            public int? NoOfManpower { get; set; }
            public string? ManpowerFromNextProcess { get; set; }
            public string? CanCarryoutOnNonWorkday { get; set; }
            public decimal? TimeGapHour { get; set; }
            public int AddedByUserID { get; set; }
            public string Description { get; set; } = "";
            public string? ModifiedBy { get; set; }
            public DateTime? ModifiedDate { get; set; }
        }

        public class PrimaryProductionDto
        {
            public long? DynamicFormDataItemID { get; set; }
            public long DynamicFormDataID { get; set; }
            public string? ProfileNo { get; set; }
            public long? MethodCodeID { get; set; }
            public string? MethodName { get; set; }
            public string? SyrupSimplexProcessName { get; set; }
            public string? SyrupSimplexLocation { get; set; }
            public string? IsthereSyrupSimplextoproduce { get; set; }
            public decimal? BatchSizeInLiters { get; set; }
            public decimal? SyrupSimplexPreparationHour { get; set; }   // decimal?
            public int? SyrupSimplexManpower { get; set; }             // int?
            public decimal? SyrupSimplexLevel2CleaningHours { get; set; }
            public int? SyrupSimplexLevel2CleaningManpower { get; set; }
            public int? SyrupSimplexNoofCampaign { get; set; }

            public string? SyrupSimplexNextProcessName { get; set; }
            public string? SyruppreparationProcessName { get; set; }
            public string? SyruppreparationLocation { get; set; }

            public decimal? SyruppreparationFirstVolumnHour { get; set; }
            public int? SyruppreparationFirstVolumnManpower { get; set; }
            public string? SyruppreparationIPQCTest { get; set; }
            public decimal? SyruppreparationTopupToVolumnHour { get; set; }
            public int? SyruppreparationTopupToVolumnManpower { get; set; }
            public string? SyruppreparationCampaignBatchesNumbers { get; set; }
            public decimal? SyruppreparationLevel1CleaningHours { get; set; }
            public int? SyruppreparationLevel1Cleaningmanpower { get; set; }
            public decimal? SyruppreparationLevel2Cleaninghours { get; set; }
            public int? SyruppreparationLevel2CleaningManpower { get; set; }
            public string? SyruppreparationNextProcessName { get; set; }

            public string? ModifiedBy { get; set; }
            public DateTime? ModifiedDate { get; set; }
        }

        public async Task<List<PrimaryProductionDto>> GetPrimaryRowsAsync()
        {
            using (var connection = CreateConnection())
            {
                const string PrimarySelectSql = @"SELECT
                    DynamicFormDataItemID,
                    DynamicFormDataID,
                    ProfileNo,
                    [13192_MethodCode] as MethodName,
                    ISNULL(TRY_CAST(NULLIF(REPLACE([13193_BatchSizeL], '-', ''), '') AS DECIMAL(18,2)), 0) AS BatchSizeInLiters,
                    [13192_MethodCode_UId] AS MethodCodeID,
                    [13229_2003_ProductionPlanningProcess] AS SyrupSimplexProcessName,
                    [13229_1954_Location] AS SyrupSimplexLocation,
                    [13229_IsthereSyrupSimplextoproduce] AS IsthereSyrupSimplextoproduce,

                    -- ensure empty strings become NULL and cast to decimal
                    TRY_CAST(NULLIF([13229_1955_1PreparationHour], '') AS DECIMAL(18,4)) AS SyrupSimplexPreparationHour,

                    -- manpower: convert empty string -> NULL then to INT (use COALESCE to 0 if you prefer non-null)
                    TRY_CAST(NULLIF([13229_1956_SyrupSimplexManpower], '') AS INT) AS SyrupSimplexManpower,

                    TRY_CAST(NULLIF([13229_1957_Level2CleaningHours], '') AS DECIMAL(18,4)) AS SyrupSimplexLevel2CleaningHours,
                    TRY_CAST(NULLIF([13229_1958_Level2CleaningManpower], '') AS INT) AS SyrupSimplexLevel2CleaningManpower,
                    TRY_CAST(NULLIF([13229_2009_NoofCampaign], '') AS INT) AS SyrupSimplexNoofCampaign,

                    [13229_2004_NextProcessName] AS SyrupSimplexNextProcessName,
                    [13229_2004_ProductionPlanningProcess] AS SyruppreparationProcessName,
                    [13202_Location] AS SyruppreparationLocation,

                    TRY_CAST(NULLIF([13203_1PreparationfirstVolumnHour], '') AS DECIMAL(18,4)) AS SyruppreparationFirstVolumnHour,
                    TRY_CAST(NULLIF([13204_1PreparationFirstVolumnManpower], '') AS INT) AS SyruppreparationFirstVolumnManpower,
                    [13205_2IPQCtest] AS SyruppreparationIPQCTest,
                    TRY_CAST(NULLIF([13206_3PreparationTopuptoVolumnHour], '') AS DECIMAL(18,4)) AS SyruppreparationTopupToVolumnHour,
                    TRY_CAST(NULLIF([13207_3PreparationTopuptoVolumnManpower], '') AS INT) AS SyruppreparationTopupToVolumnManpower,
                    [13208_CampaignBatchesNumbers] AS SyruppreparationCampaignBatchesNumbers,
                    TRY_CAST(NULLIF([13209_Level1CleaningHours], '') AS DECIMAL(18,4)) AS SyruppreparationLevel1CleaningHours,
                    TRY_CAST(NULLIF([13210_Level1Cleaningmanpower], '') AS INT) AS SyruppreparationLevel1Cleaningmanpower,
                    TRY_CAST(NULLIF([13211_Level2Cleaninghours], '') AS DECIMAL(18,4)) AS SyruppreparationLevel2Cleaninghours,
                    TRY_CAST(NULLIF([13212_Level2CleaningManpower], '') AS INT) AS SyruppreparationLevel2CleaningManpower,
                    [13264_ProductionPlanningProcess] AS SyruppreparationNextProcessName,
                    ModifiedBy,
                    ModifiedDate
                FROM DynamicForm_ProductiontimingNMachineInfosyrup";
                var rows = (await connection.QueryAsync<PrimaryProductionDto>(PrimarySelectSql)).ToList();
                return rows;
            }
        }

        public async Task<HashSet<long>> GetExistingDynamicFormIdsAsync(IEnumerable<long> ids)
        {
            using (var connection = CreateConnection())
            {
                if (ids == null) return new HashSet<long>();
                var list = ids.Where(i => i != 0).Distinct().ToArray();
                if (!list.Any()) return new HashSet<long>();
                var sql = "SELECT DynamicFormDataID FROM SyrupPlanning WHERE DynamicFormDataID IN @Ids";
                var exist = await connection.QueryAsync<long>(sql, new { Ids = list });
                return exist.ToHashSet();
            }
        }


        public async Task<bool> ExistsInSyrupPlanningAsync(long dynamicFormDataId, int week, int month, int year)
        {
            if (dynamicFormDataId <= 0) return false;

            const string sql = @"SELECT TOP 1 1 FROM SyrupPlanning WHERE DynamicFormDataID = @DynamicFormDataID  AND ISNULL(WeekOfMonth, 0) = @WeekOfMonth  AND ISNULL(Month, 0) = @Month  AND ISNULL(Year, 0) = @Year";
            using (var connection = CreateConnection())
            {
                var found = await connection.QueryFirstOrDefaultAsync<int?>(sql, new
                {
                    DynamicFormDataID = dynamicFormDataId,
                    WeekOfMonth = week,
                    Month = month,
                    Year = year
                });

                return found.HasValue && found.Value == 1;
            }


        }

        public async Task<bool> ExistsGeneratePlanningAsync(int week,int year)
        {
            if (week <= 0) return false;

            const string sql = @"SELECT TOP 1 1 FROM SyrupPlanning WHERE ISNULL(WeekOfMonth, 0) = @WeekOfMonth  AND ISNULL(Year, 0) = @Year";
            using (var connection = CreateConnection())
            {
                var found = await connection.QueryFirstOrDefaultAsync<int?>(sql, new
                {
                   
                    WeekOfMonth = week,                    
                    Year = year
                });

                return found.HasValue && found.Value == 1;
            }

           
        }


        public async Task<SyncResult> SyncPrimaryToSyrupPlanningAsync()
        {
            var primaryRows = await GetPrimaryRowsAsync();
            var ids = primaryRows.Select(r => r.DynamicFormDataID).Distinct().ToList();


            var insertedCount = 0;
            var skipped = 0;

            var rowsToInsert = new List<SyrupPlanning>();
            //var existing = await GetExistingDynamicFormIdsAsync(ids);
            foreach (var p in primaryRows)
            {
                var refDate = DateTime.Now;
                var week = GetWeekOfMonth(refDate);
                var month = refDate.Month;
                var year = refDate.Year;

                var exists = await ExistsInSyrupPlanningAsync(p.DynamicFormDataID, week, month, year);
                if (exists)
                {
                    skipped++;
                    continue;
                }


                var syrup = MapPrimaryToSyrup(p); // use your existing mapper
                syrup.WeekOfMonth = week;
                syrup.AddedByUserID = 1;
                syrup.ModifiedByUserID = 1;
                syrup.Month = month;
                syrup.Year = year;
                syrup.AddedDate = refDate;
                syrup.ModifiedDate = refDate;

                //rowsToInsert.Add(syrup);
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var insertedId = await connection.QuerySingleAsync<long>(InsertSyrupPlanningSql, syrup);


                        var packingRows = (await connection.QueryAsync<PackingDto>(packingSelectPerId, new { Id = p.DynamicFormDataID })).ToList();

                        // map packingRows -> SyrupFilling (set SyrupPlanningID to insertedId)
                        var childFillings = packingRows.Select(r =>
                        {
                            return new SyrupPlanning.SyrupFilling
                            {
                                SyrupPlanningID = insertedId,
                                DynamicFormID = r.DynamicFormID,
                                DynamicFormDataID = r.DynamicFormDataID,
                                DynamicFormDataItemID = r.DynamicFormDataItemID,
                                ProcessName_Primary = r.ProcessName_Primary ?? "",
                                ProfileNo = r.ProfileNo ?? "",
                                PlanningType_Primary = r.PlanningType_Primary,
                                ModifiedBy = r.ModifiedBy ?? "",
                                ModifiedDate = r.ModifiedDate ?? DateTime.UtcNow,
                                PrimaryFillingMachine = r.PrimaryFillingMachine,
                                FillingHours_Level1 = r.FillingHours_Level1,
                                FillingManpower_Level1 = r.FillingManpower_Level1,
                                Speed_BottlePerMinute = r.Speed_BottlePerMinute,
                                ChangePackingFillingHours = r.ChangePackingFillingHours,
                                FillingHours_Level2 = r.FillingHours_Level2,
                                FillingManpower_Level2 = r.FillingManpower_Level2,
                                SecondarySameAsPrimaryTime = r.SecondarySameAsPrimaryTime,
                                SecondaryPackingHours = r.SecondaryPackingHours,
                                SecondaryManpower = r.SecondaryManpower,
                                ProcessName_Secondary = r.ProcessName_Secondary,
                                NextProcessName_Secondary = r.NextProcessName_Secondary,
                                RequireOfflinePacking = r.RequireOfflinePacking,
                                AddedByUserID = r.AddedByUserID,
                                Description = r.Description
                            };
                        }).ToList();

                        if (childFillings.Any())
                        {
                            await connection.ExecuteAsync(insertFillingSql, childFillings);
                        }

                        // 2) fetch otherRows for this DynamicFormDataID
                        var otherRows = (await connection.QueryAsync<OtherDto>(otherSelectPerId, new { Id = p.DynamicFormDataID })).ToList();

                        var childOthers = otherRows.Select(o => new SyrupPlanning.SyrupOtherProcess
                        {                            
                            DynamicFormDataItemID = o.DynamicFormDataItemID,
                            SyrupPlanningID = insertedId,
                            DynamicFormDataID = o.DynamicFormDataID,
                            OtherJobsInformation = o.OtherJobsInformation,
                            ProcessName = o.ProcessName,
                            ProfileNo = o.ProfileNo,
                            SyrupOtherProcessNextProcess = o.SyrupOtherProcessNextProcess,
                            MustCompleteForNext = o.MustCompleteForNext,
                            LocationOfProcess = o.LocationOfProcess,
                            ManhoursOrHours = o.ManhoursOrHours,
                            NoOfManpower = o.NoOfManpower,
                            ManpowerFromNextProcess = o.ManpowerFromNextProcess,
                            CanCarryoutOnNonWorkday = o.CanCarryoutOnNonWorkday,
                            TimeGapHour = o.TimeGapHour,
                            AddedByUserID = o.AddedByUserID,
                            Description = o.Description,
                            ModifiedBy = o.ModifiedBy,
                            ModifiedDate = DateTime.Now
                        }).ToList();

                        if (childOthers.Any())
                        {
                            await connection.ExecuteAsync(insertOtherSql, childOthers);
                        }



                    }
                    catch
                    {
                        // Handle exceptions
                    }
                }

                insertedCount++;
            }

            return new SyncResult
            {
                ReadCount = primaryRows.Count,
                InsertedCount = insertedCount,
                SkippedCount = skipped
            };

            //if (!rowsToInsert.Any())
            // return new SyncResult { ReadCount = primaryRows.Count, InsertedCount = 0, SkippedCount = primaryRows.Count };

            //using (var connection = CreateConnection())
            //{
            //        try
            //        {

            //            var affected = await connection.ExecuteAsync(InsertSyrupPlanningSql, rowsToInsert);                     

            //            return new SyncResult
            //            {
            //                ReadCount = primaryRows.Count,
            //                InsertedCount = affected,
            //                SkippedCount = primaryRows.Count - rowsToInsert.Count
            //            };
            //        }
            //        catch
            //        {

            //            throw;
            //        }               

            //}

        }
        private SyrupPlanning MapPrimaryToSyrup(PrimaryProductionDto s)
        {
            var md = s.ModifiedDate ?? DateTime.UtcNow;
            var syrup = new SyrupPlanning
            {
                DynamicFormDataItemID = s.DynamicFormDataItemID ?? 0,
                MethodCodeLineID = 0,
                DynamicFormDataID = s.DynamicFormDataID,
                MethodName = s.MethodName,
                MethodCodeID = s.MethodCodeID ?? 0,
                ProfileNo = s.ProfileNo,
                MethodCode = null, // source didn't provide MethodCode text; only MethodCodeID. set null or map appropriately
                BatchSizeInLiters = s.BatchSizeInLiters ?? 0m,
                RestrictionOnPlanningDay = null,
                ProcessName = s.SyrupSimplexProcessName,
                IsthereSyrupSimplextoproduce = s.IsthereSyrupSimplextoproduce,
                SyrupSimplexProcessName = s.SyrupSimplexProcessName,
                SyrupSimplexLocation = s.SyrupSimplexLocation,
                SyrupSimplexPreparationHour = s.SyrupSimplexPreparationHour?.ToString(CultureInfo.InvariantCulture),
                SyrupSimplexManpower = s.SyrupSimplexManpower ?? 0,
                SyrupSimplexLevel2CleaningHours = s.SyrupSimplexLevel2CleaningHours?.ToString(CultureInfo.InvariantCulture),
                SyrupSimplexLevel2CleaningManpower = s.SyrupSimplexLevel2CleaningManpower ?? 0,
                SyrupSimplexNoofCampaign = s.SyrupSimplexNoofCampaign?? 0,
                SyrupSimplexNextProcessName = s.SyrupSimplexNextProcessName,

                SyruppreparationProcessName = s.SyruppreparationProcessName,
                SyruppreparationLocation = s.SyruppreparationLocation,
                SyruppreparationFirstVolumnHour = s.SyruppreparationFirstVolumnHour?.ToString(CultureInfo.InvariantCulture),
                SyruppreparationFirstVolumnManpower = s.SyruppreparationFirstVolumnManpower?.ToString(CultureInfo.InvariantCulture),
                SyruppreparationIPQCTest = s.SyruppreparationIPQCTest,
                SyruppreparationTopupToVolumnHour = s.SyruppreparationTopupToVolumnHour?.ToString(CultureInfo.InvariantCulture),
                SyruppreparationTopupToVolumnManpower = s.SyruppreparationTopupToVolumnManpower?.ToString(),
                SyruppreparationCampaignBatchesNumbers = s.SyruppreparationCampaignBatchesNumbers,
                SyruppreparationLevel1CleaningHours = s.SyruppreparationLevel1CleaningHours?.ToString(CultureInfo.InvariantCulture),
                SyruppreparationLevel1Cleaningmanpower = s.SyruppreparationLevel1Cleaningmanpower?.ToString(CultureInfo.InvariantCulture),
                SyruppreparationLevel2Cleaninghours = s.SyruppreparationLevel2Cleaninghours?.ToString(CultureInfo.InvariantCulture),
                SyruppreparationLevel2CleaningManpower = s.SyruppreparationLevel2CleaningManpower?.ToString(CultureInfo.InvariantCulture),
                SyruppreparationNextProcessName = s.SyruppreparationNextProcessName,

                PreparationPerHour = 0m,
                Level2CleaningHours = s.SyrupSimplexLevel2CleaningHours ?? s.SyruppreparationLevel2Cleaninghours ?? 0m,
                Level2CleaningManpower = s.SyrupSimplexLevel2CleaningManpower ?? s.SyruppreparationLevel2CleaningManpower ?? 0,
                NoOfCampaign = s.SyrupSimplexNoofCampaign?.ToString(),
                NextProcessName = s.SyruppreparationNextProcessName,

                PreparationFirstVolumePerHour = s.SyruppreparationFirstVolumnHour,
                PreparationFirstVolumeManpower = s.SyruppreparationFirstVolumnManpower ?? 0,
                IpqcTestRequired = ConvertToBool(s.SyruppreparationIPQCTest),
                PreparationTopUpPerHour = s.SyruppreparationTopupToVolumnHour,
                PreparationTopUpManpower = s.SyruppreparationTopupToVolumnManpower,
                CampaignBatches = TryParseInt(s.SyruppreparationCampaignBatchesNumbers) ?? 0,
                Level1CleaningHours = s.SyruppreparationLevel1CleaningHours,
                Level1CleaningManpower = s.SyruppreparationLevel1Cleaningmanpower,
                SyrupLevel2CleaningHours = s.SyruppreparationLevel2Cleaninghours,
                SyrupLevel2CleaningManpower = s.SyruppreparationLevel2CleaningManpower,
                NextProcessNameAfterPreparation = s.SyruppreparationNextProcessName,

                AddedByUserID = null,
                ModifiedByUserID = null,
                AddedDate = md,
                ModifiedDate = md,
                WeekOfMonth = GetWeekOfMonth(DateTime.Now),
                Month = md.Month,
                Year = md.Year
            };

            return syrup;
        }
        private static bool ConvertToBool(string? v)
        {
            if (string.IsNullOrWhiteSpace(v)) return false;
            v = v.Trim().ToLowerInvariant();
            if (v == "1" || v == "yes" || v == "true" || v == "y") return true;
            if (v == "0" || v == "no" || v == "false" || v == "n") return false;
            if (int.TryParse(v, out var n)) return n != 0;
            return false;
        }
        private static int? TryParseInt(string? v)
        {
            if (string.IsNullOrWhiteSpace(v)) return null;
            if (int.TryParse(v, out var n)) return n;
            var digits = new string(v.Where(char.IsDigit).ToArray());
            if (int.TryParse(digits, out n)) return n;
            return null;
        }
    }
}
