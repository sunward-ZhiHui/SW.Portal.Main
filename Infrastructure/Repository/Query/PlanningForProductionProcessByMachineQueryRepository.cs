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
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Dynamic;
using System.Data.SqlClient;
using Newtonsoft.Json.Serialization;

namespace Infrastructure.Repository.Query
{
    public class PlanningForProductionProcessByMachineQueryRepository : DbConnector, IPlanningForProductionProcessByMachineQueryRepository
    {
        public PlanningForProductionProcessByMachineQueryRepository(IConfiguration configuration)
            : base(configuration)
        {
        }
        public async Task<IReadOnlyList<PlanningForProductionProcessByMachine>> GetAllByAsync()
        {
            try
            {
                var query = "select t1.*,t2.Value as TypeOfProduction,t7.Value as TypeOfProcess,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser,t6.Value as ProductionPlanningProcess from PlanningForProductionProcessByMachine t1\r\nLEFT JOIN ApplicationMasterDetail t2 ON t2.ApplicationMasterDetailID=t1.TypeOfProductionID\r\nLEFT JOIN ApplicationMasterDetail t6 ON t6.ApplicationMasterDetailID=t1.ProductionPlanningProcessID\r\nLEFT JOIN ApplicationMasterDetail t7 ON t7.ApplicationMasterDetailID=t1.TypeOfProcessID\r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nJOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\n";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<PlanningForProductionProcessByMachine>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<PlanningForProductionProcessByMachine> GetPlanningForProductionProcessByMachineSession(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);
                var query = "select t1.*,t2.Value as TypeOfProduction,t7.Value as TypeOfProcess,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser,t6.Value as ProductionPlanningProcess from PlanningForProductionProcessByMachine t1\r\nJOIN ApplicationMasterDetail t2 ON t2.ApplicationMasterDetailID=t1.TypeOfProductionID\r\nJOIN ApplicationMasterDetail t6 ON t6.ApplicationMasterDetailID=t1.ProductionPlanningProcessID\r\nJOIN ApplicationMasterDetail t7 ON t7.ApplicationMasterDetailID=t1.TypeOfProcessID\r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nJOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID where t1.SessionId=@SessionId;\r\n";
                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<PlanningForProductionProcessByMachine>(query,parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<PlanningForProductionProcessByMachine> DeletePlanningForProductionProcessByMachine(PlanningForProductionProcessByMachine value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("PlanningForProductionProcessByMachineId", value.PlanningForProductionProcessByMachineId);
                        var query = "DELETE FROM PlanningForProductionProcessByMachineRelated WHERE PlanningForProductionProcessByMachineId= @PlanningForProductionProcessByMachineId;";
                        query += "DELETE FROM PlanningForProductionProcessByMachine WHERE PlanningForProductionProcessByMachineId= @PlanningForProductionProcessByMachineId";
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
        public async Task<PlanningForProductionProcessByMachine> InsertOrUpdatePlanningForProductionProcessByMachine(PlanningForProductionProcessByMachine value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("TypeOfProductionID", value.TypeOfProductionId);
                    parameters.Add("ProductionPlanningProcessID", value.ProductionPlanningProcessId);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("TypeOfProcessID", value.TypeOfProcessId);
                    parameters.Add("PlanningForProductionProcessByMachineId", value.PlanningForProductionProcessByMachineId);
                    parameters.Add("SquenceForPlanning", value.SquenceForPlanning, DbType.String);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    parameters.Add("SessionId", value.SessionId, DbType.Guid);
                    if (value.PlanningForProductionProcessByMachineId > 0)
                    {
                        var query = "UPDATE PlanningForProductionProcessByMachine SET AddedByUserID=@AddedByUserID,ModifiedByUserID=@ModifiedByUserID,AddedDate=@AddedDate,ModifiedDate=@ModifiedDate,SessionId=@SessionId,SquenceForPlanning=@SquenceForPlanning,TypeOfProductionID=@TypeOfProductionID,ProductionPlanningProcessID=@ProductionPlanningProcessID,TypeOfProcessId=@TypeOfProcessId,StatusCodeID = @StatusCodeID\r" +
                            "WHERE PlanningForProductionProcessByMachineId = @PlanningForProductionProcessByMachineId";
                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO PlanningForProductionProcessByMachine(SquenceForPlanning,TypeOfProductionID,ProductionPlanningProcessID,StatusCodeID,TypeOfProcessId,SessionId,AddedByUserID," +
                         "ModifiedByUserID,AddedDate,ModifiedDate) OUTPUT INSERTED.PlanningForProductionProcessByMachineId VALUES " +
                            "(@SquenceForPlanning,@TypeOfProductionID,@ProductionPlanningProcessID,@StatusCodeID,@TypeOfProcessId,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        value.PlanningForProductionProcessByMachineId = rowsAffected;
                    }
                    return value;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<PlanningForProductionProcessByMachineRelated>> GetAllPlanningForProductionProcessByMachineRelatedAsync(long? PlanningForProductionProcessByMachineId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("PlanningForProductionProcessByMachineId", PlanningForProductionProcessByMachineId);
                var query = "select t1.*,t6.ProfileNo as AssetIDWithModel,t2.Value as PlanningForProductionProcessByMachine,t3.CodeValue as StatusCode,t4.UserName as AddedByUser,t5.UserName as ModifiedByUser from PlanningForProductionProcessByMachineRelated t1\r\nLEFT JOIN ApplicationMasterDetail t2 ON t2.ApplicationMasterDetailID=t1.FixAssetMachineNameRequipmentID\r\nJOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID\r\nJOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\nLEFT JOIN DynamicFormData t6 ON t6.DynamicFormDataID=t1.AssetIDWithModelId where t1.PlanningForProductionProcessByMachineId=@PlanningForProductionProcessByMachineId\r\n";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<PlanningForProductionProcessByMachineRelated>(query,parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<PlanningForProductionProcessByMachineRelated> InsertOrUpdatePlanningForProductionProcessByMachineRelated(PlanningForProductionProcessByMachineRelated value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("PlanningForProductionProcessByMachineRelatedId", value.PlanningForProductionProcessByMachineRelatedId);
                    parameters.Add("FixAssetMachineNameRequipmentId", value.FixAssetMachineNameRequipmentId);
                    parameters.Add("StatusCodeID", value.StatusCodeID);
                    parameters.Add("AssetIDWithModelId", value.AssetIDWithModelId);
                    parameters.Add("PlanningForProductionProcessByMachineId", value.PlanningForProductionProcessByMachineId);
                    parameters.Add("AddedByUserID", value.AddedByUserID);
                    parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                    parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                    parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                    parameters.Add("SessionId", value.SessionId, DbType.Guid);
                    if (value.PlanningForProductionProcessByMachineRelatedId > 0)
                    {
                        var query = "UPDATE PlanningForProductionProcessByMachineRelated SET AssetIDWithModelId=@AssetIDWithModelId,PlanningForProductionProcessByMachineId=@PlanningForProductionProcessByMachineId,AddedByUserID=@AddedByUserID,ModifiedByUserID=@ModifiedByUserID,AddedDate=@AddedDate,ModifiedDate=@ModifiedDate,SessionId=@SessionId,FixAssetMachineNameRequipmentId=@FixAssetMachineNameRequipmentId,StatusCodeID = @StatusCodeID\r" +
                            "WHERE PlanningForProductionProcessByMachineRelatedId = @PlanningForProductionProcessByMachineRelatedId";
                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO PlanningForProductionProcessByMachineRelated(AssetIDWithModelId,PlanningForProductionProcessByMachineId,FixAssetMachineNameRequipmentId,StatusCodeID,SessionId,AddedByUserID," +
                         "ModifiedByUserID,AddedDate,ModifiedDate) OUTPUT INSERTED.PlanningForProductionProcessByMachineRelatedId VALUES " +
                            "(@AssetIDWithModelId,@PlanningForProductionProcessByMachineId,@FixAssetMachineNameRequipmentId,@StatusCodeID,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        value.PlanningForProductionProcessByMachineRelatedId = rowsAffected;
                    }
                    return value;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<PlanningForProductionProcessByMachineRelated> DeletePlanningForProductionProcessByMachineRelated(PlanningForProductionProcessByMachineRelated value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("PlanningForProductionProcessByMachineRelatedId", value.PlanningForProductionProcessByMachineRelatedId);
                        var query = "DELETE FROM PlanningForProductionProcessByMachineRelated WHERE PlanningForProductionProcessByMachineRelatedId= @PlanningForProductionProcessByMachineRelatedId;";
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
    }

}
