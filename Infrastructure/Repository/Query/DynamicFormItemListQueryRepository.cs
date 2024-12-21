using Core.Entities;
using Core.Entities.Views;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Repository.Query
{
    public class DynamicFormItemListQueryRepository : QueryRepository<DynamicFormItem>, IDynamicFormItemQueryRepository
    {
        public DynamicFormItemListQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }


        public async Task<IReadOnlyList<DynamicFormItem>> GetAllAsync()
        {
            try
            {
                var query = "select DFI.AutoNumber,DFI.DynamicFormItemID,\r\nDFI.TransactionDate,DFI.SessionId,\r\nCM.CodeValue as TransactionTypeName,\r\nDFI.Reason,\r\nDFI.StockGroupAppParentID,DFI.DepartmentAppParentID,DFI.ItemGroupAppParentID,\r\nt1.Value as StockGroupAppParent,t2.Value as DepartmentAppParent,t3.Value as ItemGroupAppParent,\r\nPt.Description as CompanyName,DFI.Description FROM DynamicFormItem DFI \r\nLeft Join CodeMaster CM on CM.CodeID = DFI.TransactionTypeID \r\nLeft join Plant Pt on Pt.PlantID = DFI.CompanyId\r\nLeft join ApplicationMasterChild t1 on t1.ApplicationMasterChildID = DFI.StockGroupAppParentID\r\nLeft join ApplicationMasterChild t2 on t2.ApplicationMasterChildID = DFI.DepartmentAppParentID\r\nLeft join ApplicationMasterChild t3 on t3.ApplicationMasterChildID = DFI.ItemGroupAppParentID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormItem>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Insert(DynamicFormItem dynamicformitem)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("AutoNumber", dynamicformitem.AutoNumber);
                        parameters.Add("AddedByUserID", dynamicformitem.AddedByUserID);
                        parameters.Add("AddedDate", dynamicformitem.AddedDate, DbType.DateTime);
                        parameters.Add("Description", dynamicformitem.Description, DbType.String);
                        parameters.Add("StatusCodeID", dynamicformitem.StatusCodeID);
                        parameters.Add("TransactionDate", dynamicformitem.TransactionDate, DbType.DateTime);
                        parameters.Add("SessionId", dynamicformitem.SessionId, DbType.Guid);
                        parameters.Add("CompanyID", dynamicformitem.CompanyID);
                        parameters.Add("TransactionTypeID", dynamicformitem.TransactionTypeID);
                        parameters.Add("StockGroupAppParentID", dynamicformitem.StockGroupAppParentID);
                        parameters.Add("DepartmentAppParentID", dynamicformitem.DepartmentAppParentID);
                        parameters.Add("ItemGroupAppParentID", dynamicformitem.ItemGroupAppParentID);
                        parameters.Add("Reason", dynamicformitem.Reason, DbType.String);
                        var query = "INSERT INTO DynamicFormItem(AutoNumber,AddedByUserID,AddedDate,Description,StatusCodeID,TransactionDate,SessionId,CompanyID,TransactionTypeID,StockGroupAppParentID,DepartmentAppParentID,ItemGroupAppParentID,Reason) " +
                                    "OUTPUT INSERTED.DynamicFormItemID VALUES (@AutoNumber,@AddedByUserID,@AddedDate,@Description,@StatusCodeID,@TransactionDate,@SessionId,@CompanyID,@TransactionTypeID,@StockGroupAppParentID,@DepartmentAppParentID,@ItemGroupAppParentID,@Reason)";

                        var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        return lastInsertedRecordId;
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

        public async Task<long> Update(DynamicFormItem dynamicformitem)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("AutoNumber", dynamicformitem.AutoNumber);
                        parameters.Add("DynamicFormItemID", dynamicformitem.DynamicFormItemID);
                        parameters.Add("ModifiedDate", dynamicformitem.ModifiedDate, DbType.DateTime);
                        parameters.Add("ModifiedByUserID", dynamicformitem.ModifiedByUserID);
                        parameters.Add("Description", dynamicformitem.Description, DbType.String);
                        parameters.Add("StatusCodeID", dynamicformitem.StatusCodeID);
                        parameters.Add("TransactionDate", dynamicformitem.TransactionDate, DbType.DateTime);
                        parameters.Add("SessionId", dynamicformitem.SessionId, DbType.Guid);
                        parameters.Add("CompanyID", dynamicformitem.CompanyID);
                        parameters.Add("TransactionTypeID", dynamicformitem.TransactionTypeID);
                        parameters.Add("StockGroupAppParentID", dynamicformitem.StockGroupAppParentID);
                        parameters.Add("DepartmentAppParentID", dynamicformitem.DepartmentAppParentID);
                        parameters.Add("ItemGroupAppParentID", dynamicformitem.ItemGroupAppParentID);
                        parameters.Add("Reason", dynamicformitem.Reason, DbType.String);
                        var query = @"UPDATE DynamicFormItem SET Reason=@Reason,AutoNumber = @AutoNumber,StockGroupAppParentID=@StockGroupAppParentID,DepartmentAppParentID=@DepartmentAppParentID,ItemGroupAppParentID=@ItemGroupAppParentID,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,Description=@Description,StatusCodeID=@StatusCodeID,TransactionDate=@TransactionDate,SessionId=@SessionId,CompanyID=@CompanyID,TransactionTypeID=@TransactionTypeID where DynamicFormItemID = @DynamicFormItemID";



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



        public async Task<long> Delete(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);

                    var query = "DELETE  FROM DynamicFormItem WHERE DynamicFormItemID = @id";


                    var rowsAffected = await connection.ExecuteAsync(query, parameters);

                    return rowsAffected;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Update(DynamicFormItemLine DynamicFormItemLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormItemLineID", DynamicFormItemLine.DynamicFormItemLineID);
                        parameters.Add("DynamicFormItemID", DynamicFormItemLine.DynamicFormItemID);
                        parameters.Add("Qty", DynamicFormItemLine.Qty);
                        parameters.Add("ModifiedByUserID", DynamicFormItemLine.ModifiedByUserID);
                        parameters.Add("ModifiedDate", DynamicFormItemLine.ModifiedDate);
                        parameters.Add("ItemDynamicFormTypeID", DynamicFormItemLine.ItemDynamicFormTypeID);
                        parameters.Add("StatusCodeID", DynamicFormItemLine.StatusCodeID);
                        parameters.Add("ItemDynamicFormDataID", DynamicFormItemLine.ItemDynamicFormDataID);
                        parameters.Add("Description", DynamicFormItemLine.Description, DbType.String);
                        parameters.Add("SessionId", DynamicFormItemLine.SessionId, DbType.Guid);
                        parameters.Add("ManufacturingDate", DynamicFormItemLine.ManufacturingDate, DbType.DateTime);
                        parameters.Add("ExpireDate", DynamicFormItemLine.ExpireDate, DbType.DateTime);
                        parameters.Add("BatchNo", DynamicFormItemLine.BatchNo, DbType.String);
                        var query = @"UPDATE DynamicFormItemLine SET BatchNo=@BatchNo,ExpireDate=@ExpireDate,ManufacturingDate=@ManufacturingDate,DynamicFormItemID = @DynamicFormItemID, Qty = @Qty,ModifiedByUserID =@ModifiedByUserID, ModifiedDate = @ModifiedDate,
          ItemDynamicFormTypeID=@ItemDynamicFormTypeID,StatusCodeID=@StatusCodeID,ItemDynamicFormDataID=@ItemDynamicFormDataID,Description=@Description,SessionId=@SessionId
    
          where DynamicFormItemLineID = @DynamicFormItemLineID";



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

        public async Task<long> Insert(DynamicFormItemLine DynamicFormItemLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormItemID", DynamicFormItemLine.DynamicFormItemID);
                        parameters.Add("Qty", DynamicFormItemLine.Qty);
                        parameters.Add("ModifiedByUserID", DynamicFormItemLine.ModifiedByUserID);
                        parameters.Add("ModifiedDate", DynamicFormItemLine.ModifiedDate);
                        parameters.Add("ItemDynamicFormTypeID", DynamicFormItemLine.ItemDynamicFormTypeID);
                        parameters.Add("StatusCodeID", DynamicFormItemLine.StatusCodeID);
                        parameters.Add("ItemDynamicFormDataID", DynamicFormItemLine.ItemDynamicFormDataID);
                        parameters.Add("Description", DynamicFormItemLine.Description, DbType.String);
                        parameters.Add("SessionId", DynamicFormItemLine.SessionId, DbType.Guid);
                        parameters.Add("ManufacturingDate", DynamicFormItemLine.ManufacturingDate, DbType.DateTime);
                        parameters.Add("ExpireDate", DynamicFormItemLine.ExpireDate, DbType.DateTime);
                        parameters.Add("BatchNo", DynamicFormItemLine.BatchNo, DbType.String);
                        var query = "INSERT INTO DynamicFormItemLine(BatchNo,ExpireDate,ManufacturingDate,DynamicFormItemID,Qty,ModifiedByUserID,ModifiedDate,ItemDynamicFormTypeID,StatusCodeID,ItemDynamicFormDataID,Description,SessionId) " +
                                    "OUTPUT INSERTED.DynamicFormItemLineID VALUES (@BatchNo,@ExpireDate,@ManufacturingDate,@DynamicFormItemID,@Qty,@ModifiedByUserID,@ModifiedDate,@ItemDynamicFormTypeID,@StatusCodeID,@ItemDynamicFormDataID,@Description,@SessionId)";


                        var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        return lastInsertedRecordId;
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

        public async Task<long> DeleteLine(long DynamicFormItemLineID)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormItemLineID", DynamicFormItemLineID);

                        var query = "DELETE  FROM DynamicFormItemLine WHERE DynamicFormItemLineID = @DynamicFormItemLineID";


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

        public async Task<IReadOnlyList<DynamicFormItemLine>> GetAllDynamicLineAsync(long DynamicFormItemID)
        {

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormItemID", DynamicFormItemID);

                var query = "select t1.*,t2.Name as ItemDynamicFormType,t3.ProfileNo as ItemDynamicFormData from DynamicFormItemLine t1 LEFT JOIN \r\nDynamicForm t2 ON t1.ItemDynamicFormTypeID=t2.ID LEFT JOIN \r\nDynamicFormData t3 ON t3.DynamicFormDataID=t1.ItemDynamicFormDataID Where t1.DynamicFormItemID = @DynamicFormItemID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormItemLine>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }

        public async Task<IReadOnlyList<DynamicForm>> GetAllDynamicFormDropdownAsync()
        {
            try
            {
                var query = "select * from DynamicForm where name like '%Item Card -%'";

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

        public async Task<IReadOnlyList<DynamicFormItem>> GetAllDynamicAsync(long DynamicFormItemID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormItemID", DynamicFormItemID);

                var query = @"
							select DFI.AutoNumber,DFI.DynamicFormItemID,DFI.SessionId,TransactionDate,CM.CodeValue as TransactionTypeName,Pt.Description as CompanyName,DFI.Description
                            FROM DynamicFormItem DFI 
                Left Join CodeMaster CM on CM.CodeID = DFI.TransactionTypeID Left join Plant Pt on Pt.PlantID = DFI.CompanyId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormItem>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<DynamicFormItem> GetDynamicFormItemBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = @"select t1.*,Pt.Description as CompanyName , CM.CodeValue as TransactionTypeName from DynamicFormItem t1  
                     Left join Plant Pt on Pt.PlantID = t1.CompanyId
					  Left Join CodeMaster CM on CM.CodeID = t1.TransactionTypeID  
                    WHERE  t1.SessionId=@SessionId";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<DynamicFormItem>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
