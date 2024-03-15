using Core.Entities;
using Core.Entities.Views;
using Core.Helpers;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.Data.Edm.Values;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
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

                        //var query = "DELETE  FROM AttributeHeader WHERE AttributeID = @id";
                        var query = "Update  AttributeHeader  SET AttributeIsVisible=0  WHERE AttributeID = @id";


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

                var query = "SELECT *,(CASE WHEN AttributeIsVisible is null  THEN 1  ELSE AttributeIsVisible END) AS AttributeIsVisible, FROM AttributeHeader Where AttributeIsVisible=1 OR AttributeIsVisible IS NULL AND  AttributeID = @ID ";
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
        public async Task<AttributeHeader> GetAllBySessionAttributeName(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.*,(CASE WHEN t1.AttributeIsVisible is null  THEN 1  ELSE t1.AttributeIsVisible END) AS AttributeIsVisible,t6. UserName as AddedBy,t7. UserName as ModifiedBy,t2.CodeValue as ControlTypes,t5.Plantcode as AttributeCompany,t4.DisplayName as DataSourceDisplayName,t4.DataSourceTable from AttributeHeader t1  JOIN CodeMaster t2 ON t2.CodeID=t1.ControlTypeId\n\r" +
                    "LEFT JOIN AttributeHeaderDataSource t4 ON t4.AttributeHeaderDataSourceID=t1.DataSourceId\n\r" +
                    "LEFT JOIN ApplicationUSer t6 ON t6.UserId=t1.AddedbyuserId\n\r" +
                    "LEFT JOIN ApplicationUSer t7 ON t7.UserId=t1.ModifiedByUserID\n\r" +
                    "LEFT JOIN Plant t5 ON t5.PlantID=t1.AttributeCompanyId Where (t1.AttributeIsVisible=1 OR t1.AttributeIsVisible IS NULL) AND t1.SessionId=@SessionId\r\n";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<AttributeHeader>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<AttributeHeader>> GetAllAttributeName(bool? IsSubForm, string? type, long? subId)
        {
            try
            {
                var query = "select t1.*,(CASE WHEN t1.AttributeIsVisible is null  THEN 1  ELSE t1.AttributeIsVisible END) AS AttributeIsVisible,t6. UserName as AddedBy,t7. UserName as ModifiedBy,t2.CodeValue as ControlTypes,t5.Plantcode as AttributeCompany,t4.DisplayName as DataSourceDisplayName,t4.DataSourceTable from AttributeHeader t1  JOIN CodeMaster t2 ON t2.CodeID=t1.ControlTypeId\n\r" +
                    "LEFT JOIN AttributeHeaderDataSource t4 ON t4.AttributeHeaderDataSourceID=t1.DataSourceId\n\r" +
                    "LEFT JOIN ApplicationUSer t6 ON t6.UserId=t1.AddedbyuserId\n\r" +
                    "LEFT JOIN ApplicationUSer t7 ON t7.UserId=t1.ModifiedByUserID\n\r" +
                    "LEFT JOIN Plant t5 ON t5.PlantID=t1.AttributeCompanyId\r\n";
                if (IsSubForm == true)
                {
                    query += "where (t1.AttributeIsVisible=1 OR t1.AttributeIsVisible IS NULL) AND t1.IsSubForm=1";
                    if (type == "SubAttributeID")
                    {
                        query += "\n\rAND t1.SubAttributeID=" + subId;
                    }
                    if (type == "SubAttributeDetailID")
                    {
                        query += "\n\rAND t1.SubAttributeDetailID=" + subId;
                    }
                }
                else
                {
                    query += "where (t1.AttributeIsVisible=1 OR t1.AttributeIsVisible IS NULL) AND t1.IsSubForm is null OR t1.IsSubForm=0;";

                }

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
                var query = "SELECT tt1.* from (select t1.*,t2.CodeValue as ControlTypes,t4.DisplayName as DataSourceDisplayName,t4.DataSourceTable from AttributeHeader t1  JOIN CodeMaster t2 ON t2.CodeID=t1.ControlTypeId\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t4 ON t4.AttributeHeaderDataSourceID=t1.DataSourceId WHERE \r\n" +
                    "t1.AttributeID \r\n" +
                    "Not In (select t3.AttributeID from DynamicFormSectionAttribute t3 where t3.AttributeID>0  AND t3.DynamicFormSectionID=" + dynamicFormSectionId + ") \r\n" +
                    "or t1.AttributeID=" + attributeID + ")tt1 Where (tt1.AttributeIsVisible=1 OR tt1.AttributeIsVisible IS NULL) AND (tt1.IsSubForm =0 or tt1.IsSubForm is null)";
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
        public AttributeHeader GetAllAttributeNameCheckValidation(AttributeHeader attributeHeader)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("AttributeName", attributeHeader.AttributeName);
                parameters.Add("SubAttributeDetailId", attributeHeader.SubAttributeDetailId);
                parameters.Add("SubAttributeId", attributeHeader.SubAttributeId);
                if (attributeHeader.AttributeID > 0)
                {
                    parameters.Add("AttributeID", attributeHeader.AttributeID);
                    query += "SELECT * FROM AttributeHeader Where  (AttributeIsVisible=1 OR AttributeIsVisible IS NULL) AND AttributeID!=@AttributeID AND AttributeName = @AttributeName\n\r";

                }
                else
                {
                    query += "SELECT * FROM AttributeHeader Where  (AttributeIsVisible=1 OR AttributeIsVisible IS NULL) AND AttributeName = @AttributeName\n\r";
                }
                if (attributeHeader.IsSubForm == true)
                {
                    if (attributeHeader.SubFormType == "SubAttributeID")
                    {
                        query += "ANd SubAttributeId =@SubAttributeId ANd IsSubForm =1\n\r";
                    }
                    if (attributeHeader.SubFormType == "SubAttributeDetailID")
                    {
                        query += "ANd SubAttributeDetailId=@SubAttributeDetailId ANd IsSubForm =1\n\r";
                    }
                    query += "ANd IsSubForm =1";
                }
                else
                {
                    query += "ANd (IsSubForm =0 OR IsSubForm is null)";
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<AttributeHeader>(query, parameters);
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
                    var query = "select t1.DynamicFormSectionID,t1.SectionName,t1.SessionID,t1.StatusCodeID,t1.Instruction,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.SortOrderBy," +
                        "(select t4.IsVisible from DynamicFormSectionSecurity t4 Where t4.UserID=" + UserId + " AND  t4.DynamicFormSectionID=t1.DynamicFormSectionID) as IsVisible," +
                        "(select t4.IsReadOnly from DynamicFormSectionSecurity t4 Where t4.UserID=" + UserId + " AND  t4.DynamicFormSectionID=t1.DynamicFormSectionID) as IsReadOnly," +
                        "(select t4.IsReadWrite from DynamicFormSectionSecurity t4 Where t4.UserID=" + UserId + " AND  t4.DynamicFormSectionID=t1.DynamicFormSectionID) as IsReadWrite," +
                        "(select COUNT(t2.UserID) from DynamicFormSectionSecurity t2 Where t2.DynamicFormSectionID=t1.DynamicFormSectionID) as IsPermissionCount," +
                        "(select COUNT(t3.UserID) from DynamicFormSectionSecurity t3 Where t3.UserID=" + UserId + " AND  t3.DynamicFormSectionID=t1.DynamicFormSectionID) as IsLoginUsers," +
                        "(select COUNT(t5.UserID) from DynamicFormSectionWorkFlow t5 Where t5.UserID=" + UserId + " AND  t5.DynamicFormSectionID=t1.DynamicFormSectionID) as IsWorkFlowByUser,\r\n" +
                        "(select COUNT(t6.DynamicFormSectionID) from DynamicFormSectionWorkFlow t6 Where  t6.DynamicFormSectionID=t1.DynamicFormSectionID) as IsWorkFlowBy " +
                        "from DynamicFormSection t1\n\r" +
                         "JOIN DynamicForm t10 ON t1.DynamicFormID=t10.ID\r\n" +
                        "where (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND  t1.DynamicFormID=" + dynamicForm.ID + " order by  t1.SortOrderBy asc;\n\r";
                    query += "select t1.*,(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,t5.SectionName,t11.DataSourceTable as PlantDropDownWithOtherDataSourceTable,t9.sessionId as DynamicFormSessionId,t6.IsDynamicFormDropTagBox,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.DataSourceId,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t7.CodeValue as ControlType,t5.DynamicFormID,t6.DynamicFormID as DynamicFormGridDropDownID from DynamicFormSectionAttribute t1\r\n" +
                        "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                        "JOIN DynamicForm t10 ON t5.DynamicFormID=t10.ID\r\n" +
                        "JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                        "LEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID\r\n" +
                        "LEFT JOIN DynamicForm t9 ON t9.ID=t6.DynamicFormID\r\n" +
                         "LEFT JOIN AttributeHeaderDataSource t11 ON t11.AttributeHeaderDataSourceID=t1.PlantDropDownWithOtherDataSourceId\r\n" +
                        "JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\nWhere (t6.AttributeIsVisible=1 OR t6.AttributeIsVisible IS NULL) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t1.IsVisible= 1 OR t1.IsVisible is null) AND t5.DynamicFormID=" + dynamicForm.ID + " order by t1.SortOrderBy asc;";
                    query += "Select * from Plant;";
                    var results = await connection.QueryMultipleAsync(query);
                    attributeHeaderListModel.DynamicFormSection = results.Read<DynamicFormSection>().ToList();
                    attributeHeaderListModel.DynamicFormSectionAttribute = results.Read<DynamicFormSectionAttribute>().ToList();
                    attributeHeaderListModel.Plant = results.Read<Plant>().ToList();
                }
                if (attributeHeaderListModel.DynamicFormSectionAttribute != null)
                {
                    string? plantCode = null;
                    if (attributeHeaderListModel.Plant != null && attributeHeaderListModel.Plant.Count > 0 && dynamicForm.CompanyId > 0)
                    {
                        plantCode = attributeHeaderListModel.Plant.FirstOrDefault(f => f.PlantCode != null && f.PlantID == dynamicForm.CompanyId)?.PlantCode;
                        plantCode = plantCode != null ? plantCode.ToLower() : null;
                    }
                    List<long?> DynamicFormIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.DataSourceTable == "DynamicGrid" && a.DynamicFormGridDropDownId > 0).Select(a => a.DynamicFormGridDropDownId).Distinct().ToList();
                    if (DynamicFormIds != null && DynamicFormIds.Count > 0)
                    {
                        attributeHeaderListModel.DropDownOptionsGridListModel = await GetDynamicFormGridModelAsync(DynamicFormIds, UserId, dynamicForm.CompanyId, plantCode);
                    }
                    List<long?> attributeIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.AttributeId > 0).Select(a => a.AttributeId).Distinct().ToList();
                    List<long> attributeSubFormIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.AttributeId > 0 && a.ControlTypeId == 2710).Select(a => a.AttributeId.Value).Distinct().ToList();
                    List<string?> dataSourceTableIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.DataSourceTable != null).Select(a => a.DataSourceTable).Distinct().ToList();
                    var attributeResultDetails = await GetAttributeDetails(attributeIds, attributeSubFormIds, "Main", dynamicForm.CompanyId, plantCode);
                    var attributeDetails = attributeResultDetails.AttributeDetails;
                    attributeHeaderListModel.AttributeDetails = attributeDetails != null && attributeDetails.Count > 0 ? attributeDetails.Where(w => attributeIds.Contains(w.AttributeID)).ToList() : new List<AttributeDetails>();
                    var AttributeDetailsIds = attributeHeaderListModel.AttributeDetails.Select(s => s.AttributeDetailID).ToList();
                    var attributeSubResultDetails = await GetAttributeDetails(new List<long?>(), AttributeDetailsIds, "Sub", dynamicForm.CompanyId, plantCode);
                    var dataSourceList = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(dynamicForm.CompanyId, dataSourceTableIds, plantCode);
                    if (attributeHeaderListModel.AttributeDetails != null && attributeHeaderListModel.AttributeDetails.Count > 0)
                    {
                        attributeHeaderListModel.AttributeDetails.ForEach(z =>
                        {
                            z.SubAttributeHeaders = attributeSubResultDetails.AttributeHeader.Where(q => q.SubAttributeDetailId == z.AttributeDetailID).ToList();
                        });
                    }
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
                            s.SubAttributeHeaders = attributeResultDetails.AttributeHeader.Where(x => x.SubAttributeId == s.AttributeId).ToList();
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
        private List<DynamicFormSectionAttribute> GenerateFixedDataName()
        {
            List<DynamicFormSectionAttribute> dynamicFormSectionAttributes = new List<DynamicFormSectionAttribute>();
            DynamicFormSectionAttribute dynamicFormSectionAttribute = new DynamicFormSectionAttribute();
            dynamicFormSectionAttribute.DynamicAttributeName = "AttributeDetailID";
            dynamicFormSectionAttribute.DataType = typeof(long);
            dynamicFormSectionAttributes.Add(dynamicFormSectionAttribute);


            DynamicFormSectionAttribute dynamicFormSectionAttribute1 = new DynamicFormSectionAttribute();
            dynamicFormSectionAttribute1.DynamicAttributeName = "AttributeDetailName";
            dynamicFormSectionAttribute1.DataType = typeof(string);
            dynamicFormSectionAttributes.Add(dynamicFormSectionAttribute1);

            DynamicFormSectionAttribute dynamicFormSectionAttribute2 = new DynamicFormSectionAttribute();
            dynamicFormSectionAttribute2.DynamicAttributeName = "IsDynamicFormDataGrid";
            dynamicFormSectionAttribute2.DataType = typeof(bool?);
            dynamicFormSectionAttributes.Add(dynamicFormSectionAttribute2);

            DynamicFormSectionAttribute dynamicFormSectionAttribute3 = new DynamicFormSectionAttribute();
            dynamicFormSectionAttribute3.DynamicAttributeName = "CurrentUserName";
            dynamicFormSectionAttribute3.DataType = typeof(string);
            dynamicFormSectionAttributes.Add(dynamicFormSectionAttribute3);

            DynamicFormSectionAttribute dynamicFormSectionAttribute4 = new DynamicFormSectionAttribute();
            dynamicFormSectionAttribute4.DynamicAttributeName = "ModifiedBy";
            dynamicFormSectionAttribute4.DataType = typeof(string);
            dynamicFormSectionAttributes.Add(dynamicFormSectionAttribute4);

            DynamicFormSectionAttribute dynamicFormSectionAttribute5 = new DynamicFormSectionAttribute();
            dynamicFormSectionAttribute5.DynamicAttributeName = "ModifiedDate";
            dynamicFormSectionAttribute5.DataType = typeof(DateTime?);
            dynamicFormSectionAttributes.Add(dynamicFormSectionAttribute5);


            DynamicFormSectionAttribute dynamicFormSectionAttribute6 = new DynamicFormSectionAttribute();
            dynamicFormSectionAttribute6.DynamicAttributeName = "StatusName";
            dynamicFormSectionAttribute6.DataType = typeof(string);
            dynamicFormSectionAttributes.Add(dynamicFormSectionAttribute6);

            DynamicFormSectionAttribute dynamicFormSectionAttribute7 = new DynamicFormSectionAttribute();
            dynamicFormSectionAttribute7.DynamicAttributeName = "DynamicFormId";
            dynamicFormSectionAttribute7.DataType = typeof(long?);
            dynamicFormSectionAttributes.Add(dynamicFormSectionAttribute7);
            DynamicFormSectionAttribute dynamicFormSectionAttribute8 = new DynamicFormSectionAttribute();
            dynamicFormSectionAttribute8.DynamicAttributeName = "SessionId";
            dynamicFormSectionAttribute8.DataType = typeof(Guid?);
            dynamicFormSectionAttributes.Add(dynamicFormSectionAttribute8);
            return dynamicFormSectionAttributes;
        }
        public async Task<DropDownOptionsGridListModel> GetDynamicFormGridModelAsync(List<long?> dynamicFormIds, long? userId, long? companyId, string plantCode)
        {
            try
            {
                DropDownOptionsGridListModel dropDownOptionsGridListModel = new DropDownOptionsGridListModel();
                List<ExpandoObject>? _dynamicformObjectDataList = new List<ExpandoObject>();
                List<DynamicFormData> dynamicFormDatas = new List<DynamicFormData>();
                DynamicFormGridModel attributeHeaderListModel = new DynamicFormGridModel();
                List<DropDownGridOptionsModel> dropDownGridOptionsModel1 = new List<DropDownGridOptionsModel>();
                using (var connection = CreateConnection())
                {
                    var dynamicFormIdss = dynamicFormIds != null && dynamicFormIds.Count() > 0 ? dynamicFormIds : new List<long?>() { -1 };
                    var query = "select t1.*,(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,t5.SectionName,t9.sessionId as DynamicFormSessionId,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.IsDynamicFormDropTagBox,t6.DataSourceId,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t7.CodeValue as ControlType,t5.DynamicFormID,t6.DynamicFormID as DynamicFormGridDropDownID from " +
                        "DynamicFormSectionAttribute t1 " +
                        "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId " +
                        "JOIN DynamicForm t10 ON t10.ID=t5.DynamicFormID\r\n" +
                        "JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID " +
                        "LEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID " +
                        "LEFT JOIN DynamicForm t9 ON t9.ID=t6.DynamicFormID " +
                        "JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID Where (t6.AttributeIsVisible=1 OR t6.AttributeIsVisible IS NULL) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t1.IsVisible= 1 OR t1.IsVisible is null) AND t5.DynamicFormID in(" + string.Join(',', dynamicFormIdss) + ") order by t1.SortOrderBy asc;";

                    query += "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode,t5.Name,t5.ScreenID,\r\n" +
                   "(select COUNT(t6.DocumentID) from Documents t6 where t6.SessionID = t1.SessionID AND t6.IsLatest = 1 AND(t6.IsDelete IS NULL OR t6.IsDelete = 0)) as IsFileprofileTypeDocument,(CASE WHEN t1.DynamicFormDataGridID>0  THEN 1  ELSE 0 END) AS IsDynamicFormDataGrid\r\n" +
                   "from DynamicFormData t1\r\n" + "JOIN ApplicationUser t2 ON t2.UserID = t1.AddedByUserID\r\n" + "JOIN ApplicationUser t3 ON t3.UserID = t1.ModifiedByUserID\r\n" +
                   "JOIN DynamicForm t5 ON t5.ID = t1.DynamicFormID\r\n" + "JOIN CodeMaster t4 ON t4.CodeID = t1.StatusCodeID WHERE t1.DynamicFormId IN (" + string.Join(',', dynamicFormIdss) + ");";
                    query += "select t1.*,t4.UserName as ApprovedByUser,t5.DynamicFormId,\r\n" +
                      "CONCAT(case when t2.NickName is NULL then  t2.FirstName ELSE  t2.NickName END,' | ',t2.LastName) as ApprovalUser,\r\n" +
                      "CASE WHEN t1.IsApproved=1  THEN 'Approved' WHEN t1.IsApproved =0 THEN 'Rejected' ELSE 'Pending' END AS ApprovedStatus\r\n" +
                      "FROM DynamicFormApproved t1 \r\n" +
                      "JOIN Employee t2 ON t1.UserID=t2.UserID \r\n" +
                      "JOIN DynamicFormData t5 ON t5.DynamicFormDataId=t1.DynamicFormDataId \r\n" +
                      "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ApprovedByUserId WHERE (t5.IsDeleted=0 or t5.IsDeleted is null) AND t5.DynamicFormId IN (" + string.Join(',', dynamicFormIdss) + ") order by t1.DynamicFormApprovedId asc;\r\n";
                    query += "select  * from AttributeDetails WHERE (Disabled=0 OR Disabled IS NULL);";
                    query += "select  * from DynamicForm WHere (IsDeleted=0 or IsDeleted is null) AND ID IN (" + string.Join(',', dynamicFormIdss) + ");";
                    var results = await connection.QueryMultipleAsync(query);
                    attributeHeaderListModel.DynamicFormSectionAttribute = results.Read<DynamicFormSectionAttribute>().ToList();
                    attributeHeaderListModel.DynamicFormData = results.Read<DynamicFormData>().ToList();
                    attributeHeaderListModel.DynamicFormApproved = results.Read<DynamicFormApproved>().ToList();
                    attributeHeaderListModel.AttributeDetails = results.Read<AttributeDetails>().ToList();
                    attributeHeaderListModel.DynamicForm = results.Read<DynamicForm>().ToList();
                }
                if (attributeHeaderListModel.DynamicFormSectionAttribute != null && attributeHeaderListModel.DynamicFormSectionAttribute.Count > 0)
                {
                    List<string?> dataSourceTableIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.DataSourceTable != null).Select(a => a.DataSourceTable).Distinct().ToList();
                    var dataSourceList = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(companyId, dataSourceTableIds, plantCode);
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
                            s.DataType = typeof(string);
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
                            s.DataType = typeof(string);
                        }
                        else if (s.ControlType == "TagBox")
                        {
                            s.DataType = typeof(string);
                        }
                        else if (s.ControlType == "ListBox")
                        {
                            if (s.IsMultiple == true)
                            {
                                s.DataType = typeof(string);
                            }
                            else
                            {
                                s.DataType = typeof(string);
                            }
                        }
                        else
                        {
                            s.DataType = typeof(string);
                        }
                    });
                }
                if (dynamicFormIds != null && dynamicFormIds.Count > 0)
                {
                    dynamicFormIds.ForEach(s =>
                    {
                        var dynamicForms = attributeHeaderListModel.DynamicForm.FirstOrDefault(f => f.ID == s);
                        var dynamicFormData = attributeHeaderListModel.DynamicFormData.Where(w => w.DynamicFormId == s).ToList();
                        var dynamicFormSectionAttribute = attributeHeaderListModel.DynamicFormSectionAttribute.Where(w => w.DynamicFormId == s).OrderBy(o => o.SortOrderBy).ToList();
                        var dynamicFormSectionAttributeClass = attributeHeaderListModel.DynamicFormSectionAttribute.Where(w => w.DynamicFormId == s).OrderBy(o => o.SortOrderBy).ToList();
                        dynamicFormSectionAttributeClass.AddRange(GenerateFixedDataName());
                        Type[] typesList = dynamicFormSectionAttributeClass.Select(s => s.DataType).ToArray();
                        string[] objectNameList = dynamicFormSectionAttributeClass.Select(s => s.DynamicAttributeName).ToArray();

                        DropDownGridOptionsModel dropDownGridOptionsModel = new DropDownGridOptionsModel();
                        dropDownGridOptionsModel.DynamicFormId = s;
                        dropDownGridOptionsModel.DropDownOptionsModels = GenerateDropDownList(s, dynamicFormSectionAttribute);
                        dropDownGridOptionsModel1.Add(dropDownGridOptionsModel);

                        var dynamicFormApproved = attributeHeaderListModel.DynamicFormApproved.Where(w => w.DynamicFormId == s).OrderBy(o => o.DynamicFormApprovedId).ToList();
                        if (dynamicFormData != null && dynamicFormData.Count > 0)
                        {
                            dynamicFormData.ForEach(s =>
                            {
                                dynamic obj = new System.Dynamic.ExpandoObject();
                                var dict = (IDictionary<string, object>)obj;
                                dynamic jsonObj = new object();
                                if (IsValidJson(s.DynamicFormItem))
                                {
                                    jsonObj = JsonConvert.DeserializeObject(s.DynamicFormItem);
                                }
                                if (s.IsSendApproval == true)
                                {
                                    var approvedList = dynamicFormApproved.Where(w => w.DynamicFormDataId == s.DynamicFormDataId).ToList();
                                    if (approvedList != null && approvedList.Count() > 0)
                                    {
                                        s.DynamicFormApproved = approvedList;
                                        var approved = approvedList.Where(w => w.IsApproved == true).ToList();
                                        var approvedPending = approvedList.Where(w => w.IsApproved == null).ToList();
                                        if (approved != null && approved.Count() > 0 && approvedList.Count() == approved.Count())
                                        {
                                            s.ApprovalStatusId = 4;
                                            s.ApprovalStatus = "Approved Done";
                                            s.StatusName = "Completed";
                                        }
                                        else
                                        {
                                            var rejected = approvedList.Where(w => w.IsApproved == false).FirstOrDefault();
                                            if (rejected != null)
                                            {
                                                s.IsApproved = rejected.IsApproved;
                                                s.ApprovalStatusId = 3;
                                                s.ApprovalStatus = "Rejected";
                                                s.RejectedDate = rejected.ApprovedDate;
                                                s.RejectedUserId = rejected.UserId;
                                                s.RejectedUser = rejected.ApprovalUser;
                                                s.StatusName = "Rejected";
                                                s.CurrentUserId = rejected.UserId;
                                                s.CurrentUserName = rejected.ApprovalUser;
                                            }
                                            else
                                            {
                                                if (approvedPending != null && approvedPending.Count > 0)
                                                {
                                                    if (approved != null && approved.Count() > 0)
                                                    {
                                                        var isapproved = approved.OrderByDescending(o => o.DynamicFormApprovedId).FirstOrDefault(w => w.IsApproved == true);
                                                        if (isapproved != null)
                                                        {
                                                            s.IsApproved = isapproved.IsApproved;
                                                            s.ApprovalStatusId = 2;
                                                            s.ApprovalStatus = "Approved";
                                                            s.ApprovedDate = isapproved.ApprovedDate;
                                                            s.ApprovedUserId = isapproved.UserId;
                                                            s.ApprovedUser = isapproved.ApprovalUser;
                                                            s.CurrentUserId = isapproved.UserId;
                                                            s.CurrentUserName = isapproved.ApprovalUser;
                                                        }
                                                    }
                                                    var isapprovedPending = approvedPending.OrderBy(o => o.DynamicFormApprovedId).FirstOrDefault(w => w.IsApproved == null);
                                                    if (isapprovedPending != null)
                                                    {
                                                        s.IsApproved = isapprovedPending.IsApproved;
                                                        s.ApprovalStatusId = 1;
                                                        s.ApprovalStatus = "Pending";
                                                        s.PendingUserId = isapprovedPending.UserId;
                                                        s.PendingUser = isapprovedPending.ApprovalUser;
                                                        s.StatusName = "Pending";
                                                        s.CurrentUserId = isapprovedPending.UserId;
                                                        s.CurrentUserName = isapprovedPending.ApprovalUser;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                obj.AttributeDetailID = s.DynamicFormDataId;
                                obj.AttributeDetailName = s.ProfileNo;
                                obj.ModifiedBy = s.ModifiedBy;
                                obj.DynamicFormId = s.DynamicFormId;
                                obj.CurrentUserName = s.CurrentUserName;
                                obj.ModifiedDate = s.ModifiedDate;
                                obj.StatusName = s.StatusName;
                                obj.IsDynamicFormDataGrid = s.IsDynamicFormDataGrid;
                                obj.SessionId = s.SessionId;
                                var dynamicFormSectionAttribute = attributeHeaderListModel.DynamicFormSectionAttribute.Where(w => w.DynamicFormId == s.DynamicFormId).ToList();
                                if (dynamicFormSectionAttribute != null && dynamicFormSectionAttribute.Count > 0)
                                {
                                    dynamicFormSectionAttribute.ForEach(b =>
                                    {
                                        b.DynamicAttributeName = b.DynamicFormSectionAttributeId + "_" + b.AttributeName;
                                        string attrName = b.DynamicAttributeName;
                                        var Names = jsonObj.ContainsKey(attrName);
                                        if (Names == true)
                                        {
                                            var itemValue = jsonObj[attrName];
                                            if (itemValue is JArray)
                                            {
                                                List<long?> listData = itemValue.ToObject<List<long?>>();
                                                var listName = attributeHeaderListModel.AttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.DropDownTypeId == b.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                dict[attrName] = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                            }
                                            else
                                            {
                                                if (b.ControlType == "ComboBox" || b.ControlType == "Radio" || b.ControlType == "RadioGroup")
                                                {
                                                    long? Svalues = itemValue == null ? null : (long)itemValue;
                                                    if (Svalues != null)
                                                    {
                                                        var listName = attributeHeaderListModel.AttributeDetails.Where(a => a.AttributeDetailID == Svalues && a.AttributeDetailName != null && a.DropDownTypeId == b.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                        dict[attrName] = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;

                                                    }
                                                    else
                                                    {
                                                        dict[attrName] = string.Empty;
                                                    }
                                                }
                                                else if (b.ControlType == "ListBox" && b.IsMultiple == false)
                                                {
                                                    long? Svalues = itemValue == null ? null : (long)itemValue;
                                                    if (Svalues != null)
                                                    {
                                                        var listName = attributeHeaderListModel.AttributeDetails.Where(a => a.AttributeDetailID == Svalues && a.DropDownTypeId == b.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                        dict[attrName] = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                    }
                                                    else
                                                    {
                                                        dict[attrName] = string.Empty;
                                                    }
                                                }
                                                else if (b.ControlType == "DateEdit")
                                                {
                                                    DateTime? values = itemValue == null ? null : (DateTime)itemValue;
                                                    dict[attrName] = values;
                                                }
                                                else if (b.ControlType == "TimeEdit")
                                                {
                                                    TimeSpan? values = itemValue == null ? null : (TimeSpan)itemValue;
                                                    dict[attrName] = values;
                                                }
                                                else if (b.ControlType == "SpinEdit")
                                                {
                                                    if (b.IsSpinEditType == "decimal")
                                                    {
                                                        decimal? values = itemValue == null ? null : (decimal)itemValue;
                                                        dict[attrName] = values;
                                                    }
                                                    else
                                                    {
                                                        int? values = itemValue == null ? null : (int)itemValue;
                                                        dict[attrName] = values;
                                                    }
                                                }
                                                else if (b.ControlType == "CheckBox")
                                                {
                                                    bool? values = itemValue == null ? false : (bool)itemValue;
                                                    dict[attrName] = values;
                                                }
                                                else
                                                {
                                                    dict[attrName] = (string)itemValue;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            dict[attrName] = string.Empty;
                                        }


                                    });
                                }
                                _dynamicformObjectDataList.Add(obj);
                            });
                        }
                        //if (userId > 0 && dynamicFormData != null && dynamicFormData.Count > 0)
                        // {
                        //   dynamicFormData = dynamicFormData.Where(w => w.CurrentUserId == userId).ToList();
                        // }
                    });
                }
                dropDownOptionsGridListModel.DropDownGridOptionsModel = dropDownGridOptionsModel1;
                dropDownOptionsGridListModel.ObjectData = _dynamicformObjectDataList;
                return dropDownOptionsGridListModel;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private List<DropDownOptionsModel> GenerateDropDownList(long? DynamicFormId, List<DynamicFormSectionAttribute> dynamicFormSectionAttribute)
        {
            List<DropDownOptionsModel> dataColumnNames = new List<DropDownOptionsModel>
                        {
                           // new DropDownOptionsModel() { Value = "AttributeDetailID", Text = "DynamicFormDataId", Type = "Form", IsVisible = false,OrderBy=1 },
                            new DropDownOptionsModel() { Value = "AttributeDetailName", Text = "Profile No", Type = "Form",OrderBy=1,AttributeDetailID=1, AttributeDetailName = "Profile No" },
                            //new DropDownOptionsModel() { Value = "IsDynamicFormDataGrid", Text = "Is Grid", Type = "Form",OrderBy=3 }
                        };
            if (dynamicFormSectionAttribute != null && dynamicFormSectionAttribute.Count > 0)
            {
                var counts = 1;
                dynamicFormSectionAttribute.ForEach(a =>
                {
                    a.DynamicAttributeName = a.DynamicFormSectionAttributeId + "_" + a.AttributeName;
                    counts += 1;
                    dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = a.DynamicAttributeName, Text = a.DisplayName, Type = "DynamicForm", Id = a.DynamicFormSectionAttributeId, IsVisible = a.IsDisplayTableHeader, AttributeDetailName = a.DisplayName });
                });
                counts++;
                dataColumnNames.Add(new DropDownOptionsModel() { Value = "CurrentUserName", Text = "Current User", Type = "Form", OrderBy = counts, AttributeDetailID = counts, AttributeDetailName = "Current User" });
                counts++;
                dataColumnNames.Add(new DropDownOptionsModel() { Value = "ModifiedBy", Text = "ModifiedBy", Type = "Form", OrderBy = counts, AttributeDetailID = counts, AttributeDetailName = "ModifiedBy" });
                counts++;
                dataColumnNames.Add(new DropDownOptionsModel() { Value = "ModifiedDate", Text = "ModifiedDate", Type = "Form", OrderBy = counts, AttributeDetailID = counts, AttributeDetailName = "ModifiedDate" });
                counts++;
                dataColumnNames.Add(new DropDownOptionsModel() { Value = "StatusName", Text = "Status", Type = "Form", OrderBy = counts, AttributeDetailID = counts, AttributeDetailName = "Status" });
                //counts++;
                //dataColumnNames.Add(new DropDownOptionsModel() { Value = "DynamicFormId", Text = "DynamicFormId", Type = "Form", OrderBy = counts, AttributeDetailID = counts, DynamicFormId = DynamicFormId });
            }
            else
            {
                dataColumnNames.Add(new DropDownOptionsModel() { Value = "CurrentUserName", Text = "Current User", Type = "Form", OrderBy = 2, AttributeDetailID = 2, AttributeDetailName = "Current User" });
                dataColumnNames.Add(new DropDownOptionsModel() { Value = "ModifiedBy", Text = "ModifiedBy", Type = "Form", OrderBy = 3, AttributeDetailID = 3, AttributeDetailName = "ModifiedBy" });
                dataColumnNames.Add(new DropDownOptionsModel() { Value = "ModifiedDate", Text = "ModifiedDate", Type = "Form", OrderBy = 4, AttributeDetailID = 4, AttributeDetailName = "ModifiedDate" });
                dataColumnNames.Add(new DropDownOptionsModel() { Value = "StatusName", Text = "Status", Type = "Form", OrderBy = 5, AttributeDetailID = 5, AttributeDetailName = "Status" });
                //dataColumnNames.Add(new DropDownOptionsModel() { Value = "DynamicFormId", Text = "DynamicFormId", Type = "Form", OrderBy = 8, AttributeDetailID = 8, DynamicFormId = DynamicFormId });
            }
            return dataColumnNames;
        }
        private static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<AttributeDetailsAdds> GetAttributeDetails(List<long?> AttributeIds, List<long> id, string? Type, long? companyId, string plantCode)
        {
            try
            {
                AttributeDetailsAdds attributeDetailsAdds = new AttributeDetailsAdds();
                id = id != null && id.Count > 0 ? id : new List<long>() { -1 };
                AttributeIds = AttributeIds != null && AttributeIds.Count > 0 ? AttributeIds : new List<long?>() { -1 };
                var query = "select  * from AttributeDetails WHERE (Disabled=0 OR Disabled IS NULL);\n\r";
                query += "select t6.*,t9.AttributeID as SubAttributeID,t6.IsDynamicFormDropTagBox,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.DataSourceId,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t7.CodeValue as ControlType,t6.DynamicFormID as DynamicFormGridDropDownID from \r\nAttributeHeader t6\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID\r\nLEFT JOIN AttributeHeader t9 ON t9.AttributeID=t6.SubAttributeID\r\n" +
                "JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID where (t6.AttributeIsVisible=1 or t6.AttributeIsVisible is NULL) AND t6.IsSubForm=1\n\r";
                if (Type == "Main")
                {
                    query += "AND t6.SubAttributeID in(" + string.Join(',', id) + ") ";
                }
                if (Type == "Sub")
                {
                    query += "AND t6.SubAttributeDetailID in(" + string.Join(',', id) + ") ";
                }
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryMultipleAsync(query);
                    attributeDetailsAdds.AttributeDetails = result.Read<AttributeDetails>().ToList();
                    attributeDetailsAdds.AttributeHeader = result.Read<AttributeHeader>().ToList();
                }
                if (attributeDetailsAdds.AttributeHeader.Count > 0)
                {
                    List<string?> dataSourceTableIds = attributeDetailsAdds.AttributeHeader.Where(a => a.DataSourceTable != null).Select(a => a.DataSourceTable).Distinct().ToList();
                    var dataSourceList = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(companyId, dataSourceTableIds, plantCode);
                    attributeDetailsAdds.AttributeHeader.ForEach(s =>
                    {
                        if (Type == "Main")
                        {
                            s.SubDynamicAttributeName = s.SubAttributeId + "_" + s.AttributeID + "_Main_" + s.AttributeName;
                        }
                        if (Type == "Sub")
                        {

                            s.SubDynamicAttributeName = s.SubAttributeDetailId + "_" + s.AttributeID + "_Sub_" + s.AttributeName;
                        }
                        if (s.ControlType == "TextBox" || s.ControlType == "Memo")
                        {
                            s.SubDataType = typeof(string);
                        }
                        else if (s.ControlType == "ComboBox" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                        {
                            if (s.DropDownTypeId == "Data Source")
                            {
                                s.SubAttributeDetails = dataSourceList.Where(w => w.DropDownTypeId == s.DataSourceTable).ToList();
                            }
                            else
                            {
                                s.SubAttributeDetails = attributeDetailsAdds.AttributeDetails.Where(w => w.AttributeID == s.AttributeID).ToList();
                            }
                            s.SubDataType = typeof(long?);
                        }
                        else if (s.ControlType == "DateEdit")
                        {
                            s.SubDataType = typeof(DateTime?);
                        }
                        else if (s.ControlType == "TimeEdit")
                        {
                            s.SubDataType = typeof(TimeSpan?);
                        }
                        else if (s.ControlType == "SpinEdit")
                        {
                            if (s.IsAttributeSpinEditType == "decimal")
                            {
                                s.SubDataType = typeof(decimal?);
                            }
                            else
                            {
                                s.SubDataType = typeof(int?);
                            }
                        }
                        else if (s.ControlType == "CheckBox")
                        {
                            s.SubDataType = typeof(bool?);
                        }
                        else if (s.ControlType == "TagBox")
                        {
                            if (s.DropDownTypeId == "Data Source")
                            {
                                s.SubAttributeDetails = dataSourceList.Where(w => w.DropDownTypeId == s.DataSourceTable).ToList();
                            }
                            else
                            {
                                s.SubAttributeDetails = attributeDetailsAdds.AttributeDetails.Where(w => w.AttributeID == s.AttributeID).ToList();
                            }
                            s.SubDataType = typeof(IEnumerable<long?>);
                        }
                        else if (s.ControlType == "ListBox")
                        {
                            if (s.DropDownTypeId == "Data Source")
                            {
                                s.SubAttributeDetails = dataSourceList.Where(w => w.DropDownTypeId == s.DataSourceTable).ToList();
                            }
                            else
                            {
                                s.SubAttributeDetails = attributeDetailsAdds.AttributeDetails.Where(w => w.AttributeID == s.AttributeID).ToList();
                            }
                            if (s.IsMultiple == true)
                            {
                                s.SubDataType = typeof(IEnumerable<long?>);
                            }
                            else
                            {
                                s.SubDataType = typeof(long?);
                            }
                        }
                        else
                        {
                            s.SubDataType = typeof(string);
                        }
                    });
                }
                return attributeDetailsAdds;
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
                var query = "SELECT * FROM AttributeHeader WHERE (AttributeIsVisible=1 OR AttributeIsVisible IS NULL) AND AttrubuteID = @Id";
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
                        parameters.Add("AttributeID", attributeHeader.AttributeID);
                        parameters.Add("AttributeName", attributeHeader.AttributeName, DbType.String);
                        parameters.Add("IsInternal", attributeHeader.IsInternal);
                        parameters.Add("Description", attributeHeader.Description, DbType.String);
                        parameters.Add("AttributeCompanyId", attributeHeader.AttributeCompanyId);
                        parameters.Add("ControlType", attributeHeader.ControlType);
                        parameters.Add("EntryMask", attributeHeader.EntryMask);
                        parameters.Add("RegExp", attributeHeader.RegExp);
                        parameters.Add("ModifiedByUserID", attributeHeader.ModifiedByUserID);
                        parameters.Add("AddedByUserID", attributeHeader.AddedByUserID);
                        parameters.Add("AddedDate", attributeHeader.AddedDate);
                        parameters.Add("ModifiedDate", attributeHeader.ModifiedDate);
                        parameters.Add("StatusCodeID", attributeHeader.StatusCodeID);
                        parameters.Add("SessionId", attributeHeader.SessionId);
                        parameters.Add("ControlTypeId", attributeHeader.ControlTypeId);
                        parameters.Add("IsMultiple", attributeHeader.IsMultiple);
                        parameters.Add("IsRequired", attributeHeader.IsRequired);
                        parameters.Add("DropDownTypeId", attributeHeader.DropDownTypeId);
                        parameters.Add("DataSourceId", attributeHeader.DataSourceId);
                        parameters.Add("DynamicFormId", attributeHeader.DynamicFormId);
                        parameters.Add("IsDynamicFormDropTagBox", attributeHeader.IsDynamicFormDropTagBox == true ? true : null);
                        parameters.Add("RequiredMessage", attributeHeader.RequiredMessage, DbType.String);
                        parameters.Add("IsSubForm", attributeHeader.IsSubForm == true ? true : null);
                        parameters.Add("SubAttributeDetailId", attributeHeader.IsSubForm == true ? attributeHeader.SubAttributeDetailId : null);
                        parameters.Add("SubAttributeId", attributeHeader.IsSubForm == true ? attributeHeader.SubAttributeId : null);
                        parameters.Add("IsAttributeSpinEditType", attributeHeader.IsAttributeSpinEditType, DbType.String);
                        parameters.Add("IsAttributeDisplayTableHeader", attributeHeader.IsAttributeDisplayTableHeader);
                        parameters.Add("AttributeFormToolTips", attributeHeader.AttributeFormToolTips, DbType.String);
                        parameters.Add("AttributeIsVisible", attributeHeader.AttributeIsVisible);
                        parameters.Add("AttributeRadioLayout", attributeHeader.AttributeRadioLayout, DbType.String);
                        if (attributeHeader.AttributeID > 0)
                        {
                            var Addquerys = "UPDATE AttributeHeader SET AttributeRadioLayout=@AttributeRadioLayout,AttributeIsVisible=@AttributeIsVisible,AttributeFormToolTips=@AttributeFormToolTips,IsAttributeSpinEditType=@IsAttributeSpinEditType,IsAttributeDisplayTableHeader=@IsAttributeDisplayTableHeader,SubAttributeId=@SubAttributeId,SubAttributeDetailId=@SubAttributeDetailId,IsSubForm=@IsSubForm,IsDynamicFormDropTagBox=@IsDynamicFormDropTagBox,DynamicFormId=@DynamicFormId,AttributeName = @AttributeName,IsInternal=@IsInternal,Description=@Description," +
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
                            var query = @"INSERT INTO AttributeHeader(AttributeRadioLayout,AttributeIsVisible,AttributeFormToolTips,IsAttributeDisplayTableHeader,IsAttributeSpinEditType,SubAttributeId,SubAttributeDetailId,IsSubForm,IsDynamicFormDropTagBox,DynamicFormId,AttributeCompanyId,AttributeName,IsInternal,Description,ControlType,EntryMask,RegExp,AddedByUserID,AddedDate,SessionId,StatusCodeID,ControlTypeId,IsMultiple,IsRequired,RequiredMessage,DropDownTypeId,DataSourceId) 
              OUTPUT INSERTED.AttributeID  -- Replace 'YourIDColumn' with the actual column name of your IDENTITY column
              VALUES (@AttributeRadioLayout,@AttributeIsVisible,@AttributeFormToolTips,@IsAttributeDisplayTableHeader,@IsAttributeSpinEditType,@SubAttributeId,@SubAttributeDetailId,@IsSubForm,@IsDynamicFormDropTagBox,@DynamicFormId,@AttributeCompanyId,@AttributeName,@IsInternal,@Description,@ControlType,@EntryMask,@RegExp,@AddedByUserID,@AddedDate,@SessionId,@StatusCodeID,@ControlTypeId,@IsMultiple,@IsRequired,@RequiredMessage,@DropDownTypeId,@DataSourceId)";

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
