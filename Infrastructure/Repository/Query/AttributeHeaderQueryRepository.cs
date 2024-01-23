using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
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
    public class AttributeHeaderQueryRepository : QueryRepository<AttributeHeader>, IAttributeQueryRepository
    {
        private readonly IDynamicFormDataSourceQueryRepository _dynamicFormDataSourceQueryRepository;

        public AttributeHeaderQueryRepository(IConfiguration configuration, IDynamicFormDataSourceQueryRepository dynamicFormDataSourceQueryRepository)
            : base(configuration)
        {
            _dynamicFormDataSourceQueryRepository = dynamicFormDataSourceQueryRepository;
        }

        public async Task<long> DeleteAsync(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("id", id);

                        var query = "DELETE  FROM AttributeHeader WHERE AttributeID = @id";


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

        public async Task<IReadOnlyList<AttributeHeader>> GetAllAsync(long ID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ID", ID, DbType.Int64);

                var query = "SELECT * FROM AttributeHeader Where AttributeID = @ID ";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AttributeHeader>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<AttributeHeader>> GetAllAttributeName()
        {
            try
            {
                var query = "select t1.*,t2.CodeValue as ControlType from AttributeHeader t1  JOIN CodeMaster t2 ON t2.CodeID=t1.ControlTypeId";


                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AttributeHeader>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<AttributeHeaderDataSource>> GetAttributeHeaderDataSource()
        {
            try
            {
                var query = "select t1.* from AttributeHeaderDataSource t1";


                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AttributeHeaderDataSource>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<AttributeHeader>> GetAllAttributeNameNotInDynamicForm(long? dynamicFormSectionId, long? attributeID)
        {
            try
            {
                var query = "select t1.*,t2.CodeValue as ControlType,t4.DisplayName as DataSourceDisplayName,t4.DataSourceTable from AttributeHeader t1  JOIN CodeMaster t2 ON t2.CodeID=t1.ControlTypeId\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t4 ON t4.AttributeHeaderDataSourceID=t1.DataSourceId WHERE \r\n" +
                    "t1.AttributeID \r\n" +
                    "Not In (select t3.AttributeID from DynamicFormSectionAttribute t3 where t3.AttributeID>0  AND t3.DynamicFormSectionID=" + dynamicFormSectionId + ") \r\n" +
                    "or t1.AttributeID=" + attributeID + "";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AttributeHeader>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public IReadOnlyList<AttributeHeader> GetAllAttributeNameCheckValidation(string? value)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("AttributeName", value, DbType.Int64);
                var query = "select * from AttributeHeader where AttributeName=@AttributeName";
                using (var connection = CreateConnection())
                {
                    return connection.Query<AttributeHeader>(query).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<AttributeHeaderListModel> GetAllAttributeNameAsync(DynamicForm dynamicForm, long? UserId)
        {
            try
            {
                AttributeHeaderListModel attributeHeaderListModel = new AttributeHeaderListModel();

                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync("select t1.DynamicFormSectionID,t1.SectionName,t1.SessionID,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.SortOrderBy," +
                        "(select t4.IsVisible from DynamicFormSectionSecurity t4 Where t4.UserID=" + UserId + " AND  t4.DynamicFormSectionID=t1.DynamicFormSectionID) as IsVisible," +
                        "(select t4.IsReadOnly from DynamicFormSectionSecurity t4 Where t4.UserID=" + UserId + " AND  t4.DynamicFormSectionID=t1.DynamicFormSectionID) as IsReadOnly," +
                        "(select t4.IsReadWrite from DynamicFormSectionSecurity t4 Where t4.UserID=" + UserId + " AND  t4.DynamicFormSectionID=t1.DynamicFormSectionID) as IsReadWrite," +
                        "(select COUNT(t2.UserID) from DynamicFormSectionSecurity t2 Where t2.DynamicFormSectionID=t1.DynamicFormSectionID) as IsPermissionCount," +
                        "(select COUNT(t3.UserID) from DynamicFormSectionSecurity t3 Where t3.UserID=1 AND  t3.DynamicFormSectionID=t1.DynamicFormSectionID) as IsLoginUsers " +
                        "from DynamicFormSection t1 where t1.DynamicFormID=" + dynamicForm.ID + " order by  t1.SortOrderBy asc;" +
                        "select t1.*,(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,t5.SectionName,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.DataSourceId,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t7.CodeValue as ControlType,t5.DynamicFormID from DynamicFormSectionAttribute t1\r\n" +
                        "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                        "JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                        "LEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID\r\n" +
                        "JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\nWhere (t1.IsVisible= 1 OR t1.IsVisible is null) AND t5.DynamicFormID=" + dynamicForm.ID + " order by t1.SortOrderBy asc;");
                    attributeHeaderListModel.DynamicFormSection = results.Read<DynamicFormSection>().ToList();
                    attributeHeaderListModel.DynamicFormSectionAttribute = results.Read<DynamicFormSectionAttribute>().ToList();
                }
                if (attributeHeaderListModel.DynamicFormSectionAttribute != null)
                {
                    List<long?> attributeIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.AttributeId > 0).Select(a => a.AttributeId).ToList();
                    List<string?> dataSourceTableIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.DataSourceTable != null).Select(a => a.DataSourceTable).Distinct().ToList();
                    attributeHeaderListModel.AttributeDetails = await GetAttributeDetails(attributeIds);
                    var dataSourceList = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(dynamicForm.CompanyId, dataSourceTableIds);
                    attributeHeaderListModel.AttributeDetails.AddRange(dataSourceList);
                    attributeHeaderListModel.DynamicFormSectionAttribute.ForEach(s =>
                    {
                        s.AttributeName = string.IsNullOrEmpty(s.AttributeName) ? string.Empty : char.ToUpper(s.AttributeName[0]) + s.AttributeName.Substring(1);
                        s.DynamicAttributeName = s.DynamicFormSectionAttributeId + "_" + s.AttributeName;
                        if (s.ControlType == "TextBox" || s.ControlType == "Memo")
                        {
                            s.DataType = typeof(string);
                        }
                        else if (s.ControlType == "ComboBox" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                        {
                            s.DataType = typeof(long?);
                        }
                        else if (s.ControlType == "DateEdit")
                        {
                            s.DataType = typeof(DateTime?);
                        }
                        else if (s.ControlType == "TimeEdit")
                        {
                            s.DataType = typeof(TimeSpan?);
                        }
                        else if (s.ControlType == "SpinEdit")
                        {
                            if (s.IsSpinEditType == "decimal")
                            {
                                s.DataType = typeof(decimal?);
                            }
                            else
                            {
                                s.DataType = typeof(int?);
                            }
                        }
                        else if (s.ControlType == "CheckBox")
                        {
                            s.DataType = typeof(bool?);
                        }
                        else if (s.ControlType == "TagBox")
                        {
                            s.DataType = typeof(IEnumerable<long?>);
                        }
                        else if (s.ControlType == "ListBox")
                        {
                            if (s.IsMultiple == true)
                            {
                                s.DataType = typeof(IEnumerable<long?>);
                            }
                            else
                            {
                                s.DataType = typeof(long?);
                            }
                        }
                        else
                        {
                            s.DataType = typeof(string);
                        }
                    });

                }
                return attributeHeaderListModel;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<AttributeDetails>> GetAttributeDetails(List<long?> id)
        {
            try
            {
                id = id != null && id.Count > 0 ? id : new List<long?>() { -1 };
                var query = "select  * from AttributeDetails where AttributeID in(" + string.Join(',', id) + ")";
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                    return result != null && result.Count() > 0 ? result : new List<AttributeDetails>();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<AttributeHeader> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM AttributeHeader WHERE AttrubuteID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<AttributeHeader>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<DynamicForm>> GetComboBoxList()
        {
            try
            {
                var query = "select DISTINCT ScreenID from  DynamicForm";


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

        public async Task<long> Insert(AttributeHeader attributeHeader)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        if (attributeHeader.AttributeID > 0)
                        {
                            parameters.Add("AttributeID", attributeHeader.AttributeID);
                            parameters.Add("AttributeName", attributeHeader.AttributeName, DbType.String);
                            parameters.Add("IsInternal", attributeHeader.IsInternal);
                            parameters.Add("Description", attributeHeader.Description, DbType.String);
                            parameters.Add("AttributeCompanyId", attributeHeader.AttributeCompanyId);
                            parameters.Add("ControlType", attributeHeader.ControlType);
                            parameters.Add("EntryMask", attributeHeader.EntryMask);
                            parameters.Add("RegExp", attributeHeader.RegExp);
                            parameters.Add("ModifiedByUserID", attributeHeader.ModifiedByUserID);
                            parameters.Add("ModifiedDate", attributeHeader.ModifiedDate);
                            parameters.Add("StatusCodeID", attributeHeader.StatusCodeID);
                            parameters.Add("ControlTypeId", attributeHeader.ControlTypeId);
                            parameters.Add("IsMultiple", attributeHeader.IsMultiple);
                            parameters.Add("IsRequired", attributeHeader.IsRequired);
                            parameters.Add("DropDownTypeId", attributeHeader.DropDownTypeId);
                            parameters.Add("DataSourceId", attributeHeader.DataSourceId);
                            parameters.Add("RequiredMessage", attributeHeader.RequiredMessage, DbType.String);
                            var Addquerys = "UPDATE AttributeHeader SET AttributeName = @AttributeName,IsInternal=@IsInternal,Description=@Description," +
                                "ControlType=@ControlType,EntryMask=@EntryMask, " +
                                "RegExp=@RegExp,ModifiedByUserID=@ModifiedByUserID, " +
                                "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID, " +
                                "ControlTypeId=@ControlTypeId,IsMultiple=@IsMultiple, " +
                                "IsRequired=@IsRequired,RequiredMessage=@RequiredMessage,DropDownTypeId=@DropDownTypeId,DataSourceId=@DataSourceId,AttributeCompanyId=@AttributeCompanyId " +
                                "WHERE  AttributeID = @AttributeID";
                            await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parameters);
                        }
                        else
                        {
                            parameters.Add("AttributeCompanyId", attributeHeader.AttributeCompanyId);
                            parameters.Add("AttributeName", attributeHeader.AttributeName, DbType.String);
                            parameters.Add("IsInternal", attributeHeader.IsInternal);
                            parameters.Add("Description", attributeHeader.Description, DbType.String);
                            parameters.Add("DropDownTypeId", attributeHeader.DropDownTypeId);
                            parameters.Add("DataSourceId", attributeHeader.DataSourceId);
                            parameters.Add("ControlType", attributeHeader.ControlType);
                            parameters.Add("EntryMask", attributeHeader.EntryMask);
                            parameters.Add("RegExp", attributeHeader.RegExp);
                            parameters.Add("AddedByUserID", attributeHeader.AddedByUserID);
                            parameters.Add("AddedDate", attributeHeader.AddedDate);
                            parameters.Add("SessionId", attributeHeader.SessionId);
                            parameters.Add("StatusCodeID", attributeHeader.StatusCodeID);
                            parameters.Add("ControlTypeId", attributeHeader.ControlTypeId);
                            parameters.Add("IsMultiple", attributeHeader.IsMultiple);
                            parameters.Add("IsRequired", attributeHeader.IsRequired);
                            parameters.Add("RequiredMessage", attributeHeader.RequiredMessage, DbType.String);


                            var query = @"INSERT INTO AttributeHeader(AttributeCompanyId,AttributeName,IsInternal,Description,ControlType,EntryMask,RegExp,AddedByUserID,AddedDate,SessionId,StatusCodeID,ControlTypeId,IsMultiple,IsRequired,RequiredMessage,DropDownTypeId,DataSourceId) 
              OUTPUT INSERTED.AttributeID  -- Replace 'YourIDColumn' with the actual column name of your IDENTITY column
              VALUES (@AttributeCompanyId,@AttributeName,@IsInternal,@Description,@ControlType,@EntryMask,@RegExp,@AddedByUserID,@AddedDate,@SessionId,@StatusCodeID,@ControlTypeId,@IsMultiple,@IsRequired,@RequiredMessage,@DropDownTypeId,@DataSourceId)";

                            var insertedId = await connection.ExecuteScalarAsync<int>(query, parameters);
                            attributeHeader.AttributeID = insertedId;
                        }

                        return attributeHeader.AttributeID;

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

        public Task<long> UpdateAsync(AttributeHeader attributeHeader)
        {
            throw new NotImplementedException();
        }
    }
}
