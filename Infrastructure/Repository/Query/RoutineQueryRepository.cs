using Core.Entities;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class RoutineQueryRepository : QueryRepository<ProductionActivityRoutineAppLine>, IRoutineQueryRepository
    {
        public RoutineQueryRepository(IConfiguration configuration)
           : base(configuration)
        {

        }

        public async Task<long> Delete(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("id", id);

                            var query = "DELETE  FROM ProductionActivityAppline WHERE ID = @id";


                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<ProductionActivityRoutineAppLine>> GetAllAsync()
        {
            try
            {
                var query = "select t1.* from ProductionActivityRoutineAppLine t1 ";
                   

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionActivityRoutineAppLine>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public ProductionActivityRoutineAppLine GetDynamicFormScreenNameCheckValidation(string? value, long id)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("ScreenID", value);
                if (id > 0)
                {
                    parameters.Add("ID", id);
                    parameters.Add("ScreenID", value);

                    query = "SELECT * FROM ProductionActivityRoutineAppLine Where ID!=@id AND ScreenID = @ScreenID";
                }
                else
                {
                    query = "SELECT * FROM ProductionActivityRoutineAppLine Where ScreenID = @ScreenID";
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<ProductionActivityRoutineAppLine>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Insert(ProductionActivityRoutineAppLine productionActivityRoutineAppLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("LocationId", productionActivityRoutineAppLine.LocationId);
                            parameters.Add("VisaMasterId", productionActivityRoutineAppLine.VisaMasterId);
                            parameters.Add("RoutineStatusId", productionActivityRoutineAppLine.RoutineStatusId);
                            parameters.Add("ProductActivityCaseLineId", productionActivityRoutineAppLine.ProductActivityCaseLineId);
                            parameters.Add("AddedByUserID", productionActivityRoutineAppLine.AddedByUserID);
                            parameters.Add("ModifiedByUserID", productionActivityRoutineAppLine.ModifiedByUserID);
                            parameters.Add("AddedDate", productionActivityRoutineAppLine.AddedDate, DbType.DateTime);
                            parameters.Add("ModifiedDate", productionActivityRoutineAppLine.ModifiedDate, DbType.DateTime);
                            parameters.Add("StatusCodeID", productionActivityRoutineAppLine.StatusCodeID);
                            parameters.Add("IsApproval", productionActivityRoutineAppLine.ProdActivityResultId);
                            parameters.Add("FileProfileTypeId", productionActivityRoutineAppLine.ProdActivityCategoryChildId);
                            parameters.Add("IsUpload", productionActivityRoutineAppLine.ProdActivityActionChildD);
                            var query = "INSERT INTO ProductionActivityRoutineAppLine(Name,ScreenID,SessionID,AttributeID,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsApproval,FileProfileTypeId,IsUpload) VALUES " +
                                "(@Name,@ScreenID,@SessionID,@AttributeID,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsApproval,@FileProfileTypeId,@IsUpload)";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }


                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }

            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<long> Update(DynamicForm dynamicForm)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("AttributeID", dynamicForm.AttributeID);
                            parameters.Add("ID", dynamicForm.ID);
                            parameters.Add("Name", dynamicForm.Name);
                            parameters.Add("ScreenID", dynamicForm.ScreenID);
                            parameters.Add("AddedByUserID", dynamicForm.AddedByUserID);
                            parameters.Add("ModifiedByUserID", dynamicForm.ModifiedByUserID);
                            parameters.Add("AddedDate", dynamicForm.AddedDate, DbType.DateTime);
                            parameters.Add("ModifiedDate", dynamicForm.ModifiedDate, DbType.DateTime);
                            parameters.Add("StatusCodeID", dynamicForm.StatusCodeID);
                            parameters.Add("IsApproval", dynamicForm.IsApproval);
                            parameters.Add("FileProfileTypeId", dynamicForm.FileProfileTypeId);
                            parameters.Add("IsUpload", dynamicForm.IsUpload);
                            var query = " UPDATE DynamicForm SET AttributeID = @AttributeID,Name =@Name,ScreenID =@ScreenID,ModifiedByUserID=@ModifiedByUserID," +
                                "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,IsApproval=@IsApproval,IsUpload=@IsUpload,FileProfileTypeId=@FileProfileTypeId WHERE ID = @ID";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public int? GeDynamicFormSectionSort(long? id)
        {
            try
            {
                int? SortOrderBy = 1;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", id);

                query = "SELECT * FROM DynamicFormSection Where DynamicFormId = @DynamicFormId order by  SortOrderBy desc";
                using (var connection = CreateConnection())
                {
                    var result = connection.QueryFirstOrDefault<DynamicFormSection>(query, parameters);
                    if (result != null)
                    {
                        SortOrderBy = result.SortOrderBy + 1;
                    }
                }
                return SortOrderBy;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
