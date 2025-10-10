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
                var query = @"SELECT *,CONCAT(P.PlantCode ,'-', P.Description)  as CompanyName,CONCAT(C.Name,'-',C.Code) as CustomerName,AMD.Value as PlanningCategoryName from StockInformationMaster SM
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
            const string sql = @"select DynamicFormDataID, [13229_IsthereSyrupSimplextoproduce] as IsthereSyrupSimplextoproduce,
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
                var query = @"select DynamicFormDataItemID as ID,ProfileNo,ModifiedBy,ModifiedDate from DynamicForm_ProductionTimingSyrupOthers
                                where DynamicFormDataGridId = 85012";
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

    }
}
