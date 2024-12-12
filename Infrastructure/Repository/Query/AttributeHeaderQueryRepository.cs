using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Helpers;
using Core.Repositories.Query;
using Dapper;
using DevExpress.Data.Filtering.Helpers;
using Google.Protobuf.Collections;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.AspNetCore.Components;
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
using System.Text.Json;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;
using static iText.Kernel.Utils.CompareTool;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static iText.Svg.SvgConstants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Repository.Query
{
    public class AttributeHeaderQueryRepository : QueryRepository<AttributeHeader>, IAttributeQueryRepository
    {
        private readonly IDynamicFormDataSourceQueryRepository _dynamicFormDataSourceQueryRepository;
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public AttributeHeaderQueryRepository(IConfiguration configuration, IDynamicFormDataSourceQueryRepository dynamicFormDataSourceQueryRepository, IDynamicFormQueryRepository dynamicFormQueryRepository)
            : base(configuration)
        {
            _dynamicFormDataSourceQueryRepository = dynamicFormDataSourceQueryRepository;
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }

        public async Task<long> DeleteAsync(AttributeHeader attributeHeader)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {

                        var parameters = new DynamicParameters();
                        parameters.Add("AttributeID", attributeHeader.AttributeID);
                        var query = "Update  AttributeHeader  SET IsDeleted=1  WHERE AttributeID = @AttributeID;";
                        if (attributeHeader.AttributeSortBy > 0)
                        {
                            var result = await UpdateAttributeHeaderSort(attributeHeader);
                            if (result != null && result.Count() > 0)
                            {
                                var sortby = attributeHeader.AttributeSortBy;
                                result.ForEach(s =>
                                {
                                    query += "Update  AttributeHeader SET AttributeSortBy=" + sortby + "  WHERE AttributeID =" + s.AttributeID + ";";
                                    sortby++;
                                });
                            }
                        }
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
        public async Task<List<AttributeHeader>> UpdateAttributeHeaderSort(AttributeHeader attributeHeader)
        {
            List<AttributeHeader> attributes = new List<AttributeHeader>();
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                if (attributeHeader.SubAttributeDetailId > 0 || attributeHeader.SubAttributeId > 0)
                {
                    parameters.Add("SubAttributeId", attributeHeader.SubAttributeId);
                    parameters.Add("SubAttributeDetailId", attributeHeader.SubAttributeDetailId);
                    parameters.Add("AttributeSortBy", attributeHeader.AttributeSortBy);
                    if (attributeHeader.SubAttributeDetailId > 0)
                    {
                        query = "SELECT AttributeID,SubAttributeId,AttributeSortBy,SubAttributeDetailId FROM AttributeHeader Where (IsDeleted=0 or IsDeleted is null) AND SubAttributeDetailId = @SubAttributeDetailId AND AttributeSortBy>@AttributeSortBy order by AttributeSortBy asc";
                    }
                    if (attributeHeader.SubAttributeId > 0)
                    {
                        query = "SELECT AttributeID,SubAttributeId,AttributeSortBy,SubAttributeDetailId FROM AttributeHeader Where (IsDeleted=0 or IsDeleted is null) AND SubAttributeId = @SubAttributeId AND AttributeSortBy>@AttributeSortBy order by AttributeSortBy asc";
                    }
                }
                using (var connection = CreateConnection())
                {
                    attributes = (await connection.QueryAsync<AttributeHeader>(query, parameters)).ToList();
                }
                return attributes;
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

                var query = "SELECT *,(CASE WHEN AttributeIsVisible is null  THEN 1  ELSE AttributeIsVisible END) AS AttributeIsVisible, FROM AttributeHeader Where IsDeleted=0 OR IsDeleted IS NULL AND  AttributeID = @ID ";
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
                var query = "select t1.*,(CASE WHEN t1.AttributeIsVisible is null  THEN 1  ELSE t1.AttributeIsVisible END) AS AttributeIsVisible,t6. UserName as AddedBy,t7. UserName as ModifiedBy,t2.CodeValue as ControlType,t2.CodeValue as ControlTypes,t5.Plantcode as AttributeCompany,t4.DisplayName as DataSourceDisplayName,t4.DataSourceTable from AttributeHeader t1  JOIN CodeMaster t2 ON t2.CodeID=t1.ControlTypeId\n\r" +
                    "LEFT JOIN AttributeHeaderDataSource t4 ON t4.AttributeHeaderDataSourceID=t1.DataSourceId\n\r" +
                    "LEFT JOIN ApplicationUSer t6 ON t6.UserId=t1.AddedbyuserId\n\r" +
                    "LEFT JOIN ApplicationUSer t7 ON t7.UserId=t1.ModifiedByUserID\n\r" +
                    "LEFT JOIN Plant t5 ON t5.PlantID=t1.AttributeCompanyId Where (t1.IsDeleted=0 OR t1.IsDeleted IS NULL) AND t1.SessionId=@SessionId\r\n";

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
                var query = "select t1.*,t9.Name as DynamicFormName,(CASE WHEN t1.AttributeIsVisible is null  THEN 1  ELSE t1.AttributeIsVisible END) AS AttributeIsVisible,t6. UserName as AddedBy,t7. UserName as ModifiedBy,t2.CodeValue as ControlType,t2.CodeValue as ControlTypes,t5.Plantcode as AttributeCompany,t4.DisplayName as DataSourceDisplayName,t4.DataSourceTable from AttributeHeader t1  " +
                    "JOIN CodeMaster t2 ON t2.CodeID=t1.ControlTypeId\n\r" +
                    "LEFT JOIN AttributeHeaderDataSource t4 ON t4.AttributeHeaderDataSourceID=t1.DataSourceId\n\r" +
                    "JOIN ApplicationUSer t6 ON t6.UserId=t1.AddedbyuserId\n\r" +
                    "LEFT JOIN ApplicationUSer t7 ON t7.UserId=t1.ModifiedByUserID\n\r" +
                    "LEFT JOIN DynamicForm t9 ON t9.ID=t1.DynamicFormID\n\r" +
                    "LEFT JOIN Plant t5 ON t5.PlantID=t1.AttributeCompanyId\r\n";
                if (IsSubForm == true)
                {
                    query += "where (t1.IsDeleted=0 OR t1.IsDeleted IS NULL) AND t1.IsSubForm=1";
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
                    query += "where (t9.IsDeleted=0 or t9.IsDeleted is null) AND (t1.IsDeleted=0 OR t1.IsDeleted IS NULL) AND t1.IsSubForm is null OR t1.IsSubForm=0;";

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
        public async Task<IReadOnlyList<DynamicFormFilter>> GetFilterDataSource()
        {

            try
            {
                var query = "select t1.* from DynamicFormFilter t1";


                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormFilter>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<AttributeDetails>> GetAttributeDetailsDataSource(long? AttributeId)
        {
            try
            {
                var query = "select  CONCAT('Attr_',AttributeDetailId)as AttributeDetailNameId,AttributeDetailId,AttributeDetailName,AttributeID,Description from AttributeDetails WHERE AttributeID=@AttributeId AND (Disabled=0 OR Disabled IS NULL);";
                var parameters = new DynamicParameters();
                parameters.Add("AttributeId", AttributeId);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AttributeDetails>(query, parameters)).ToList();
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
                var query = "SELECT tt1.* from (select t1.*,t2.CodeValue as ControlTypes,(case when t1.IsFilterDataSource=1 then  'Filter Data Source' ELSE t1.DropDownTypeID END) as DropDownTypeIDs,(case when t1.DynamicFormID>0 then  t5.Name ELSE (case when t1.IsFilterDataSource=1 then  tt4.DisplayName ELSE t4.DisplayName END) END) as DataSourceDisplayName,\r\n(case when t1.IsFilterDataSource=1 then  tt4.TableName ELSE t4.DataSourceTable END) as DataSourceTable\r\n from AttributeHeader t1  JOIN CodeMaster t2 ON t2.CodeID=t1.ControlTypeId\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t4 ON t4.AttributeHeaderDataSourceID=t1.DataSourceId LEFT JOIN DynamicFormFilter tt4 ON tt4.DynamicFilterId=t1.FilterDataSocurceID LEFT JOIN DynamicForm t5 ON t5.ID=t1.DynamicFormID WHERE \r\n" +
                    "(t5.IsDeleted=0 OR t5.IsDeleted IS NULL) AND t1.AttributeID \r\n" +
                    "Not In (select t3.AttributeID from DynamicFormSectionAttribute t3 where t3.AttributeID>0  AND t3.DynamicFormSectionID=" + dynamicFormSectionId + ") \r\n" +
                    "or t1.AttributeID=" + attributeID + ")tt1 Where (tt1.IsDeleted=0 OR tt1.IsDeleted IS NULL) AND (tt1.IsSubForm =0 or tt1.IsSubForm is null)";
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
                    query += "SELECT * FROM AttributeHeader Where  (IsDeleted=0 OR IsDeleted IS NULL) AND AttributeID!=@AttributeID AND AttributeName = @AttributeName\n\r";

                }
                else
                {
                    query += "SELECT * FROM AttributeHeader Where  (IsDeleted=0 OR IsDeleted IS NULL) AND AttributeName = @AttributeName\n\r";
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
        public async Task<IReadOnlyList<DropDownOptionsListModel>> GetApplicationMasterParentByList(IDictionary<string, object> dynamicsData, long? applicationMasterParentId)
        {
            List<DropDownOptionsListModel> dropDownOptionsListModels = new List<DropDownOptionsListModel>();
            ApplicationMasterParentByListModel ApplicationMasterParentByListModel = new ApplicationMasterParentByListModel();
            try
            {
                var dynamicscounts = dynamicsData.Count();
                if (dynamicscounts > 0 && applicationMasterParentId > 0)
                {
                    var query = "select t1.DynamicFormSectionAttributeId,t1.ApplicationMasterIds,t1.AttributeId,\r\n(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,t5.SectionName,t6.IsDynamicFormDropTagBox,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.DataSourceId,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t7.CodeValue as ControlType,t5.DynamicFormID,t6.DynamicFormID as DynamicFormGridDropDownID from DynamicFormSectionAttribute t1\r\nJOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId  \r\nJOIN DynamicForm t10 ON t5.DynamicFormID=t10.ID  \r\nJOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID  \r\nLEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID  \r\nJOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID  Where (t6.IsDeleted=0 OR t6.IsDeleted IS NULL) AND (t6.AttributeIsVisible=1 OR t6.AttributeIsVisible IS NULL) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t1.IsVisible= 1 OR t1.IsVisible is null)  \r\nAND t6.DropDownTypeID='Data Source' AND t8.DataSourceTable='ApplicationMasterParent'\r\nAND  (',' + RTRIM(t1.ApplicationMasterIDs) + ',') LIKE '%,' + '" + applicationMasterParentId + "' + ',%'\r\norder by t1.SortOrderBy asc\r\n;";
                    query += "Select * from ApplicationMasterParent;";
                    using (var connection = CreateConnection())
                    {
                        var results = await connection.QueryMultipleAsync(query);

                        ApplicationMasterParentByListModel.DynamicFormSectionAttribute = results.Read<DynamicFormSectionAttribute>().ToList();
                        ApplicationMasterParentByListModel.ApplicationMasterParent = results.Read<ApplicationMasterParent>().ToList();
                        var dynamicFormIds = ApplicationMasterParentByListModel.DynamicFormSectionAttribute.Select(s => s.DynamicFormId).Distinct().ToList();
                        dynamicFormIds = dynamicFormIds != null && dynamicFormIds.Count() > 0 ? dynamicFormIds : new List<long?>() { -1 };

                        var query1 = "select DynamicFormItem,DynamicFormID,DynamicFormDataId from DynamicFormData where (IsDeleted=0 or IsDeleted is null) AND DynamicFormID  in(" + string.Join(',', dynamicFormIds) + ");\n\r";
                        var results1 = await connection.QueryMultipleAsync(query1);
                        ApplicationMasterParentByListModel.DynamicFormData = results1.Read<DynamicFormData>().ToList();
                        List<long> DynamicFormDataIDs = new List<long>();
                        //if (ApplicationMasterParentByListModel.DynamicFormData != null && ApplicationMasterParentByListModel.DynamicFormData.Count() > 0)
                        //{
                        //    ApplicationMasterParentByListModel.DynamicFormData.ForEach(f =>
                        //    {
                        //        if (f.DynamicFormItem != null && IsValidJson(f.DynamicFormItem))
                        //        {
                        //            dynamic jsonObjs = new object();
                        //            jsonObjs = JsonConvert.DeserializeObject(f.DynamicFormItem);
                        //            var secAttr = ApplicationMasterParentByListModel.DynamicFormSectionAttribute.FirstOrDefault(w => w.DynamicFormId == f.DynamicFormId);
                        //            if (secAttr != null)
                        //            {
                        //                if (secAttr.ControlType == "ComboBox" && secAttr.DataSourceTable == "ApplicationMasterParent" && !string.IsNullOrEmpty(secAttr.ApplicationMasterIds))
                        //                {
                        //                    string attrName = secAttr.DynamicFormSectionAttributeId + "_" + secAttr.AttributeName;
                        //                    var Names = jsonObjs.ContainsKey(attrName);
                        //                    if (Names == true)
                        //                    {
                        //                        var applicationMasterIds = secAttr.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                        //                        if (applicationMasterIds != null && applicationMasterIds.Count() > 0)
                        //                        {
                        //                            var ab = ApplicationMasterParentByListModel.ApplicationMasterParent.Where(z => z.ApplicationMasterParentCodeId > 0 && applicationMasterIds.Contains(z.ApplicationMasterParentCodeId) && z.ApplicationMasterParentCodeId == applicationMasterParentId).FirstOrDefault();
                        //                            if (ab != null)
                        //                            {
                        //                                List<ApplicationMasterParent> nameDatas = new List<ApplicationMasterParent>();
                        //                                var namesattr = secAttr.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_AppMasterPar";
                        //                                var SubNamess = jsonObjs.ContainsKey(namesattr);
                        //                                if (SubNamess == true)
                        //                                {
                        //                                    nameDatas.Add(ab);
                        //                                    RemoveApplicationMasterParentSingleDataItem(ab, secAttr, nameDatas, ApplicationMasterParentByListModel.ApplicationMasterParent);
                        //                                    if (nameDatas != null && nameDatas.Count() > 0)
                        //                                    {
                        //                                        nameDatas.ForEach(n =>
                        //                                        {
                        //                                            var namesattr = secAttr.DynamicFormSectionAttributeId + "_" + n.ApplicationMasterParentCodeId + "_AppMasterPar";
                        //                                            var SubNamess = jsonObjs.ContainsKey(namesattr);
                        //                                            if (SubNamess == true)
                        //                                            {
                        //                                                var itemValue = jsonObjs[namesattr];
                        //                                                long? values = itemValue == null ? null : (long)itemValue;
                        //                                                n.ApplicationMasterChildId = values;
                        //                                            }

                        //                                        });
                        //                                        var exitsCount = nameDatas.Where(w => w.ApplicationMasterChildId > 0).ToList();
                        //                                        if (exitsCount.Count() == dynamicscounts)
                        //                                        {
                        //                                            exitsCount.ForEach(e =>
                        //                                            {
                        //                                                long? KeyValue = Convert.ToInt64(dynamicsData.Where(w => w.Key == e.ApplicationMasterParentCodeId.ToString()).FirstOrDefault().Key);
                        //                                                long? Value = Convert.ToInt64(dynamicsData.Where(w => w.Key == e.ApplicationMasterParentCodeId.ToString()).FirstOrDefault().Value);
                        //                                                if (e.ApplicationMasterParentCodeId == KeyValue && e.ApplicationMasterChildId == Value)
                        //                                                {
                        //                                                    DynamicFormDataIDs.Add(f.DynamicFormDataId);
                        //                                                }
                        //                                                else
                        //                                                {
                        //                                                    if (DynamicFormDataIDs != null && DynamicFormDataIDs.Count() > 0)
                        //                                                    {
                        //                                                        DynamicFormDataIDs.Remove(f.DynamicFormDataId);
                        //                                                    }
                        //                                                }
                        //                                            });
                        //                                        }
                        //                                    }
                        //                                }
                        //                            }

                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    });
                        //}

                        if (dynamicscounts == 3)
                        {
                            DynamicFormDataIDs = loadDynamicformData(dynamicsData, ApplicationMasterParentByListModel, applicationMasterParentId);

                            if (DynamicFormDataIDs.Contains(-1))
                            {


                                var keyValueList = dynamicsData.ToList();

                                // Check if the dictionary is not empty
                                if (keyValueList.Any())
                                {
                                    // Get the last key-value pair
                                    var lastKeyValuePair = keyValueList.Last();

                                    // Remove the last key-value pair from the dictionary
                                    dynamicsData.Remove(lastKeyValuePair.Key);
                                }

                                DynamicFormDataIDs = loadDynamicformData(dynamicsData, ApplicationMasterParentByListModel, applicationMasterParentId);
                            }
                        }
                        else
                        {
                            DynamicFormDataIDs = loadDynamicformData(dynamicsData, ApplicationMasterParentByListModel, applicationMasterParentId);
                        }


                        DynamicFormDataIDs = DynamicFormDataIDs != null && DynamicFormDataIDs.Count() > 0 ? DynamicFormDataIDs.Distinct().ToList() : new List<long>() { -1 };
                        var query4 = "select DynamicFormItem,DynamicFormID,DynamicFormDataId,ProfileNo,SessionId from DynamicFormData where (IsDeleted=0 or IsDeleted is null) AND DynamicFormDataGridID in(" + string.Join(',', DynamicFormDataIDs) + ");\n\r";
                        var results4 = await connection.QueryMultipleAsync(query4);
                        ApplicationMasterParentByListModel.DynamicFormData2 = results4.Read<DynamicFormData>().ToList();


                        var dynamicFormIdss = ApplicationMasterParentByListModel.DynamicFormData2.Select(s => s.DynamicFormId).Distinct().ToList();
                        dynamicFormIdss = dynamicFormIdss != null && dynamicFormIdss.Count() > 0 ? dynamicFormIdss : new List<long?>() { -1 };
                        var query2 = "select t1.DynamicFormSectionAttributeId,t1.AttributeId,t1.ApplicationMasterIds,\r\n(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,t5.SectionName,t6.IsDynamicFormDropTagBox,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.DataSourceId,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t7.CodeValue as ControlType,t5.DynamicFormID,t6.DynamicFormID as DynamicFormGridDropDownID from DynamicFormSectionAttribute t1\r\nJOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId  \r\nJOIN DynamicForm t10 ON t5.DynamicFormID=t10.ID  \r\nJOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID  \r\nLEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID  \r\nJOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID  Where (t6.IsDeleted=0 OR t6.IsDeleted IS NULL) AND (t6.AttributeIsVisible=1 OR t6.AttributeIsVisible IS NULL) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t1.IsVisible= 1 OR t1.IsVisible is null)  \r\nAND t6.DropDownTypeID Is null AND t6.DataSourceID Is Null\r\nAND t6.ControlTypeId IN(2701,2702)\r\nAND t5.DynamicFormID IN(" + string.Join(',', dynamicFormIdss) + ")\r\norder by t1.SortOrderBy asc";
                        var results2 = await connection.QueryMultipleAsync(query2);
                        ApplicationMasterParentByListModel.DynamicFormSectionAttribute2 = results2.Read<DynamicFormSectionAttribute>().ToList();
                        var AttributeIds = ApplicationMasterParentByListModel.DynamicFormSectionAttribute2.Select(s => s.AttributeId).Distinct().ToList();
                        AttributeIds = AttributeIds != null && AttributeIds.Count() > 0 ? AttributeIds : new List<long?>() { -1 };
                        var query3 = "select * from AttributeDetails where  AttributeId in(" + string.Join(',', AttributeIds) + ");\n\r";
                        var results3 = await connection.QueryMultipleAsync(query3);
                        ApplicationMasterParentByListModel.AttributeDetails = results3.Read<AttributeDetails>().ToList();
                    }
                    if (ApplicationMasterParentByListModel.DynamicFormData2 != null && ApplicationMasterParentByListModel.DynamicFormData2.Count() > 0)
                    {
                        ApplicationMasterParentByListModel.DynamicFormData2.ForEach(s =>
                        {
                            dynamic jsonObj = new object();
                            if (s.DynamicFormItem != null && IsValidJson(s.DynamicFormItem))
                            {
                                DropDownOptionsListModel dropDownOptionsListModel = new DropDownOptionsListModel();
                                int? count = 0;
                                jsonObj = JsonConvert.DeserializeObject(s.DynamicFormItem);
                                dropDownOptionsListModel.Id = s.DynamicFormDataId;
                                dropDownOptionsListModel.ProfileNo = s.ProfileNo;
                                dropDownOptionsListModel.SessionId = s.SessionId;
                                dropDownOptionsListModel.DynamicFormId = s.DynamicFormId;
                                var SectionAttr = ApplicationMasterParentByListModel.DynamicFormSectionAttribute2.Where(f => f.DynamicFormId == s.DynamicFormId).ToList();
                                if (SectionAttr != null && SectionAttr.Count() > 0)
                                {
                                    SectionAttr.ForEach(a =>
                                    {
                                        string attrName = a.DynamicFormSectionAttributeId + "_" + a.AttributeName;
                                        var Names = jsonObj.ContainsKey(attrName);
                                        if (Names == true)
                                        {
                                            var itemValue = jsonObj[attrName];
                                            if (a.ControlType == "ComboBox")
                                            {
                                                long? values = itemValue == null ? -1 : (long)itemValue;
                                                var desc = ApplicationMasterParentByListModel.AttributeDetails != null ? ApplicationMasterParentByListModel.AttributeDetails.Where(v => v.AttributeDetailID == values).FirstOrDefault()?.Description : string.Empty;
                                                var listss = ApplicationMasterParentByListModel.AttributeDetails != null ? ApplicationMasterParentByListModel.AttributeDetails.Where(v => v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                                                dropDownOptionsListModel.Value = listss;
                                                dropDownOptionsListModel.Description = desc;
                                                dropDownOptionsListModel.ValueId = values > 0 ? values : 0;
                                                dropDownOptionsListModel.Label = a.DisplayName;
                                                count = 1;
                                            }
                                            if (a.ControlType == "TextBox")
                                            {
                                                dropDownOptionsListModel.Text = (string)itemValue;
                                            }
                                        }
                                        else
                                        {

                                        }
                                    });
                                }
                                if (count == 1)
                                {
                                    dropDownOptionsListModels.Add(dropDownOptionsListModel);
                                }
                            }
                        });
                    }
                }
                return dropDownOptionsListModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private List<long> loadDynamicformData(IDictionary<string, object> dynamicsData, ApplicationMasterParentByListModel ApplicationMasterParentByListModel, long? applicationMasterParentId)
        {
            List<DropDownOptionsListModel> dropDownOptionsListModels = new List<DropDownOptionsListModel>();

            var dynamicscounts = dynamicsData.Count();
            List<long> DynamicFormDataIDs = new List<long>();
            if (ApplicationMasterParentByListModel.DynamicFormData != null && ApplicationMasterParentByListModel.DynamicFormData.Count() > 0)
            {
                ApplicationMasterParentByListModel.DynamicFormData.ForEach(f =>
                {
                    if (f.DynamicFormItem != null && IsValidJson(f.DynamicFormItem))
                    {
                        dynamic jsonObjs = new object();
                        jsonObjs = JsonConvert.DeserializeObject(f.DynamicFormItem);
                        var secAttr = ApplicationMasterParentByListModel.DynamicFormSectionAttribute.FirstOrDefault(w => w.DynamicFormId == f.DynamicFormId);
                        if (secAttr != null)
                        {
                            if (secAttr.ControlType == "ComboBox" && secAttr.DataSourceTable == "ApplicationMasterParent" && !string.IsNullOrEmpty(secAttr.ApplicationMasterIds))
                            {
                                string attrName = secAttr.DynamicFormSectionAttributeId + "_" + secAttr.AttributeName;
                                var Names = jsonObjs.ContainsKey(attrName);
                                if (Names == true)
                                {
                                    var applicationMasterIds = secAttr.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                                    if (applicationMasterIds != null && applicationMasterIds.Count() > 0)
                                    {
                                        var ab = ApplicationMasterParentByListModel.ApplicationMasterParent.Where(z => z.ApplicationMasterParentCodeId > 0 && applicationMasterIds.Contains(z.ApplicationMasterParentCodeId) && z.ApplicationMasterParentCodeId == applicationMasterParentId).FirstOrDefault();
                                        if (ab != null)
                                        {
                                            List<ApplicationMasterParent> nameDatas = new List<ApplicationMasterParent>();
                                            var namesattr = secAttr.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_AppMasterPar";
                                            var SubNamess = jsonObjs.ContainsKey(namesattr);
                                            if (SubNamess == true)
                                            {
                                                nameDatas.Add(ab);
                                                RemoveApplicationMasterParentSingleDataItem(ab, secAttr, nameDatas, ApplicationMasterParentByListModel.ApplicationMasterParent);
                                                if (nameDatas != null && nameDatas.Count() > 0)
                                                {
                                                    nameDatas.ForEach(n =>
                                                    {
                                                        var namesattr = secAttr.DynamicFormSectionAttributeId + "_" + n.ApplicationMasterParentCodeId + "_AppMasterPar";
                                                        var SubNamess = jsonObjs.ContainsKey(namesattr);
                                                        if (SubNamess == true)
                                                        {
                                                            var itemValue = jsonObjs[namesattr];
                                                            long? values = itemValue == null ? null : (long)itemValue;
                                                            n.ApplicationMasterChildId = values;
                                                        }

                                                    });
                                                    var exitsCount = nameDatas.Where(w => w.ApplicationMasterChildId > 0).ToList();
                                                    // if (exitsCount.Count() == dynamicscounts)
                                                    //{
                                                    exitsCount.ForEach(e =>
                                                    {
                                                        long? KeyValue = Convert.ToInt64(dynamicsData.Where(w => w.Key == e.ApplicationMasterParentCodeId.ToString()).FirstOrDefault().Key);
                                                        long? Value = Convert.ToInt64(dynamicsData.Where(w => w.Key == e.ApplicationMasterParentCodeId.ToString()).FirstOrDefault().Value);
                                                        if (e.ApplicationMasterParentCodeId == KeyValue && e.ApplicationMasterChildId == Value)
                                                        {
                                                            var exitss = DynamicFormDataIDs.Where(a => a == f.DynamicFormDataId).Count();
                                                            if (exitss == 0)
                                                            {
                                                                DynamicFormDataIDs.Add(f.DynamicFormDataId);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (DynamicFormDataIDs != null && DynamicFormDataIDs.Count() > 0)
                                                            {
                                                                if (e.ApplicationMasterChildId != Value)
                                                                {
                                                                    DynamicFormDataIDs.Remove(f.DynamicFormDataId);
                                                                }
                                                            }
                                                        }
                                                    });
                                                    //}
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                });
            }
            return DynamicFormDataIDs = DynamicFormDataIDs != null && DynamicFormDataIDs.Count() > 0 ? DynamicFormDataIDs.Distinct().ToList() : new List<long>() { -1 };
        }

        public async Task<IReadOnlyList<DropDownOptionsListModel>> GetApplicationMasterParentByMobileList(IDictionary<string, JsonElement> dynamicsData, long? applicationMasterParentId)
        {
            List<DropDownOptionsListModel> dropDownOptionsListModels = new List<DropDownOptionsListModel>();
            ApplicationMasterParentByListModel ApplicationMasterParentByListModel = new ApplicationMasterParentByListModel();
            try
            {
                var dynamicscounts = dynamicsData.Count();
                if (dynamicscounts > 0 && applicationMasterParentId > 0)
                {
                    var query = "select t1.DynamicFormSectionAttributeId,t1.ApplicationMasterIds,t1.AttributeId,\r\n(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,t5.SectionName,t6.IsDynamicFormDropTagBox,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.DataSourceId,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t7.CodeValue as ControlType,t5.DynamicFormID,t6.DynamicFormID as DynamicFormGridDropDownID from DynamicFormSectionAttribute t1\r\nJOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId  \r\nJOIN DynamicForm t10 ON t5.DynamicFormID=t10.ID  \r\nJOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID  \r\nLEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID  \r\nJOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID  Where (t6.IsDeleted=0 OR t6.IsDeleted IS NULL) AND (t6.AttributeIsVisible=1 OR t6.AttributeIsVisible IS NULL) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t1.IsVisible= 1 OR t1.IsVisible is null)  \r\nAND t6.DropDownTypeID='Data Source' AND t8.DataSourceTable='ApplicationMasterParent'\r\nAND  (',' + RTRIM(t1.ApplicationMasterIDs) + ',') LIKE '%,' + '" + applicationMasterParentId + "' + ',%'\r\norder by t1.SortOrderBy asc\r\n;";
                    query += "Select * from ApplicationMasterParent;";
                    using (var connection = CreateConnection())
                    {
                        var results = await connection.QueryMultipleAsync(query);

                        ApplicationMasterParentByListModel.DynamicFormSectionAttribute = results.Read<DynamicFormSectionAttribute>().ToList();
                        ApplicationMasterParentByListModel.ApplicationMasterParent = results.Read<ApplicationMasterParent>().ToList();
                        var dynamicFormIds = ApplicationMasterParentByListModel.DynamicFormSectionAttribute.Select(s => s.DynamicFormId).Distinct().ToList();
                        dynamicFormIds = dynamicFormIds != null && dynamicFormIds.Count() > 0 ? dynamicFormIds : new List<long?>() { -1 };

                        var query1 = "select DynamicFormItem,DynamicFormID,DynamicFormDataId from DynamicFormData where (IsDeleted=0 or IsDeleted is null) AND DynamicFormID  in(" + string.Join(',', dynamicFormIds) + ");\n\r";
                        var results1 = await connection.QueryMultipleAsync(query1);
                        ApplicationMasterParentByListModel.DynamicFormData = results1.Read<DynamicFormData>().ToList();
                        List<long> DynamicFormDataIDs = new List<long>();

                        if (dynamicscounts == 3)
                        {
                            DynamicFormDataIDs = loadDynamicformDatamobile(dynamicsData, ApplicationMasterParentByListModel, applicationMasterParentId);

                            if (DynamicFormDataIDs.Contains(-1))
                            {


                                var keyValueList = dynamicsData.ToList();

                                // Check if the dictionary is not empty
                                if (keyValueList.Any())
                                {
                                    // Get the last key-value pair
                                    var lastKeyValuePair = keyValueList.Last();

                                    // Remove the last key-value pair from the dictionary
                                    dynamicsData.Remove(lastKeyValuePair.Key);
                                }

                                DynamicFormDataIDs = loadDynamicformDatamobile(dynamicsData, ApplicationMasterParentByListModel, applicationMasterParentId);
                            }
                        }
                        else
                        {
                            DynamicFormDataIDs = loadDynamicformDatamobile(dynamicsData, ApplicationMasterParentByListModel, applicationMasterParentId);
                        }


                        DynamicFormDataIDs = DynamicFormDataIDs != null && DynamicFormDataIDs.Count() > 0 ? DynamicFormDataIDs.Distinct().ToList() : new List<long>() { -1 };
                        var query4 = "select DynamicFormItem,DynamicFormID,DynamicFormDataId,ProfileNo,SessionId from DynamicFormData where (IsDeleted=0 or IsDeleted is null) AND DynamicFormDataGridID in(" + string.Join(',', DynamicFormDataIDs) + ");\n\r";
                        var results4 = await connection.QueryMultipleAsync(query4);
                        ApplicationMasterParentByListModel.DynamicFormData2 = results4.Read<DynamicFormData>().ToList();


                        var dynamicFormIdss = ApplicationMasterParentByListModel.DynamicFormData2.Select(s => s.DynamicFormId).Distinct().ToList();
                        dynamicFormIdss = dynamicFormIdss != null && dynamicFormIdss.Count() > 0 ? dynamicFormIdss : new List<long?>() { -1 };
                        var query2 = "select t1.DynamicFormSectionAttributeId,t1.AttributeId,t1.ApplicationMasterIds,\r\n(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,t5.SectionName,t6.IsDynamicFormDropTagBox,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.DataSourceId,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t7.CodeValue as ControlType,t5.DynamicFormID,t6.DynamicFormID as DynamicFormGridDropDownID from DynamicFormSectionAttribute t1\r\nJOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId  \r\nJOIN DynamicForm t10 ON t5.DynamicFormID=t10.ID  \r\nJOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID  \r\nLEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID  \r\nJOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID  Where (t6.IsDeleted=0 OR t6.IsDeleted IS NULL) AND (t6.AttributeIsVisible=1 OR t6.AttributeIsVisible IS NULL) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t1.IsVisible= 1 OR t1.IsVisible is null)  \r\nAND t6.DropDownTypeID Is null AND t6.DataSourceID Is Null\r\nAND t6.ControlTypeId IN(2701,2702)\r\nAND t5.DynamicFormID IN(" + string.Join(',', dynamicFormIdss) + ")\r\norder by t1.SortOrderBy asc";
                        var results2 = await connection.QueryMultipleAsync(query2);
                        ApplicationMasterParentByListModel.DynamicFormSectionAttribute2 = results2.Read<DynamicFormSectionAttribute>().ToList();
                        var AttributeIds = ApplicationMasterParentByListModel.DynamicFormSectionAttribute2.Select(s => s.AttributeId).Distinct().ToList();
                        AttributeIds = AttributeIds != null && AttributeIds.Count() > 0 ? AttributeIds : new List<long?>() { -1 };
                        var query3 = "select * from AttributeDetails where  AttributeId in(" + string.Join(',', AttributeIds) + ");\n\r";
                        var results3 = await connection.QueryMultipleAsync(query3);
                        ApplicationMasterParentByListModel.AttributeDetails = results3.Read<AttributeDetails>().ToList();
                    }
                    if (ApplicationMasterParentByListModel.DynamicFormData2 != null && ApplicationMasterParentByListModel.DynamicFormData2.Count() > 0)
                    {
                        ApplicationMasterParentByListModel.DynamicFormData2.ForEach(s =>
                        {
                            dynamic jsonObj = new object();
                            if (s.DynamicFormItem != null && IsValidJson(s.DynamicFormItem))
                            {
                                DropDownOptionsListModel dropDownOptionsListModel = new DropDownOptionsListModel();
                                int? count = 0;
                                jsonObj = JsonConvert.DeserializeObject(s.DynamicFormItem);
                                dropDownOptionsListModel.Id = s.DynamicFormDataId;
                                dropDownOptionsListModel.ProfileNo = s.ProfileNo;
                                dropDownOptionsListModel.SessionId = s.SessionId;
                                dropDownOptionsListModel.DynamicFormId = s.DynamicFormId;
                                var SectionAttr = ApplicationMasterParentByListModel.DynamicFormSectionAttribute2.Where(f => f.DynamicFormId == s.DynamicFormId).ToList();
                                if (SectionAttr != null && SectionAttr.Count() > 0)
                                {
                                    SectionAttr.ForEach(a =>
                                    {
                                        string attrName = a.DynamicFormSectionAttributeId + "_" + a.AttributeName;
                                        var Names = jsonObj.ContainsKey(attrName);
                                        if (Names == true)
                                        {
                                            var itemValue = jsonObj[attrName];
                                            if (a.ControlType == "ComboBox")
                                            {
                                                long? values = itemValue == null ? -1 : (long)itemValue;
                                                var desc = ApplicationMasterParentByListModel.AttributeDetails != null ? ApplicationMasterParentByListModel.AttributeDetails.Where(v => v.AttributeDetailID == values).FirstOrDefault()?.Description : string.Empty;
                                                var listss = ApplicationMasterParentByListModel.AttributeDetails != null ? ApplicationMasterParentByListModel.AttributeDetails.Where(v => v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                                                dropDownOptionsListModel.Value = listss;
                                                dropDownOptionsListModel.Description = desc;
                                                dropDownOptionsListModel.ValueId = values > 0 ? values : 0;
                                                dropDownOptionsListModel.Label = a.DisplayName;
                                                count = 1;
                                            }
                                            if (a.ControlType == "TextBox")
                                            {
                                                dropDownOptionsListModel.Text = (string)itemValue;
                                            }
                                        }
                                        else
                                        {

                                        }
                                    });
                                }
                                if (count == 1)
                                {
                                    dropDownOptionsListModels.Add(dropDownOptionsListModel);
                                }
                            }
                        });
                    }
                }
                return dropDownOptionsListModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private List<long> loadDynamicformDatamobile(IDictionary<string, JsonElement> jsondata, ApplicationMasterParentByListModel ApplicationMasterParentByListModel, long? applicationMasterParentId)
        {
            List<DropDownOptionsListModel> dropDownOptionsListModels = new List<DropDownOptionsListModel>();



            // Convert to IDictionary<string, object>
            IDictionary<string, object> dynamicsData = ConvertToObjectDictionary(jsondata);

            var dynamicscounts = dynamicsData.Count();
            List<long> DynamicFormDataIDs = new List<long>();
            if (ApplicationMasterParentByListModel.DynamicFormData != null && ApplicationMasterParentByListModel.DynamicFormData.Count() > 0)
            {
                ApplicationMasterParentByListModel.DynamicFormData.ForEach(f =>
                {
                    if (f.DynamicFormItem != null && IsValidJson(f.DynamicFormItem))
                    {
                        dynamic jsonObjs = new object();
                        jsonObjs = JsonConvert.DeserializeObject(f.DynamicFormItem);
                        var secAttr = ApplicationMasterParentByListModel.DynamicFormSectionAttribute.FirstOrDefault(w => w.DynamicFormId == f.DynamicFormId);
                        if (secAttr != null)
                        {
                            if (secAttr.ControlType == "ComboBox" && secAttr.DataSourceTable == "ApplicationMasterParent" && !string.IsNullOrEmpty(secAttr.ApplicationMasterIds))
                            {
                                string attrName = secAttr.DynamicFormSectionAttributeId + "_" + secAttr.AttributeName;
                                var Names = jsonObjs.ContainsKey(attrName);
                                if (Names == true)
                                {
                                    var applicationMasterIds = secAttr.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                                    if (applicationMasterIds != null && applicationMasterIds.Count() > 0)
                                    {
                                        var ab = ApplicationMasterParentByListModel.ApplicationMasterParent.Where(z => z.ApplicationMasterParentCodeId > 0 && applicationMasterIds.Contains(z.ApplicationMasterParentCodeId) && z.ApplicationMasterParentCodeId == applicationMasterParentId).FirstOrDefault();
                                        if (ab != null)
                                        {
                                            List<ApplicationMasterParent> nameDatas = new List<ApplicationMasterParent>();
                                            var namesattr = secAttr.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_AppMasterPar";
                                            var SubNamess = jsonObjs.ContainsKey(namesattr);
                                            if (SubNamess == true)
                                            {
                                                nameDatas.Add(ab);
                                                RemoveApplicationMasterParentSingleDataItem(ab, secAttr, nameDatas, ApplicationMasterParentByListModel.ApplicationMasterParent);
                                                if (nameDatas != null && nameDatas.Count() > 0)
                                                {
                                                    nameDatas.ForEach(n =>
                                                    {
                                                        var namesattr = secAttr.DynamicFormSectionAttributeId + "_" + n.ApplicationMasterParentCodeId + "_AppMasterPar";
                                                        var SubNamess = jsonObjs.ContainsKey(namesattr);
                                                        if (SubNamess == true)
                                                        {
                                                            var itemValue = jsonObjs[namesattr];
                                                            long? values = itemValue == null ? null : (long)itemValue;
                                                            n.ApplicationMasterChildId = values;
                                                        }

                                                    });
                                                    var exitsCount = nameDatas.Where(w => w.ApplicationMasterChildId > 0).ToList();
                                                    if (exitsCount.Count() == dynamicscounts)
                                                    {
                                                        exitsCount.ForEach(e =>
                                                        {
                                                            long? KeyValue = Convert.ToInt64(dynamicsData.Where(w => w.Key == e.ApplicationMasterParentCodeId.ToString()).FirstOrDefault().Key);
                                                            long? Value = Convert.ToInt64(dynamicsData.Where(w => w.Key == e.ApplicationMasterParentCodeId.ToString()).FirstOrDefault().Value);
                                                            if (e.ApplicationMasterParentCodeId == KeyValue && e.ApplicationMasterChildId == Value)
                                                            {
                                                                var exitss = DynamicFormDataIDs.Where(a => a == f.DynamicFormDataId).Count();
                                                                if (exitss == 0)
                                                                {
                                                                    DynamicFormDataIDs.Add(f.DynamicFormDataId);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (DynamicFormDataIDs != null && DynamicFormDataIDs.Count() > 0)
                                                                {
                                                                    if (e.ApplicationMasterChildId != Value)
                                                                    {
                                                                        DynamicFormDataIDs.Remove(f.DynamicFormDataId);
                                                                    }
                                                                }
                                                            }
                                                        });

                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                });
            }
            return DynamicFormDataIDs = DynamicFormDataIDs != null && DynamicFormDataIDs.Count() > 0 ? DynamicFormDataIDs.Distinct().ToList() : new List<long>() { -1 };
        }


        public static IDictionary<string, object> ConvertToObjectDictionary(IDictionary<string, JsonElement> jsonElementData)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();

            foreach (var kvp in jsonElementData)
            {
                object value;

                // Convert JsonElement to object
                switch (kvp.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        value = kvp.Value.GetString();
                        break;
                    case JsonValueKind.Number:
                        value = kvp.Value.GetInt32();
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        value = kvp.Value.GetBoolean();
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported JsonValueKind: {kvp.Value.ValueKind}");
                }

                result.Add(kvp.Key, value);
            }

            return result;
        }

        public static T DeserializeToObject<T>(IDictionary<string, object> dictionary)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(dictionary);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }


        void RemoveApplicationMasterParentSingleDataItem(ApplicationMasterParent applicationMasterParent, DynamicFormSectionAttribute dynamicFormSectionAttribute, List<ApplicationMasterParent> dataColumnNames, List<ApplicationMasterParent> applicationMasterParents)
        {
            if (applicationMasterParent != null)
            {
                var listss = applicationMasterParents.FirstOrDefault(f => f.ParentId == applicationMasterParent.ApplicationMasterParentCodeId);
                if (listss != null)
                {
                    var nameData = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + listss.ApplicationMasterParentCodeId + "_AppMasterPar";
                    dataColumnNames.Add(listss);
                    RemoveApplicationMasterParentSingleDataItem(listss, dynamicFormSectionAttribute, dataColumnNames, applicationMasterParents);
                }
            }
        }
        public async Task<AttributeHeaderListModel> GetAllAttributeNameAsync(DynamicForm dynamicForm, long? UserId, bool? IsSubFormLoad)
        {
            try
            {
                AttributeHeaderListModel attributeHeaderListModel = new AttributeHeaderListModel();
                var dynamicFormSectionAttributeSecurity = new List<DynamicFormSectionAttributeSecurity>();
                var DynamicFormSectionAttrFormulaFunctionList = new List<DynamicFormSectionAttrFormulaFunction>();
                var applicationMasters = new List<ApplicationMaster>(); var applicationMasterParent = new List<ApplicationMasterParent>(); var dynamicFormSectionAttributeSection = new List<DynamicFormSectionAttributeSection>();
                using (var connection = CreateConnection())
                {
                    var query = "select t1.IsAutoNumberEnabled,t1.DynamicFormSectionID,t1.SectionName,t1.SessionID,t1.StatusCodeID,t1.Instruction,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.SortOrderBy," +
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
                    query += "select (case when t1.IsDynamicFormGridDropdownMultiple is NULL then  0 ELSE t1.IsDynamicFormGridDropdownMultiple END) as IsDynamicFormGridDropdownMultiple,t1.IsDynamicFormGridDropdown,t1.GridDropDownDynamicFormID,t12.Name as GridDropDownDynamicFormName,t1.DynamicFormSectionAttributeID,t1.DynamicFormSectionID,t1.SessionID,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.AttributeID,t1.SortOrderBy,t1.ColSpan,t1.DisplayName,t1.IsMultiple,t1.IsRequired,t1.RequiredMessage,t1.IsSpinEditType,t1.FormUsedCount,t1.IsDisplayTableHeader,t1.FormToolTips,t1.IsVisible,t1.RadioLayout,t1.IsRadioCheckRemarks,t1.RemarksLabelName,t1.IsDeleted,t1.IsPlantLoadDependency,t1.PlantDropDownWithOtherDataSourceID,t1.PlantDropDownWithOtherDataSourceLabelName,t1.PlantDropDownWithOtherDataSourceIDs,t1.IsSetDefaultValue,t1.IsDefaultReadOnly,t1.ApplicationMasterID,t1.ApplicationMasterIDs,t1.IsDisplayDropDownHeader\n\r" +
                        ",(case when t1.IsDependencyMultiple is NULL then  0 ELSE t1.IsDependencyMultiple END) as IsDependencyMultiple,(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,t5.SectionName,t11.DataSourceTable as PlantDropDownWithOtherDataSourceTable,t9.sessionId as DynamicFormSessionId,t6.IsDynamicFormDropTagBox,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.DataSourceId," +
                        "t8.DisplayName as DataSourceDisplayName,\r\n" +
                        "t8.DataSourceTable  as DataSourceTable," +
                        "t7.CodeValue as ControlType,t5.DynamicFormID,t6.DynamicFormID as DynamicFormGridDropDownID,t6.FilterDataSocurceID,t1.FormulaTextBox \r\n" +
                        "from\r\n" +
                        "DynamicFormSectionAttribute t1\r\n" +
                        "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                        "JOIN DynamicForm t10 ON t5.DynamicFormID=t10.ID\r\n" +
                        "JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                        "LEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID\r\n" +
                        "LEFT JOIN DynamicForm t9 ON t9.ID=t6.DynamicFormID\r\n" +
                         "LEFT JOIN AttributeHeaderDataSource t11 ON t11.AttributeHeaderDataSourceID=t1.PlantDropDownWithOtherDataSourceId\r\n" +
                        "JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\n" +
                        "LEFT JOIN DynamicForm t12 ON t12.ID=t1.GridDropDownDynamicFormID\r\n" +
                        "Where (t9.IsDeleted=0 OR t9.IsDeleted IS NULL) AND (t6.IsDeleted=0 OR t6.IsDeleted IS NULL) AND (t6.AttributeIsVisible=1 OR t6.AttributeIsVisible IS NULL) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t1.IsVisible= 1 OR t1.IsVisible is null) AND t5.DynamicFormID=" + dynamicForm.ID + " order by t1.SortOrderBy asc;";
                    query += "Select plantId,PlantCode,Description from Plant;";
                    query += "Select t1.HeaderDataSourceId,t1.AttributeHeaderDataSourceId,t1.DisplayName,t1.DataSourceTable,(Select COUNT(*) as IsDynamicFormFilterBy from DynamicFormFilterBy t2 where t2.AttributeHeaderDataSourceID=t1.AttributeHeaderDataSourceID)as IsDynamicFormFilterBy from AttributeHeaderDataSource t1;";
                    query += "Select DynamicFormSectionAttributeSecurityId,DynamicFormSectionAttributeId,UserId,IsAccess,IsViewFormatOnly from DynamicFormSectionAttributeSecurity;";
                    query += "Select ApplicationMasterId,ApplicationMasterName,ApplicationMasterDescription,ApplicationMasterCodeId from ApplicationMaster;";
                    query += "Select ApplicationMasterParentId,ApplicationMasterParentCodeId,ApplicationMasterName,Description,ParentId from ApplicationMasterParent;";
                    query += "select t1.*,t2.SequenceNo,t2.DynamicFormSectionAttributeID from DynamicFormSectionAttributeSection t1 JOIN DynamicFormSectionAttributeSectionParent t2 ON t1.DynamicFormSectionAttributeSectionParentID=t2.DynamicFormSectionAttributeSectionParentID;";
                    query += "SELECT t1.*,t2.Type,t2.FormulaFunctionName FROM DynamicFormSectionAttrFormulaFunction t1 JOIN DynamicFormSectionAttrFormulaMasterFunction t2 ON t1.DynamicFormSectionAttrFormulaMasterFuntionId=t2.MasterID;";
                    var results = await connection.QueryMultipleAsync(query);
                    attributeHeaderListModel.DynamicFormSection = results.Read<DynamicFormSection>().ToList();
                    attributeHeaderListModel.DynamicFormSectionAttribute = results.Read<DynamicFormSectionAttribute>().ToList();
                    attributeHeaderListModel.Plant = results.Read<Plant>().ToList();
                    attributeHeaderListModel.AttributeHeaderDataSource = results.Read<AttributeHeaderDataSource>().ToList();
                    dynamicFormSectionAttributeSecurity = results.Read<DynamicFormSectionAttributeSecurity>().ToList();
                    applicationMasters = results.Read<ApplicationMaster>().ToList();
                    applicationMasterParent = results.Read<ApplicationMasterParent>().ToList();
                    dynamicFormSectionAttributeSection = results.Read<DynamicFormSectionAttributeSection>().ToList();
                    attributeHeaderListModel.ApplicationMasterParent = applicationMasterParent;
                    DynamicFormSectionAttrFormulaFunctionList = results.Read<DynamicFormSectionAttrFormulaFunction>().ToList();
                }
                if (attributeHeaderListModel.ApplicationMasterParent != null && attributeHeaderListModel.ApplicationMasterParent.Count() > 0)
                {
                    attributeHeaderListModel.ApplicationMasterParent = attributeHeaderListModel.ApplicationMasterParent.OrderBy(o => o.ParentId).ToList();
                    attributeHeaderListModel.ApplicationMasterParent.ForEach(z =>
                    {
                        if (z.ParentId == null)
                        {
                            z.DummyNo = 1;
                        }
                        else
                        {
                            var exits = attributeHeaderListModel.ApplicationMasterParent.Where(w => w.ApplicationMasterParentCodeId == z.ParentId).FirstOrDefault();
                            if (exits != null)
                            {
                                z.DummyNo = exits.DummyNo + 1;
                            }
                        }
                    });
                }
                if (attributeHeaderListModel.DynamicFormSectionAttribute != null)
                {
                    string? plantCode = null;
                    if (attributeHeaderListModel.Plant != null && attributeHeaderListModel.Plant.Count > 0 && dynamicForm.CompanyId > 0)
                    {
                        plantCode = attributeHeaderListModel.Plant.FirstOrDefault(f => f.PlantCode != null && f.PlantID == dynamicForm.CompanyId)?.PlantCode;
                        plantCode = plantCode != null ? plantCode.ToLower() : null;
                    }
                    List<long?> DynamicFormSectionAttributeIds = attributeHeaderListModel.DynamicFormSectionAttribute.Select(a => (long?)a.DynamicFormSectionAttributeId).Distinct().ToList();
                    attributeHeaderListModel.DynamicFormSectionAttrFormulaFunctions = DynamicFormSectionAttrFormulaFunctionList.Where(w => DynamicFormSectionAttributeIds.Contains(w.DynamicFormSectionAttributeId)).ToList();
                    attributeHeaderListModel.DynamicFormSectionAttributeSections = dynamicFormSectionAttributeSection.Where(q => DynamicFormSectionAttributeIds.Contains(q.DynamicFormSectionAttributeId)).ToList();
                    List<string?> appMasterNames = new List<string?>() { "ApplicationMasterParent", "ApplicationMaster" };
                    List<long?> DynamicGridFormIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => appMasterNames.Contains(a.DataSourceTable) && a.IsDynamicFormGridDropdown == true && a.GridDropDownDynamicFormID > 0).Select(z => z.GridDropDownDynamicFormID).Distinct().ToList();
                    List<long?> DynamicFormIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.DataSourceTable == "DynamicGrid" && a.DynamicFormGridDropDownId > 0).Select(a => a.DynamicFormGridDropDownId).Distinct().ToList();
                    if (DynamicGridFormIds != null && DynamicGridFormIds.Count > 0)
                    {
                        if (DynamicFormIds == null)
                        {
                            DynamicFormIds = new List<long?>();
                        }
                        DynamicFormIds.AddRange(DynamicGridFormIds);
                        //var DropDownOptionsFromGridListModel = await GetDynamicFormGridModelAsync(DynamicGridFormIds, UserId, dynamicForm.CompanyId, plantCode, applicationMasters, applicationMasterParent, null, false);
                    }
                    if (DynamicFormIds != null && DynamicFormIds.Count > 0)
                    {
                        DynamicFormIds = DynamicFormIds.Distinct().ToList();
                        attributeHeaderListModel.DropDownOptionsGridListModel = await GetDynamicFormGridModelAsync(DynamicFormIds, UserId, dynamicForm.CompanyId, plantCode, applicationMasters, applicationMasterParent, null, false);
                    }
                    List<string?> dataSourceTableIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.DataSourceTable != null && a.DropDownTypeId == "Data Source").Select(a => a.DataSourceTable).Distinct().ToList();
                    List<long?> applicationMasterIds = new List<long?>(); List<long?> ApplicationMasterParentIds = new List<long?>();
                    AttributeDetailsAdds attributeResultDetails = new AttributeDetailsAdds();
                    attributeHeaderListModel.AttributeDetails = new List<AttributeDetails>();
                    // if (IsSubFormLoad == true)
                    // {
                    List<long?> attributeIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.AttributeId > 0).Select(a => a.AttributeId).Distinct().ToList();
                    List<long> attributeSubFormIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.AttributeId > 0 && a.ControlTypeId == 2710).Select(a => a.AttributeId.Value).Distinct().ToList();
                    attributeResultDetails = await GetAttributeDetails(attributeIds, attributeSubFormIds, "Main", dynamicForm.CompanyId, plantCode, applicationMasters, applicationMasterParent);
                    var attributeDetails = attributeResultDetails.AttributeDetails;
                    attributeHeaderListModel.AttributeDetails = attributeDetails != null && attributeDetails.Count > 0 ? attributeDetails.Where(w => attributeIds.Contains(w.AttributeID)).ToList() : new List<AttributeDetails>();
                    var AttributeDetailsIds = attributeHeaderListModel.AttributeDetails.Select(s => s.AttributeDetailID).ToList();
                    List<long?> applicationMasterSubIds = new List<long?>();
                    var attributeSubResultDetails = await GetAttributeDetails(new List<long?>(), AttributeDetailsIds, "Sub", dynamicForm.CompanyId, plantCode, applicationMasters, applicationMasterParent);

                    if (attributeHeaderListModel.AttributeDetails != null && attributeHeaderListModel.AttributeDetails.Count > 0)
                    {
                        attributeHeaderListModel.AttributeDetails.ForEach(z =>
                        {
                            z.SubAttributeHeaders = attributeSubResultDetails.AttributeHeader.Where(q => q.SubAttributeDetailId == z.AttributeDetailID).ToList();
                        });
                    }
                    // }
                    var dataSourceList = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(dynamicForm.CompanyId, dataSourceTableIds, plantCode, applicationMasterIds, ApplicationMasterParentIds);
                    attributeHeaderListModel.AttributeDetails.AddRange(dataSourceList);
                    attributeHeaderListModel.DynamicFormSectionAttribute.ForEach(s =>
                    {
                        s.DynamicFormSectionAttributeSecurity = dynamicFormSectionAttributeSecurity != null && dynamicFormSectionAttributeSecurity.Count > 0 ? dynamicFormSectionAttributeSecurity.Where(d => d.DynamicFormSectionAttributeId == s.DynamicFormSectionAttributeId).ToList() : new List<DynamicFormSectionAttributeSecurity>();
                        s.AttributeName = string.IsNullOrEmpty(s.AttributeName) ? string.Empty : char.ToUpper(s.AttributeName[0]) + s.AttributeName.Substring(1);
                        s.DynamicAttributeName = s.DynamicFormSectionAttributeId + "_" + s.AttributeName;
                        if (s.DataSourceTable == "DynamicGrid")
                        {
                            s.DynamicGridDynamicFormData = attributeHeaderListModel.DropDownOptionsGridListModel.DynamicFormData.Where(x => x.DynamicFormId == s.DynamicFormGridDropDownId).ToList();
                        }
                        if (s.IsPlantLoadDependency == true && !string.IsNullOrEmpty(s.PlantDropDownWithOtherDataSourceIds))
                        {
                            var PlantDropDownWithOtherDataSourceListIds = s.PlantDropDownWithOtherDataSourceIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (PlantDropDownWithOtherDataSourceListIds.Count > 0)
                            {
                                s.AttributeHeaderDataSource = attributeHeaderListModel.AttributeHeaderDataSource.Where(z => z.DataSourceTable != null && PlantDropDownWithOtherDataSourceListIds.Contains(z.AttributeHeaderDataSourceId)).ToList();
                            }
                        }
                        if (s.DataSourceTable == "ApplicationMaster" && !string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                s.ApplicationMaster = applicationMasters.Where(z => z.ApplicationMasterId > 0 && applicationMasterIds.Contains(z.ApplicationMasterId)).ToList();
                            }
                        }
                        if (s.DataSourceTable == "ApplicationMasterParent" && !string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                s.ApplicationMasterParents = applicationMasterParent.Where(z => z.ApplicationMasterParentCodeId > 0 && applicationMasterIds.Contains(z.ApplicationMasterParentCodeId)).ToList();
                            }
                        }
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
        public async Task<DropDownOptionsGridListModel> GetDynamicGridNested(List<long?> DynamicFormDataId, long? userId)
        {
            try
            {
                DropDownOptionsGridListModel dropDownOptionsGridListModel = new DropDownOptionsGridListModel();
                DynamicFormData dynamicFormData = new DynamicFormData();
                List<DynamicFormData> dynamicFormDatas = new List<DynamicFormData>(); List<ApplicationMaster> applicationMasters = new List<ApplicationMaster>();
                List<ApplicationMasterParent> applicationMasterParents = new List<ApplicationMasterParent>();
                List<long?> dynamicFormIds = new List<long?>();
                var parameters = new DynamicParameters();
                var DynamicFormDataIds = DynamicFormDataId != null && DynamicFormDataId.Count > 0 ? DynamicFormDataId : new List<long?>() { -1 };

                var query = "select t1.DynamicFormDataID,t1.DynamicFormID,t1.DynamicFormDataGridID from DynamicFormData t1 JOIN DynamicForm t2 ON t2.ID=t1.DynamicFormID where t1.DynamicFormDataGridID IN (" + string.Join(',', DynamicFormDataIds) + ") AND (t1.IsDeleted=0 OR t1.IsDeleted is null) AND (t2.IsDeleted=0 OR t2.IsDeleted is null);\r\n";
                query += "Select * from ApplicationMaster;";
                query += "Select * from ApplicationMasterParent;";
                query += "select t2.CompanyID,t3.PlantCode,t1.DynamicFormDataID,t1.DynamicFormID from DynamicFormData t1 \r\nJOIN DynamicForm t2 ON t1.DynamicFormID=t2.ID \r\nJOIN Plant t3 ON t2.CompanyID=t3.PlantID \r\nWHERE t1.DynamicFormDataID IN (" + string.Join(',', DynamicFormDataIds) + ");";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query);
                    dynamicFormDatas = results.Read<DynamicFormData>().ToList();
                    applicationMasters = results.Read<ApplicationMaster>().ToList();
                    applicationMasterParents = results.Read<ApplicationMasterParent>().ToList();
                    var dynamicFormDataItems = results.Read<DynamicFormData>().ToList();
                    dynamicFormData = dynamicFormDataItems != null ? dynamicFormDataItems.FirstOrDefault() : new DynamicFormData();
                }
                if (dynamicFormDatas != null && dynamicFormDatas.Count() > 0)
                {
                    dynamicFormIds = dynamicFormDatas.Select(s => s.DynamicFormId).Distinct().ToList();
                }
                dropDownOptionsGridListModel = await GetDynamicFormGridModelAsync(dynamicFormIds, userId, dynamicFormData?.CompanyId, dynamicFormData?.PlantCode, applicationMasters, applicationMasterParents, DynamicFormDataId, false);
                //dropDownOptionsGridListModel.DynamicFormData = dynamicFormDatas;
                return dropDownOptionsGridListModel;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DropDownOptionsGridListModel> GetDynamicGridDropDownById(List<long?> DynamicFormId, long? userId)
        {
            try
            {
                DropDownOptionsGridListModel dropDownOptionsGridListModel = new DropDownOptionsGridListModel();
                List<DynamicForm> dynamicFormDatas = new List<DynamicForm>(); List<ApplicationMaster> applicationMasters = new List<ApplicationMaster>();
                List<ApplicationMasterParent> applicationMasterParents = new List<ApplicationMasterParent>();
                DynamicForm dynamicFormData = new DynamicForm();
                var parameters = new DynamicParameters();
                List<long?> dynamicFormIds = new List<long?>();
                var query = "select t1.*,t3.PlantCode as CompanyName from DynamicForm t1  \r\nJOIN Plant t3 ON t1.CompanyID=t3.PlantID \r\nWHERE  (t1.IsDeleted is null OR t1.IsDeleted=0) AND t1.ID =" + DynamicFormId.First() + ";";
                query += "Select * from ApplicationMaster;";
                query += "Select * from ApplicationMasterParent;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query);
                    dynamicFormDatas = results.Read<DynamicForm>().ToList();
                    applicationMasters = results.Read<ApplicationMaster>().ToList();
                    applicationMasterParents = results.Read<ApplicationMasterParent>().ToList();
                    dynamicFormData = dynamicFormDatas != null ? dynamicFormDatas.FirstOrDefault() : new DynamicForm();
                }
                if (dynamicFormDatas != null && dynamicFormDatas.Count() > 0)
                {
                    dynamicFormIds.AddRange(DynamicFormId);
                    dropDownOptionsGridListModel = await GetDynamicFormGridModelAsync(dynamicFormIds, userId, dynamicFormData?.CompanyId, dynamicFormData?.CompanyName, applicationMasters, applicationMasterParents, null, true);
                    //dropDownOptionsGridListModel.DynamicFormData = dynamicFormDatas;
                }
                return dropDownOptionsGridListModel;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DropDownOptionsGridListModel> GetDynamicFormGridModelAsync(List<long?> dynamicFormIds, long? userId, long? companyId, string plantCode, List<ApplicationMaster> applicationMasters, List<ApplicationMasterParent> applicationMasterParent, List<long?> DynamicFormDataId, bool? isTableHeader)
        {
            try
            {
                DropDownOptionsGridListModel dropDownOptionsGridListModel = new DropDownOptionsGridListModel();
                List<ExpandoObject>? _dynamicformObjectDataList = new List<ExpandoObject>();
                List<DynamicFormData> dynamicFormDatas = new List<DynamicFormData>();
                DynamicFormGridModel attributeHeaderListModel = new DynamicFormGridModel();
                List<DropDownGridOptionsModel> dropDownGridOptionsModel1 = new List<DropDownGridOptionsModel>();
                var DynamicFormDataIds = DynamicFormDataId != null && DynamicFormDataId.Count > 0 ? DynamicFormDataId : new List<long?>() { -1 };
                using (var connection = CreateConnection())
                {
                    var dynamicFormIdss = dynamicFormIds != null && dynamicFormIds.Count() > 0 ? dynamicFormIds : new List<long?>() { -1 };
                    if (isTableHeader == false)
                    {
                        var query1 = "select t1.DynamicFormDataID,t1.DynamicFormID,t3.DynamicFormDataGridID,t4.Name as DynamicFormName,t3.DynamicFormID as DynamicFormDataGridFormID from DynamicFormData t1  \r\nJOIN DynamicForm t2 ON t1.DynamicFormID=t2.ID \r\nJOIN DynamicFormData t3 ON t3.DynamicFormDataGridID=t1.DynamicFormDataID\r\nJOIN DynamicForm t4 ON t3.DynamicFormID=t4.ID\r\nwhere  (t2.IsDeleted=0 OR t2.IsDeleted IS NULL) AND (t1.IsDeleted=0 OR t1.IsDeleted IS NULL)\r\nAND (t4.IsDeleted=0 OR t4.IsDeleted IS NULL) AND t1.DynamicFormID in(" + string.Join(',', dynamicFormIdss) + ");";
                        var resultData = (await connection.QueryAsync<DynamicFormData>(query1)).ToList();
                        dropDownOptionsGridListModel.DynamicFormData = resultData;

                        if (resultData != null && resultData.Count() > 0)
                        {
                            dynamicFormIdss.AddRange(resultData.Select(q => q.DynamicFormDataGridFormId).Distinct().ToList());
                        }
                    }
                    var query = "select t12.Name as GridDropDownDynamicFormName,(case when t1.IsDynamicFormGridDropdownMultiple is NULL then  0 ELSE t1.IsDynamicFormGridDropdownMultiple END) as IsDynamicFormGridDropdownMultiple,t1.IsDynamicFormGridDropdown,t1.GridDropDownDynamicFormID,t1.DynamicFormSectionAttributeID,t1.DynamicFormSectionID,t1.SessionID,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.AttributeID,t1.SortOrderBy,t1.ColSpan,t1.DisplayName,t1.IsMultiple,t1.IsRequired,t1.RequiredMessage,t1.IsSpinEditType,t1.FormUsedCount,t1.IsDisplayTableHeader,t1.FormToolTips,t1.IsVisible,t1.RadioLayout,t1.IsRadioCheckRemarks,t1.RemarksLabelName,t1.IsDeleted,t1.IsPlantLoadDependency,t1.PlantDropDownWithOtherDataSourceID,t1.PlantDropDownWithOtherDataSourceLabelName,t1.PlantDropDownWithOtherDataSourceIDs,t1.IsSetDefaultValue,t1.IsDefaultReadOnly,t1.ApplicationMasterID,t1.ApplicationMasterIDs,t1.IsDisplayDropDownHeader," +
                        "(case when t1.IsDisplayDropDownHeader is NULL then  0 ELSE t1.IsDisplayDropDownHeader END) as IsDisplayDropDownHeader," +
                        "(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,(case when t1.IsDependencyMultiple is NULL then  0 ELSE t1.IsDependencyMultiple END) as IsDependencyMultiple," +
                        "t5.SectionName,t9.sessionId as DynamicFormSessionId,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.IsDynamicFormDropTagBox,t6.DataSourceId," +
                        " t8.DisplayName  as DataSourceDisplayName,\r\n" +
                        " t8.DataSourceTable  as DataSourceTable," +
                        "t7.CodeValue as ControlType,t5.DynamicFormID,t6.DynamicFormID as DynamicFormGridDropDownID,t6.FilterDataSocurceID \r\n  from " +
                        "DynamicFormSectionAttribute t1 " +
                        "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId " +
                        "JOIN DynamicForm t10 ON t10.ID=t5.DynamicFormID\r\n" +
                        "JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID " +
                        "LEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID " +
                        "LEFT JOIN DynamicForm t9 ON t9.ID=t6.DynamicFormID " +
                        "JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID " +
                        "LEFT JOIN DynamicForm t12 ON t12.ID=t1.GridDropDownDynamicFormID\r\n" +
                        "\n\rWhere (t10.IsDeleted=0 OR t10.IsDeleted IS NULL) AND (t6.IsDeleted=0 OR t6.IsDeleted IS NULL) AND (t6.AttributeIsVisible=1 OR t6.AttributeIsVisible IS NULL) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t1.IsVisible= 1 OR t1.IsVisible is null) AND t5.DynamicFormID in(" + string.Join(',', dynamicFormIdss) + ") order by t1.SortOrderBy asc;";
                    query += "select t6.DynamicFormID as DynamicFormGridId,t1.DynamicFormDataID,t1.DynamicFormID,t1.SessionID,t1.DynamicFormItem,t1.IsSendApproval,t1.FileProfileSessionID,t1.ProfileID,t1.ProfileNo,t1.DynamicFormDataGridID,t1.SortOrderByNo,t1.GridSortOrderByNo,t1.DynamicFormSectionGridAttributeID,t6.ProfileNo as DynamicFormDataGridProfileNo,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode,t5.Name,t5.ScreenID,\r\n" +
                   "(select COUNT(t6.DocumentID) from Documents t6 where t6.SessionID = t1.SessionID AND t6.IsLatest = 1 AND(t6.IsDelete IS NULL OR t6.IsDelete = 0)) as IsFileprofileTypeDocument,(CASE WHEN t1.DynamicFormDataGridID>0  THEN 1  ELSE 0 END) AS IsDynamicFormDataGrid\r\n" +
                   "from DynamicFormData t1\r\n" + "JOIN ApplicationUser t2 ON t2.UserID = t1.AddedByUserID\r\n" + "JOIN ApplicationUser t3 ON t3.UserID = t1.ModifiedByUserID\r\n" +
                   "JOIN DynamicForm t5 ON t5.ID = t1.DynamicFormID\r\n" +
                   "LEFT JOIN DynamicFormData t6 ON t6.DynamicFormDataID = t1.DynamicFormDataGridID\n\r" +
                   "JOIN CodeMaster t4 ON t4.CodeID = t1.StatusCodeID WHERE (t1.IsDeleted=0 OR t1.IsDeleted IS NULL) AND (t5.IsDeleted=0 OR t5.IsDeleted IS NULL) AND t1.DynamicFormId IN (" + string.Join(',', dynamicFormIdss) + ")";
                    if (DynamicFormDataId != null && DynamicFormDataId.Count() > 0)
                    {
                        query += "AND t1.DynamicFormDataGridID IN (" + string.Join(',', DynamicFormDataIds) + ");\n\r";
                    }
                    else
                    {
                        query += ";\n\r";
                    }
                    //query += "select t1.DynamicFormApprovedId,t1.DynamicFormApprovalId,t1.DynamicFormDataId,t1.IsApproved,t1.UserId,t1.ApprovedByUserId,t4.UserName as ApprovedByUser,t5.DynamicFormId,\r\n" +
                    //  "CONCAT(case when t2.NickName is NULL then  t2.FirstName ELSE  t2.NickName END,' | ',t2.LastName) as ApprovalUser,\r\n" +
                    //  "CASE WHEN t1.IsApproved=1  THEN 'Approved' WHEN t1.IsApproved =0 THEN 'Rejected' ELSE 'Pending' END AS ApprovedStatus\r\n" +
                    //  "FROM DynamicFormApproved t1 \r\n" +
                    //  "JOIN Employee t2 ON t1.UserID=t2.UserID \r\n" +
                    //  "JOIN DynamicFormData t5 ON t5.DynamicFormDataId=t1.DynamicFormDataId \r\n" +
                    //  "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ApprovedByUserId WHERE (t5.IsDeleted=0 or t5.IsDeleted is null) AND t5.DynamicFormId IN (" + string.Join(',', dynamicFormIdss) + ") order by t1.DynamicFormApprovedId asc;\r\n";
                    query += "select  AttributeDetailID,AttributeDetailName,AttributeID,Description,SortOrder,Disabled,SessionID,StatusCodeID,AddedByUserID,AddedDate,ModifiedByUserID,ModifiedDate,FormUsedCount from AttributeDetails WHERE (Disabled=0 OR Disabled IS NULL);";
                    query += "select  ID,Name,ScreenID,SessionID,AttributeID,IsApproval,IsUpload,FileProfileTypeID,CompanyID,ProfileID,IsGridForm from DynamicForm WHere (IsDeleted=0 or IsDeleted is null) AND ID IN (" + string.Join(',', dynamicFormIdss) + ");";
                    query += "Select t1.HeaderDataSourceId,t1.AttributeHeaderDataSourceId,t1.DisplayName,t1.DataSourceTable,(Select COUNT(*) as IsDynamicFormFilterBy from DynamicFormFilterBy t2 where t2.AttributeHeaderDataSourceID=t1.AttributeHeaderDataSourceID)as IsDynamicFormFilterBy from AttributeHeaderDataSource t1;";
                    var results = await connection.QueryMultipleAsync(query);
                    attributeHeaderListModel.DynamicFormSectionAttribute = results.Read<DynamicFormSectionAttribute>().ToList();
                    attributeHeaderListModel.DynamicFormData = results.Read<DynamicFormData>().ToList();
                    //attributeHeaderListModel.DynamicFormApproved = results.Read<DynamicFormApproved>().ToList();
                    attributeHeaderListModel.AttributeDetails = results.Read<AttributeDetails>().ToList();
                    attributeHeaderListModel.DynamicForm = results.Read<DynamicForm>().ToList();
                    attributeHeaderListModel.AttributeHeaderDataSource = results.Read<AttributeHeaderDataSource>().ToList();
                }
                dropDownOptionsGridListModel.DynamicFormListData = attributeHeaderListModel.DynamicFormData;
                if (attributeHeaderListModel.DynamicFormSectionAttribute != null && attributeHeaderListModel.DynamicFormSectionAttribute.Count > 0)
                {
                    List<long?> ApplicationMasterIds = new List<long?>(); List<long?> ApplicationMasterParentIds = new List<long?>();
                    List<string?> dataSourceTableIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.DataSourceTable != null && a.DropDownTypeId == "Data Source").Select(a => a.DataSourceTable).Distinct().ToList();
                    List<string?> filterDataSourceTableIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.DataSourceTable != null && a.DropDownTypeId == "Filter Data Source").Select(a => a.DataSourceTable).Distinct().ToList();

                    var dataSourceList = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(companyId, dataSourceTableIds, plantCode, ApplicationMasterIds, ApplicationMasterParentIds);
                    attributeHeaderListModel.AttributeDetails.AddRange(dataSourceList);
                    attributeHeaderListModel.DynamicFormSectionAttribute.ForEach(s =>
                    {
                        s.AttributeName = string.IsNullOrEmpty(s.AttributeName) ? string.Empty : char.ToUpper(s.AttributeName[0]) + s.AttributeName.Substring(1);
                        s.DynamicAttributeName = s.DynamicFormSectionAttributeId + "_" + s.AttributeName;
                        if (s.IsPlantLoadDependency == true && !string.IsNullOrEmpty(s.PlantDropDownWithOtherDataSourceIds))
                        {
                            var PlantDropDownWithOtherDataSourceListIds = s.PlantDropDownWithOtherDataSourceIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (PlantDropDownWithOtherDataSourceListIds.Count > 0)
                            {
                                s.AttributeHeaderDataSource = attributeHeaderListModel.AttributeHeaderDataSource.Where(z => z.DataSourceTable != null && PlantDropDownWithOtherDataSourceListIds.Contains(z.AttributeHeaderDataSourceId)).ToList();
                            }
                        }
                        if (s.DataSourceTable == "ApplicationMaster" && !string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                s.ApplicationMasterIdsListIds = applicationMasterIds;
                                s.ApplicationMaster = applicationMasters.Where(z => z.ApplicationMasterId > 0 && applicationMasterIds.Contains(z.ApplicationMasterId)).ToList();
                            }
                        }
                        if (s.DataSourceTable == "ApplicationMasterParent" && !string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                s.ApplicationMasterParents = applicationMasterParent.Where(z => z.ApplicationMasterParentCodeId > 0 && applicationMasterIds.Contains(z.ApplicationMasterParentCodeId)).ToList();
                            }
                        }
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
                    List<string?> dataSourceTableIdss = new List<string?>(); List<long?> applicationMasterIds = new List<long?>(); List<long?> applicationMasterParentIds = new List<long?>();
                    applicationMasterIds = attributeHeaderListModel.DynamicFormSectionAttribute != null && attributeHeaderListModel.DynamicFormSectionAttribute.Count() > 0 ? attributeHeaderListModel.DynamicFormSectionAttribute.Where(w => w.ApplicationMaster.Count > 0).SelectMany(a => a.ApplicationMaster.Select(s => (long?)s.ApplicationMasterId)).ToList().Distinct().ToList() : new List<long?>();
                    applicationMasterParentIds = attributeHeaderListModel.DynamicFormSectionAttribute != null && attributeHeaderListModel.DynamicFormSectionAttribute.Count() > 0 ? attributeHeaderListModel.DynamicFormSectionAttribute.Where(w => w.ApplicationMasterParents.Count > 0).SelectMany(a => a.ApplicationMasterParents.Select(s => (long?)s.ApplicationMasterParentCodeId)).ToList().Distinct().ToList() : new List<long?>();
                    if (applicationMasterIds.Count > 0)
                    {
                        dataSourceTableIdss.Add("ApplicationMaster");
                    }
                    if (applicationMasterParentIds.Count > 0)
                    {
                        dataSourceTableIdss.Add("ApplicationMasterParent");
                    }
                    if (attributeHeaderListModel.DynamicFormSectionAttribute != null && attributeHeaderListModel.DynamicFormSectionAttribute.Count() > 0)
                    {
                        dataSourceTableIdss.AddRange(attributeHeaderListModel.DynamicFormSectionAttribute.Where(w => w.AttributeHeaderDataSource.Count > 0).SelectMany(a => a.AttributeHeaderDataSource.Select(s => s.DataSourceTable)).ToList().Distinct().ToList());
                    }
                    var PlantDependencySubAttributeDetails = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(null, dataSourceTableIdss, null, applicationMasterIds, applicationMasterParentIds);
                    dynamicFormIds.ForEach(d =>
                    {
                        var dynamicForms = attributeHeaderListModel.DynamicForm.FirstOrDefault(f => f.ID == d);
                        var dynamicFormData = attributeHeaderListModel.DynamicFormData.Where(w => w.DynamicFormId == d).ToList();
                        var dynamicFormSectionAttribute = attributeHeaderListModel.DynamicFormSectionAttribute.Where(w => w.DynamicFormId == d).OrderBy(o => o.SortOrderBy).ToList();
                        var dynamicFormSectionAttributeClass = attributeHeaderListModel.DynamicFormSectionAttribute.Where(w => w.DynamicFormId == d).OrderBy(o => o.SortOrderBy).ToList();
                        dynamicFormSectionAttributeClass.AddRange(GenerateFixedDataName());
                        Type[] typesList = dynamicFormSectionAttributeClass.Select(s => s.DataType).ToArray();
                        string[] objectNameList = dynamicFormSectionAttributeClass.Select(s => s.DynamicAttributeName).ToArray();

                        DropDownGridOptionsModel dropDownGridOptionsModel = new DropDownGridOptionsModel();
                        dropDownGridOptionsModel.DynamicFormId = d;
                        //dropDownGridOptionsModel.DynamicFormDataId = DynamicFormDataId;
                        if (isTableHeader == true)
                        {
                            dropDownGridOptionsModel.DropDownOptionsModels = GenerateDropDownHeaderList(dynamicForms, dynamicFormSectionAttribute, applicationMasterParent);
                        }
                        else
                        {
                            dropDownGridOptionsModel.DropDownOptionsModels = GenerateDropDownList(dynamicForms, dynamicFormSectionAttribute, applicationMasterParent);
                        }
                        dropDownGridOptionsModel1.Add(dropDownGridOptionsModel);

                        // var dynamicFormApproved = attributeHeaderListModel.DynamicFormApproved.Where(w => w.DynamicFormId == s).OrderBy(o => o.DynamicFormApprovedId).ToList();
                        if (dynamicFormData != null && dynamicFormData.Count > 0)
                        {
                            dynamicFormData.ForEach(async s =>
                            {
                                dynamic obj = new System.Dynamic.ExpandoObject();
                                var dict = (IDictionary<string, object>)obj;
                                dynamic jsonObj = new object();
                                if (IsValidJson(s.DynamicFormItem))
                                {
                                    jsonObj = JsonConvert.DeserializeObject(s.DynamicFormItem);
                                }
                                /*if (s.IsSendApproval == true)
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
                                }*/
                                obj.AttributeDetailID = s.DynamicFormDataId;
                                obj.AttributeDetailName = s.ProfileNo;
                                obj.ModifiedBy = s.ModifiedBy;
                                obj.DynamicFormId = s.DynamicFormId;
                                obj.DynamicFormDataGridId = s.DynamicFormDataGridId;
                                obj.CurrentUserName = s.CurrentUserName;
                                obj.ModifiedDate = s.ModifiedDate;
                                obj.StatusName = s.StatusName;
                                obj.DynamicFormDataId = s.DynamicFormDataId;
                                obj.IsDynamicFormDataGrid = s.IsDynamicFormDataGrid;
                                obj.DynamicFormGridId = s.DynamicFormGridId;
                                obj.DynamicFormDataGridProfileNo = s.DynamicFormDataGridProfileNo;
                                obj.SessionId = s.SessionId;
                                obj.DynamicFormName = s.Name;
                                var dynamicFormSectionAttribute = new List<DynamicFormSectionAttribute>();
                                if (isTableHeader == true)
                                {
                                    dynamicFormSectionAttribute = attributeHeaderListModel.DynamicFormSectionAttribute;
                                }
                                else
                                {
                                    dynamicFormSectionAttribute = attributeHeaderListModel.DynamicFormSectionAttribute.Where(w => w.DynamicFormId == s.DynamicFormId).ToList();
                                }

                                if (dynamicFormSectionAttribute != null && dynamicFormSectionAttribute.Count > 0)
                                {


                                    dynamicFormSectionAttribute.ForEach(async b =>
                                    {
                                        b.DynamicAttributeName = b.DynamicFormSectionAttributeId + "_" + b.AttributeName;
                                        string attrName = b.DynamicAttributeName;
                                        var Names = jsonObj.ContainsKey(attrName);
                                        if (Names == true)
                                        {

                                            if (b.DataSourceTable == "ApplicationMaster")
                                            {
                                                dict[attrName] = string.Empty;
                                                if (b.ApplicationMaster != null && b.ApplicationMaster.Count() > 0)
                                                {
                                                    b.ApplicationMaster.ForEach(ab =>
                                                         {
                                                             var setNameData = ab.ApplicationMasterCodeId + "_AppMaster";
                                                             var keysetNameDataExits = ((IDictionary<string, object>)dict).ContainsKey(setNameData);
                                                             var nameData = b.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_AppMaster";
                                                             var SubNamess = jsonObj.ContainsKey(nameData);
                                                             if (SubNamess == true)
                                                             {
                                                                 var itemValue = jsonObj[nameData];
                                                                 if (itemValue is JArray)
                                                                 {
                                                                     List<long?> listData = itemValue.ToObject<List<long?>>();
                                                                     if (b.IsMultiple == true || b.ControlType == "TagBox")
                                                                     {
                                                                         var listName = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.ApplicationMasterId == ab.ApplicationMasterId && a.DropDownTypeId == b.DataSourceTable).Select(s => s.AttributeDetailName).ToList() : new List<string?>();
                                                                         dict[nameData] = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                                     }
                                                                     else
                                                                     {
                                                                         dict[nameData] = string.Empty;
                                                                     }
                                                                 }
                                                                 else
                                                                 {
                                                                     if (b.ControlType == "ComboBox")
                                                                     {
                                                                         long? values = itemValue == null ? -1 : (long)itemValue;
                                                                         var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == b.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                                                                         dict[nameData] = listss;
                                                                         if (keysetNameDataExits == false)
                                                                         {
                                                                             dict[setNameData] = values > 0 ? values : 0;
                                                                         }
                                                                     }
                                                                     else
                                                                     {
                                                                         if (b.ControlType == "ListBox" && b.IsMultiple == false)
                                                                         {
                                                                             long? values = itemValue == null ? -1 : (long)itemValue;
                                                                             var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == b.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                                                                             dict[nameData] = listss;
                                                                         }
                                                                         else
                                                                         {
                                                                             dict[nameData] = string.Empty;
                                                                         }
                                                                     }
                                                                 }
                                                             }

                                                         });
                                                }
                                            }
                                            else if (b.DataSourceTable == "ApplicationMasterParent")
                                            {
                                                dict[attrName] = string.Empty;
                                                List<ApplicationMasterParent> nameDatas = new List<ApplicationMasterParent>();
                                                b.ApplicationMasterParents.ForEach(ab =>
                                                     {
                                                         nameDatas.Add(ab);
                                                         RemoveApplicationMasterParentSingleNameItem(ab, b, nameDatas, applicationMasterParent);

                                                     });
                                                if (nameDatas != null && nameDatas.Count() > 0)
                                                {
                                                    nameDatas.ForEach(n =>
                                                         {
                                                             loadApplicationMasterParentData(jsonObj, b, n, dict, PlantDependencySubAttributeDetails);
                                                         });
                                                }
                                            }
                                            else
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
                                                        if (b.ControlType == "ComboBox" && b.IsPlantLoadDependency == true && b.AttributeHeaderDataSource.Count() > 0 && PlantDependencySubAttributeDetails != null && PlantDependencySubAttributeDetails.Count() > 0)
                                                        {

                                                            b.AttributeHeaderDataSource.ForEach(dd =>
                                                                 {
                                                                     var nameData = b.DynamicFormSectionAttributeId + "_" + dd.AttributeHeaderDataSourceId + "_" + dd.DataSourceTable + "_Dependency";
                                                                     var itemDepExits = jsonObj.ContainsKey(nameData);
                                                                     if (itemDepExits == true)
                                                                     {
                                                                         var itemDepValue = jsonObj[nameData];
                                                                         if (b.IsDependencyMultiple == true)
                                                                         {
                                                                             if (itemDepValue is JArray)
                                                                             {
                                                                                 List<long?> listData = itemDepValue.ToObject<List<long?>>();
                                                                                 var listName = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.DropDownTypeId == dd.DataSourceTable).Select(s => s.AttributeDetailName).ToList() : new List<string?>();
                                                                                 dict[nameData] = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                                             }
                                                                             else
                                                                             {
                                                                                 dict[nameData] = string.Empty;
                                                                             }
                                                                         }
                                                                         else
                                                                         {
                                                                             long? valuesDep = itemDepValue == null ? -1 : (long)itemDepValue;
                                                                             var listss = PlantDependencySubAttributeDetails.Where(v => dd.DataSourceTable == v.DropDownTypeId && v.AttributeDetailID == valuesDep).FirstOrDefault()?.AttributeDetailName;
                                                                             dict[nameData] = listss;
                                                                         }
                                                                     }

                                                                 });
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
                                                        if (b.ControlTypeId == 2712 && b.DynamicFormGridDropDownId > 0 && isTableHeader == true)
                                                        {
                                                            List<long?> dynamicFormSubIds = new List<long?>() { b.DynamicFormGridDropDownId };
                                                            // var jsonResult = await GetDynamicFormGridModelAsync(dynamicFormSubIds, userId, companyId, plantCode, applicationMasters, applicationMasterParent, null, isTableHeader);
                                                            // dict[attrName] = b.DynamicFormId;
                                                            dict[attrName] = "A";
                                                            // dict[attrName] = JsonConvert.SerializeObject(jsonResult);
                                                        }
                                                        else
                                                        {
                                                            dict[attrName] = (string)itemValue;
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (b.ControlType == "ComboBox" && b.IsPlantLoadDependency == true && b.AttributeHeaderDataSource.Count() > 0)
                                            {
                                                dict[attrName] = string.Empty;
                                                b.AttributeHeaderDataSource.ForEach(dd =>
                                                     {
                                                         var nameData = b.DynamicFormSectionAttributeId + "_" + dd.AttributeHeaderDataSourceId + "_" + dd.DataSourceTable + "_Dependency";
                                                         dict[nameData] = string.Empty;
                                                     });
                                            }
                                            else
                                            {
                                                if (b.DataSourceTable == "ApplicationMaster")
                                                {
                                                    dict[attrName] = string.Empty;
                                                    if (b.ApplicationMaster.Count() > 0)
                                                    {
                                                        b.ApplicationMaster.ForEach(ab =>
                                                             {
                                                                 var nameData = b.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_AppMaster";
                                                                 dict[nameData] = string.Empty;
                                                             });
                                                    }
                                                }
                                                else if (b.DataSourceTable == "ApplicationMasterParent")
                                                {
                                                    dict[attrName] = string.Empty;
                                                    List<ApplicationMasterParent> nameDatas = new List<ApplicationMasterParent>();
                                                    b.ApplicationMasterParents.ForEach(ab =>
                                                         {
                                                             nameDatas.Add(ab);
                                                             RemoveApplicationMasterParentSingleNameItem(ab, b, nameDatas, applicationMasterParent);
                                                         });
                                                    if (nameDatas != null && nameDatas.Count() > 0)
                                                    {
                                                        nameDatas.ForEach(n =>
                                                        {
                                                            var nameData = b.DynamicFormSectionAttributeId + "_" + n.ApplicationMasterParentCodeId + "_AppMasterPar";
                                                            //dict[n.ApplicationMasterParentCodeId + "_AppMasterPar"] = string.Empty;
                                                            dict[nameData] = string.Empty;
                                                        });
                                                    }

                                                }
                                                else
                                                {
                                                    dict[attrName] = string.Empty;
                                                }
                                            }
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
        void loadApplicationMasterParentData(dynamic jsonObj, DynamicFormSectionAttribute s, ApplicationMasterParent nameDatas, IDictionary<string, object> objectData, IReadOnlyList<AttributeDetails> PlantDependencySubAttributeDetails)
        {

            var nameData = s.DynamicFormSectionAttributeId + "_" + nameDatas.ApplicationMasterParentCodeId + "_AppMasterPar";
            var setNameData = nameDatas.ApplicationMasterParentCodeId + "_AppMasterPar";
            var SubNamess = jsonObj.ContainsKey(nameData);
            var keysetNameDataExits = ((IDictionary<string, object>)objectData).ContainsKey(setNameData);
            if (SubNamess == true)
            {
                var itemValue = jsonObj[nameData];
                if (itemValue is JArray)
                {
                    List<long?> listData = itemValue.ToObject<List<long?>>();
                    if (s.IsMultiple == true || s.ControlType == "TagBox")
                    {
                        var listName = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.DropDownTypeId == s.DataSourceTable).Select(s => s.AttributeDetailName).ToList() : new List<string?>();
                        objectData[nameData] = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                        if (keysetNameDataExits == false)
                        {
                            objectData[setNameData] = listData;
                        }
                    }
                    else
                    {
                        objectData[nameData] = string.Empty;
                        // objectData[setNameData] = string.Empty;
                    }
                }
                else
                {
                    if (s.ControlType == "ComboBox")
                    {
                        long? values = itemValue == null ? -1 : (long)itemValue;
                        var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                        objectData[nameData] = listss;
                        objectData[setNameData] = values > 0 ? values : 0;
                        if (keysetNameDataExits == false)
                        {
                            objectData[setNameData] = values > 0 ? values : 0;
                        }
                    }
                    else
                    {
                        if (s.ControlType == "ListBox" && s.IsMultiple == false)
                        {
                            long? values = itemValue == null ? -1 : (long)itemValue;
                            var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                            objectData[nameData] = listss;
                            if (keysetNameDataExits == false)
                            {
                                objectData[setNameData] = values > 0 ? values : 0;
                            }
                        }
                        else
                        {
                            objectData[nameData] = string.Empty;
                            //objectData[setNameData] = string.Empty;
                        }
                    }
                }
            }
            else
            {
                objectData[nameData] = string.Empty;
                // objectData[setNameData] = string.Empty;
            }


        }
        void RemoveApplicationMasterParentSingleItem(ApplicationMasterParent applicationMasterParent, DynamicFormSectionAttribute dynamicFormSectionAttribute, List<DropDownOptionsModel> dataColumnNames, int counts, List<ApplicationMasterParent> applicationMasterParents)
        {
            if (applicationMasterParent != null)
            {
                var listss = applicationMasterParents.FirstOrDefault(f => f.ParentId == applicationMasterParent.ApplicationMasterParentCodeId);
                if (listss != null)
                {
                    var nameData = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + listss.ApplicationMasterParentCodeId + "_AppMasterPar";
                    dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = nameData, AttributeDetailName = listss.ApplicationMasterName, Text = listss.ApplicationMasterName, Type = "DynamicForm", IsVisible = true });
                    RemoveApplicationMasterParentSingleItem(listss, dynamicFormSectionAttribute, dataColumnNames, counts, applicationMasterParents);
                    counts++;
                }
            }
        }
        void RemoveApplicationMasterParentSingleNameItem(ApplicationMasterParent applicationMasterParent, DynamicFormSectionAttribute dynamicFormSectionAttribute, List<ApplicationMasterParent> dataColumnNames, List<ApplicationMasterParent> applicationMasterParents)
        {
            if (applicationMasterParent != null)
            {
                var listss = applicationMasterParents.FirstOrDefault(f => f.ParentId == applicationMasterParent.ApplicationMasterParentCodeId);
                if (listss != null)
                {
                    var nameData = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + listss.ApplicationMasterParentCodeId + "_AppMasterPar";
                    dataColumnNames.Add(listss);
                    RemoveApplicationMasterParentSingleNameItem(listss, dynamicFormSectionAttribute, dataColumnNames, applicationMasterParents);
                }
            }
        }

        private List<DropDownOptionsModel> GenerateDropDownHeaderList(DynamicForm _dynamicForm, List<DynamicFormSectionAttribute> dynamicFormSectionAttribute, List<ApplicationMasterParent> applicationMasterParent)
        {
            List<DropDownOptionsModel> dataColumnNames = new List<DropDownOptionsModel>
                        {
                           // new DropDownOptionsModel() { Value = "AttributeDetailID", Text = "DynamicFormDataId", Type = "Form", IsVisible = false,OrderBy=1 },
                            new DropDownOptionsModel() { Value = "AttributeDetailName", Text = "Profile No", Type = "Form",OrderBy=1,AttributeDetailID=1, AttributeDetailName = "Profile No" },
                            //new DropDownOptionsModel() { Value = "IsDynamicFormDataGrid", Text = "Is Grid", Type = "Form",OrderBy=3 }
                        };
            if (dynamicFormSectionAttribute != null && dynamicFormSectionAttribute.Count > 0)
            {
                var dynamicFormSectionAttributes = dynamicFormSectionAttribute.Where(w => w.IsDisplayTableHeader == true).ToList();
                if (dynamicFormSectionAttributes != null && dynamicFormSectionAttributes.Count > 0)
                {
                    var counts = 1;
                    dynamicFormSectionAttributes.ForEach(a =>
                    {
                        a.DynamicAttributeName = a.DynamicFormSectionAttributeId + "_" + a.AttributeName;
                        counts += 1;
                        if (a.DataSourceTable == "ApplicationMaster")
                        {
                            if (a.IsDisplayTableHeader == true && a.ApplicationMaster.Count() > 0)
                            {
                                dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = a.DynamicAttributeName, Text = a.DisplayName, Type = "DynamicForm", Id = a.DynamicFormSectionAttributeId, IsVisible = a.IsDisplayTableHeader.Value, AttributeDetailName = a.DisplayName });
                                a.ApplicationMaster.ForEach(ab =>
                                {
                                    if (a.IsVisible == true)
                                    {
                                        var nameData = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_AppMaster";
                                        dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = nameData, AttributeDetailName = ab.ApplicationMasterName, Text = ab.ApplicationMasterName, Type = "DynamicForm", IsVisible = true });
                                        //if (a.ControlTypeId == 2702 && a.DropDownTypeId == "Data Source" && a.DataSourceTable == "ApplicationMaster" && a.IsDynamicFormGridDropdown == true)
                                        //{
                                        //    var appendDependency = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_" + a.GridDropDownDynamicFormID + "_GridAppMaster";
                                        //    dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = appendDependency, AttributeDetailName = a.GridDropDownDynamicFormName, Text = a.GridDropDownDynamicFormName, Type = "DynamicForm", IsVisible = true });
                                        //    counts++;
                                        //}
                                    }
                                    counts++;
                                });
                            }
                        }
                        else if (a.DataSourceTable == "ApplicationMasterParent")
                        {

                            if (a.IsDisplayTableHeader == true && a.ApplicationMasterParents.Count() > 0)
                            {
                                dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = a.DynamicAttributeName, Text = a.DisplayName, Type = "DynamicForm", Id = a.DynamicFormSectionAttributeId, IsVisible = a.IsDisplayTableHeader.Value, AttributeDetailName = a.DisplayName });
                                a.ApplicationMasterParents.ForEach(ab =>
                                {
                                    var nameData = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_AppMasterPar";
                                    dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = nameData, AttributeDetailName = ab.ApplicationMasterName, Text = ab.ApplicationMasterName, Type = "DynamicForm", IsVisible = true });
                                    RemoveApplicationMasterParentSingleItem(ab, a, dataColumnNames, counts, applicationMasterParent);
                                    counts++;
                                    //if (a.ControlTypeId == 2702 && a.DropDownTypeId == "Data Source" && a.DataSourceTable == "ApplicationMasterParent" && a.IsDynamicFormGridDropdown == true)
                                    //{
                                    //    var appendDependency = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_" + a.GridDropDownDynamicFormID + "_GridAppMaster";
                                    //    dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = appendDependency, AttributeDetailName = a.GridDropDownDynamicFormName, Text = a.GridDropDownDynamicFormName, Type = "DynamicForm", IsVisible = true });
                                    //    counts++;
                                    //}
                                });
                            }
                        }
                        else
                        {
                            dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = a.DynamicAttributeName, Text = a.DisplayName, Type = "DynamicForm", Id = a.DynamicFormSectionAttributeId, IsVisible = a.IsDisplayTableHeader.Value, AttributeDetailName = a.DisplayName });
                        }
                        if (a.ControlType == "ComboBox" && a.IsPlantLoadDependency == true && a.AttributeHeaderDataSource.Count() > 0)
                        {
                            a.AttributeHeaderDataSource.ForEach(dd =>
                            {
                                if (a.IsVisible == true)
                                {
                                    var nameData = a.DynamicFormSectionAttributeId + "_" + dd.AttributeHeaderDataSourceId + "_" + dd.DataSourceTable + "_Dependency";
                                    dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = nameData, Text = dd.DisplayName, AttributeDetailName = dd.DisplayName, Type = "DynamicForm", IsVisible = true });
                                    counts++;
                                }
                            });
                        }
                    });
                    counts++;
                }
            }
            return dataColumnNames;
        }

        private List<DropDownOptionsModel> GenerateDropDownList(DynamicForm _dynamicForm, List<DynamicFormSectionAttribute> dynamicFormSectionAttribute, List<ApplicationMasterParent> applicationMasterParent)
        {
            List<DropDownOptionsModel> dataColumnNames = new List<DropDownOptionsModel>
                        {
                           // new DropDownOptionsModel() { Value = "AttributeDetailID", Text = "DynamicFormDataId", Type = "Form", IsVisible = false,OrderBy=1 },
                            new DropDownOptionsModel() { Value = "AttributeDetailName", Text = "Profile No", Type = "Form",OrderBy=1,AttributeDetailID=1, AttributeDetailName = "Profile No" },
                            //new DropDownOptionsModel() { Value = "IsDynamicFormDataGrid", Text = "Is Grid", Type = "Form",OrderBy=3 }
                        };
            if (dynamicFormSectionAttribute != null && dynamicFormSectionAttribute.Count > 0)
            {
                var dynamicFormSectionAttributes = dynamicFormSectionAttribute.Where(w => w.IsDisplayDropDownHeader == true).ToList();
                if (dynamicFormSectionAttributes != null && dynamicFormSectionAttributes.Count > 0)
                {
                    var counts = 1;
                    dynamicFormSectionAttributes.ForEach(a =>
                    {
                        a.DynamicAttributeName = a.DynamicFormSectionAttributeId + "_" + a.AttributeName;
                        counts += 1;
                        if (a.DataSourceTable == "ApplicationMaster")
                        {
                            if (a.IsDisplayDropDownHeader == true && a.ApplicationMaster.Count() > 0)
                            {
                                dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = a.DynamicAttributeName, Text = a.DisplayName, Type = "DynamicForm", Id = a.DynamicFormSectionAttributeId, IsVisible = a.IsDisplayDropDownHeader.Value, AttributeDetailName = a.DisplayName });
                                a.ApplicationMaster.ForEach(ab =>
                                {
                                    if (a.IsVisible == true)
                                    {
                                        var nameData = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_AppMaster";
                                        dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = nameData, AttributeDetailName = ab.ApplicationMasterName, Text = ab.ApplicationMasterName, Type = "DynamicForm", IsVisible = true });
                                        //if (a.ControlTypeId == 2702 && a.DropDownTypeId == "Data Source"  && a.IsDynamicFormGridDropdown == true)
                                        //{
                                        //    var appendDependency = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_" + a.GridDropDownDynamicFormID + "_GridAppMaster";
                                        //    dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = appendDependency, AttributeDetailName = a.GridDropDownDynamicFormName, Text = a.GridDropDownDynamicFormName, Type = "DynamicForm", IsVisible = true });
                                        //    counts++;
                                        //}
                                    }
                                    counts++;
                                });
                            }
                        }
                        else if (a.DataSourceTable == "ApplicationMasterParent")
                        {

                            if (a.IsDisplayDropDownHeader == true && a.ApplicationMasterParents.Count() > 0)
                            {
                                dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = a.DynamicAttributeName, Text = a.DisplayName, Type = "DynamicForm", Id = a.DynamicFormSectionAttributeId, IsVisible = a.IsDisplayDropDownHeader.Value, AttributeDetailName = a.DisplayName });
                                a.ApplicationMasterParents.ForEach(ab =>
                                {
                                    var nameData = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_AppMasterPar";
                                    dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = nameData, AttributeDetailName = ab.ApplicationMasterName, Text = ab.ApplicationMasterName, Type = "DynamicForm", IsVisible = true });
                                    RemoveApplicationMasterParentSingleItem(ab, a, dataColumnNames, counts, applicationMasterParent);
                                    counts++;
                                    //if (a.ControlTypeId == 2702 && a.DropDownTypeId == "Data Source" && a.IsDynamicFormGridDropdown == true)
                                    //{
                                    //    var appendDependency = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_" + a.GridDropDownDynamicFormID + "_GridAppMaster";
                                    //    dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = appendDependency, AttributeDetailName = a.GridDropDownDynamicFormName, Text = a.GridDropDownDynamicFormName, Type = "DynamicForm", IsVisible = true });
                                    //    counts++;
                                    //}
                                });
                            }
                        }
                        else
                        {
                            dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = a.DynamicAttributeName, Text = a.DisplayName, Type = "DynamicForm", Id = a.DynamicFormSectionAttributeId, IsVisible = a.IsDisplayDropDownHeader.Value, AttributeDetailName = a.DisplayName });
                        }
                        if (a.ControlType == "ComboBox" && a.IsPlantLoadDependency == true && a.AttributeHeaderDataSource.Count() > 0)
                        {
                            a.AttributeHeaderDataSource.ForEach(dd =>
                            {
                                if (a.IsVisible == true)
                                {
                                    var nameData = a.DynamicFormSectionAttributeId + "_" + dd.AttributeHeaderDataSourceId + "_" + dd.DataSourceTable + "_Dependency";
                                    dataColumnNames.Add(new DropDownOptionsModel() { OrderBy = counts, AttributeDetailID = counts, Value = nameData, Text = dd.DisplayName, AttributeDetailName = dd.DisplayName, Type = "DynamicForm", IsVisible = true });
                                    counts++;
                                }
                            });
                        }
                    });
                    counts++;
                }
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
        public async Task<AttributeDetailsAdds> GetAttributeDetails(List<long?> AttributeIds, List<long> id, string? Type, long? companyId, string plantCode, List<ApplicationMaster> applicationMasters, List<ApplicationMasterParent> applicationMasterParents)
        {
            try
            {
                AttributeDetailsAdds attributeDetailsAdds = new AttributeDetailsAdds();
                // if (id != null && id.Count > 0)
                // {
                id = id != null && id.Count > 0 ? id : new List<long>() { -1 };
                // AttributeIds = AttributeIds != null && AttributeIds.Count > 0 ? AttributeIds : new List<long?>() { -1 };
                var query = "select  *,CONCAT('Attr_',AttributeDetailID) as AttributeDetailNameId from AttributeDetails WHERE (Disabled=0 OR Disabled IS NULL);\n\r";
                query += "select t6.*,t9.AttributeID as SubAttributeID,t6.IsDynamicFormDropTagBox,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.DataSourceId,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t7.CodeValue as ControlType,t6.DynamicFormID as DynamicFormGridDropDownID,t6.ApplicationMasterSubFormId from \r\nAttributeHeader t6\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID\r\nLEFT JOIN AttributeHeader t9 ON t9.AttributeID=t6.SubAttributeID\r\n" +
                "JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID where (t6.IsDeleted=0 or t6.IsDeleted is NULL) AND (t6.AttributeIsVisible=1 or t6.AttributeIsVisible is NULL) AND t6.IsSubForm=1\n\r";
                if (Type == "Main")
                {
                    query += "AND t6.SubAttributeID in(" + string.Join(',', id) + ")\n\r";
                }
                if (Type == "Sub")
                {
                    query += "AND t6.SubAttributeDetailID in(" + string.Join(',', id) + ")\n\r";
                }
                query += "order by t6.AttributeSortBy asc\n\r";
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryMultipleAsync(query);
                    attributeDetailsAdds.AttributeDetails = result.Read<AttributeDetails>().ToList();
                    attributeDetailsAdds.AttributeHeader = result.Read<AttributeHeader>().ToList();
                }
                if (attributeDetailsAdds.AttributeHeader.Count > 0)
                {
                    List<long?> ApplicationMasterParentIds = new List<long?>();
                    List<string?> appmasterParentidss = attributeDetailsAdds.AttributeHeader.Where(a => a.DataSourceId == 13 && !string.IsNullOrWhiteSpace(a.SubApplicationMasterIDs) && !string.IsNullOrEmpty(a.SubApplicationMasterIDs)).Select(a => a.SubApplicationMasterIDs).Distinct().ToList();
                    ApplicationMasterParentIds = appmasterParentidss != null && appmasterParentidss.Count > 0 ? string.Join(",", appmasterParentidss).Split(",").Select(a => (long?)Convert.ToDouble(a)).ToList() : new List<long?>();

                    List<string?> appmasteridss = attributeDetailsAdds.AttributeHeader.Where(a => a.DataSourceId == 12 && !string.IsNullOrWhiteSpace(a.SubApplicationMasterIDs) && !string.IsNullOrEmpty(a.SubApplicationMasterIDs)).Select(a => a.SubApplicationMasterIDs).Distinct().ToList();
                    List<long?> applicationMasterIds = appmasteridss != null && appmasteridss.Count > 0 ? string.Join(",", appmasteridss).Split(",").Select(a => (long?)Convert.ToDouble(a)).ToList() : new List<long?>();
                    List<string?> dataSourceTableIds = attributeDetailsAdds.AttributeHeader.Where(a => a.DataSourceTable != null && a.DropDownTypeId == "Data Source").Select(a => a.DataSourceTable).Distinct().ToList();
                    if (applicationMasterIds != null && applicationMasterIds.Count > 0)
                    {
                        dataSourceTableIds.Add("ApplicationMaster");
                    }
                    if (ApplicationMasterParentIds != null && ApplicationMasterParentIds.Count > 0)
                    {
                        dataSourceTableIds.Add("ApplicationMasterParent");
                    }
                    if (dataSourceTableIds != null && dataSourceTableIds.Count() > 0)
                    {
                        dataSourceTableIds = dataSourceTableIds.Distinct().ToList();
                    }
                    else
                    {
                        dataSourceTableIds = new List<string?>();
                    }
                    var dataSourceList = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(companyId, dataSourceTableIds, plantCode, applicationMasterIds == null ? new List<long?>() : applicationMasterIds, ApplicationMasterParentIds == null ? new List<long?>() : ApplicationMasterParentIds);
                    attributeDetailsAdds.AttributeHeader.ForEach(s =>
                    {
                        if (s.DataSourceTable == "ApplicationMaster" && !string.IsNullOrEmpty(s.SubApplicationMasterIDs))
                        {
                            var applicationMasterIds = s.SubApplicationMasterIDs.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                s.SubApplicationMasterIdsListIds = applicationMasterIds;
                                s.SubApplicationMaster = applicationMasters.Where(z => z.ApplicationMasterId > 0 && applicationMasterIds.Contains(z.ApplicationMasterId)).ToList();
                            }
                        }
                        if (s.DataSourceTable == "ApplicationMasterParent" && !string.IsNullOrEmpty(s.SubApplicationMasterIDs))
                        {
                            var applicationMasterIds = s.SubApplicationMasterIDs.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                s.SubApplicationMasterParentIdsListIds = applicationMasterIds;
                                s.SubApplicationMasterParent = applicationMasterParents.Where(z => z.ApplicationMasterParentCodeId > 0 && applicationMasterIds.Contains(z.ApplicationMasterParentCodeId)).ToList();
                            }
                        }
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
                //}
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
                var query = "SELECT * FROM AttributeHeader WHERE (IsDeleted=0 OR IsDeleted IS NULL) AND AttrubuteID = @Id";
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
        public int? GeAttributeHeaderSort(AttributeHeader attributeHeader)
        {
            try
            {
                int? SortOrderBy = 0;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                if (attributeHeader.IsSubForm == true && attributeHeader.AttributeSortBy == null)
                {
                    if (attributeHeader.SubAttributeId > 0 || attributeHeader.SubAttributeDetailId > 0)
                    {
                        parameters.Add("SubAttributeDetailId", attributeHeader.SubAttributeDetailId);
                        parameters.Add("SubAttributeId", attributeHeader.SubAttributeId);

                        if (attributeHeader.SubAttributeId > 0)
                        {
                            query = "SELECT AttributeID,SubAttributeId,AttributeSortBy,SubAttributeDetailId FROM AttributeHeader Where (IsDeleted=0 or IsDeleted is null) AND SubAttributeId = @SubAttributeId AND AttributeSortBy>0 order by  AttributeSortBy desc";
                        }
                        if (attributeHeader.SubAttributeDetailId > 0)
                        {
                            query = "SELECT AttributeID,SubAttributeId,AttributeSortBy,SubAttributeDetailId FROM AttributeHeader Where (IsDeleted=0 or IsDeleted is null) AND SubAttributeDetailId = @SubAttributeDetailId AND AttributeSortBy>0 order by  AttributeSortBy desc";
                        }
                        using (var connection = CreateConnection())
                        {
                            SortOrderBy = 1;
                            var result = connection.QueryFirstOrDefault<AttributeHeader>(query, parameters);
                            if (result != null)
                            {
                                SortOrderBy = result.AttributeSortBy + 1;
                            }
                        }
                    }
                }
                return SortOrderBy;
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
                        parameters.Add("ApplicationMasterSubFormId", attributeHeader.ApplicationMasterSubFormId);
                        parameters.Add("AttributeRadioLayout", attributeHeader.AttributeRadioLayout, DbType.String);
                        parameters.Add("AttrDescription", attributeHeader.AttrDescription, DbType.String);
                        parameters.Add("IsFilterDataSource", attributeHeader.IsFilterDataSource == true ? true : null);
                        parameters.Add("FilterDataSocurceId", attributeHeader.IsFilterDataSource == true ? attributeHeader.FilterDataSocurceId > 0 ? attributeHeader.FilterDataSocurceId : null : null);
                        int? AttributeSortBy = null;
                        if (attributeHeader.IsSubForm == true)
                        {
                            if (attributeHeader.SubAttributeDetailId > 0 || attributeHeader.SubAttributeId > 0)
                            {
                                var sortBy = GeAttributeHeaderSort(attributeHeader);
                                AttributeSortBy = attributeHeader.AttributeSortBy;
                                if (attributeHeader.AttributeSortBy > 0)
                                {
                                }
                                else
                                {
                                    AttributeSortBy = sortBy > 0 ? sortBy : null;
                                }
                            }
                        }
                        parameters.Add("AttributeSortBy", AttributeSortBy);
                        parameters.Add("SubApplicationMasterIds", attributeHeader.SubApplicationMasterIdsListIds != null && attributeHeader.SubApplicationMasterIdsListIds.Count() > 0 ? string.Join(",", attributeHeader.SubApplicationMasterIdsListIds) : null, DbType.String);
                        if (attributeHeader.AttributeID > 0)
                        {
                            var Addquerys = "UPDATE AttributeHeader SET FilterDataSocurceId=@FilterDataSocurceId,IsFilterDataSource=@IsFilterDataSource,AttributeSortBy=@AttributeSortBy,AttrDescription=@AttrDescription,SubApplicationMasterIds=@SubApplicationMasterIds,ApplicationMasterSubFormId=@ApplicationMasterSubFormId,AttributeRadioLayout=@AttributeRadioLayout,AttributeIsVisible=@AttributeIsVisible,AttributeFormToolTips=@AttributeFormToolTips,IsAttributeSpinEditType=@IsAttributeSpinEditType,IsAttributeDisplayTableHeader=@IsAttributeDisplayTableHeader,SubAttributeId=@SubAttributeId,SubAttributeDetailId=@SubAttributeDetailId,IsSubForm=@IsSubForm,IsDynamicFormDropTagBox=@IsDynamicFormDropTagBox,DynamicFormId=@DynamicFormId,AttributeName = @AttributeName,IsInternal=@IsInternal,Description=@Description," +
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
                            var query = @"INSERT INTO AttributeHeader(FilterDataSocurceId,IsFilterDataSource,AttributeSortBy,AttrDescription,SubApplicationMasterIds,ApplicationMasterSubFormId,AttributeRadioLayout,AttributeIsVisible,AttributeFormToolTips,IsAttributeDisplayTableHeader,IsAttributeSpinEditType,SubAttributeId,SubAttributeDetailId,IsSubForm,IsDynamicFormDropTagBox,DynamicFormId,AttributeCompanyId,AttributeName,IsInternal,Description,ControlType,EntryMask,RegExp,AddedByUserID,AddedDate,SessionId,StatusCodeID,ControlTypeId,IsMultiple,IsRequired,RequiredMessage,DropDownTypeId,DataSourceId) 
              OUTPUT INSERTED.AttributeID  -- Replace 'YourIDColumn' with the actual column name of your IDENTITY column
              VALUES (@FilterDataSocurceId,@IsFilterDataSource,@AttributeSortBy,@AttrDescription,@SubApplicationMasterIds,@ApplicationMasterSubFormId,@AttributeRadioLayout,@AttributeIsVisible,@AttributeFormToolTips,@IsAttributeDisplayTableHeader,@IsAttributeSpinEditType,@SubAttributeId,@SubAttributeDetailId,@IsSubForm,@IsDynamicFormDropTagBox,@DynamicFormId,@AttributeCompanyId,@AttributeName,@IsInternal,@Description,@ControlType,@EntryMask,@RegExp,@AddedByUserID,@AddedDate,@SessionId,@StatusCodeID,@ControlTypeId,@IsMultiple,@IsRequired,@RequiredMessage,@DropDownTypeId,@DataSourceId)";

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
        public async Task<List<AttributeHeader>> GetUpdateAttributeHeaderSortOrder(AttributeHeader attributeHeader)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("SubAttributeId", attributeHeader.SubAttributeId);
                parameters.Add("SubAttributeDetailId", attributeHeader.SubAttributeDetailId);
                var from = attributeHeader.SortOrderAnotherBy > attributeHeader.AttributeSortBy ? attributeHeader.AttributeSortBy : attributeHeader.SortOrderAnotherBy;
                var to = attributeHeader.SortOrderAnotherBy > attributeHeader.AttributeSortBy ? attributeHeader.SortOrderAnotherBy : attributeHeader.AttributeSortBy;
                parameters.Add("SortOrderByFrom", from);
                parameters.Add("SortOrderByTo", to);
                if (attributeHeader.SubAttributeId > 0)
                {
                    query = "SELECT AttributeID,SubAttributeId,AttributeSortBy,SubAttributeDetailId FROM AttributeHeader Where (IsDeleted=0 or IsDeleted is null) AND SubAttributeId = @SubAttributeId  AND AttributeSortBy>@SortOrderByFrom and AttributeSortBy<=@SortOrderByTo order by AttributeSortBy asc";
                }
                if (attributeHeader.SubAttributeDetailId > 0)
                {
                    query = "SELECT AttributeID,SubAttributeId,AttributeSortBy,SubAttributeDetailId FROM AttributeHeader Where (IsDeleted=0 or IsDeleted is null) AND SubAttributeDetailId = @SubAttributeDetailId  AND AttributeSortBy>@SortOrderByFrom and AttributeSortBy<=@SortOrderByTo order by AttributeSortBy asc";
                }
                if (attributeHeader.SortOrderAnotherBy > attributeHeader.AttributeSortBy)
                {
                    if (attributeHeader.SubAttributeId > 0)
                    {
                        query = "SELECT AttributeID,SubAttributeId,AttributeSortBy,SubAttributeDetailId FROM AttributeHeader Where (IsDeleted=0 or IsDeleted is null) AND SubAttributeId = @SubAttributeId  AND AttributeSortBy>=@SortOrderByFrom and AttributeSortBy<@SortOrderByTo order by AttributeSortBy asc";
                    }
                    if (attributeHeader.SubAttributeDetailId > 0)
                    {
                        query = "SELECT AttributeID,SubAttributeId,AttributeSortBy,SubAttributeDetailId FROM AttributeHeader Where (IsDeleted=0 or IsDeleted is null) AND SubAttributeDetailId = @SubAttributeDetailId  AND AttributeSortBy>=@SortOrderByFrom and AttributeSortBy<@SortOrderByTo order by AttributeSortBy asc";
                    }
                }
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
        public async Task<AttributeHeader> UpdateAttributeHeaderSortOrder(AttributeHeader attributeHeader)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                        var query = string.Empty;
                        int? SortOrder = attributeHeader.SortOrderAnotherBy > attributeHeader.AttributeSortBy ? (attributeHeader.AttributeSortBy + 1) : attributeHeader.SortOrderAnotherBy;
                        query += "Update  AttributeHeader SET AttributeSortBy=" + attributeHeader.AttributeSortBy + "  WHERE (IsDeleted=0 or IsDeleted is null) AND AttributeID =" + attributeHeader.AttributeID + ";";
                        if (SortOrder > 0)
                        {
                            var result = await GetUpdateAttributeHeaderSortOrder(attributeHeader);
                            if (result != null && result.Count > 0)
                            {

                                result.ForEach(s =>
                                {
                                    query += "Update  AttributeHeader SET AttributeSortBy=" + SortOrder + "  WHERE (IsDeleted=0 or IsDeleted is null) AND AttributeID =" + s.AttributeID + ";";

                                    SortOrder++;
                                });

                            }
                        }
                        var rowsAffected = await connection.ExecuteAsync(query, null);
                        return attributeHeader;
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
        public Task<long> UpdateAsync(AttributeHeader attributeHeader)
        {
            throw new NotImplementedException();
        }
        public async Task<AttributeHeaderListModel> GetAllAttributeNameByIdAsync(List<long?> dynamicForm, long? UserId)
        {
            try
            {
                AttributeHeaderListModel attributeHeaderListModel = new AttributeHeaderListModel();
                var dynamicFormSectionAttributeSecurity = new List<DynamicFormSectionAttributeSecurity>();
                var DynamicFormAll = new List<DynamicForm>();
                var applicationMasters = new List<ApplicationMaster>(); var applicationMasterParent = new List<ApplicationMasterParent>(); var dynamicFormSectionAttributeSection = new List<DynamicFormSectionAttributeSection>();
                using (var connection = CreateConnection())
                {
                    var query = "select t1.DynamicFormSectionID,t1.SectionName,t1.SessionID,t1.StatusCodeID,t1.Instruction,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.SortOrderBy " +
                        "from DynamicFormSection t1\n\r" +
                         "JOIN DynamicForm t10 ON t1.DynamicFormID=t10.ID\r\n" +
                        "where (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND  t1.DynamicFormID IN (" + string.Join(',', dynamicForm) + ") order by  t1.SortOrderBy asc;\n\r";
                    query += "select (case when t1.IsDynamicFormGridDropdownMultiple is NULL then  0 ELSE t1.IsDynamicFormGridDropdownMultiple END) as IsDynamicFormGridDropdownMultiple,t1.IsDynamicFormGridDropdown,t1.GridDropDownDynamicFormID,t12.Name as GridDropDownDynamicFormName,t1.DynamicFormSectionAttributeID,t1.DynamicFormSectionID,t1.SessionID,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.AttributeID,t1.SortOrderBy,t1.ColSpan,t1.DisplayName,t1.IsMultiple,t1.IsRequired,t1.RequiredMessage,t1.IsSpinEditType,t1.FormUsedCount,t1.IsDisplayTableHeader,t1.FormToolTips,t1.IsVisible,t1.RadioLayout,t1.IsRadioCheckRemarks,t1.RemarksLabelName,t1.IsDeleted,t1.IsPlantLoadDependency,t1.PlantDropDownWithOtherDataSourceID,t1.PlantDropDownWithOtherDataSourceLabelName,t1.PlantDropDownWithOtherDataSourceIDs,t1.IsSetDefaultValue,t1.IsDefaultReadOnly,t1.ApplicationMasterID,t1.ApplicationMasterIDs,t1.IsDisplayDropDownHeader\n\r" +
                        ",(case when t1.IsDependencyMultiple is NULL then  0 ELSE t1.IsDependencyMultiple END) as IsDependencyMultiple,(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,t5.SectionName,t11.DataSourceTable as PlantDropDownWithOtherDataSourceTable,t9.sessionId as DynamicFormSessionId,t6.IsDynamicFormDropTagBox,t6.AttributeName,t6.ControlTypeId,t6.DropDownTypeId,t6.DataSourceId," +
                        "t8.DisplayName as DataSourceDisplayName,\r\n" +
                        "t8.DataSourceTable  as DataSourceTable," +
                        "t7.CodeValue as ControlType,t5.DynamicFormID,t6.DynamicFormID as DynamicFormGridDropDownID,t6.FilterDataSocurceID \r\n" +
                        "from\r\n" +
                        "DynamicFormSectionAttribute t1\r\n" +
                        "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                        "JOIN DynamicForm t10 ON t5.DynamicFormID=t10.ID\r\n" +
                        "JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                        "LEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID\r\n" +
                        "LEFT JOIN DynamicForm t9 ON t9.ID=t6.DynamicFormID\r\n" +
                         "LEFT JOIN AttributeHeaderDataSource t11 ON t11.AttributeHeaderDataSourceID=t1.PlantDropDownWithOtherDataSourceId\r\n" +
                        "JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\n" +
                        "LEFT JOIN DynamicForm t12 ON t12.ID=t1.GridDropDownDynamicFormID\r\n" +
                        "Where (t9.IsDeleted=0 OR t9.IsDeleted IS NULL) AND (t6.IsDeleted=0 OR t6.IsDeleted IS NULL) AND (t1.IsVisible=1 OR t1.IsVisible IS NULL) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t1.IsVisible= 1 OR t1.IsVisible is null) AND t5.DynamicFormID IN (" + string.Join(',', dynamicForm) + ") order by t1.SortOrderBy asc;";
                    query += "Select plantId,PlantCode,Description from Plant;";
                    query += "Select t1.HeaderDataSourceId,t1.AttributeHeaderDataSourceId,t1.DisplayName,t1.DataSourceTable,(Select COUNT(*) as IsDynamicFormFilterBy from DynamicFormFilterBy t2 where t2.AttributeHeaderDataSourceID=t1.AttributeHeaderDataSourceID)as IsDynamicFormFilterBy from AttributeHeaderDataSource t1;";
                    query += "Select DynamicFormSectionAttributeSecurityId,DynamicFormSectionAttributeId,UserId,IsAccess,IsViewFormatOnly from DynamicFormSectionAttributeSecurity;";
                    query += "Select ApplicationMasterId,ApplicationMasterName,ApplicationMasterDescription,ApplicationMasterCodeId from ApplicationMaster;";
                    query += "Select ApplicationMasterParentId,ApplicationMasterParentCodeId,ApplicationMasterName,Description,ParentId from ApplicationMasterParent;";
                    query += "select t1.*,t2.DynamicFormSectionAttributeID,t2.SequenceNo from DynamicFormSectionAttributeSection t1 JOIN DynamicFormSectionAttributeSectionParent t2 ON t1.DynamicFormSectionAttributeSectionParentID=t2.DynamicFormSectionAttributeSectionParentID;";
                    query += "select * from dynamicform where Id IN (" + string.Join(',', dynamicForm) + ");";
                    var results = await connection.QueryMultipleAsync(query);
                    attributeHeaderListModel.DynamicFormSection = results.Read<DynamicFormSection>().ToList();
                    attributeHeaderListModel.DynamicFormSectionAttribute = results.Read<DynamicFormSectionAttribute>().ToList();
                    attributeHeaderListModel.Plant = results.Read<Plant>().ToList();
                    attributeHeaderListModel.AttributeHeaderDataSource = results.Read<AttributeHeaderDataSource>().ToList();
                    dynamicFormSectionAttributeSecurity = results.Read<DynamicFormSectionAttributeSecurity>().ToList();
                    applicationMasters = results.Read<ApplicationMaster>().ToList();
                    applicationMasterParent = results.Read<ApplicationMasterParent>().ToList();
                    dynamicFormSectionAttributeSection = results.Read<DynamicFormSectionAttributeSection>().ToList();
                    attributeHeaderListModel.ApplicationMasterParent = applicationMasterParent;
                    DynamicFormAll = results.Read<DynamicForm>().ToList();
                    attributeHeaderListModel.DynamicFormAll = DynamicFormAll;
                }
                if (attributeHeaderListModel.DynamicFormSectionAttribute != null)
                {
                    string? plantCode = null;

                    List<long?> DynamicFormSectionAttributeIds = attributeHeaderListModel.DynamicFormSectionAttribute.Select(a => (long?)a.DynamicFormSectionAttributeId).Distinct().ToList();
                    attributeHeaderListModel.DynamicFormSectionAttributeSections = dynamicFormSectionAttributeSection.Where(q => DynamicFormSectionAttributeIds.Contains(q.DynamicFormSectionAttributeId)).ToList();
                    List<string?> appMasterNames = new List<string?>() { "ApplicationMasterParent", "ApplicationMaster" };
                    List<long?> DynamicGridFormIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => appMasterNames.Contains(a.DataSourceTable) && a.IsDynamicFormGridDropdown == true && a.GridDropDownDynamicFormID > 0).Select(z => z.GridDropDownDynamicFormID).Distinct().ToList();
                    List<long?> DynamicFormIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.DataSourceTable == "DynamicGrid" && a.DynamicFormGridDropDownId > 0).Select(a => a.DynamicFormGridDropDownId).Distinct().ToList();
                    if (DynamicGridFormIds != null && DynamicGridFormIds.Count > 0)
                    {
                        if (DynamicFormIds == null)
                        {
                            DynamicFormIds = new List<long?>();
                        }
                        DynamicFormIds.AddRange(DynamicGridFormIds);
                        //var DropDownOptionsFromGridListModel = await GetDynamicFormGridModelAsync(DynamicGridFormIds, UserId, dynamicForm.CompanyId, plantCode, applicationMasters, applicationMasterParent, null, false);
                    }
                    if (DynamicFormIds != null && DynamicFormIds.Count > 0)
                    {
                        DynamicFormIds = DynamicFormIds.Distinct().ToList();
                        attributeHeaderListModel.DropDownOptionsGridListModel = await GetDynamicFormGridModelAsync(DynamicFormIds, UserId, null, plantCode, applicationMasters, applicationMasterParent, null, false);
                    }
                    List<string?> dataSourceTableIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.DataSourceTable != null && a.DropDownTypeId == "Data Source").Select(a => a.DataSourceTable).Distinct().ToList();
                    List<long?> applicationMasterIds = new List<long?>(); List<long?> ApplicationMasterParentIds = new List<long?>();
                    AttributeDetailsAdds attributeResultDetails = new AttributeDetailsAdds();
                    attributeHeaderListModel.AttributeDetails = new List<AttributeDetails>();
                    // if (IsSubFormLoad == true)
                    // {
                    List<long?> attributeIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.AttributeId > 0).Select(a => a.AttributeId).Distinct().ToList();
                    List<long> attributeSubFormIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.AttributeId > 0 && a.ControlTypeId == 2710).Select(a => a.AttributeId.Value).Distinct().ToList();
                    attributeResultDetails = await GetAttributeDetails(attributeIds, attributeSubFormIds, "Main", null, plantCode, applicationMasters, applicationMasterParent);
                    var attributeDetails = attributeResultDetails.AttributeDetails;
                    attributeHeaderListModel.AttributeDetails = attributeDetails != null && attributeDetails.Count > 0 ? attributeDetails.Where(w => attributeIds.Contains(w.AttributeID)).ToList() : new List<AttributeDetails>();
                    var AttributeDetailsIds = attributeHeaderListModel.AttributeDetails.Select(s => s.AttributeDetailID).ToList();
                    List<long?> applicationMasterSubIds = new List<long?>();
                    var attributeSubResultDetails = await GetAttributeDetails(new List<long?>(), AttributeDetailsIds, "Sub", null, plantCode, applicationMasters, applicationMasterParent);

                    if (attributeHeaderListModel.AttributeDetails != null && attributeHeaderListModel.AttributeDetails.Count > 0)
                    {
                        attributeHeaderListModel.AttributeDetails.ForEach(z =>
                        {
                            z.SubAttributeHeaders = attributeSubResultDetails.AttributeHeader.Where(q => q.SubAttributeDetailId == z.AttributeDetailID).ToList();
                        });
                    }
                    // }
                    var dataSourceList = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(null, dataSourceTableIds, plantCode, applicationMasterIds, ApplicationMasterParentIds);
                    attributeHeaderListModel.AttributeDetails.AddRange(dataSourceList);
                    attributeHeaderListModel.DynamicFormSectionAttribute.ForEach(s =>
                    {
                        s.DynamicFormSectionAttributeSecurity = dynamicFormSectionAttributeSecurity != null && dynamicFormSectionAttributeSecurity.Count > 0 ? dynamicFormSectionAttributeSecurity.Where(d => d.DynamicFormSectionAttributeId == s.DynamicFormSectionAttributeId).ToList() : new List<DynamicFormSectionAttributeSecurity>();
                        s.AttributeName = string.IsNullOrEmpty(s.AttributeName) ? string.Empty : char.ToUpper(s.AttributeName[0]) + s.AttributeName.Substring(1);
                        s.DynamicAttributeName = s.DynamicFormSectionAttributeId + "_" + s.AttributeName;
                        if (s.DataSourceTable == "DynamicGrid")
                        {
                            s.DynamicGridDynamicFormData = attributeHeaderListModel.DropDownOptionsGridListModel.DynamicFormData.Where(x => x.DynamicFormId == s.DynamicFormGridDropDownId).ToList();
                        }
                        if (s.IsPlantLoadDependency == true && !string.IsNullOrEmpty(s.PlantDropDownWithOtherDataSourceIds))
                        {
                            var PlantDropDownWithOtherDataSourceListIds = s.PlantDropDownWithOtherDataSourceIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (PlantDropDownWithOtherDataSourceListIds.Count > 0)
                            {
                                s.AttributeHeaderDataSource = attributeHeaderListModel.AttributeHeaderDataSource.Where(z => z.DataSourceTable != null && PlantDropDownWithOtherDataSourceListIds.Contains(z.AttributeHeaderDataSourceId)).ToList();
                            }
                        }
                        if (s.DataSourceTable == "ApplicationMaster" && !string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                s.ApplicationMaster = applicationMasters.Where(z => z.ApplicationMasterId > 0 && applicationMasterIds.Contains(z.ApplicationMasterId)).ToList();
                            }
                        }
                        if (s.DataSourceTable == "ApplicationMasterParent" && !string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                s.ApplicationMasterParents = applicationMasterParent.Where(z => z.ApplicationMasterParentCodeId > 0 && applicationMasterIds.Contains(z.ApplicationMasterParentCodeId)).ToList();
                            }
                        }
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
        public async Task<List<DynamicFormWorkFlowFormReportItems>> GetDynamicFormWorkFlowFormReport(List<long> Ids)
        {
            try
            {
                List<DynamicFormWorkFlowFormReportItems> result = new List<DynamicFormWorkFlowFormReportItems>();
                List<DynamicFormWorkFlowFormApprovedReportItems> dynamicFormWorkFlowFormApprovedReportItems = new List<DynamicFormWorkFlowFormApprovedReportItems>();
                List<DynamicFormWorkFlowSectionForm> DynamicFormWorkFlowSectionForm = new List<DynamicFormWorkFlowSectionForm>();
                if (Ids.Count() > 0)
                {
                    var query = "select tt3.DynamicFormWorkFlowFormID,tt3.DynamicFormDataID,tt3.UserID as AssignedUserId,tt3.CompletedDate,tt3.SequenceNo,tt3.IsAllowDelegateUserForm,tt3.IsPendingApproval,tt3.IsDelegateUser,tt3.DelegateUserId,tt3.CurrentApprovalUserId,tt4.UserName as AssignedUser,tt5.UserName as DelegateUser,t4.UserName as CurrentApprovalUserName from (select tt2.* from(select tt1.*,(case when tt1.DelegateWorkFlowFormChangedId>0 THEN  1 ELSE  0 END) as IsDelegateUser,\r\n(case when tt1.DynamicFormWorkFlowFormTotalCount=tt1.DynamicFormWorkFlowFormCount THEN  0 ELSE  1 END) as IsPendingApproval,\r\n(case when tt1.DelegateUserId>0 THEN  tt1.DelegateUserId ELSE  tt1.UserID END) as CurrentApprovalUserId  from (select t1.*,\r\n(Select TOP(1) t2.UserID from DynamicFormWorkFlowFormDelegate t2 where t2.DynamicFormWorkFlowFormID=t1.DynamicFormWorkFlowFormID order by t2.DynamicFormWorkFlowFormDelegateID desc) as DelegateUserId,\r\n(Select TOP(1) t3.DynamicFormWorkFlowFormDelegateID from DynamicFormWorkFlowFormDelegate t3 where t3.DynamicFormWorkFlowFormID=t1.DynamicFormWorkFlowFormID order by t3.DynamicFormWorkFlowFormDelegateID desc) as DelegateWorkFlowFormChangedId,\r\n(select count(t5.DynamicFormWorkFlowFormID) from DynamicFormWorkFlowForm t5 where t5.DynamicFormDataID=t1.DynamicFormDataID) as DynamicFormWorkFlowFormTotalCount,\r\n(select count(t55.DynamicFormWorkFlowFormID) from DynamicFormWorkFlowForm t55 where t55.DynamicFormDataID=t1.DynamicFormDataID And t55.FlowStatusID=1) as DynamicFormWorkFlowFormCount\r\n from DynamicFormWorkFlowForm t1 )tt1 )tt2)tt3 " +
                        "JOIN ApplicationUser t4 ON t4.UserID=tt3.CurrentApprovalUserId " +
                        "LEFT JOIN ApplicationUser tt4 ON tt4.UserID=tt3.UserID " +
                        "LEFT JOIN ApplicationUser tt5 ON tt5.UserID=tt3.DelegateUserId WHere tt3.DynamicFormDataID in(" + string.Join(',', Ids) + ");";

                    using (var connection = CreateConnection())
                    {
                        result = (await connection.QueryAsync<DynamicFormWorkFlowFormReportItems>(query)).ToList();
                        if (result != null && result.Count > 0)
                        {
                            var formIds = result.Select(s => s.DynamicFormWorkFlowFormId).ToList();
                            var query1 = "select t1.*,t2.SectionName from DynamicFormWorkFlowSectionForm t1 JOIN DynamicFormSection t2 ON t1.DynamicFormSectionID=t2.DynamicFormSectionID\r\n where t1.DynamicFormWorkFlowFormId in(" + string.Join(',', formIds) + ");";
                            query1 += "select tt3.DynamicFormWorkFlowApprovedFormID,tt3.DynamicFormWorkFlowFormID,tt3.IsApproved,tt3.ApprovedDescription,tt3.ApprovedDate,tt3.SequenceNo,tt3.IsDelegateUser,tt3.IsPendingApproval,tt3.UserID as AssignedUserId,tt3.DelegateUserId,tt3.CurrentApprovalUserId,tt5.UserName as AssignedUser,tt6.UserName as DelegateUser,tt4.UserName as CurrentApprovalUserName from(select tt2.* from(select tt1.*,\r\n(case when tt1.DynamicFormWorkFlowApprovedFormChangedId>0 THEN  1 ELSE  0 END) as IsDelegateUser,\r\n(case when tt1.DynamicFormWorkFlowFormTotalCount=tt1.DynamicFormWorkFlowFormCount THEN  0 ELSE  1 END) as IsPendingApproval,\r\n(case when tt1.DelegateUserId>0 THEN  tt1.DelegateUserId ELSE  tt1.UserID END) as CurrentApprovalUserId from(select t1.*,\r\n(Select TOP(1) t3.DynamicFormWorkFlowApprovedFormChangedID from DynamicFormWorkFlowApprovedFormChanged t3 where t3.DynamicFormWorkFlowApprovedFormID=t1.DynamicFormWorkFlowApprovedFormID order by t3.DynamicFormWorkFlowApprovedFormChangedID desc) as DynamicFormWorkFlowApprovedFormChangedId,\r\n(Select TOP(1) t2.UserID from DynamicFormWorkFlowApprovedFormChanged t2 where t2.DynamicFormWorkFlowApprovedFormID=t1.DynamicFormWorkFlowApprovedFormID order by t2.DynamicFormWorkFlowApprovedFormChangedID desc) as DelegateUserId,\r\n(select count(t5.DynamicFormWorkFlowFormID) from DynamicFormWorkFlowForm t5 where t5.DynamicFormDataID=t4.DynamicFormDataID) as DynamicFormWorkFlowFormTotalCount,\r\n(select count(t55.DynamicFormWorkFlowFormID) from DynamicFormWorkFlowForm t55 where t55.DynamicFormDataID=t4.DynamicFormDataID And t55.FlowStatusID=1) as DynamicFormWorkFlowFormCount,t4.SequenceNo\r\nfrom DynamicFormWorkFlowApprovedForm t1 JOIN DynamicFormWorkFlowForm t4 ON t4.DynamicFormWorkFlowFormID=t1.DynamicFormWorkFlowFormID )tt1)tt2)\r\ntt3 JOIN ApplicationUser tt4 ON tt4.UserID=tt3.CurrentApprovalUserId  \r\nLEFT JOIN ApplicationUser tt5 ON tt5.UserID=tt3.UserID " +
                                "LEFT JOIN ApplicationUser tt6 ON tt6.UserID=tt3.DelegateUserId where tt3.DynamicFormWorkFlowFormId in(" + string.Join(',', formIds) + ");";
                            var results = await connection.QueryMultipleAsync(query1);
                            DynamicFormWorkFlowSectionForm = results.Read<DynamicFormWorkFlowSectionForm>().ToList();
                            dynamicFormWorkFlowFormApprovedReportItems = results.Read<DynamicFormWorkFlowFormApprovedReportItems>().ToList();
                        }
                    }
                    if (result != null && result.Count > 0)
                    {
                        result.ForEach(s =>
                        {
                            var res = DynamicFormWorkFlowSectionForm.Where(w => w.DynamicFormWorkFlowFormID == s.DynamicFormWorkFlowFormId && w.SectionName != null && w.SectionName != null).Select(a => a.SectionName).Distinct().ToList();
                            s.SectionName = res != null && res.Count() > 0 ? string.Join(',', res) : string.Empty;
                            s.DynamicFormWorkFlowFormApprovedReportItems = dynamicFormWorkFlowFormApprovedReportItems.Where(w => w.DynamicFormWorkFlowFormID == s.DynamicFormWorkFlowFormId).ToList();
                        });
                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormDataResponse>> GetAllDynamicFormAttributeAllApiAsync(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId, string? BasUrl, bool? IsAll)
        {
            var _dynamicformObjectDataList = new List<DynamicFormDataResponse>();
            try
            {
                // string BasUrl = "";
                List<long?> Ids = new List<long?>();
                List<long?> userIds = new List<long?>();
                List<Guid?> SessionIds = new List<Guid?>();
                var parameters = new DynamicParameters();
                List<ApplicationUser> appUsers = new List<ApplicationUser>();
                List<DynamicFormWorkFlowFormReportItems> DynamicFormWorkFlowFormReportItems = new List<DynamicFormWorkFlowFormReportItems>();
                DynamicFormWorkFlowFormReportItems dynamicFormWorkFlowFormReportItems = new DynamicFormWorkFlowFormReportItems();
                List<DynamicFormWorkFlowFormApprovedReportItems> dynamicFormWorkFlowFormApprovedReportItems = new List<DynamicFormWorkFlowFormApprovedReportItems>();
                DynamicFormWorkFlowFormApprovedReportItems dynamicFormWorkFlowFormApprovedReportItems1 = new DynamicFormWorkFlowFormApprovedReportItems();
                dynamicFormWorkFlowFormApprovedReportItems.Add(dynamicFormWorkFlowFormApprovedReportItems1);
                dynamicFormWorkFlowFormReportItems.DynamicFormWorkFlowFormApprovedReportItems = dynamicFormWorkFlowFormApprovedReportItems;
                DynamicFormWorkFlowFormReportItems.Add(dynamicFormWorkFlowFormReportItems);
                var query = string.Empty;
                if (DynamicFormSessionId != null)
                {
                    var _dynamicForm = new DynamicForm();
                    var _dynamicFormDataGrid = new DynamicFormData();
                    List<DynamicForm> dynamicFormAll = new List<DynamicForm>();
                    List<DynamicFormData> dynamicFormData = new List<DynamicFormData>();
                    List<DynamicFormData> dynamicFormGridData = new List<DynamicFormData>();

                    parameters.Add("SessionId", DynamicFormSessionId, DbType.Guid);
                    parameters.Add("DynamicGridFormSessionID", DynamicFormDataSessionId, DbType.Guid);
                    parameters.Add("DynamicFormDataGridSessionId", DynamicFormDataGridSessionId, DbType.Guid);
                    using (var connection = CreateConnection())
                    {
                        if (IsAll == true)
                        {
                            if (DynamicFormDataSessionId != null)
                            {
                                query += "Select ID,Name,ScreenID,SessionID,AttributeID,IsApproval,IsUpload,FileProfileTypeID,CompanyID,ProfileID,IsGridForm from DynamicForm where SessionId =@DynamicFormDataGridSessionId AND (IsDeleted=0 OR IsDeleted IS NULL);";
                            }
                            else
                            {
                                query += "Select ID,Name,ScreenID,SessionID,AttributeID,IsApproval,IsUpload,FileProfileTypeID,CompanyID,ProfileID,IsGridForm from DynamicForm where SessionId =@SessionId AND (IsDeleted=0 OR IsDeleted IS NULL);";

                            }
                        }
                        else
                        {
                            query += "Select ID,Name,ScreenID,SessionID,AttributeID,IsApproval,IsUpload,FileProfileTypeID,CompanyID,ProfileID,IsGridForm from DynamicForm where SessionId =@SessionId AND (IsDeleted=0 OR IsDeleted IS NULL);";
                        }
                        var result = await connection.QueryMultipleAsync(query, parameters);
                        _dynamicForm = result.Read<DynamicForm>().FirstOrDefault();
                        var query1 = string.Empty;
                        if (_dynamicForm != null && _dynamicForm.ID > 0)
                        {
                            Ids.Add(_dynamicForm?.ID);
                        }
                    }
                    if (_dynamicForm != null && _dynamicForm.ID > 0)
                    {
                        AttributeHeaderListModel _AttributeHeader = new AttributeHeaderListModel();
                        _AttributeHeader = await GetAllAttributeNameByIdAsync(Ids, 1);

                        if (_AttributeHeader != null && _AttributeHeader.DynamicFormSectionAttribute != null && _AttributeHeader.DynamicFormSectionAttribute.Count > 0)
                        {
                            List<string?> dataSourceTableIds = new List<string?>() { "Plant" };
                            var DataSourceTablelists = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.AttributeHeaderDataSource.Count > 0).SelectMany(a => a.AttributeHeaderDataSource.Select(s => s.DataSourceTable)).ToList().Distinct().ToList();
                            dataSourceTableIds.AddRange(DataSourceTablelists); List<long?> ApplicationMasterIds = new List<long?>(); List<long?> ApplicationMasterParentIds = new List<long?>();
                            ApplicationMasterIds = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.ApplicationMaster.Count > 0).SelectMany(a => a.ApplicationMaster.Select(s => (long?)s.ApplicationMasterId)).ToList().Distinct().ToList();
                            ApplicationMasterParentIds = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.ApplicationMasterParents.Count > 0).SelectMany(a => a.ApplicationMasterParents.Select(s => (long?)s.ApplicationMasterParentCodeId)).ToList().Distinct().ToList();
                            if (ApplicationMasterIds.Count > 0)
                            {
                                dataSourceTableIds.Add("ApplicationMaster");
                            }
                            if (ApplicationMasterParentIds != null && ApplicationMasterParentIds.Count > 0)
                            {
                                dataSourceTableIds.Add("ApplicationMasterParent");
                            }
                            var PlantDependencySubAttributeDetails = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(null, dataSourceTableIds, null, ApplicationMasterIds, ApplicationMasterParentIds != null ? ApplicationMasterParentIds : new List<long?>());
                            var PlantDependencySubAttributeDetailss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.ToList() : new List<AttributeDetails>();
                            _dynamicformObjectDataList = GetDynamicFormDataAttributeAllLists(_AttributeHeader, dynamicFormData, _dynamicForm, PlantDependencySubAttributeDetailss, BasUrl, dynamicFormGridData, appUsers, DynamicFormWorkFlowFormReportItems);
                        }
                    }
                }
                return _dynamicformObjectDataList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }


        }
        private List<DynamicFormDataResponse> GetDynamicFormDataAttributeAllLists(AttributeHeaderListModel _AttributeHeader, List<DynamicFormData> dynamicFormDataList, DynamicForm _dynamicForm, List<AttributeDetails> PlantDependencySubAttributeDetails, string BasUrl, List<DynamicFormData> dynamicFormGridData, List<ApplicationUser> appUsers, List<DynamicFormWorkFlowFormReportItems> dynamicFormWorkFlowFormReportItems)
        {
            var _dynamicformObjectDataList = new List<DynamicFormDataResponse>();
            if (_AttributeHeader != null && _AttributeHeader.DynamicFormSectionAttribute != null && _AttributeHeader.DynamicFormSectionAttribute.Count > 0)
            {
                DynamicFormDataResponse dynamicFormData = new DynamicFormDataResponse();
                dynamic jsonObj = new object();
                DropDownModel dropDownModel = new DropDownModel();
                dropDownModel.Text = string.Empty; dropDownModel.Value = string.Empty;
                string strJson = JsonConvert.SerializeObject(dropDownModel);
                jsonObj = JsonConvert.DeserializeObject(strJson);
                IDictionary<string, object> objectDataList = new ExpandoObject();
                IDictionary<string, object> objectData = new ExpandoObject();
                List<object> list = new List<object>();
                List<DynamicFormReportItems> dynamicFormReportItems = new List<DynamicFormReportItems>();
                objectDataList["dynamicFormDataId"] = null;
                objectDataList["profileNo"] = string.Empty;
                objectDataList["name"] = _dynamicForm?.Name;
                objectDataList["SessionId"] = null;

               // dynamicFormData.DynamicFormWorkFlowFormReportItems = dynamicFormWorkFlowFormReportItems;
                if (_AttributeHeader != null && _AttributeHeader.DynamicFormSectionAttribute != null && _AttributeHeader.DynamicFormSectionAttribute.Count > 0)
                {
                    var _dynamicFormSectionAttributeList = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.DynamicFormId == _dynamicForm?.ID).ToList();
                    _dynamicFormSectionAttributeList.ForEach(s =>
                    {
                        string attrName = s.DynamicAttributeName;
                        var opts = new Dictionary<object, object>();
                        if (s.ControlTypeId == 2712)
                        {
                            var url = "?DynamicFormSessionId=" + s.DynamicFormSessionId;

                            opts.Add("Label", s.DisplayName);
                            opts.Add("Value", s.DynamicFormSessionId);
                            opts.Add("IsGrid", true);
                            opts.Add("DynamicFormSessionId", _dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId);
                            opts.Add("DynamicFormDataSessionId", null);
                            opts.Add("DynamicFormDataGridSessionId", s.DynamicFormSessionId);
                            opts.Add("DynamicFormSectionGridAttributeSessionId", s.SessionId);
                            opts.Add("DynamicGridFormId", _dynamicForm.ID);
                            opts.Add("DynamicGridFormDataId", null);
                            opts.Add("DynamicGridFormDataGridId", s.DynamicFormGridDropDownId);
                            opts.Add("DynamicFormSectionGridAttributeId", s.DynamicFormSectionAttributeId);
                            opts.Add("Url", url);
                            objectData[attrName] = opts;
                            objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = s.DynamicFormSessionId;
                            DynamicFormReportItems dynamicFormReportItems1 = new DynamicFormReportItems();
                            dynamicFormReportItems1.AttrId = attrName;
                            dynamicFormReportItems1.Label = s.DisplayName;
                            dynamicFormReportItems1.IsGrid = true;
                            dynamicFormReportItems1.DynamicFormSessionId = _dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId;
                            dynamicFormReportItems1.DynamicFormDataSessionId = null;
                            dynamicFormReportItems1.DynamicFormDataGridSessionId = s.DynamicFormSessionId;
                            dynamicFormReportItems1.DynamicGridFormId = _dynamicForm.ID;
                            dynamicFormReportItems1.DynamicGridFormDataId = null;
                            dynamicFormReportItems1.DynamicGridFormDataGridId = s.DynamicFormGridDropDownId;
                            dynamicFormReportItems1.Url = BasUrl + url;
                            dynamicFormReportItems1.DynamicFormSectionGridAttributeId = s.DynamicFormSectionAttributeId;
                            dynamicFormReportItems1.DynamicFormSectionGridAttributeSessionId = s.SessionId;
                            dynamicFormReportItems.Add(dynamicFormReportItems1);
                        }
                        else
                        {
                            if (s.ControlType == "ComboBox" && s.IsPlantLoadDependency == true && s.AttributeHeaderDataSource.Count() > 0)
                            {
                                opts.Add("Label", s.DisplayName);
                                opts.Add("Value", string.Empty);
                                objectData[attrName] = opts;
                                objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                DynamicFormReportItems dynamicFormReportItems5 = new DynamicFormReportItems();
                                dynamicFormReportItems5.AttrId = attrName;
                                dynamicFormReportItems5.Label = s.DisplayName;
                                dynamicFormReportItems5.Value = string.Empty;
                                dynamicFormReportItems.Add(dynamicFormReportItems5);
                                s.AttributeHeaderDataSource.ForEach(dd =>
                                {
                                    var opts1 = new Dictionary<object, object>();
                                    opts1.Add("Label", dd.DisplayName);
                                    var nameData = s.DynamicFormSectionAttributeId + "_" + dd.AttributeHeaderDataSourceId + "_" + dd.DataSourceTable + "_Dependency";
                                    opts1.Add("Value", string.Empty);
                                    objectData[nameData] = opts1;
                                    objectDataList[nameData + "$" + dd.DisplayName.Replace(" ", "_")] = string.Empty;
                                    DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                    dynamicFormReportItems55.AttrId = nameData;
                                    dynamicFormReportItems55.Label = dd.DisplayName;
                                    dynamicFormReportItems55.Value = string.Empty;
                                    dynamicFormReportItems.Add(dynamicFormReportItems55);
                                });

                            }
                            else
                            {
                                if (s.DataSourceTable == "ApplicationMasterParent")
                                {
                                    opts.Add("Label", s.DisplayName);
                                    opts.Add("Value", string.Empty);
                                    objectData[attrName] = opts;
                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                    DynamicFormReportItems dynamicFormReportItems56 = new DynamicFormReportItems();
                                    dynamicFormReportItems56.AttrId = attrName;
                                    dynamicFormReportItems56.Label = s.DisplayName;
                                    dynamicFormReportItems56.Value = string.Empty;
                                    dynamicFormReportItems.Add(dynamicFormReportItems56);
                                    List<ApplicationMasterParent> nameDatas = new List<ApplicationMasterParent>();
                                    s.ApplicationMasterParents.ForEach(ab =>
                                    {
                                        nameDatas.Add(ab);
                                        RemoveApplicationMasterParentSingleNameItemApi(ab, s, nameDatas, _AttributeHeader.ApplicationMasterParent);
                                        if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMasterParent" && s.IsDynamicFormGridDropdown == true)
                                        {
                                            var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                            var opts1 = new Dictionary<object, object>();
                                            opts1.Add("Label", s.GridDropDownDynamicFormName);
                                            opts1.Add("Value", string.Empty);
                                            objectData[appendDependency] = opts1;
                                            objectDataList[appendDependency + "$" + s.GridDropDownDynamicFormName.Replace(" ", "_")] = string.Empty;
                                            DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                            dynamicFormReportItems55.AttrId = appendDependency;
                                            dynamicFormReportItems55.Label = s.GridDropDownDynamicFormName;
                                            dynamicFormReportItems55.Value = string.Empty;
                                            dynamicFormReportItems.Add(dynamicFormReportItems55);
                                        }
                                    });
                                    if (nameDatas != null && nameDatas.Count() > 0)
                                    {
                                        nameDatas.ForEach(n =>
                                        {
                                            var opts1 = new Dictionary<object, object>();
                                            opts1.Add("Label", n.ApplicationMasterName);
                                            var nameData = s.DynamicFormSectionAttributeId + "_" + n.ApplicationMasterParentCodeId + "_AppMasterPar";
                                            opts1.Add("Value", string.Empty);
                                            objectData[nameData] = opts1;
                                            objectDataList[nameData + "$" + n.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                            DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                            dynamicFormReportItems55.AttrId = nameData;
                                            dynamicFormReportItems55.Label = n.ApplicationMasterName;
                                            dynamicFormReportItems55.Value = string.Empty;
                                            dynamicFormReportItems.Add(dynamicFormReportItems55);
                                        });
                                    }

                                }
                                else if (s.DataSourceTable == "ApplicationMaster")
                                {
                                    opts.Add("Label", s.DisplayName);
                                    opts.Add("Value", string.Empty);
                                    objectData[attrName] = opts;
                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                    DynamicFormReportItems dynamicFormReportItems57 = new DynamicFormReportItems();
                                    dynamicFormReportItems57.AttrId = attrName;
                                    dynamicFormReportItems57.Label = s.DisplayName;
                                    dynamicFormReportItems57.Value = string.Empty;
                                    dynamicFormReportItems.Add(dynamicFormReportItems57);
                                    if (s.ApplicationMaster.Count() > 0)
                                    {
                                        s.ApplicationMaster.ForEach(ab =>
                                        {
                                            var opts1 = new Dictionary<object, object>();
                                            opts1.Add("Label", ab.ApplicationMasterName);
                                            var nameData = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_AppMaster";
                                            opts1.Add("Value", string.Empty);
                                            objectData[nameData] = opts1;
                                            objectDataList[attrName + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                            DynamicFormReportItems dynamicFormReportItems51 = new DynamicFormReportItems();
                                            dynamicFormReportItems51.AttrId = nameData;
                                            dynamicFormReportItems51.Label = ab.ApplicationMasterName;
                                            dynamicFormReportItems51.Value = string.Empty;
                                            dynamicFormReportItems.Add(dynamicFormReportItems51);
                                            if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMaster" && s.IsDynamicFormGridDropdown == true)
                                            {
                                                var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                                var opts12 = new Dictionary<object, object>();
                                                opts12.Add("Label", s.GridDropDownDynamicFormName);
                                                opts12.Add("Value", string.Empty);
                                                objectData[appendDependency] = opts12;
                                                objectDataList[appendDependency + "$" + s.GridDropDownDynamicFormName.Replace(" ", "_")] = string.Empty;
                                                DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                                dynamicFormReportItems55.AttrId = appendDependency;
                                                dynamicFormReportItems55.Label = s.GridDropDownDynamicFormName;
                                                dynamicFormReportItems55.Value = string.Empty;
                                                dynamicFormReportItems.Add(dynamicFormReportItems55);
                                            }
                                        });
                                    }

                                }
                                else if (s.ControlType == "CheckBox" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                                {
                                    opts.Add("Label", s.DisplayName);
                                    opts.Add("Value", s.ControlType == "CheckBox" ? false : string.Empty);
                                    objectData[attrName] = opts;
                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = s.ControlType == "CheckBox" ? false : string.Empty;
                                    DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                    dynamicFormReportItems58.AttrId = attrName;
                                    dynamicFormReportItems58.Label = s.DisplayName;
                                    dynamicFormReportItems58.Value = string.Empty;
                                    dynamicFormReportItems.Add(dynamicFormReportItems58);
                                    if (s.ControlType == "CheckBox")
                                    {
                                        loadSubHeaders(s.SubAttributeHeaders, s, jsonObj, dynamicFormReportItems, objectData, objectDataList);
                                    }
                                    else
                                    {
                                        var attrDetails = _AttributeHeader.AttributeDetails.Where(mm => mm.AttributeID == s.AttributeId && mm.DropDownTypeId == null).ToList();
                                        if (attrDetails != null && attrDetails.Count > 0)
                                        {
                                            attrDetails.ForEach(u =>
                                            {
                                                if (u.SubAttributeHeaders != null && u.SubAttributeHeaders.Count() > 0)
                                                {
                                                    loadSubHeaders(u.SubAttributeHeaders, s, jsonObj, dynamicFormReportItems, objectData, objectDataList);
                                                }
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    opts.Add("Label", s.DisplayName);
                                    opts.Add("Value", string.Empty);
                                    objectData[attrName] = opts;
                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                    DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                    dynamicFormReportItems58.AttrId = attrName;
                                    dynamicFormReportItems58.Label = s.DisplayName;
                                    dynamicFormReportItems58.Value = string.Empty;
                                    dynamicFormReportItems.Add(dynamicFormReportItems58);
                                }
                            }
                        }



                    });
                    list.Add(objectData);
                    // dynamicFormData.ObjectDataList = list;
                    dynamicFormData.ObjectDataItems = objectDataList;
                    dynamicFormData.DynamicFormReportItems = dynamicFormReportItems;
                    _dynamicformObjectDataList.Add(dynamicFormData);

                }

            }
            return _dynamicformObjectDataList;
        }
        public async Task<List<DynamicFormDataResponse>> GetAllDynamicFormAllApiAsync(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, string? BasUrl, bool? IsAll)
        {
            var _dynamicformObjectDataList = new List<DynamicFormDataResponse>();
            try
            {
                List<long?> Ids = new List<long?>();
                List<long?> userIds = new List<long?>();
                List<Guid?> SessionIds = new List<Guid?>();
                var parameters = new DynamicParameters();
                List<ApplicationUser> appUsers = new List<ApplicationUser>();
                List<DynamicFormWorkFlowFormReportItems> DynamicFormWorkFlowFormReportItems = new List<DynamicFormWorkFlowFormReportItems>();
                var query = string.Empty;
                if (DynamicFormSessionId != null)
                {
                    var _dynamicForm = new DynamicForm();
                    var _dynamicFormDataGrid = new DynamicFormData();
                    List<DynamicForm> dynamicFormAll = new List<DynamicForm>();
                    List<DynamicFormData> dynamicFormData = new List<DynamicFormData>();
                    List<DynamicFormData> dynamicFormGridData = new List<DynamicFormData>();

                    parameters.Add("SessionId", DynamicFormSessionId, DbType.Guid);
                    parameters.Add("DynamicGridFormSessionID", DynamicFormDataSessionId, DbType.Guid);
                    parameters.Add("DynamicFormDataGridSessionId", DynamicFormDataGridSessionId, DbType.Guid);
                    using (var connection = CreateConnection())
                    {
                        if (IsAll == true)
                        {
                            if (DynamicFormDataSessionId != null)
                            {
                                query += "Select ID,Name,ScreenID,SessionID,AttributeID,IsApproval,IsUpload,FileProfileTypeID,CompanyID,ProfileID,IsGridForm from DynamicForm where SessionId =@DynamicFormDataGridSessionId AND (IsDeleted=0 OR IsDeleted IS NULL);";
                            }
                            else
                            {
                                query += "Select ID,Name,ScreenID,SessionID,AttributeID,IsApproval,IsUpload,FileProfileTypeID,CompanyID,ProfileID,IsGridForm from DynamicForm where SessionId =@SessionId AND (IsDeleted=0 OR IsDeleted IS NULL);";

                            }
                        }
                        else
                        {
                            query += "Select ID,Name,ScreenID,SessionID,AttributeID,IsApproval,IsUpload,FileProfileTypeID,CompanyID,ProfileID,IsGridForm from DynamicForm where SessionId =@SessionId AND (IsDeleted=0 OR IsDeleted IS NULL);";
                        }
                        var result = await connection.QueryMultipleAsync(query, parameters);
                        _dynamicForm = result.Read<DynamicForm>().FirstOrDefault();
                        var query1 = string.Empty;
                        if (_dynamicForm != null && _dynamicForm.ID > 0)
                        {
                            Ids.Add(_dynamicForm?.ID);
                            if (IsAll == true)
                            {
                                if (DynamicFormDataSessionId != null)
                                {

                                    query1 += "select * from DynamicFormData where (IsDeleted=0 OR IsDeleted IS NULL) AND DynamicFormDataGridID in (Select DynamicFormDataID from DynamicFormData where SessionId =@DynamicGridFormSessionID AND (IsDeleted=0 OR IsDeleted IS NULL));";
                                    query1 += "select * from DynamicFormData where (IsDeleted=0 OR IsDeleted IS NULL) AND DynamicFormDataGridID in(select DynamicFormDataID from DynamicFormData where (IsDeleted=0 OR IsDeleted IS NULL) AND dynamicformid=" + _dynamicForm?.ID + ");";

                                }
                                else
                                {
                                    query1 += "select * from DynamicFormData where (IsDeleted=0 OR IsDeleted IS NULL) AND dynamicformid=" + _dynamicForm?.ID + " order by SortOrderByNo asc ;";
                                    query1 += "select * from DynamicFormData where (IsDeleted=0 OR IsDeleted IS NULL) AND DynamicFormDataGridID in(select DynamicFormDataID from DynamicFormData where (IsDeleted=0 OR IsDeleted IS NULL) AND dynamicformid=" + _dynamicForm?.ID + ");";
                                }
                            }
                            else
                            {
                                query1 += "select * from DynamicFormData where (IsDeleted=0 OR IsDeleted IS NULL) AND Sessionid='" + DynamicFormDataSessionId + "' AND  dynamicformid=" + _dynamicForm?.ID + " order by SortOrderByNo asc ;";
                                query1 += "select * from DynamicFormData where (IsDeleted=0 OR IsDeleted IS NULL) " +
                                    "AND DynamicFormDataGridID IN(select DynamicFormDataID from DynamicFormData where (IsDeleted=0 OR IsDeleted IS NULL) AND Sessionid='" + DynamicFormDataSessionId + "' AND  dynamicformid=" + _dynamicForm?.ID + ") AND DynamicFormDataGridID in(select DynamicFormDataID from DynamicFormData where (IsDeleted=0 OR IsDeleted IS NULL) AND dynamicformid=" + _dynamicForm?.ID + ");";
                            }
                            if (!string.IsNullOrEmpty(query1))
                            {
                                List<long> DynamicFormDataIds = new List<long>();
                                var results = await connection.QueryMultipleAsync(query1, parameters);
                                dynamicFormData = results.Read<DynamicFormData>().ToList();
                                userIds.AddRange(dynamicFormData.Where(w => w.AddedByUserID > 0).Select(s => s.AddedByUserID).ToList());
                                userIds.AddRange(dynamicFormData.Where(w => w.ModifiedByUserID > 0).Select(s => s.ModifiedByUserID).ToList());
                                DynamicFormDataIds.AddRange(dynamicFormData.Where(w => w.DynamicFormDataId > 0).Select(s => s.DynamicFormDataId).ToList());
                                dynamicFormGridData = results.Read<DynamicFormData>().ToList();
                                if (dynamicFormGridData != null && dynamicFormGridData.Count() > 0)
                                {
                                    DynamicFormDataIds.AddRange(dynamicFormGridData.Where(w => w.DynamicFormDataId > 0).Select(s => s.DynamicFormDataId).ToList());
                                    userIds.AddRange(dynamicFormGridData.Where(w => w.AddedByUserID > 0).Select(s => s.AddedByUserID).ToList());
                                    userIds.AddRange(dynamicFormGridData.Where(w => w.ModifiedByUserID > 0).Select(s => s.ModifiedByUserID).ToList());
                                    Ids.AddRange(dynamicFormGridData.Where(w => w.DynamicFormId > 0).Select(s => s.DynamicFormId).Distinct().ToList());
                                }
                                DynamicFormWorkFlowFormReportItems = await GetDynamicFormWorkFlowFormReport(DynamicFormDataIds);
                                if (userIds.Count() > 0)
                                {
                                    var query2 = "select UserName,UserId from ApplicationUser where userId in(" + string.Join(',', userIds.Distinct()) + ");";

                                    var QuerResult = await connection.QueryMultipleAsync(query2);
                                    appUsers = QuerResult.Read<ApplicationUser>().ToList();
                                }
                            }
                        }
                    }
                    if (_dynamicForm != null && _dynamicForm.ID > 0)
                    {
                        AttributeHeaderListModel _AttributeHeader = new AttributeHeaderListModel();
                        _AttributeHeader = await GetAllAttributeNameByIdAsync(Ids, 1);

                        if (_AttributeHeader != null && _AttributeHeader.DynamicFormSectionAttribute != null && _AttributeHeader.DynamicFormSectionAttribute.Count > 0)
                        {
                            List<string?> dataSourceTableIds = new List<string?>() { "Plant" };
                            var DataSourceTablelists = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.AttributeHeaderDataSource.Count > 0).SelectMany(a => a.AttributeHeaderDataSource.Select(s => s.DataSourceTable)).ToList().Distinct().ToList();
                            dataSourceTableIds.AddRange(DataSourceTablelists); List<long?> ApplicationMasterIds = new List<long?>(); List<long?> ApplicationMasterParentIds = new List<long?>();
                            ApplicationMasterIds = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.ApplicationMaster.Count > 0).SelectMany(a => a.ApplicationMaster.Select(s => (long?)s.ApplicationMasterId)).ToList().Distinct().ToList();
                            ApplicationMasterParentIds = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.ApplicationMasterParents.Count > 0).SelectMany(a => a.ApplicationMasterParents.Select(s => (long?)s.ApplicationMasterParentCodeId)).ToList().Distinct().ToList();
                            if (ApplicationMasterIds.Count > 0)
                            {
                                dataSourceTableIds.Add("ApplicationMaster");
                            }
                            if (ApplicationMasterParentIds != null && ApplicationMasterParentIds.Count > 0)
                            {
                                dataSourceTableIds.Add("ApplicationMasterParent");
                            }
                            var PlantDependencySubAttributeDetails = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(null, dataSourceTableIds, null, ApplicationMasterIds, ApplicationMasterParentIds != null ? ApplicationMasterParentIds : new List<long?>());
                            var PlantDependencySubAttributeDetailss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.ToList() : new List<AttributeDetails>();
                            _dynamicformObjectDataList = GetDynamicFormDataAllLists(_AttributeHeader, dynamicFormData, _dynamicForm, PlantDependencySubAttributeDetailss, BasUrl, dynamicFormGridData, appUsers, DynamicFormWorkFlowFormReportItems);
                        }
                    }
                }
                return _dynamicformObjectDataList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }


        }

        public async Task<List<DynamicFormDataResponse>> GetAllDynamicFormDataOneApiAsync(Guid? DynamicFormDataSessionId)
        {
            var _dynamicformObjectDataList = new List<DynamicFormDataResponse>();
            try
            {
                List<DynamicFormReportItems> ObjectDataItems = new List<DynamicFormReportItems>();
                List<long?> Ids = new List<long?>();
                var parameters = new DynamicParameters();
                List<ApplicationUser> appUsers = new List<ApplicationUser>();
                List<DynamicFormWorkFlowFormReportItems> DynamicFormWorkFlowFormReportItems = new List<DynamicFormWorkFlowFormReportItems>();
                var query = string.Empty;
                if (DynamicFormDataSessionId != null)
                {
                    var _dynamicForm = new DynamicForm();
                    var _dynamicFormDataGrid = new DynamicFormData();
                    List<DynamicForm> dynamicFormAll = new List<DynamicForm>();
                    List<DynamicFormData> dynamicFormData = new List<DynamicFormData>();
                    List<DynamicFormData> dynamicFormGridData = new List<DynamicFormData>();

                    parameters.Add("SessionId", DynamicFormDataSessionId, DbType.Guid);
                    using (var connection = CreateConnection())
                    {
                        query += "Select * from DynamicFormData where SessionId =@SessionId AND (IsDeleted=0 OR IsDeleted IS NULL);";

                        var result = await connection.QueryMultipleAsync(query, parameters);
                        dynamicFormData = result.Read<DynamicFormData>().ToList();
                        if (dynamicFormData != null && dynamicFormData.Count() > 0)
                        {
                            var query2 = "select *  from DynamicForm where ID in(" + string.Join(',', dynamicFormData.FirstOrDefault()?.DynamicFormId) + ");";

                            var QuerResult = await connection.QueryMultipleAsync(query2);
                            _dynamicForm = QuerResult.Read<DynamicForm>().FirstOrDefault();
                        }
                    }
                    if (_dynamicForm != null && _dynamicForm.ID > 0)
                    {
                        Ids.Add(_dynamicForm.ID);
                        AttributeHeaderListModel _AttributeHeader = new AttributeHeaderListModel();
                        _AttributeHeader = await GetAllAttributeNameByIdAsync(Ids, 1);

                        if (_AttributeHeader != null && _AttributeHeader.DynamicFormSectionAttribute != null && _AttributeHeader.DynamicFormSectionAttribute.Count > 0)
                        {
                            List<string?> dataSourceTableIds = new List<string?>() { "Plant" };
                            var DataSourceTablelists = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.AttributeHeaderDataSource.Count > 0).SelectMany(a => a.AttributeHeaderDataSource.Select(s => s.DataSourceTable)).ToList().Distinct().ToList();
                            dataSourceTableIds.AddRange(DataSourceTablelists); List<long?> ApplicationMasterIds = new List<long?>(); List<long?> ApplicationMasterParentIds = new List<long?>();
                            ApplicationMasterIds = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.ApplicationMaster.Count > 0).SelectMany(a => a.ApplicationMaster.Select(s => (long?)s.ApplicationMasterId)).ToList().Distinct().ToList();
                            ApplicationMasterParentIds = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.ApplicationMasterParents.Count > 0).SelectMany(a => a.ApplicationMasterParents.Select(s => (long?)s.ApplicationMasterParentCodeId)).ToList().Distinct().ToList();
                            if (ApplicationMasterIds.Count > 0)
                            {
                                dataSourceTableIds.Add("ApplicationMaster");
                            }
                            if (ApplicationMasterParentIds != null && ApplicationMasterParentIds.Count > 0)
                            {
                                dataSourceTableIds.Add("ApplicationMasterParent");
                            }
                            var PlantDependencySubAttributeDetails = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(null, dataSourceTableIds, null, ApplicationMasterIds, ApplicationMasterParentIds != null ? ApplicationMasterParentIds : new List<long?>());
                            var PlantDependencySubAttributeDetailss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.ToList() : new List<AttributeDetails>();
                            _dynamicformObjectDataList = GetDynamicFormDataAllLists(_AttributeHeader, dynamicFormData, _dynamicForm, PlantDependencySubAttributeDetailss, string.Empty, dynamicFormGridData, appUsers, DynamicFormWorkFlowFormReportItems);
                        }
                    }
                }
                //if (_dynamicformObjectDataList != null && _dynamicformObjectDataList.Count() > 0)
                //{
                //    ObjectDataItems = _dynamicformObjectDataList.FirstOrDefault()?.DynamicFormReportItems;
                //}
                return _dynamicformObjectDataList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }


        }
        private List<DynamicFormDataResponse> GetDynamicFormDataAllLists(AttributeHeaderListModel _AttributeHeader, List<DynamicFormData> dynamicFormDataList, DynamicForm _dynamicForm, List<AttributeDetails> PlantDependencySubAttributeDetails, string BasUrl, List<DynamicFormData> dynamicFormGridData, List<ApplicationUser> appUsers, List<DynamicFormWorkFlowFormReportItems> dynamicFormWorkFlowFormReportItems)
        {
            var _dynamicformObjectDataList = new List<DynamicFormDataResponse>();
            List<object> listItems = new List<object>();
            if (_AttributeHeader != null && _AttributeHeader.DynamicFormSectionAttribute != null && _AttributeHeader.DynamicFormSectionAttribute.Count > 0)
            {
                dynamicFormDataList = dynamicFormDataList.Where(w => w.DynamicFormId == _dynamicForm?.ID).ToList();
                if (dynamicFormDataList != null && dynamicFormDataList.Count > 0)
                {
                    dynamicFormDataList.ToList().ForEach(r =>
                    {
                        DynamicFormDataResponse dynamicFormData = new DynamicFormDataResponse();
                        dynamic jsonObj = new object();
                        if (IsValidJson(r.DynamicFormItem))
                        {
                            jsonObj = JsonConvert.DeserializeObject(r.DynamicFormItem);
                        }

                        IDictionary<string, object> objectDataList = new ExpandoObject();
                        IDictionary<string, object> objectData = new ExpandoObject();
                        List<object> list = new List<object>();
                        List<DynamicFormReportItems> dynamicFormReportItems = new List<DynamicFormReportItems>();
                        dynamicFormData.SortOrderByNo = r.SortOrderByNo;
                        dynamicFormData.DynamicFormDataId = r.DynamicFormDataId;
                        dynamicFormData.ProfileNo = r.ProfileNo;
                        dynamicFormData.SessionId = r.SessionId;
                        dynamicFormData.ScreenID = r.ScreenID;
                        dynamicFormData.DynamicFormId = r.DynamicFormId;
                        dynamicFormData.Name = r.Name;
                        dynamicFormData.ModifiedBy = appUsers.FirstOrDefault(f => f.UserID == r.ModifiedByUserID)?.UserName;
                        dynamicFormData.AddedBy = appUsers.FirstOrDefault(f => f.UserID == r.AddedByUserID)?.UserName;
                        dynamicFormData.ModifiedDate = r.ModifiedDate;
                        dynamicFormData.AddedDate = r.AddedDate;
                        dynamicFormData.CurrentUserName = r.CurrentUserName;
                        dynamicFormData.StatusName = r.StatusName;
                        dynamicFormData.DynamicFormDataGridId = r.DynamicFormDataGridId;
                        dynamicFormData.IsDynamicFormDataGrid = r.IsDynamicFormDataGrid;
                        dynamicFormData.IsFileprofileTypeDocument = r.IsFileprofileTypeDocument;
                        dynamicFormData.DynamicFormSectionGridAttributeId = r.DynamicFormSectionGridAttributeId;
                        dynamicFormData.DynamicFormSectionGridAttributeSessionId = r.DynamicFormSectionGridAttributeSessionId;
                        dynamicFormData.IsDraft = r.IsDraft;
                        dynamicFormData.DynamicFormWorkFlowFormReportItems = dynamicFormWorkFlowFormReportItems.Where(w => w.DynamicFormDataId == r.DynamicFormDataId).OrderBy(o => o.SequenceNo).ToList();
                        objectDataList["dynamicFormDataId"] = r.DynamicFormDataId;
                        objectDataList["profileNo"] = r.ProfileNo;
                        objectDataList["name"] = _dynamicForm?.Name;
                        objectDataList["SessionId"] = r.SessionId;
                        if (_AttributeHeader != null && _AttributeHeader.DynamicFormSectionAttribute != null && _AttributeHeader.DynamicFormSectionAttribute.Count > 0)
                        {
                            var _dynamicFormSectionAttributeList = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.DynamicFormId == _dynamicForm?.ID).ToList();
                            _dynamicFormSectionAttributeList.ForEach(s =>
                            {

                                string attrName = s.DynamicAttributeName;
                                var opts = new Dictionary<object, object>();

                                var Names = jsonObj.ContainsKey(attrName);
                                if (Names == true)
                                {
                                    if (s.ControlTypeId == 2712)
                                    {
                                        var url = "?DynamicFormSessionId=" + (_dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId) + "&&DynamicFormDataSessionId=" + r.SessionId + "&&DynamicFormDataGridSessionId=" + s.DynamicFormSessionId + "&&DynamicFormSectionGridAttributeSessionId=" + s.SessionId;
                                        opts.Add("Label", s.DisplayName);
                                        opts.Add("Value", s.DynamicFormSessionId);
                                        opts.Add("IsGrid", true);
                                        opts.Add("DynamicFormSessionId", _dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId);
                                        opts.Add("DynamicFormDataSessionId", r.SessionId);
                                        opts.Add("DynamicFormDataGridSessionId", s.DynamicFormSessionId);
                                        opts.Add("DynamicFormSectionGridAttributeSessionId", s.SessionId);
                                        opts.Add("DynamicGridFormId", _dynamicForm.ID);
                                        opts.Add("DynamicGridFormDataId", r.DynamicFormDataId);
                                        opts.Add("DynamicGridFormDataGridId", s.DynamicFormGridDropDownId);
                                        opts.Add("DynamicFormSectionGridAttributeId", s.DynamicFormSectionAttributeId);
                                        opts.Add("Url", url);
                                        objectData[attrName] = opts;
                                        objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = s.DynamicFormSessionId;
                                        DynamicFormReportItems dynamicFormReportItems1 = new DynamicFormReportItems();
                                        dynamicFormReportItems1.AttrId = attrName;
                                        dynamicFormReportItems1.Label = s.DisplayName;
                                        dynamicFormReportItems1.IsGrid = true;
                                        dynamicFormReportItems1.DynamicFormSessionId = _dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId;
                                        dynamicFormReportItems1.DynamicFormDataSessionId = r.SessionId;
                                        dynamicFormReportItems1.DynamicFormDataGridSessionId = s.DynamicFormSessionId;
                                        dynamicFormReportItems1.DynamicGridFormDataGridId = s.DynamicFormGridDropDownId;
                                        dynamicFormReportItems1.Url = BasUrl + url;
                                        dynamicFormReportItems1.DynamicGridFormId = _dynamicForm.ID;
                                        dynamicFormReportItems1.DynamicGridFormDataId = r.DynamicFormDataId;
                                        dynamicFormReportItems1.DynamicFormSectionGridAttributeId = s.DynamicFormSectionAttributeId;
                                        dynamicFormReportItems1.DynamicFormSectionGridAttributeSessionId = s.SessionId;
                                        var gridDataItem = dynamicFormGridData?.Where(a => a.DynamicFormDataGridId == r.DynamicFormDataId && a.DynamicFormId == s.DynamicFormGridDropDownId && a.DynamicFormSectionGridAttributeId == s.DynamicFormSectionAttributeId).OrderBy(o => o.GridSortOrderByNo).ToList();
                                        var _dynamicForms = _AttributeHeader.DynamicFormAll?.FirstOrDefault(f => f.ID == s.DynamicFormGridDropDownId);
                                        var res = GetDynamicFormDataAllLists(_AttributeHeader, gridDataItem, _dynamicForms, PlantDependencySubAttributeDetails, BasUrl, dynamicFormGridData, appUsers, dynamicFormWorkFlowFormReportItems);
                                        var datass = res?.Select(s => s.ObjectDataItems).ToList();
                                       // dynamicFormReportItems1.GridItems = res;
                                        dynamicFormReportItems1.GridSingleItems = datass != null ? datass : new List<dynamic?>();
                                        dynamicFormReportItems.Add(dynamicFormReportItems1);
                                    }
                                    else
                                    {
                                        if (s.DataSourceTable == "ApplicationMaster")
                                        {
                                            opts.Add("Label", s.DisplayName);
                                            opts.Add("Value", string.Empty);
                                            objectData[attrName] = opts;
                                            objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                            DynamicFormReportItems dynamicFormReportItems2 = new DynamicFormReportItems();
                                            dynamicFormReportItems2.AttrId = attrName;
                                            dynamicFormReportItems2.Label = s.DisplayName;
                                            dynamicFormReportItems2.Value = string.Empty;
                                            dynamicFormReportItems.Add(dynamicFormReportItems2);
                                            if (s.ApplicationMaster != null && s.ApplicationMaster.Count() > 0)
                                            {
                                                s.ApplicationMaster.ForEach(ab =>
                                                {
                                                    var opts1 = new Dictionary<object, object>();
                                                    DynamicFormReportItems dynamicFormReportItems3 = new DynamicFormReportItems();
                                                    opts1.Add("Label", ab.ApplicationMasterName);
                                                    var nameData = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_AppMaster";
                                                    var SubNamess = jsonObj.ContainsKey(nameData);
                                                    if (SubNamess == true)
                                                    {
                                                        var itemValue = jsonObj[nameData];
                                                        if (itemValue is JArray)
                                                        {
                                                            List<long?> listData = itemValue.ToObject<List<long?>>();
                                                            if (s.IsMultiple == true || s.ControlType == "TagBox")
                                                            {
                                                                var listName = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.ApplicationMasterId == ab.ApplicationMasterId && a.DropDownTypeId == s.DataSourceTable).Select(s => s.AttributeDetailName).ToList() : new List<string?>();
                                                                var lists = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                                opts1.Add("Value", lists);
                                                                objectData[nameData] = opts1;
                                                                objectDataList[nameData + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = lists;
                                                                dynamicFormReportItems3.AttrId = nameData;
                                                                dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                                dynamicFormReportItems3.Value = lists;
                                                                dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                            }
                                                            else
                                                            {
                                                                opts1.Add("Value", string.Empty);
                                                                objectData[nameData] = opts1;
                                                                objectDataList[nameData + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                                                dynamicFormReportItems3.AttrId = nameData;
                                                                dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                                dynamicFormReportItems3.Value = string.Empty;
                                                                dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (s.ControlType == "ComboBox")
                                                            {
                                                                long? values = itemValue == null ? -1 : (long)itemValue;
                                                                var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                                                                opts1.Add("Value", listss);
                                                                objectData[nameData] = opts1;
                                                                objectDataList[nameData + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = listss;
                                                                dynamicFormReportItems3.AttrId = nameData;
                                                                dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                                dynamicFormReportItems3.Value = listss;
                                                                dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                            }
                                                            else
                                                            {
                                                                if (s.ControlType == "ListBox" && s.IsMultiple == false)
                                                                {
                                                                    long? values = itemValue == null ? -1 : (long)itemValue;
                                                                    var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                                                                    opts1.Add("Value", listss);
                                                                    objectData[nameData] = opts1;
                                                                    objectDataList[nameData + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = listss;
                                                                    dynamicFormReportItems3.AttrId = nameData;
                                                                    dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                                    dynamicFormReportItems3.Value = listss;
                                                                    dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                                }
                                                                else
                                                                {
                                                                    opts1.Add("Value", string.Empty);
                                                                    objectData[nameData] = opts1;
                                                                    objectDataList[nameData + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                                                    dynamicFormReportItems3.AttrId = nameData;
                                                                    dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                                    dynamicFormReportItems3.Value = string.Empty;
                                                                    dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                                }
                                                            }
                                                        }
                                                        if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMaster" && s.IsDynamicFormGridDropdown == true)
                                                        {
                                                            var collection = _AttributeHeader.DropDownOptionsGridListModel.ObjectData;
                                                            var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                                            var displayNames = string.Empty;
                                                            if (collection != null)
                                                            {
                                                                var itemDepExits = jsonObj.ContainsKey(appendDependency);
                                                                if (itemDepExits == true)
                                                                {
                                                                    var itemDepValue = jsonObj[appendDependency];
                                                                    if (s.IsDynamicFormGridDropdownMultiple == true)
                                                                    {
                                                                        if (itemDepValue is JArray)
                                                                        {
                                                                            List<long?> listData = itemDepValue.ToObject<List<long?>>();

                                                                            if (listData != null && listData.Count > 0)
                                                                            {
                                                                                listData.ForEach(l =>
                                                                                {
                                                                                    if (collection != null)
                                                                                    {
                                                                                        foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                                        {
                                                                                            dynamic eod = v;
                                                                                            if ((long?)eod.AttributeDetailID == l)
                                                                                            {
                                                                                                displayNames += eod.AttributeDetailName + ",";
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                });
                                                                            }
                                                                            displayNames = displayNames.TrimEnd(',');

                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        long? valuesDep = itemDepValue == null ? -1 : (long)itemDepValue;
                                                                        foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                        {
                                                                            dynamic eod = v;
                                                                            if ((long?)eod.AttributeDetailID == valuesDep)
                                                                            {
                                                                                displayNames += eod.AttributeDetailName;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            var opts12 = new Dictionary<object, object>();
                                                            opts12.Add("Label", s.GridDropDownDynamicFormName);
                                                            opts12.Add("Value", displayNames);
                                                            objectData[appendDependency] = opts12;
                                                            objectDataList[appendDependency + "$" + s.GridDropDownDynamicFormName.Replace(" ", "_")] = displayNames;
                                                            DynamicFormReportItems dynamicFormReportItems4 = new DynamicFormReportItems();
                                                            dynamicFormReportItems4.AttrId = appendDependency;
                                                            dynamicFormReportItems4.Label = s.GridDropDownDynamicFormName;
                                                            dynamicFormReportItems4.Value = displayNames;
                                                            dynamicFormReportItems.Add(dynamicFormReportItems4);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dynamicFormReportItems3.AttrId = nameData;
                                                        dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                        dynamicFormReportItems3.Value = string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                    }

                                                });
                                            }

                                        }
                                        else if (s.DataSourceTable == "ApplicationMasterParent")
                                        {
                                            opts.Add("Label", s.DisplayName);
                                            opts.Add("Value", string.Empty);
                                            objectData[attrName] = opts;
                                            objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                            DynamicFormReportItems dynamicFormReportItems21 = new DynamicFormReportItems();
                                            dynamicFormReportItems21.AttrId = attrName;
                                            dynamicFormReportItems21.Label = s.DisplayName;
                                            dynamicFormReportItems21.Value = string.Empty;
                                            dynamicFormReportItems.Add(dynamicFormReportItems21);
                                            List<ApplicationMasterParent> nameDatas = new List<ApplicationMasterParent>();
                                            s.ApplicationMasterParents.ForEach(ab =>
                                            {
                                                nameDatas.Add(ab);
                                                RemoveApplicationMasterParentSingleNameItemApi(ab, s, nameDatas, _AttributeHeader.ApplicationMasterParent);

                                                if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMasterParent" && s.IsDynamicFormGridDropdown == true)
                                                {
                                                    var collection = _AttributeHeader.DropDownOptionsGridListModel.ObjectData;
                                                    var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                                    var displayNames = string.Empty;
                                                    if (collection != null)
                                                    {
                                                        var itemDepExits = jsonObj.ContainsKey(appendDependency);
                                                        if (itemDepExits == true)
                                                        {
                                                            var itemDepValue = jsonObj[appendDependency];
                                                            if (s.IsDynamicFormGridDropdownMultiple == true)
                                                            {
                                                                if (itemDepValue is JArray)
                                                                {
                                                                    List<long?> listData = itemDepValue.ToObject<List<long?>>();

                                                                    if (listData != null && listData.Count > 0)
                                                                    {
                                                                        listData.ForEach(l =>
                                                                        {
                                                                            if (collection != null)
                                                                            {
                                                                                foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                                {
                                                                                    dynamic eod = v;
                                                                                    if ((long?)eod.AttributeDetailID == l)
                                                                                    {
                                                                                        displayNames += eod.AttributeDetailName + ",";
                                                                                    }
                                                                                }
                                                                            }
                                                                        });
                                                                    }
                                                                    displayNames = displayNames.TrimEnd(',');

                                                                }
                                                            }
                                                            else
                                                            {
                                                                long? valuesDep = itemDepValue == null ? -1 : (long)itemDepValue;
                                                                foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                {
                                                                    dynamic eod = v;
                                                                    if ((long?)eod.AttributeDetailID == valuesDep)
                                                                    {
                                                                        displayNames += eod.AttributeDetailName;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    var opts12 = new Dictionary<object, object>();
                                                    opts12.Add("Label", s.GridDropDownDynamicFormName);
                                                    opts12.Add("Value", displayNames);
                                                    objectData[appendDependency] = opts12;
                                                    objectDataList[appendDependency + "$" + s.GridDropDownDynamicFormName.Replace(" ", "_")] = displayNames;
                                                    DynamicFormReportItems dynamicFormReportItems4 = new DynamicFormReportItems();
                                                    dynamicFormReportItems4.AttrId = appendDependency;
                                                    dynamicFormReportItems4.Label = s.GridDropDownDynamicFormName;
                                                    dynamicFormReportItems4.Value = displayNames;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems4);
                                                }

                                            });
                                            if (nameDatas != null && nameDatas.Count() > 0)
                                            {
                                                nameDatas.ForEach(n =>
                                                {
                                                    var opts1 = new Dictionary<object, object>();
                                                    opts1.Add("Label", n.ApplicationMasterName);
                                                    var nameDataPar = s.DynamicFormSectionAttributeId + "_" + n.ApplicationMasterParentCodeId + "_AppMasterPar";
                                                    DynamicFormReportItems dynamicFormReportItems22 = new DynamicFormReportItems();
                                                    dynamicFormReportItems22.AttrId = nameDataPar;
                                                    dynamicFormReportItems22.Label = n.ApplicationMasterName;
                                                    loadApplicationMasterParentDataApi(jsonObj, s, nameDataPar, opts1, PlantDependencySubAttributeDetails, objectDataList, n.ApplicationMasterName, dynamicFormReportItems22);
                                                    objectData[nameDataPar] = opts1;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems22);
                                                });
                                            }

                                        }
                                        else
                                        {
                                            var itemValue = jsonObj[attrName];
                                            var collection = _AttributeHeader.DropDownOptionsGridListModel.ObjectData;

                                            var ValueSet = string.Empty;
                                            if (itemValue is JArray)
                                            {
                                                List<long?> listData = itemValue.ToObject<List<long?>>();
                                                if (s.DataSourceTable == "DynamicGrid")
                                                {
                                                    string displayNames = string.Empty;

                                                    if (listData != null && listData.Count > 0)
                                                    {
                                                        listData.ForEach(l =>
                                                        {
                                                            if (collection != null)
                                                            {
                                                                foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                {
                                                                    dynamic eod = v;
                                                                    if ((long?)eod.AttributeDetailID == l)
                                                                    {
                                                                        displayNames += eod.AttributeDetailName + ",";
                                                                    }
                                                                }
                                                            }
                                                        });
                                                    }

                                                    ValueSet = displayNames.TrimEnd(',');
                                                }
                                                else
                                                {
                                                    var listName = _AttributeHeader.AttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.DropDownTypeId == s.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                    if (s.DataSourceTable == "FinishedProdOrderLine" || s.DataSourceTable == "FinishedProdOrderLineProductionInProgress")
                                                    {
                                                        listName = _AttributeHeader.AttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.DropDownTypeId == s.DataSourceTable).Select(s => s.NameList).ToList();
                                                    }
                                                    ValueSet = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;

                                                }
                                                opts.Add("Label", s.DisplayName);
                                                opts.Add("Value", ValueSet);
                                                objectData[attrName] = opts;
                                                objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = ValueSet;
                                                DynamicFormReportItems dynamicFormReportItems4 = new DynamicFormReportItems();
                                                dynamicFormReportItems4.AttrId = attrName;
                                                dynamicFormReportItems4.Label = s.DisplayName;
                                                dynamicFormReportItems4.Value = ValueSet;
                                                dynamicFormReportItems.Add(dynamicFormReportItems4);
                                            }
                                            else
                                            {
                                                List<AttributeHeader> SubAttrsHeader = new List<AttributeHeader>();
                                                if (s.ControlType == "ComboBox" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                                                {
                                                    opts.Add("Label", s.DisplayName);
                                                    var ValueSets = string.Empty;
                                                    long? Svalues = itemValue == null ? null : (long)itemValue;
                                                    if (Svalues != null)
                                                    {
                                                        if (s.DataSourceTable == "DynamicGrid")
                                                        {
                                                            var displayNames = string.Empty;
                                                            if (collection != null)
                                                            {
                                                                foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                {
                                                                    dynamic eod = v;
                                                                    if ((long?)eod.AttributeDetailID == Svalues)
                                                                    {
                                                                        displayNames += eod.AttributeDetailName;
                                                                    }
                                                                }
                                                            }
                                                            ValueSets = displayNames;
                                                        }
                                                        else
                                                        {
                                                            var listName = _AttributeHeader.AttributeDetails.Where(a => a.AttributeDetailID == Svalues && a.AttributeDetailName != null && a.DropDownTypeId == s.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                            if (s.DataSourceTable == "FinishedProdOrderLine" || s.DataSourceTable == "FinishedProdOrderLineProductionInProgress")
                                                            {
                                                                listName = _AttributeHeader.AttributeDetails.Where(a => a.AttributeDetailID == Svalues && a.AttributeDetailName != null && a.DropDownTypeId == s.DataSourceTable).Select(s => s.NameList).ToList();
                                                            }
                                                            ValueSets = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ValueSets = string.Empty;
                                                    }
                                                    opts.Add("Value", ValueSets);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = ValueSets;
                                                    DynamicFormReportItems dynamicFormReportItems5 = new DynamicFormReportItems();
                                                    dynamicFormReportItems5.AttrId = attrName;
                                                    dynamicFormReportItems5.Label = s.DisplayName;
                                                    dynamicFormReportItems5.Value = ValueSets;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems5);
                                                    // if (Svalues > 0)
                                                    // {
                                                    if (s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                                                    {
                                                        var attrDetails = _AttributeHeader.AttributeDetails.Where(mm => mm.AttributeID == s.AttributeId && mm.DropDownTypeId == null).ToList();
                                                        if (attrDetails != null && attrDetails.Count > 0)
                                                        {
                                                            attrDetails.ForEach(u =>
                                                            {
                                                                if (u.SubAttributeHeaders != null && u.SubAttributeHeaders.Count() > 0)
                                                                {
                                                                    loadSubHeaders(u.SubAttributeHeaders, s, jsonObj, dynamicFormReportItems, objectData, objectDataList);
                                                                }
                                                            });
                                                        }
                                                        //var attrDetails = _AttributeHeader.AttributeDetails.Where(mm => mm.AttributeDetailID == Svalues && mm.DropDownTypeId == null).FirstOrDefault()?.SubAttributeHeaders;
                                                        //if (attrDetails != null && attrDetails.Count > 0)
                                                        //{
                                                        //    SubAttrsHeader = attrDetails;
                                                        //}
                                                    }
                                                    // }
                                                    if (s.ControlType == "ComboBox" && s.IsPlantLoadDependency == true && s.AttributeHeaderDataSource.Count() > 0 && PlantDependencySubAttributeDetails != null && PlantDependencySubAttributeDetails.Count() > 0)
                                                    {
                                                        s.AttributeHeaderDataSource.ForEach(dd =>
                                                        {
                                                            var opts1 = new Dictionary<object, object>();
                                                            opts1.Add("Label", dd.DisplayName);
                                                            var nameData = s.DynamicFormSectionAttributeId + "_" + dd.AttributeHeaderDataSourceId + "_" + dd.DataSourceTable + "_Dependency";
                                                            var itemDepExits = jsonObj.ContainsKey(nameData);
                                                            if (itemDepExits == true)
                                                            {
                                                                var itemDepValue = jsonObj[nameData];
                                                                if (s.IsDependencyMultiple == true)
                                                                {
                                                                    if (itemDepValue is JArray)
                                                                    {
                                                                        List<long?> listData = itemDepValue.ToObject<List<long?>>();

                                                                        var listName = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.DropDownTypeId == dd.DataSourceTable).Select(s => s.AttributeDetailName).ToList() : new List<string?>();
                                                                        if (dd.DataSourceTable == "FinishedProdOrderLine" || dd.DataSourceTable == "FinishedProdOrderLineProductionInProgress")
                                                                        {
                                                                            listName = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.DropDownTypeId == dd.DataSourceTable).Select(s => s.NameList).ToList() : new List<string?>();
                                                                        }
                                                                        var lists = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                                        opts1.Add("Value", lists);
                                                                        objectData[nameData] = opts1;
                                                                        objectDataList[nameData + "$" + dd.DisplayName.Replace(" ", "_")] = lists;
                                                                        DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                                        dynamicFormReportItems6.AttrId = nameData;
                                                                        dynamicFormReportItems6.Label = dd.DisplayName;
                                                                        dynamicFormReportItems6.Value = lists;
                                                                        dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                                    }
                                                                    else
                                                                    {
                                                                        opts1.Add("Value", string.Empty);
                                                                        objectData[nameData] = opts1;
                                                                        objectDataList[nameData + "$" + dd.DisplayName.Replace(" ", "_")] = string.Empty;
                                                                        DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                                        dynamicFormReportItems6.AttrId = nameData;
                                                                        dynamicFormReportItems6.Label = dd.DisplayName;
                                                                        dynamicFormReportItems6.Value = string.Empty;
                                                                        dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    long? valuesDep = itemDepValue == null ? -1 : (long)itemDepValue;
                                                                    var listss = PlantDependencySubAttributeDetails.Where(v => dd.DataSourceTable == v.DropDownTypeId && v.AttributeDetailID == valuesDep).FirstOrDefault()?.AttributeDetailName;
                                                                    if (dd.DataSourceTable == "FinishedProdOrderLine" || dd.DataSourceTable == "FinishedProdOrderLineProductionInProgress")
                                                                    {
                                                                        listss = PlantDependencySubAttributeDetails.Where(v => dd.DataSourceTable == v.DropDownTypeId && v.AttributeDetailID == valuesDep).FirstOrDefault()?.NameList;
                                                                    }
                                                                    opts1.Add("Value", listss);
                                                                    objectData[nameData] = opts1;
                                                                    objectDataList[nameData + "$" + dd.DisplayName.Replace(" ", "_")] = listss;
                                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                                    dynamicFormReportItems6.AttrId = nameData;
                                                                    dynamicFormReportItems6.Label = dd.DisplayName;
                                                                    dynamicFormReportItems6.Value = listss;
                                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                                dynamicFormReportItems6.AttrId = nameData;
                                                                dynamicFormReportItems6.Label = dd.DisplayName;
                                                                dynamicFormReportItems6.Value = string.Empty;
                                                                dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                            }

                                                        });
                                                    }
                                                }
                                                else if (s.ControlType == "ListBox" && s.IsMultiple == false)
                                                {
                                                    opts.Add("Label", s.DisplayName);
                                                    var ValueSets = string.Empty;
                                                    long? Svalues = itemValue == null ? null : (long)itemValue;
                                                    if (Svalues != null)
                                                    {
                                                        var listName = _AttributeHeader.AttributeDetails.Where(a => a.AttributeDetailID == Svalues && a.DropDownTypeId == s.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                        if (s.DataSourceTable == "FinishedProdOrderLine" || s.DataSourceTable == "FinishedProdOrderLineProductionInProgress")
                                                        {
                                                            listName = _AttributeHeader.AttributeDetails.Where(a => a.AttributeDetailID == Svalues && a.DropDownTypeId == s.DataSourceTable).Select(s => s.NameList).ToList();
                                                        }
                                                        ValueSets = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                    }
                                                    else
                                                    {
                                                        ValueSets = string.Empty;
                                                    }
                                                    opts.Add("Value", ValueSets);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = ValueSets;
                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                    dynamicFormReportItems6.AttrId = attrName;
                                                    dynamicFormReportItems6.Label = s.DisplayName;
                                                    dynamicFormReportItems6.Value = ValueSets;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                }
                                                else if (s.ControlType == "DateEdit")
                                                {
                                                    DateTime? values = itemValue == null ? null : (DateTime)itemValue;
                                                    opts.Add("Label", s.DisplayName);
                                                    opts.Add("Value", values);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = values;
                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                    dynamicFormReportItems6.AttrId = attrName;
                                                    dynamicFormReportItems6.Label = s.DisplayName;
                                                    dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                }
                                                else if (s.ControlType == "TimeEdit")
                                                {
                                                    TimeSpan? values = itemValue == null ? null : (TimeSpan)itemValue;
                                                    opts.Add("Label", s.DisplayName);
                                                    opts.Add("Value", values);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = values;
                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                    dynamicFormReportItems6.AttrId = attrName;
                                                    dynamicFormReportItems6.Label = s.DisplayName;
                                                    dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                }
                                                else if (s.ControlType == "SpinEdit")
                                                {
                                                    if (s.IsSpinEditType == "decimal")
                                                    {
                                                        decimal? values = itemValue == null ? null : (decimal)itemValue;
                                                        opts.Add("Label", s.DisplayName);
                                                        opts.Add("Value", values);
                                                        objectData[attrName] = opts;
                                                        objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = values;
                                                        DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                        dynamicFormReportItems6.AttrId = attrName;
                                                        dynamicFormReportItems6.Label = s.DisplayName;
                                                        dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                    }
                                                    else
                                                    {
                                                        int? values = itemValue == null ? null : (int)itemValue;
                                                        opts.Add("Label", s.DisplayName);
                                                        opts.Add("Value", values);
                                                        objectData[attrName] = opts;
                                                        objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = values;
                                                        DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                        dynamicFormReportItems6.AttrId = attrName;
                                                        dynamicFormReportItems6.Label = s.DisplayName;
                                                        dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                    }
                                                }
                                                else if (s.ControlType == "CheckBox")
                                                {
                                                    bool? values = itemValue == null ? false : (bool)itemValue;
                                                    opts.Add("Label", s.DisplayName);
                                                    opts.Add("Value", values);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = values;
                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                    dynamicFormReportItems6.AttrId = attrName;
                                                    dynamicFormReportItems6.Label = s.DisplayName;
                                                    dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                    //if (values == true)
                                                    // {
                                                    SubAttrsHeader = s.SubAttributeHeaders;
                                                    // }
                                                    loadSubHeaders(SubAttrsHeader, s, jsonObj, dynamicFormReportItems, objectData, objectDataList);
                                                }
                                                else
                                                {
                                                    opts.Add("Label", s.DisplayName);
                                                    opts.Add("Value", (string)itemValue);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = (string)itemValue;
                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                    dynamicFormReportItems6.AttrId = attrName;
                                                    dynamicFormReportItems6.Label = s.DisplayName;
                                                    dynamicFormReportItems6.Value = (string)itemValue;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                }


                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    if (s.ControlTypeId == 2712)
                                    {
                                        var url = "?DynamicFormSessionId=" + (_dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId) + "&&DynamicFormDataSessionId=" + r.SessionId + "&&DynamicFormDataGridSessionId=" + s.DynamicFormSessionId + "&&DynamicFormSectionGridAttributeSessionId=" + s.SessionId;

                                        opts.Add("Label", s.DisplayName);
                                        opts.Add("Value", s.DynamicFormSessionId);
                                        opts.Add("IsGrid", true);
                                        opts.Add("DynamicFormSessionId", _dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId);
                                        opts.Add("DynamicFormDataSessionId", r.SessionId);
                                        opts.Add("DynamicFormDataGridSessionId", s.DynamicFormSessionId);
                                        opts.Add("DynamicFormSectionGridAttributeSessionId", s.SessionId);
                                        opts.Add("DynamicGridFormId", _dynamicForm.ID);
                                        opts.Add("DynamicGridFormDataId", r.DynamicFormDataId);
                                        opts.Add("DynamicGridFormDataGridId", s.DynamicFormGridDropDownId);
                                        opts.Add("DynamicFormSectionGridAttributeId", s.DynamicFormSectionAttributeId);
                                        opts.Add("Url", url);
                                        objectData[attrName] = opts;
                                        objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = s.DynamicFormSessionId;
                                        DynamicFormReportItems dynamicFormReportItems1 = new DynamicFormReportItems();
                                        dynamicFormReportItems1.AttrId = attrName;
                                        dynamicFormReportItems1.Label = s.DisplayName;
                                        dynamicFormReportItems1.IsGrid = true;
                                        dynamicFormReportItems1.DynamicFormSessionId = _dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId;
                                        dynamicFormReportItems1.DynamicFormDataSessionId = r.SessionId;
                                        dynamicFormReportItems1.DynamicFormDataGridSessionId = s.DynamicFormSessionId;
                                        dynamicFormReportItems1.DynamicGridFormId = _dynamicForm.ID;
                                        dynamicFormReportItems1.DynamicGridFormDataId = r.DynamicFormDataId;
                                        dynamicFormReportItems1.DynamicGridFormDataGridId = s.DynamicFormGridDropDownId;
                                        dynamicFormReportItems1.Url = BasUrl + url;
                                        dynamicFormReportItems1.DynamicFormSectionGridAttributeId = s.DynamicFormSectionAttributeId;
                                        dynamicFormReportItems1.DynamicFormSectionGridAttributeSessionId = s.SessionId;
                                        dynamicFormReportItems.Add(dynamicFormReportItems1);
                                    }
                                    else
                                    {
                                        if (s.ControlType == "ComboBox" && s.IsPlantLoadDependency == true && s.AttributeHeaderDataSource.Count() > 0)
                                        {
                                            opts.Add("Label", s.DisplayName);
                                            opts.Add("Value", string.Empty);
                                            objectData[attrName] = opts;
                                            objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                            DynamicFormReportItems dynamicFormReportItems5 = new DynamicFormReportItems();
                                            dynamicFormReportItems5.AttrId = attrName;
                                            dynamicFormReportItems5.Label = s.DisplayName;
                                            dynamicFormReportItems5.Value = string.Empty;
                                            dynamicFormReportItems.Add(dynamicFormReportItems5);
                                            s.AttributeHeaderDataSource.ForEach(dd =>
                                            {
                                                var opts1 = new Dictionary<object, object>();
                                                opts1.Add("Label", dd.DisplayName);
                                                var nameData = s.DynamicFormSectionAttributeId + "_" + dd.AttributeHeaderDataSourceId + "_" + dd.DataSourceTable + "_Dependency";
                                                opts1.Add("Value", string.Empty);
                                                objectData[nameData] = opts1;
                                                objectDataList[nameData + "$" + dd.DisplayName.Replace(" ", "_")] = string.Empty;
                                                DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                                dynamicFormReportItems55.AttrId = nameData;
                                                dynamicFormReportItems55.Label = dd.DisplayName;
                                                dynamicFormReportItems55.Value = string.Empty;
                                                dynamicFormReportItems.Add(dynamicFormReportItems55);
                                            });

                                        }
                                        else
                                        {
                                            if (s.DataSourceTable == "ApplicationMasterParent")
                                            {
                                                opts.Add("Label", s.DisplayName);
                                                opts.Add("Value", string.Empty);
                                                objectData[attrName] = opts;
                                                objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                                DynamicFormReportItems dynamicFormReportItems56 = new DynamicFormReportItems();
                                                dynamicFormReportItems56.AttrId = attrName;
                                                dynamicFormReportItems56.Label = s.DisplayName;
                                                dynamicFormReportItems56.Value = string.Empty;
                                                dynamicFormReportItems.Add(dynamicFormReportItems56);
                                                List<ApplicationMasterParent> nameDatas = new List<ApplicationMasterParent>();
                                                s.ApplicationMasterParents.ForEach(ab =>
                                                {
                                                    nameDatas.Add(ab);
                                                    RemoveApplicationMasterParentSingleNameItemApi(ab, s, nameDatas, _AttributeHeader.ApplicationMasterParent);
                                                    if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMasterParent" && s.IsDynamicFormGridDropdown == true)
                                                    {
                                                        var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                                        var opts1 = new Dictionary<object, object>();
                                                        opts1.Add("Label", s.GridDropDownDynamicFormName);
                                                        opts1.Add("Value", string.Empty);
                                                        objectData[appendDependency] = opts1;
                                                        objectDataList[appendDependency + "$" + s.GridDropDownDynamicFormName.Replace(" ", "_")] = string.Empty;
                                                        DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                                        dynamicFormReportItems55.AttrId = appendDependency;
                                                        dynamicFormReportItems55.Label = s.GridDropDownDynamicFormName;
                                                        dynamicFormReportItems55.Value = string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems55);
                                                    }
                                                });
                                                if (nameDatas != null && nameDatas.Count() > 0)
                                                {
                                                    nameDatas.ForEach(n =>
                                                    {
                                                        var opts1 = new Dictionary<object, object>();
                                                        opts1.Add("Label", n.ApplicationMasterName);
                                                        var nameData = s.DynamicFormSectionAttributeId + "_" + n.ApplicationMasterParentCodeId + "_AppMasterPar";
                                                        opts1.Add("Value", string.Empty);
                                                        objectData[nameData] = opts1;
                                                        objectDataList[nameData + "$" + n.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                                        DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                                        dynamicFormReportItems55.AttrId = nameData;
                                                        dynamicFormReportItems55.Label = n.ApplicationMasterName;
                                                        dynamicFormReportItems55.Value = string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems55);
                                                    });
                                                }

                                            }
                                            else if (s.DataSourceTable == "ApplicationMaster")
                                            {
                                                opts.Add("Label", s.DisplayName);
                                                opts.Add("Value", string.Empty);
                                                objectData[attrName] = opts;
                                                objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                                DynamicFormReportItems dynamicFormReportItems57 = new DynamicFormReportItems();
                                                dynamicFormReportItems57.AttrId = attrName;
                                                dynamicFormReportItems57.Label = s.DisplayName;
                                                dynamicFormReportItems57.Value = string.Empty;
                                                dynamicFormReportItems.Add(dynamicFormReportItems57);
                                                if (s.ApplicationMaster.Count() > 0)
                                                {
                                                    s.ApplicationMaster.ForEach(ab =>
                                                    {
                                                        var opts1 = new Dictionary<object, object>();
                                                        opts1.Add("Label", ab.ApplicationMasterName);
                                                        var nameData = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_AppMaster";
                                                        opts1.Add("Value", string.Empty);
                                                        objectData[nameData] = opts1;
                                                        objectDataList[attrName + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                                        DynamicFormReportItems dynamicFormReportItems51 = new DynamicFormReportItems();
                                                        dynamicFormReportItems51.AttrId = nameData;
                                                        dynamicFormReportItems51.Label = ab.ApplicationMasterName;
                                                        dynamicFormReportItems51.Value = string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems51);
                                                        if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMaster" && s.IsDynamicFormGridDropdown == true)
                                                        {
                                                            var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                                            var opts12 = new Dictionary<object, object>();
                                                            opts12.Add("Label", s.GridDropDownDynamicFormName);
                                                            opts12.Add("Value", string.Empty);
                                                            objectData[appendDependency] = opts12;
                                                            objectDataList[appendDependency + "$" + s.GridDropDownDynamicFormName.Replace(" ", "_")] = string.Empty;
                                                            DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                                            dynamicFormReportItems55.AttrId = appendDependency;
                                                            dynamicFormReportItems55.Label = s.GridDropDownDynamicFormName;
                                                            dynamicFormReportItems55.Value = string.Empty;
                                                            dynamicFormReportItems.Add(dynamicFormReportItems55);
                                                        }
                                                    });
                                                }

                                            }
                                            else if (s.ControlType == "CheckBox" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                                            {
                                                opts.Add("Label", s.DisplayName);
                                                opts.Add("Value", s.ControlType == "CheckBox" ? false : string.Empty);
                                                objectData[attrName] = opts;
                                                objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = s.ControlType == "CheckBox" ? false : string.Empty;
                                                DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                                dynamicFormReportItems58.AttrId = attrName;
                                                dynamicFormReportItems58.Label = s.DisplayName;
                                                dynamicFormReportItems58.Value = string.Empty;
                                                dynamicFormReportItems.Add(dynamicFormReportItems58);
                                                if (s.ControlType == "CheckBox")
                                                {
                                                    loadSubHeaders(s.SubAttributeHeaders, s, jsonObj, dynamicFormReportItems, objectData, objectDataList);
                                                }
                                                else
                                                {
                                                    var attrDetails = _AttributeHeader.AttributeDetails.Where(mm => mm.AttributeID == s.AttributeId && mm.DropDownTypeId == null).ToList();
                                                    if (attrDetails != null && attrDetails.Count > 0)
                                                    {
                                                        attrDetails.ForEach(u =>
                                                        {
                                                            if (u.SubAttributeHeaders != null && u.SubAttributeHeaders.Count() > 0)
                                                            {
                                                                loadSubHeaders(u.SubAttributeHeaders, s, jsonObj, dynamicFormReportItems, objectData, objectDataList);
                                                            }
                                                        });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                opts.Add("Label", s.DisplayName);
                                                opts.Add("Value", string.Empty);
                                                objectData[attrName] = opts;
                                                objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                                DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                                dynamicFormReportItems58.AttrId = attrName;
                                                dynamicFormReportItems58.Label = s.DisplayName;
                                                dynamicFormReportItems58.Value = string.Empty;
                                                dynamicFormReportItems.Add(dynamicFormReportItems58);
                                            }
                                        }
                                    }
                                }


                            });
                        }
                        list.Add(objectData);
                        //dynamicFormData.ObjectDataList = list;
                        dynamicFormData.ObjectDataItems = objectDataList;
                        listItems.Add(objectDataList);
                        dynamicFormData.DynamicFormReportItems = dynamicFormReportItems;
                        _dynamicformObjectDataList.Add(dynamicFormData);
                    });
                }

            }
            return _dynamicformObjectDataList;
        }
        public async Task<List<DynamicFormDataResponse>> GetAllDynamicFormApiAsync(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId, string? BasUrl, bool? IsAll)
        {
            var _dynamicformObjectDataList = new List<DynamicFormDataResponse>();
            try
            {
                _dynamicformObjectDataList = await GetAllDynamicFormAllApiAsync(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId, BasUrl, IsAll);
                return _dynamicformObjectDataList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }


        }
        public async Task<IReadOnlyList<ExpandoObject>> GetAllDataObjectDynamicFormApiAsync(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId, string? BasUrl, bool? IsAll)
        {
            var _dynamicformObjectDataList = new List<ExpandoObject>();
            try
            {
                var _dynamicformObjectDataLists = await GetAllDynamicFormAllApiAsync(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId, BasUrl, IsAll);
                if(_dynamicformObjectDataLists!=null && _dynamicformObjectDataLists.Count()>0)
                {
                     _dynamicformObjectDataList = _dynamicformObjectDataLists.Select(s => (ExpandoObject)s.ObjectDataItems).ToList();
                }
                return _dynamicformObjectDataList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }


        }
        async Task<List<DynamicFormData>> UpdateDataAsync(DynamicForm _dynamicForm, DynamicForm? _dynamicFormGrids, DynamicFormData? dynamicFormDatas, DynamicFormSectionAttribute? dynamicFormSectionAttribute, string? BasUrl)
        {
            var _dynamicformObjectDataList = new List<DynamicFormData>();
            AttributeHeaderListModel _AttributeHeader = new AttributeHeaderListModel();
            if (_dynamicFormGrids != null && _dynamicFormGrids.ID > 0 && dynamicFormDatas != null && dynamicFormDatas.DynamicFormDataId > 0)
            {
                _AttributeHeader = await GetAllAttributeNameAsync(_dynamicFormGrids, 1, false);
            }
            else
            {
                _AttributeHeader = await GetAllAttributeNameAsync(_dynamicForm, 1, false);
            }
            if (_AttributeHeader != null && _AttributeHeader.DynamicFormSectionAttribute != null && _AttributeHeader.DynamicFormSectionAttribute.Count > 0)
            {
                List<string?> dataSourceTableIds = new List<string?>() { "Plant" };
                var DataSourceTablelists = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.AttributeHeaderDataSource.Count > 0).SelectMany(a => a.AttributeHeaderDataSource.Select(s => s.DataSourceTable)).ToList().Distinct().ToList();
                dataSourceTableIds.AddRange(DataSourceTablelists); List<long?> ApplicationMasterIds = new List<long?>(); List<long?> ApplicationMasterParentIds = new List<long?>();
                ApplicationMasterIds = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.ApplicationMaster.Count > 0).SelectMany(a => a.ApplicationMaster.Select(s => (long?)s.ApplicationMasterId)).ToList().Distinct().ToList();
                ApplicationMasterParentIds = _AttributeHeader.DynamicFormSectionAttribute.Where(w => w.ApplicationMasterParents.Count > 0).SelectMany(a => a.ApplicationMasterParents.Select(s => (long?)s.ApplicationMasterParentCodeId)).ToList().Distinct().ToList();
                if (ApplicationMasterIds.Count > 0)
                {
                    dataSourceTableIds.Add("ApplicationMaster");
                }
                if (ApplicationMasterParentIds != null && ApplicationMasterParentIds.Count > 0)
                {
                    dataSourceTableIds.Add("ApplicationMasterParent");
                }
                var PlantDependencySubAttributeDetails = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(null, dataSourceTableIds, null, ApplicationMasterIds, ApplicationMasterParentIds != null ? ApplicationMasterParentIds : new List<long?>());

                var _dynamicformDataList = new List<DynamicFormData>();
                if (_dynamicFormGrids != null && _dynamicFormGrids.ID > 0 && dynamicFormDatas != null && dynamicFormDatas.DynamicFormDataId > 0)
                {
                    long? dynamicFormSectionAttributeId = dynamicFormSectionAttribute != null && dynamicFormSectionAttribute.DynamicFormSectionAttributeId > 0 ? dynamicFormSectionAttribute.DynamicFormSectionAttributeId : null;
                    var _dynamicformDataLists = await _dynamicFormQueryRepository.GetDynamicFormDataByIdAsync(_dynamicFormGrids.ID, 0, dynamicFormDatas.DynamicFormDataId, dynamicFormSectionAttributeId);
                    _dynamicformDataList = _dynamicformDataLists != null ? _dynamicformDataLists.ToList() : new List<DynamicFormData>();
                }
                else
                {
                    var _dynamicformDataLists = await _dynamicFormQueryRepository.GetDynamicFormDataByIdAsync(_dynamicForm.ID, 0, -1, null);
                    _dynamicformDataList = _dynamicformDataLists != null ? _dynamicformDataLists.ToList() : new List<DynamicFormData>();
                }
                if (_dynamicformDataList != null && _dynamicformDataList.Count > 0)
                {
                    _dynamicformDataList.ToList().ForEach(r =>
                    {
                        DynamicFormData dynamicFormData = new DynamicFormData();
                        dynamic jsonObj = new object();
                        if (IsValidJson(r.DynamicFormItem))
                        {
                            jsonObj = JsonConvert.DeserializeObject(r.DynamicFormItem);
                        }

                        IDictionary<string, object> objectDataList = new ExpandoObject();
                        IDictionary<string, object> objectData = new ExpandoObject();
                        List<object> list = new List<object>();
                        List<DynamicFormReportItems> dynamicFormReportItems = new List<DynamicFormReportItems>();
                        dynamicFormData.SortOrderByNo = r.SortOrderByNo;
                        dynamicFormData.DynamicFormDataId = r.DynamicFormDataId;
                        dynamicFormData.ProfileNo = r.ProfileNo;
                        dynamicFormData.SessionId = r.SessionId;
                        dynamicFormData.ScreenID = r.ScreenID;
                        dynamicFormData.DynamicFormId = r.DynamicFormId;
                        dynamicFormData.Name = r.Name;
                        dynamicFormData.ModifiedBy = r.ModifiedBy;
                        dynamicFormData.ModifiedDate = r.ModifiedDate;
                        dynamicFormData.CurrentUserName = r.CurrentUserName;
                        dynamicFormData.StatusName = r.StatusName;
                        dynamicFormData.DynamicFormDataGridId = r.DynamicFormDataGridId;
                        dynamicFormData.IsDynamicFormDataGrid = r.IsDynamicFormDataGrid;
                        dynamicFormData.IsFileprofileTypeDocument = r.IsFileprofileTypeDocument;
                        dynamicFormData.DynamicFormSectionGridAttributeId = r.DynamicFormSectionGridAttributeId;
                        dynamicFormData.DynamicFormSectionGridAttributeSessionId = r.DynamicFormSectionGridAttributeSessionId;
                        dynamicFormData.IsDraft = r.IsDraft;
                        if (_AttributeHeader != null && _AttributeHeader.DynamicFormSectionAttribute != null && _AttributeHeader.DynamicFormSectionAttribute.Count > 0)
                        {
                            _AttributeHeader.DynamicFormSectionAttribute.ForEach(s =>
                            {

                                string attrName = s.DynamicAttributeName;
                                var opts = new Dictionary<object, object>();

                                var Names = jsonObj.ContainsKey(attrName);
                                if (Names == true)
                                {
                                    if (s.ControlTypeId == 2712)
                                    {
                                        var url = "?DynamicFormSessionId=" + (_dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId) + "&&DynamicFormDataSessionId=" + r.SessionId + "&&DynamicFormDataGridSessionId=" + s.DynamicFormSessionId + "&&DynamicFormSectionGridAttributeSessionId=" + s.SessionId;
                                        opts.Add("Label", s.DisplayName);
                                        opts.Add("Value", s.DynamicFormSessionId);
                                        opts.Add("IsGrid", true);
                                        opts.Add("DynamicFormSessionId", _dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId);
                                        opts.Add("DynamicFormDataSessionId", r.SessionId);
                                        opts.Add("DynamicFormDataGridSessionId", s.DynamicFormSessionId);
                                        opts.Add("DynamicFormSectionGridAttributeSessionId", s.SessionId);
                                        opts.Add("DynamicGridFormId", _dynamicForm.ID);
                                        opts.Add("DynamicGridFormDataId", r.DynamicFormDataId);
                                        opts.Add("DynamicGridFormDataGridId", s.DynamicFormGridDropDownId);
                                        opts.Add("DynamicFormSectionGridAttributeId", s.DynamicFormSectionAttributeId);
                                        opts.Add("Url", url);
                                        objectData[attrName] = opts;
                                        objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = s.DynamicFormSessionId;
                                        DynamicFormReportItems dynamicFormReportItems1 = new DynamicFormReportItems();
                                        dynamicFormReportItems1.AttrId = attrName;
                                        dynamicFormReportItems1.Label = s.DisplayName;
                                        dynamicFormReportItems1.IsGrid = true;
                                        dynamicFormReportItems1.DynamicFormSessionId = _dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId;
                                        dynamicFormReportItems1.DynamicFormDataSessionId = r.SessionId;
                                        dynamicFormReportItems1.DynamicFormDataGridSessionId = s.DynamicFormSessionId;
                                        dynamicFormReportItems1.DynamicGridFormDataGridId = s.DynamicFormGridDropDownId;
                                        dynamicFormReportItems1.Url = BasUrl + url;
                                        dynamicFormReportItems1.DynamicGridFormId = _dynamicForm.ID;
                                        dynamicFormReportItems1.DynamicGridFormDataId = r.DynamicFormDataId;
                                        dynamicFormReportItems1.DynamicFormSectionGridAttributeId = s.DynamicFormSectionAttributeId;
                                        dynamicFormReportItems1.DynamicFormSectionGridAttributeSessionId = s.SessionId;
                                        dynamicFormReportItems.Add(dynamicFormReportItems1);
                                    }
                                    else
                                    {
                                        if (s.DataSourceTable == "ApplicationMaster")
                                        {
                                            opts.Add("Label", s.DisplayName);
                                            opts.Add("Value", string.Empty);
                                            objectData[attrName] = opts;
                                            objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                            DynamicFormReportItems dynamicFormReportItems2 = new DynamicFormReportItems();
                                            dynamicFormReportItems2.AttrId = attrName;
                                            dynamicFormReportItems2.Label = s.DisplayName;
                                            dynamicFormReportItems2.Value = string.Empty;
                                            dynamicFormReportItems.Add(dynamicFormReportItems2);
                                            if (s.ApplicationMaster != null && s.ApplicationMaster.Count() > 0)
                                            {
                                                s.ApplicationMaster.ForEach(ab =>
                                                {
                                                    var opts1 = new Dictionary<object, object>();
                                                    DynamicFormReportItems dynamicFormReportItems3 = new DynamicFormReportItems();
                                                    opts1.Add("Label", ab.ApplicationMasterName);
                                                    var nameData = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_AppMaster";
                                                    var SubNamess = jsonObj.ContainsKey(nameData);
                                                    if (SubNamess == true)
                                                    {
                                                        var itemValue = jsonObj[nameData];
                                                        if (itemValue is JArray)
                                                        {
                                                            List<long?> listData = itemValue.ToObject<List<long?>>();
                                                            if (s.IsMultiple == true || s.ControlType == "TagBox")
                                                            {
                                                                var listName = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.ApplicationMasterId == ab.ApplicationMasterId && a.DropDownTypeId == s.DataSourceTable).Select(s => s.AttributeDetailName).ToList() : new List<string?>();
                                                                var lists = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                                opts1.Add("Value", lists);
                                                                objectData[nameData] = opts1;
                                                                objectDataList[nameData + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = lists;
                                                                dynamicFormReportItems3.AttrId = nameData;
                                                                dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                                dynamicFormReportItems3.Value = lists;
                                                                dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                            }
                                                            else
                                                            {
                                                                opts1.Add("Value", string.Empty);
                                                                objectData[nameData] = opts1;
                                                                objectDataList[nameData + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                                                dynamicFormReportItems3.AttrId = nameData;
                                                                dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                                dynamicFormReportItems3.Value = string.Empty;
                                                                dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (s.ControlType == "ComboBox")
                                                            {
                                                                long? values = itemValue == null ? -1 : (long)itemValue;
                                                                var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                                                                opts1.Add("Value", listss);
                                                                objectData[nameData] = opts1;
                                                                objectDataList[nameData + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = listss;
                                                                dynamicFormReportItems3.AttrId = nameData;
                                                                dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                                dynamicFormReportItems3.Value = listss;
                                                                dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                            }
                                                            else
                                                            {
                                                                if (s.ControlType == "ListBox" && s.IsMultiple == false)
                                                                {
                                                                    long? values = itemValue == null ? -1 : (long)itemValue;
                                                                    var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                                                                    opts1.Add("Value", listss);
                                                                    objectData[nameData] = opts1;
                                                                    objectDataList[nameData + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = listss;
                                                                    dynamicFormReportItems3.AttrId = nameData;
                                                                    dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                                    dynamicFormReportItems3.Value = listss;
                                                                    dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                                }
                                                                else
                                                                {
                                                                    opts1.Add("Value", string.Empty);
                                                                    objectData[nameData] = opts1;
                                                                    objectDataList[nameData + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                                                    dynamicFormReportItems3.AttrId = nameData;
                                                                    dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                                    dynamicFormReportItems3.Value = string.Empty;
                                                                    dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                                }
                                                            }
                                                        }
                                                        if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMaster" && s.IsDynamicFormGridDropdown == true)
                                                        {
                                                            var collection = _AttributeHeader.DropDownOptionsGridListModel.ObjectData;
                                                            var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                                            var displayNames = string.Empty;
                                                            if (collection != null)
                                                            {
                                                                var itemDepExits = jsonObj.ContainsKey(appendDependency);
                                                                if (itemDepExits == true)
                                                                {
                                                                    var itemDepValue = jsonObj[appendDependency];
                                                                    if (s.IsDynamicFormGridDropdownMultiple == true)
                                                                    {
                                                                        if (itemDepValue is JArray)
                                                                        {
                                                                            List<long?> listData = itemDepValue.ToObject<List<long?>>();

                                                                            if (listData != null && listData.Count > 0)
                                                                            {
                                                                                listData.ForEach(l =>
                                                                                {
                                                                                    if (collection != null)
                                                                                    {
                                                                                        foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                                        {
                                                                                            dynamic eod = v;
                                                                                            if ((long?)eod.AttributeDetailID == l)
                                                                                            {
                                                                                                displayNames += eod.AttributeDetailName + ",";
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                });
                                                                            }
                                                                            displayNames = displayNames.TrimEnd(',');

                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        long? valuesDep = itemDepValue == null ? -1 : (long)itemDepValue;
                                                                        foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                        {
                                                                            dynamic eod = v;
                                                                            if ((long?)eod.AttributeDetailID == valuesDep)
                                                                            {
                                                                                displayNames += eod.AttributeDetailName;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            var opts12 = new Dictionary<object, object>();
                                                            opts12.Add("Label", s.GridDropDownDynamicFormName);
                                                            opts12.Add("Value", displayNames);
                                                            objectData[appendDependency] = opts12;
                                                            objectDataList[appendDependency + "$" + s.GridDropDownDynamicFormName.Replace(" ", "_")] = displayNames;
                                                            DynamicFormReportItems dynamicFormReportItems4 = new DynamicFormReportItems();
                                                            dynamicFormReportItems4.AttrId = appendDependency;
                                                            dynamicFormReportItems4.Label = s.GridDropDownDynamicFormName;
                                                            dynamicFormReportItems4.Value = displayNames;
                                                            dynamicFormReportItems.Add(dynamicFormReportItems4);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dynamicFormReportItems3.AttrId = nameData;
                                                        dynamicFormReportItems3.Label = ab.ApplicationMasterName;
                                                        dynamicFormReportItems3.Value = string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems3);
                                                    }

                                                });
                                            }

                                        }
                                        else if (s.DataSourceTable == "ApplicationMasterParent")
                                        {
                                            opts.Add("Label", s.DisplayName);
                                            opts.Add("Value", string.Empty);
                                            objectData[attrName] = opts;
                                            objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                            DynamicFormReportItems dynamicFormReportItems21 = new DynamicFormReportItems();
                                            dynamicFormReportItems21.AttrId = attrName;
                                            dynamicFormReportItems21.Label = s.DisplayName;
                                            dynamicFormReportItems21.Value = string.Empty;
                                            dynamicFormReportItems.Add(dynamicFormReportItems21);
                                            List<ApplicationMasterParent> nameDatas = new List<ApplicationMasterParent>();
                                            s.ApplicationMasterParents.ForEach(ab =>
                                            {
                                                nameDatas.Add(ab);
                                                RemoveApplicationMasterParentSingleNameItemApi(ab, s, nameDatas, _AttributeHeader.ApplicationMasterParent);

                                                if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMasterParent" && s.IsDynamicFormGridDropdown == true)
                                                {
                                                    var collection = _AttributeHeader.DropDownOptionsGridListModel.ObjectData;
                                                    var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                                    var displayNames = string.Empty;
                                                    if (collection != null)
                                                    {
                                                        var itemDepExits = jsonObj.ContainsKey(appendDependency);
                                                        if (itemDepExits == true)
                                                        {
                                                            var itemDepValue = jsonObj[appendDependency];
                                                            if (s.IsDynamicFormGridDropdownMultiple == true)
                                                            {
                                                                if (itemDepValue is JArray)
                                                                {
                                                                    List<long?> listData = itemDepValue.ToObject<List<long?>>();

                                                                    if (listData != null && listData.Count > 0)
                                                                    {
                                                                        listData.ForEach(l =>
                                                                        {
                                                                            if (collection != null)
                                                                            {
                                                                                foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                                {
                                                                                    dynamic eod = v;
                                                                                    if ((long?)eod.AttributeDetailID == l)
                                                                                    {
                                                                                        displayNames += eod.AttributeDetailName + ",";
                                                                                    }
                                                                                }
                                                                            }
                                                                        });
                                                                    }
                                                                    displayNames = displayNames.TrimEnd(',');

                                                                }
                                                            }
                                                            else
                                                            {
                                                                long? valuesDep = itemDepValue == null ? -1 : (long)itemDepValue;
                                                                foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                {
                                                                    dynamic eod = v;
                                                                    if ((long?)eod.AttributeDetailID == valuesDep)
                                                                    {
                                                                        displayNames += eod.AttributeDetailName;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    var opts12 = new Dictionary<object, object>();
                                                    opts12.Add("Label", s.GridDropDownDynamicFormName);
                                                    opts12.Add("Value", displayNames);
                                                    objectData[appendDependency] = opts12;
                                                    objectDataList[appendDependency + "$" + s.GridDropDownDynamicFormName.Replace(" ", "_")] = displayNames;
                                                    DynamicFormReportItems dynamicFormReportItems4 = new DynamicFormReportItems();
                                                    dynamicFormReportItems4.AttrId = appendDependency;
                                                    dynamicFormReportItems4.Label = s.GridDropDownDynamicFormName;
                                                    dynamicFormReportItems4.Value = displayNames;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems4);
                                                }

                                            });
                                            if (nameDatas != null && nameDatas.Count() > 0)
                                            {
                                                nameDatas.ForEach(n =>
                                                {
                                                    var opts1 = new Dictionary<object, object>();
                                                    opts1.Add("Label", n.ApplicationMasterName);
                                                    var nameDataPar = s.DynamicFormSectionAttributeId + "_" + n.ApplicationMasterParentCodeId + "_AppMasterPar";
                                                    DynamicFormReportItems dynamicFormReportItems22 = new DynamicFormReportItems();
                                                    dynamicFormReportItems22.AttrId = nameDataPar;
                                                    dynamicFormReportItems22.Label = n.ApplicationMasterName;
                                                    loadApplicationMasterParentDataApi(jsonObj, s, nameDataPar, opts1, PlantDependencySubAttributeDetails, objectDataList, n.ApplicationMasterName, dynamicFormReportItems22);
                                                    objectData[nameDataPar] = opts1;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems22);
                                                });
                                            }

                                        }
                                        else
                                        {
                                            var itemValue = jsonObj[attrName];
                                            var collection = _AttributeHeader.DropDownOptionsGridListModel.ObjectData;

                                            var ValueSet = string.Empty;
                                            if (itemValue is JArray)
                                            {
                                                List<long?> listData = itemValue.ToObject<List<long?>>();
                                                if (s.DataSourceTable == "DynamicGrid")
                                                {
                                                    string displayNames = string.Empty;

                                                    if (listData != null && listData.Count > 0)
                                                    {
                                                        listData.ForEach(l =>
                                                        {
                                                            if (collection != null)
                                                            {
                                                                foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                {
                                                                    dynamic eod = v;
                                                                    if ((long?)eod.AttributeDetailID == l)
                                                                    {
                                                                        displayNames += eod.AttributeDetailName + ",";
                                                                    }
                                                                }
                                                            }
                                                        });
                                                    }

                                                    ValueSet = displayNames.TrimEnd(',');
                                                }
                                                else
                                                {
                                                    var listName = _AttributeHeader.AttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.DropDownTypeId == s.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                    ValueSet = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;

                                                }
                                                opts.Add("Label", s.DisplayName);
                                                opts.Add("Value", ValueSet);
                                                objectData[attrName] = opts;
                                                objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = ValueSet;
                                                DynamicFormReportItems dynamicFormReportItems4 = new DynamicFormReportItems();
                                                dynamicFormReportItems4.AttrId = attrName;
                                                dynamicFormReportItems4.Label = s.DisplayName;
                                                dynamicFormReportItems4.Value = ValueSet;
                                                dynamicFormReportItems.Add(dynamicFormReportItems4);
                                            }
                                            else
                                            {
                                                List<AttributeHeader> SubAttrsHeader = new List<AttributeHeader>();
                                                if (s.ControlType == "ComboBox" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                                                {
                                                    opts.Add("Label", s.DisplayName);
                                                    var ValueSets = string.Empty;
                                                    long? Svalues = itemValue == null ? null : (long)itemValue;
                                                    if (Svalues != null)
                                                    {
                                                        if (s.DataSourceTable == "DynamicGrid")
                                                        {
                                                            var displayNames = string.Empty;
                                                            if (collection != null)
                                                            {
                                                                foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                                {
                                                                    dynamic eod = v;
                                                                    if ((long?)eod.AttributeDetailID == Svalues)
                                                                    {
                                                                        displayNames += eod.AttributeDetailName;
                                                                    }
                                                                }
                                                            }
                                                            ValueSets = displayNames;
                                                        }
                                                        else
                                                        {
                                                            var listName = _AttributeHeader.AttributeDetails.Where(a => a.AttributeDetailID == Svalues && a.AttributeDetailName != null && a.DropDownTypeId == s.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                            ValueSets = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ValueSets = string.Empty;
                                                    }
                                                    opts.Add("Value", ValueSets);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = ValueSets;
                                                    DynamicFormReportItems dynamicFormReportItems5 = new DynamicFormReportItems();
                                                    dynamicFormReportItems5.AttrId = attrName;
                                                    dynamicFormReportItems5.Label = s.DisplayName;
                                                    dynamicFormReportItems5.Value = ValueSets;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems5);
                                                    if (Svalues > 0)
                                                    {
                                                        var attrDetails = _AttributeHeader.AttributeDetails.Where(mm => mm.AttributeDetailID == Svalues && mm.DropDownTypeId == null).FirstOrDefault()?.SubAttributeHeaders;
                                                        if (attrDetails != null && attrDetails.Count > 0)
                                                        {
                                                            SubAttrsHeader = attrDetails;
                                                        }
                                                    }
                                                    if (s.ControlType == "ComboBox" && s.IsPlantLoadDependency == true && s.AttributeHeaderDataSource.Count() > 0 && PlantDependencySubAttributeDetails != null && PlantDependencySubAttributeDetails.Count() > 0)
                                                    {
                                                        s.AttributeHeaderDataSource.ForEach(dd =>
                                                        {
                                                            var opts1 = new Dictionary<object, object>();
                                                            opts1.Add("Label", dd.DisplayName);
                                                            var nameData = s.DynamicFormSectionAttributeId + "_" + dd.AttributeHeaderDataSourceId + "_" + dd.DataSourceTable + "_Dependency";
                                                            var itemDepExits = jsonObj.ContainsKey(nameData);
                                                            if (itemDepExits == true)
                                                            {
                                                                var itemDepValue = jsonObj[nameData];
                                                                if (s.IsDependencyMultiple == true)
                                                                {
                                                                    if (itemDepValue is JArray)
                                                                    {
                                                                        List<long?> listData = itemDepValue.ToObject<List<long?>>();

                                                                        var listName = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.DropDownTypeId == dd.DataSourceTable).Select(s => s.AttributeDetailName).ToList() : new List<string?>();
                                                                        var lists = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                                        opts1.Add("Value", lists);
                                                                        objectData[nameData] = opts1;
                                                                        objectDataList[nameData + "$" + dd.DisplayName.Replace(" ", "_")] = lists;
                                                                        DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                                        dynamicFormReportItems6.AttrId = nameData;
                                                                        dynamicFormReportItems6.Label = dd.DisplayName;
                                                                        dynamicFormReportItems6.Value = lists;
                                                                        dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                                    }
                                                                    else
                                                                    {
                                                                        opts1.Add("Value", string.Empty);
                                                                        objectData[nameData] = opts1;
                                                                        objectDataList[nameData + "$" + dd.DisplayName.Replace(" ", "_")] = string.Empty;
                                                                        DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                                        dynamicFormReportItems6.AttrId = nameData;
                                                                        dynamicFormReportItems6.Label = dd.DisplayName;
                                                                        dynamicFormReportItems6.Value = string.Empty;
                                                                        dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    long? valuesDep = itemDepValue == null ? -1 : (long)itemDepValue;
                                                                    var listss = PlantDependencySubAttributeDetails.Where(v => dd.DataSourceTable == v.DropDownTypeId && v.AttributeDetailID == valuesDep).FirstOrDefault()?.AttributeDetailName;
                                                                    opts1.Add("Value", listss);
                                                                    objectData[nameData] = opts1;
                                                                    objectDataList[nameData + "$" + dd.DisplayName.Replace(" ", "_")] = listss;
                                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                                    dynamicFormReportItems6.AttrId = nameData;
                                                                    dynamicFormReportItems6.Label = dd.DisplayName;
                                                                    dynamicFormReportItems6.Value = listss;
                                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                                dynamicFormReportItems6.AttrId = nameData;
                                                                dynamicFormReportItems6.Label = dd.DisplayName;
                                                                dynamicFormReportItems6.Value = string.Empty;
                                                                dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                            }

                                                        });
                                                    }
                                                }
                                                else if (s.ControlType == "ListBox" && s.IsMultiple == false)
                                                {
                                                    opts.Add("Label", s.DisplayName);
                                                    var ValueSets = string.Empty;
                                                    long? Svalues = itemValue == null ? null : (long)itemValue;
                                                    if (Svalues != null)
                                                    {
                                                        var listName = _AttributeHeader.AttributeDetails.Where(a => a.AttributeDetailID == Svalues && a.DropDownTypeId == s.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                        ValueSets = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                    }
                                                    else
                                                    {
                                                        ValueSets = string.Empty;
                                                    }
                                                    opts.Add("Value", ValueSets);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = ValueSets;
                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                    dynamicFormReportItems6.AttrId = attrName;
                                                    dynamicFormReportItems6.Label = s.DisplayName;
                                                    dynamicFormReportItems6.Value = ValueSets;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                }
                                                else if (s.ControlType == "DateEdit")
                                                {
                                                    DateTime? values = itemValue == null ? null : (DateTime)itemValue;
                                                    opts.Add("Label", s.DisplayName);
                                                    opts.Add("Value", values);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = values;
                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                    dynamicFormReportItems6.AttrId = attrName;
                                                    dynamicFormReportItems6.Label = s.DisplayName;
                                                    dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                }
                                                else if (s.ControlType == "TimeEdit")
                                                {
                                                    TimeSpan? values = itemValue == null ? null : (TimeSpan)itemValue;
                                                    opts.Add("Label", s.DisplayName);
                                                    opts.Add("Value", values);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = values;
                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                    dynamicFormReportItems6.AttrId = attrName;
                                                    dynamicFormReportItems6.Label = s.DisplayName;
                                                    dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                }
                                                else if (s.ControlType == "SpinEdit")
                                                {
                                                    if (s.IsSpinEditType == "decimal")
                                                    {
                                                        decimal? values = itemValue == null ? null : (decimal)itemValue;
                                                        opts.Add("Label", s.DisplayName);
                                                        opts.Add("Value", values);
                                                        objectData[attrName] = opts;
                                                        objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = values;
                                                        DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                        dynamicFormReportItems6.AttrId = attrName;
                                                        dynamicFormReportItems6.Label = s.DisplayName;
                                                        dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                    }
                                                    else
                                                    {
                                                        int? values = itemValue == null ? null : (int)itemValue;
                                                        opts.Add("Label", s.DisplayName);
                                                        opts.Add("Value", values);
                                                        objectData[attrName] = opts;
                                                        objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = values;
                                                        DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                        dynamicFormReportItems6.AttrId = attrName;
                                                        dynamicFormReportItems6.Label = s.DisplayName;
                                                        dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                    }
                                                }
                                                else if (s.ControlType == "CheckBox")
                                                {
                                                    bool? values = itemValue == null ? false : (bool)itemValue;
                                                    opts.Add("Label", s.DisplayName);
                                                    opts.Add("Value", values);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = values;
                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                    dynamicFormReportItems6.AttrId = attrName;
                                                    dynamicFormReportItems6.Label = s.DisplayName;
                                                    dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                    if (values == true)
                                                    {
                                                        SubAttrsHeader = s.SubAttributeHeaders;
                                                    }
                                                }
                                                else
                                                {
                                                    opts.Add("Label", s.DisplayName);
                                                    opts.Add("Value", (string)itemValue);
                                                    objectData[attrName] = opts;
                                                    objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = (string)itemValue;
                                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                                    dynamicFormReportItems6.AttrId = attrName;
                                                    dynamicFormReportItems6.Label = s.DisplayName;
                                                    dynamicFormReportItems6.Value = (string)itemValue;
                                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                                }
                                                loadSubHeaders(SubAttrsHeader, s, jsonObj, dynamicFormReportItems, objectData, objectDataList);

                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    if (s.ControlTypeId == 2712)
                                    {
                                        var url = "?DynamicFormSessionId=" + (_dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId) + "&&DynamicFormDataSessionId=" + r.SessionId + "&&DynamicFormDataGridSessionId=" + s.DynamicFormSessionId + "&&DynamicFormSectionGridAttributeSessionId=" + s.SessionId;

                                        opts.Add("Label", s.DisplayName);
                                        opts.Add("Value", s.DynamicFormSessionId);
                                        opts.Add("IsGrid", true);
                                        opts.Add("DynamicFormSessionId", _dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId);
                                        opts.Add("DynamicFormDataSessionId", r.SessionId);
                                        opts.Add("DynamicFormDataGridSessionId", s.DynamicFormSessionId);
                                        opts.Add("DynamicFormSectionGridAttributeSessionId", s.SessionId);
                                        opts.Add("DynamicGridFormId", _dynamicForm.ID);
                                        opts.Add("DynamicGridFormDataId", r.DynamicFormDataId);
                                        opts.Add("DynamicGridFormDataGridId", s.DynamicFormGridDropDownId);
                                        opts.Add("DynamicFormSectionGridAttributeId", s.DynamicFormSectionAttributeId);
                                        opts.Add("Url", url);
                                        objectData[attrName] = opts;
                                        objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = s.DynamicFormSessionId;
                                        DynamicFormReportItems dynamicFormReportItems1 = new DynamicFormReportItems();
                                        dynamicFormReportItems1.AttrId = attrName;
                                        dynamicFormReportItems1.Label = s.DisplayName;
                                        dynamicFormReportItems1.IsGrid = true;
                                        dynamicFormReportItems1.DynamicFormSessionId = _dynamicForm.SessionId == null ? _dynamicForm.SessionID : _dynamicForm.SessionId;
                                        dynamicFormReportItems1.DynamicFormDataSessionId = r.SessionId;
                                        dynamicFormReportItems1.DynamicFormDataGridSessionId = s.DynamicFormSessionId;
                                        dynamicFormReportItems1.DynamicGridFormId = _dynamicForm.ID;
                                        dynamicFormReportItems1.DynamicGridFormDataId = r.DynamicFormDataId;
                                        dynamicFormReportItems1.DynamicGridFormDataGridId = s.DynamicFormGridDropDownId;
                                        dynamicFormReportItems1.Url = BasUrl + url;
                                        dynamicFormReportItems1.DynamicFormSectionGridAttributeId = s.DynamicFormSectionAttributeId;
                                        dynamicFormReportItems1.DynamicFormSectionGridAttributeSessionId = s.SessionId;
                                        dynamicFormReportItems.Add(dynamicFormReportItems1);
                                    }
                                    else
                                    {
                                        if (s.ControlType == "ComboBox" && s.IsPlantLoadDependency == true && s.AttributeHeaderDataSource.Count() > 0)
                                        {
                                            opts.Add("Label", s.DisplayName);
                                            opts.Add("Value", string.Empty);
                                            objectData[attrName] = opts;
                                            objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                            DynamicFormReportItems dynamicFormReportItems5 = new DynamicFormReportItems();
                                            dynamicFormReportItems5.AttrId = attrName;
                                            dynamicFormReportItems5.Label = s.DisplayName;
                                            dynamicFormReportItems5.Value = string.Empty;
                                            dynamicFormReportItems.Add(dynamicFormReportItems5);
                                            s.AttributeHeaderDataSource.ForEach(dd =>
                                            {
                                                var opts1 = new Dictionary<object, object>();
                                                opts1.Add("Label", dd.DisplayName);
                                                var nameData = s.DynamicFormSectionAttributeId + "_" + dd.AttributeHeaderDataSourceId + "_" + dd.DataSourceTable + "_Dependency";
                                                opts1.Add("Value", string.Empty);
                                                objectData[nameData] = opts1;
                                                objectDataList[nameData + "$" + dd.DisplayName.Replace(" ", "_")] = string.Empty;
                                                DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                                dynamicFormReportItems55.AttrId = nameData;
                                                dynamicFormReportItems55.Label = dd.DisplayName;
                                                dynamicFormReportItems55.Value = string.Empty;
                                                dynamicFormReportItems.Add(dynamicFormReportItems55);
                                            });

                                        }
                                        else
                                        {
                                            if (s.DataSourceTable == "ApplicationMasterParent")
                                            {
                                                opts.Add("Label", s.DisplayName);
                                                opts.Add("Value", string.Empty);
                                                objectData[attrName] = opts;
                                                objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                                DynamicFormReportItems dynamicFormReportItems56 = new DynamicFormReportItems();
                                                dynamicFormReportItems56.AttrId = attrName;
                                                dynamicFormReportItems56.Label = s.DisplayName;
                                                dynamicFormReportItems56.Value = string.Empty;
                                                dynamicFormReportItems.Add(dynamicFormReportItems56);
                                                List<ApplicationMasterParent> nameDatas = new List<ApplicationMasterParent>();
                                                s.ApplicationMasterParents.ForEach(ab =>
                                                {
                                                    nameDatas.Add(ab);
                                                    RemoveApplicationMasterParentSingleNameItemApi(ab, s, nameDatas, _AttributeHeader.ApplicationMasterParent);
                                                    if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMasterParent" && s.IsDynamicFormGridDropdown == true)
                                                    {
                                                        var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                                        var opts1 = new Dictionary<object, object>();
                                                        opts1.Add("Label", s.GridDropDownDynamicFormName);
                                                        opts1.Add("Value", string.Empty);
                                                        objectData[appendDependency] = opts1;
                                                        objectDataList[appendDependency + "$" + s.GridDropDownDynamicFormName.Replace(" ", "_")] = string.Empty;
                                                        DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                                        dynamicFormReportItems55.AttrId = appendDependency;
                                                        dynamicFormReportItems55.Label = s.GridDropDownDynamicFormName;
                                                        dynamicFormReportItems55.Value = string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems55);
                                                    }
                                                });
                                                if (nameDatas != null && nameDatas.Count() > 0)
                                                {
                                                    nameDatas.ForEach(n =>
                                                    {
                                                        var opts1 = new Dictionary<object, object>();
                                                        opts1.Add("Label", n.ApplicationMasterName);
                                                        var nameData = s.DynamicFormSectionAttributeId + "_" + n.ApplicationMasterParentCodeId + "_AppMasterPar";
                                                        opts1.Add("Value", string.Empty);
                                                        objectData[nameData] = opts1;
                                                        objectDataList[nameData + "$" + n.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                                        DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                                        dynamicFormReportItems55.AttrId = nameData;
                                                        dynamicFormReportItems55.Label = n.ApplicationMasterName;
                                                        dynamicFormReportItems55.Value = string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems55);
                                                    });
                                                }

                                            }
                                            else if (s.DataSourceTable == "ApplicationMaster")
                                            {
                                                opts.Add("Label", s.DisplayName);
                                                opts.Add("Value", string.Empty);
                                                objectData[attrName] = opts;
                                                objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                                DynamicFormReportItems dynamicFormReportItems57 = new DynamicFormReportItems();
                                                dynamicFormReportItems57.AttrId = attrName;
                                                dynamicFormReportItems57.Label = s.DisplayName;
                                                dynamicFormReportItems57.Value = string.Empty;
                                                dynamicFormReportItems.Add(dynamicFormReportItems57);
                                                if (s.ApplicationMaster.Count() > 0)
                                                {
                                                    s.ApplicationMaster.ForEach(ab =>
                                                    {
                                                        var opts1 = new Dictionary<object, object>();
                                                        opts1.Add("Label", ab.ApplicationMasterName);
                                                        var nameData = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_AppMaster";
                                                        opts1.Add("Value", string.Empty);
                                                        objectData[nameData] = opts1;
                                                        objectDataList[attrName + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                                        DynamicFormReportItems dynamicFormReportItems51 = new DynamicFormReportItems();
                                                        dynamicFormReportItems51.AttrId = nameData;
                                                        dynamicFormReportItems51.Label = ab.ApplicationMasterName;
                                                        dynamicFormReportItems51.Value = string.Empty;
                                                        dynamicFormReportItems.Add(dynamicFormReportItems51);
                                                        if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMaster" && s.IsDynamicFormGridDropdown == true)
                                                        {
                                                            var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                                            var opts12 = new Dictionary<object, object>();
                                                            opts12.Add("Label", s.GridDropDownDynamicFormName);
                                                            opts12.Add("Value", string.Empty);
                                                            objectData[appendDependency] = opts12;
                                                            objectDataList[appendDependency + "$" + s.GridDropDownDynamicFormName.Replace(" ", "_")] = string.Empty;
                                                            DynamicFormReportItems dynamicFormReportItems55 = new DynamicFormReportItems();
                                                            dynamicFormReportItems55.AttrId = appendDependency;
                                                            dynamicFormReportItems55.Label = s.GridDropDownDynamicFormName;
                                                            dynamicFormReportItems55.Value = string.Empty;
                                                            dynamicFormReportItems.Add(dynamicFormReportItems55);
                                                        }
                                                    });
                                                }

                                            }
                                            else
                                            {
                                                opts.Add("Label", s.DisplayName);
                                                opts.Add("Value", string.Empty);
                                                objectData[attrName] = opts;
                                                objectDataList[attrName + "$" + s.DisplayName.Replace(" ", "_")] = string.Empty;
                                                DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                                dynamicFormReportItems58.AttrId = attrName;
                                                dynamicFormReportItems58.Label = s.DisplayName;
                                                dynamicFormReportItems58.Value = string.Empty;
                                                dynamicFormReportItems.Add(dynamicFormReportItems58);
                                            }
                                        }
                                    }
                                }


                            });
                        }
                        list.Add(objectData);
                        dynamicFormData.ObjectDataList = list;
                        dynamicFormData.ObjectDataItems = objectDataList;
                        dynamicFormData.DynamicFormReportItems = dynamicFormReportItems;
                        _dynamicformObjectDataList.Add(dynamicFormData);
                    });
                }
            }
            return _dynamicformObjectDataList;
        }
        void loadSubHeaders(List<AttributeHeader>? SubAttrsHeader, DynamicFormSectionAttribute dynamicFormSectionAttribute, dynamic jsonObjs, List<DynamicFormReportItems> dynamicFormReportItems, IDictionary<string, object> objectData, IDictionary<string, object> objectDataList)
        {
            if (SubAttrsHeader != null && SubAttrsHeader.Count > 0)
            {
                SubAttrsHeader.ForEach(d =>
                {
                    var subAttrName = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + d.SubDynamicAttributeName;
                    var SubNamess = jsonObjs.ContainsKey(subAttrName);
                    var optsSub = new Dictionary<object, object>();
                    if (SubNamess == true)
                    {
                        var itemValues = jsonObjs[subAttrName];
                        if (itemValues is JArray)
                        {
                            var values = (JArray)itemValues;
                            List<long?> listDatas = values.ToObject<List<long?>>();
                            if (d.IsMultiple == true || d.ControlType == "TagBox")
                            {
                                var ValueSet = string.Empty;
                                if (listDatas != null && listDatas.Count > 0 && d.SubAttributeDetails != null && d.SubAttributeDetails.Count > 0)
                                {
                                    var listName = d.SubAttributeDetails.Where(a => listDatas.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.DropDownTypeId == d.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                    ValueSet = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                }
                                optsSub.Add("Label", d.Description);
                                optsSub.Add("Value", ValueSet);
                                objectData[subAttrName] = optsSub;
                                objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = ValueSet;
                                DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                dynamicFormReportItems58.AttrId = subAttrName;
                                dynamicFormReportItems58.IsSubForm = true;
                                dynamicFormReportItems58.Label = d.Description;
                                dynamicFormReportItems58.Value = ValueSet;
                                dynamicFormReportItems.Add(dynamicFormReportItems58);
                            }
                        }
                        else if (d.ControlType == "ListBox" && d.IsMultiple == false)
                        {
                            var ValueSet = string.Empty;
                            long? values = itemValues == null ? null : (long)itemValues;
                            if (values > 0 && d.SubAttributeDetails != null && d.SubAttributeDetails.Count > 0)
                            {
                                var listName = d.SubAttributeDetails.Where(a => a.AttributeDetailID == values && a.AttributeDetailName != null && a.DropDownTypeId == d.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                ValueSet = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                            }
                            optsSub.Add("Label", d.Description);
                            optsSub.Add("Value", ValueSet);
                            objectData[subAttrName] = optsSub;
                            objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = ValueSet;
                            DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                            dynamicFormReportItems58.AttrId = subAttrName;
                            dynamicFormReportItems58.Label = d.Description;
                            dynamicFormReportItems58.IsSubForm = true;
                            dynamicFormReportItems58.Value = ValueSet;
                            dynamicFormReportItems.Add(dynamicFormReportItems58);
                        }
                        else
                        {
                            if (d.ControlType == "TextBox" || d.ControlType == "Memo")
                            {
                                var values = (string)itemValues;
                                optsSub.Add("Label", d.Description);
                                optsSub.Add("Value", values);
                                objectData[subAttrName] = optsSub;
                                objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = values;
                                DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                dynamicFormReportItems6.AttrId = subAttrName;
                                dynamicFormReportItems6.Label = d.Description;
                                dynamicFormReportItems6.IsSubForm = true;
                                dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                dynamicFormReportItems.Add(dynamicFormReportItems6);

                            }
                            else if (d.ControlType == "QR Code")
                            {
                                var values = (string)itemValues;
                                optsSub.Add("Label", d.Description);
                                optsSub.Add("Value", values);
                                objectData[subAttrName] = optsSub;
                                objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = values;
                                DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                dynamicFormReportItems6.AttrId = subAttrName;
                                dynamicFormReportItems6.Label = d.Description;
                                dynamicFormReportItems6.IsSubForm = true;
                                dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                dynamicFormReportItems.Add(dynamicFormReportItems6);

                            }
                            else if (d.ControlType == "ComboBox" || d.ControlType == "Radio" || d.ControlType == "RadioGroup")
                            {
                                long? values = itemValues == null ? null : (long)itemValues;
                                var ValueSet = string.Empty;
                                if (values > 0 && d.SubAttributeDetails != null && d.SubAttributeDetails.Count > 0)
                                {
                                    var listName = d.SubAttributeDetails.Where(a => a.AttributeDetailID == values && a.AttributeDetailName != null && a.DropDownTypeId == d.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                    ValueSet = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                }
                                optsSub.Add("Label", d.Description);
                                optsSub.Add("Value", ValueSet);
                                objectData[subAttrName] = optsSub;
                                objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = ValueSet;
                                DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                dynamicFormReportItems58.AttrId = subAttrName;
                                dynamicFormReportItems58.Label = d.Description;
                                dynamicFormReportItems58.Value = ValueSet;
                                dynamicFormReportItems58.IsSubForm = true;
                                dynamicFormReportItems.Add(dynamicFormReportItems58);
                            }
                            else if (d.ControlType == "CheckBox")
                            {
                                bool? values = itemValues == null ? false : (bool)itemValues;
                                optsSub.Add("Label", d.Description);
                                optsSub.Add("Value", values);
                                objectData[subAttrName] = optsSub;
                                objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = values;
                                DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                dynamicFormReportItems6.AttrId = subAttrName;
                                dynamicFormReportItems6.Label = d.Description;
                                dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                dynamicFormReportItems6.IsSubForm = true;
                                dynamicFormReportItems.Add(dynamicFormReportItems6);
                            }
                            else if (d.ControlType == "DateEdit")
                            {
                                DateTime? values = itemValues == null ? null : (DateTime)itemValues;
                                optsSub.Add("Label", d.Description);
                                optsSub.Add("Value", values);
                                objectData[subAttrName] = optsSub;
                                objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = values;
                                DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                dynamicFormReportItems6.AttrId = subAttrName;
                                dynamicFormReportItems6.Label = d.Description;
                                dynamicFormReportItems6.IsSubForm = true;
                                dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                dynamicFormReportItems.Add(dynamicFormReportItems6);
                            }
                            else if (d.ControlType == "TimeEdit")
                            {
                                TimeSpan? values = itemValues == null ? null : (TimeSpan)itemValues;
                                optsSub.Add("Label", d.Description);
                                optsSub.Add("Value", values);
                                objectData[subAttrName] = optsSub;
                                objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = values;
                                DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                dynamicFormReportItems6.AttrId = subAttrName;
                                dynamicFormReportItems6.Label = d.Description;
                                dynamicFormReportItems6.IsSubForm = true;
                                dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                dynamicFormReportItems.Add(dynamicFormReportItems6);
                            }
                            else if (d.ControlType == "SpinEdit")
                            {
                                if (d.IsAttributeSpinEditType == "decimal")
                                {
                                    decimal? values = itemValues == null ? null : (decimal)itemValues;
                                    optsSub.Add("Label", d.Description);
                                    optsSub.Add("Value", values);
                                    objectData[subAttrName] = optsSub;
                                    objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = values;
                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                    dynamicFormReportItems6.AttrId = subAttrName;
                                    dynamicFormReportItems6.IsSubForm = true;
                                    dynamicFormReportItems6.Label = d.Description;
                                    dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                }
                                else
                                {
                                    int? values = itemValues == null ? null : (int)itemValues;
                                    optsSub.Add("Label", d.Description);
                                    optsSub.Add("Value", values);
                                    objectData[subAttrName] = optsSub;
                                    objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = values;
                                    DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                    dynamicFormReportItems6.AttrId = subAttrName;
                                    dynamicFormReportItems6.Label = d.Description;
                                    dynamicFormReportItems6.IsSubForm = true;
                                    dynamicFormReportItems6.Value = values != null ? values.ToString() : string.Empty;
                                    dynamicFormReportItems.Add(dynamicFormReportItems6);
                                }
                            }
                        }
                        if (d.DataSourceTable == "ApplicationMaster" && d.SubApplicationMaster != null && d.SubApplicationMaster.Count() > 0)
                        {
                            var exitsMas = dynamicFormReportItems.FirstOrDefault(x => x.AttrId == subAttrName);
                            if (exitsMas == null)
                            {
                                optsSub.Add("Label", d.Description);
                                optsSub.Add("Value", string.Empty);
                                objectData[subAttrName] = optsSub;
                                objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = string.Empty;
                                DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                                dynamicFormReportItems6.AttrId = subAttrName;
                                dynamicFormReportItems6.Label = d.Description;
                                dynamicFormReportItems6.Value = string.Empty;
                                dynamicFormReportItems6.IsSubForm = true;
                                dynamicFormReportItems.Add(dynamicFormReportItems6);
                            }
                            d.SubApplicationMaster.ForEach(ab =>
                            {
                                var optsSub1 = new Dictionary<object, object>();
                                var appendAppMaster = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + d.SubDynamicAttributeName + "_" + ab.ApplicationMasterCodeId + "_SubAppMaster";
                                var SubNamesss = jsonObjs.ContainsKey(appendAppMaster);
                                if (SubNamesss == true)
                                {
                                    var itemValues = jsonObjs[appendAppMaster];
                                    if (itemValues is JArray)
                                    {
                                        var values = (JArray)itemValues;
                                        List<long?> listDatas = values.ToObject<List<long?>>();
                                        if (d.IsMultiple == true || d.ControlType == "TagBox")
                                        {
                                            var ValueSet = string.Empty;
                                            if (listDatas != null && listDatas.Count > 0 && d.SubAttributeDetails != null && d.SubAttributeDetails.Count > 0)
                                            {
                                                var listName = d.SubAttributeDetails.Where(a => listDatas.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.DropDownTypeId == d.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                ValueSet = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                            }
                                            optsSub1.Add("Label", ab.ApplicationMasterName);
                                            optsSub1.Add("Value", ValueSet);
                                            objectData[appendAppMaster] = optsSub1;
                                            objectDataList[appendAppMaster + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = ValueSet;
                                            DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                            dynamicFormReportItems58.AttrId = appendAppMaster;
                                            dynamicFormReportItems58.Label = ab.ApplicationMasterName;
                                            dynamicFormReportItems58.Value = ValueSet;
                                            dynamicFormReportItems58.IsSubForm = true;
                                            dynamicFormReportItems.Add(dynamicFormReportItems58);
                                        }
                                    }
                                    else
                                    {
                                        if (d.ControlType == "ComboBox")
                                        {
                                            long? values = itemValues == null ? null : (long)itemValues;
                                            var ValueSet = string.Empty;
                                            if (values > 0 && d.SubAttributeDetails != null && d.SubAttributeDetails.Count > 0)
                                            {
                                                var listName = d.SubAttributeDetails.Where(a => a.AttributeDetailID == values && a.AttributeDetailName != null && a.DropDownTypeId == d.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                ValueSet = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                            }
                                            optsSub1.Add("Label", ab.ApplicationMasterName);
                                            optsSub1.Add("Value", ValueSet);
                                            objectData[appendAppMaster] = optsSub1;
                                            objectDataList[appendAppMaster + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = ValueSet;
                                            DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                            dynamicFormReportItems58.AttrId = appendAppMaster;
                                            dynamicFormReportItems58.Label = ab.ApplicationMasterName;
                                            dynamicFormReportItems58.Value = ValueSet;
                                            dynamicFormReportItems58.IsSubForm = true;
                                            dynamicFormReportItems.Add(dynamicFormReportItems58);
                                        }
                                        else
                                        {
                                            if (d.ControlType == "ListBox" && d.IsMultiple == false)
                                            {
                                                long? values = itemValues == null ? null : (long)itemValues;
                                                var ValueSet = string.Empty;
                                                if (values > 0 && d.SubAttributeDetails != null && d.SubAttributeDetails.Count > 0)
                                                {
                                                    var listName = d.SubAttributeDetails.Where(a => a.AttributeDetailID == values && a.AttributeDetailName != null && a.DropDownTypeId == d.DataSourceTable).Select(s => s.AttributeDetailName).ToList();
                                                    ValueSet = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                }
                                                optsSub1.Add("Label", ab.ApplicationMasterName);
                                                optsSub1.Add("Value", ValueSet);
                                                objectData[appendAppMaster] = optsSub1;
                                                objectDataList[appendAppMaster + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = ValueSet;
                                                DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                                dynamicFormReportItems58.AttrId = appendAppMaster;
                                                dynamicFormReportItems58.Label = ab.ApplicationMasterName;
                                                dynamicFormReportItems58.Value = ValueSet;
                                                dynamicFormReportItems58.IsSubForm = true;
                                                dynamicFormReportItems.Add(dynamicFormReportItems58);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    optsSub1.Add("Label", ab.ApplicationMasterName);
                                    optsSub1.Add("Value", string.Empty);
                                    objectData[appendAppMaster] = optsSub1;
                                    objectDataList[appendAppMaster + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                    DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                    dynamicFormReportItems58.AttrId = appendAppMaster;
                                    dynamicFormReportItems58.Label = ab.ApplicationMasterName;
                                    dynamicFormReportItems58.Value = string.Empty;
                                    dynamicFormReportItems58.IsSubForm = true;
                                    dynamicFormReportItems.Add(dynamicFormReportItems58);
                                }
                            });
                        }
                    }
                    else
                    {
                        if (d.DataSourceTable == "ApplicationMaster" && d.SubApplicationMaster != null && d.SubApplicationMaster.Count() > 0)
                        {
                            optsSub.Add("Label", d.Description);
                            optsSub.Add("Value", string.Empty);
                            objectData[subAttrName] = optsSub;
                            objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = string.Empty;
                            DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                            dynamicFormReportItems6.AttrId = subAttrName;
                            dynamicFormReportItems6.Label = d.Description;
                            dynamicFormReportItems6.Value = string.Empty;
                            dynamicFormReportItems.Add(dynamicFormReportItems6);
                            d.SubApplicationMaster.ForEach(ab =>
                            {
                                var optsSub1 = new Dictionary<object, object>();
                                optsSub1.Add("Label", ab.ApplicationMasterName);
                                optsSub1.Add("Value", string.Empty);
                                var appendAppMaster = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + d.SubDynamicAttributeName + "_" + ab.ApplicationMasterCodeId + "_SubAppMaster";
                                objectData[appendAppMaster] = optsSub1;
                                objectDataList[appendAppMaster + "$" + ab.ApplicationMasterName.Replace(" ", "_")] = string.Empty;
                                DynamicFormReportItems dynamicFormReportItems58 = new DynamicFormReportItems();
                                dynamicFormReportItems58.AttrId = appendAppMaster;
                                dynamicFormReportItems58.Label = ab.ApplicationMasterName;
                                dynamicFormReportItems58.Value = string.Empty;
                                dynamicFormReportItems58.IsSubForm = true;
                                dynamicFormReportItems.Add(dynamicFormReportItems58);
                            });
                        }
                        else
                        {
                            optsSub.Add("Label", d.Description);
                            optsSub.Add("Value", d.ControlType == "CheckBox" ? false : string.Empty);
                            objectData[subAttrName] = optsSub;
                            objectDataList[subAttrName + "$" + d.Description.Replace(" ", "_")] = d.ControlType == "CheckBox" ? false : string.Empty;
                            DynamicFormReportItems dynamicFormReportItems6 = new DynamicFormReportItems();
                            dynamicFormReportItems6.AttrId = subAttrName;
                            dynamicFormReportItems6.Label = d.Description;
                            dynamicFormReportItems6.Value = string.Empty;
                            dynamicFormReportItems.Add(dynamicFormReportItems6);
                        }
                    }

                });
            }
        }
        void loadApplicationMasterParentDataApi(dynamic jsonObj, DynamicFormSectionAttribute s, string nameData, IDictionary<object, object> opts1, IReadOnlyList<AttributeDetails> PlantDependencySubAttributeDetails, IDictionary<string, object> objectDataList, string? DisName, DynamicFormReportItems dynamicFormReportItems22)
        {

            var SubNamess = jsonObj.ContainsKey(nameData);
            if (SubNamess == true)
            {
                var itemValue = jsonObj[nameData];
                if (itemValue is JArray)
                {
                    List<long?> listData = itemValue.ToObject<List<long?>>();
                    if (s.IsMultiple == true || s.ControlType == "TagBox")
                    {
                        var listName = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.DropDownTypeId == s.DataSourceTable).Select(s => s.AttributeDetailName).ToList() : new List<string?>();
                        var listsss = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                        opts1.Add("Value", listsss);
                        objectDataList[nameData + "$" + DisName.Replace(" ", "_")] = listsss;
                        dynamicFormReportItems22.Value = listsss;
                    }
                    else
                    {
                        opts1.Add("Value", string.Empty);
                        objectDataList[nameData + "$" + DisName.Replace(" ", "_")] = string.Empty;
                        dynamicFormReportItems22.Value = string.Empty;
                    }
                }
                else
                {
                    if (s.ControlType == "ComboBox")
                    {
                        long? values = itemValue == null ? -1 : (long)itemValue;
                        var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                        opts1.Add("Value", listss);
                        objectDataList[nameData + "$" + DisName.Replace(" ", "_")] = listss;
                        dynamicFormReportItems22.Value = listss;
                    }
                    else
                    {
                        if (s.ControlType == "ListBox" && s.IsMultiple == false)
                        {
                            long? values = itemValue == null ? -1 : (long)itemValue;
                            var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                            opts1.Add("Value", listss);
                            objectDataList[nameData + "$" + DisName.Replace(" ", "_")] = listss;
                            dynamicFormReportItems22.Value = listss;
                        }
                        else
                        {
                            opts1.Add("Value", string.Empty);
                            objectDataList[nameData + "$" + DisName.Replace(" ", "_")] = string.Empty;
                            dynamicFormReportItems22.Value = string.Empty;
                        }
                    }
                }
            }
            else
            {
                opts1.Add("Value", string.Empty);
                objectDataList[nameData + "$" + DisName.Replace(" ", "_")] = string.Empty;
                dynamicFormReportItems22.Value = string.Empty;
            }


        }
        void RemoveApplicationMasterParentSingleItemApi(ApplicationMasterParent applicationMasterParent, DynamicFormSectionAttribute dynamicFormSectionAttribute, List<DropDownOptionsModel> dataColumnNames, List<ApplicationMasterParent> ApplicationMasterParent)
        {
            if (applicationMasterParent != null)
            {
                var listss = ApplicationMasterParent.FirstOrDefault(f => f.ParentId == applicationMasterParent.ApplicationMasterParentCodeId);
                if (listss != null)
                {
                    var nameData = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + listss.ApplicationMasterParentCodeId + "_AppMasterPar";
                    dataColumnNames.Add(new DropDownOptionsModel() { Value = nameData, Text = listss.ApplicationMasterName, Type = "DynamicForm" });
                    RemoveApplicationMasterParentSingleItemApi(listss, dynamicFormSectionAttribute, dataColumnNames, ApplicationMasterParent);
                }
            }
        }
        void RemoveApplicationMasterParentSingleNameItemApi(ApplicationMasterParent applicationMasterParent, DynamicFormSectionAttribute dynamicFormSectionAttribute, List<ApplicationMasterParent?> dataColumnNames, List<ApplicationMasterParent> ApplicationMasterParent)
        {
            if (applicationMasterParent != null)
            {
                var listss = ApplicationMasterParent.FirstOrDefault(f => f.ParentId == applicationMasterParent.ApplicationMasterParentCodeId);
                if (listss != null)
                {
                    // var nameData = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + listss.ApplicationMasterParentCodeId + "_AppMasterPar";
                    dataColumnNames.Add(listss);
                    RemoveApplicationMasterParentSingleNameItemApi(listss, dynamicFormSectionAttribute, dataColumnNames, ApplicationMasterParent);
                }
            }
        }

        public async Task<IReadOnlyList<QCTestRequirement>> GetQcTestRequirementSummery()
        {
            try
            {
                List<QCTestRequirement> qCTestRequirements = new List<QCTestRequirement>(); List<DynamicFormData> dynamicGridFormDatas = new List<DynamicFormData>(); List<Plant> plantData = new List<Plant>();
                List<DynamicFormData> dynamicFormDatas = new List<DynamicFormData>(); List<ApplicationMasterDetail> applicationMasterDetail = new List<ApplicationMasterDetail>();
                var query = "select * from DynamicFormData where DynamicFormid=12 AND (IsDeleted=0 or IsDeleted is null);";
                query += "select t1.* from ApplicationMasterDetail  t1 JOIN ApplicationMaster t2 ON t2.ApplicationMasterID=t1.ApplicationMasterID  where ApplicationMasterCodeID in(103,130,354,356,369);";
                query += "select * from DynamicFormData where DynamicFormSectionGridAttributeID=331 AND  DynamicFormDataGridID in(select DynamicFormDataID from DynamicFormData where DynamicFormid=12 AND (IsDeleted=0 or IsDeleted is null));";
                query += "Select * from Plant;";
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryMultipleAsync(query));
                    dynamicFormDatas = result.Read<DynamicFormData>().ToList();
                    applicationMasterDetail = result.Read<ApplicationMasterDetail>().ToList();
                    dynamicGridFormDatas = result.Read<DynamicFormData>().ToList(); plantData = result.Read<Plant>().ToList();
                }
                if (dynamicFormDatas != null && dynamicFormDatas.Count() > 0)
                {
                    dynamicFormDatas.ForEach(s =>
                    {
                        QCTestRequirement QCTestRequirement = new QCTestRequirement();
                        QCTestRequirement.Reference = s.ProfileNo;
                        QCTestRequirement.PurposeOfTest = "Routine";
                        QCTestRequirement.BatchNo = "624/24014-R";
                        dynamic jsonObj = new object();
                        if (IsValidJson(s.DynamicFormItem))
                        {
                            jsonObj = JsonConvert.DeserializeObject(s.DynamicFormItem);
                        }
                        var namePlantData = "347_Attr";
                        var PlantNames = jsonObj.ContainsKey(namePlantData);
                        if (PlantNames == true)
                        {
                            var itemValue = jsonObj[namePlantData];
                            long? values = itemValue == null ? -1 : (long?)itemValue;
                            QCTestRequirement.PlantID = values;
                            QCTestRequirement.PlantCode = plantData.FirstOrDefault(f => f.PlantID == values)?.PlantCode;
                        }

                        var nameData = "363_103_AppMaster";
                        var Names = jsonObj.ContainsKey(nameData);
                        if (Names == true)
                        {
                            var itemValue = jsonObj[nameData];
                            long? values = itemValue == null ? -1 : (long?)itemValue;
                            QCTestRequirement.ItemId = values;
                            QCTestRequirement.ItemName = applicationMasterDetail.FirstOrDefault(f => f.ApplicationMasterDetailId == values)?.Value;
                        }
                        var dynamicGridFormList = dynamicGridFormDatas.Where(w => w.DynamicFormDataGridId == s.DynamicFormDataId).ToList();
                        if (dynamicGridFormList != null && dynamicGridFormList.Count() > 0)
                        {
                            List<QCTestRequirementChild> qCTestRequirementChildren = new List<QCTestRequirementChild>();
                            dynamicGridFormList.ForEach(d =>
                            {

                                dynamic jsonObjs = new object();
                                if (IsValidJson(d.DynamicFormItem))
                                {
                                    jsonObjs = JsonConvert.DeserializeObject(d.DynamicFormItem);
                                }
                                var nameDatasRoutineTest = "377_369_AppMaster";//QC Routine Test
                                var nameDatas1 = "276_354_AppMaster";//QC Testing Name
                                var nameDatas = "10683_130_AppMaster";
                                var nameDataMachine = "365_356_AppMaster";//QC Testing Machine
                                var Name = jsonObjs.ContainsKey(nameDatas);
                                var TestingName = jsonObjs.ContainsKey(nameDatas1); var TestingMachine = jsonObjs.ContainsKey(nameDataMachine);
                                long? TestingNameId = -1; long? TestingMachineId = -1;
                                var RoutineTestName = jsonObjs.ContainsKey(nameDatasRoutineTest);
                                ApplicationMasterDetail routineTestDetail = new ApplicationMasterDetail();
                                if (RoutineTestName == true)
                                {
                                    var itemRoutineValue = jsonObjs[nameDatasRoutineTest];
                                    if (itemRoutineValue is JArray)
                                    {
                                        List<long?> listData = itemRoutineValue.ToObject<List<long?>>();
                                        routineTestDetail = applicationMasterDetail.Where(q => listData.Contains(q.ApplicationMasterDetailId) && q.Value.ToLower() == "Routine Test".ToLower()).FirstOrDefault();
                                    }
                                }
                                if (routineTestDetail != null && routineTestDetail.ApplicationMasterDetailId > 0)
                                {
                                    if (TestingName == true)
                                    {
                                        var itemValues = jsonObjs[nameDatas1];
                                        TestingNameId = itemValues == null ? -1 : (long?)itemValues;
                                    }
                                    if (TestingMachine == true)
                                    {
                                        var itemMachineValues = jsonObjs[nameDataMachine];
                                        TestingMachineId = itemMachineValues == null ? -1 : (long?)itemMachineValues;
                                    }
                                    if (Name == true)
                                    {
                                        var itemValue = jsonObjs[nameDatas];
                                        if (itemValue is JArray)
                                        {
                                            List<long?> listData = itemValue.ToObject<List<long?>>();
                                            if (listData != null && listData.Count() > 0)
                                            {
                                                listData.ForEach(a =>
                                                {
                                                    QCTestRequirementChild qCTestRequirementChild = new QCTestRequirementChild();
                                                    qCTestRequirementChild.TestName = applicationMasterDetail.FirstOrDefault(f => f.ApplicationMasterDetailId == TestingNameId)?.Value;
                                                    qCTestRequirementChild.Process = applicationMasterDetail.FirstOrDefault(f => f.ApplicationMasterDetailId == a)?.Value;
                                                    qCTestRequirementChild.Equipment = applicationMasterDetail.FirstOrDefault(f => f.ApplicationMasterDetailId == TestingMachineId)?.Value;
                                                    qCTestRequirementChild.QCReferenceNo = "624/24014-" + routineTestDetail.Description + "-" + applicationMasterDetail.FirstOrDefault(f => f.ApplicationMasterDetailId == a)?.Description + "-" + applicationMasterDetail.FirstOrDefault(f => f.ApplicationMasterDetailId == TestingNameId)?.Description + "-1";
                                                    qCTestRequirementChildren.Add(qCTestRequirementChild);
                                                });
                                            }
                                        }
                                    }
                                }
                            });
                            QCTestRequirement.QCTestRequirementChild = qCTestRequirementChildren;
                        }
                        qCTestRequirements.Add(QCTestRequirement);
                    });
                }
                return qCTestRequirements;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
