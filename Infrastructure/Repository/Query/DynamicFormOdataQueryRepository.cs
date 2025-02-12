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
using Core.EntityModel;
using System.Dynamic;

namespace Infrastructure.Repository.Query
{
    public class DynamicFormOdataQueryRepository : DbConnector, IDynamicFormOdataQueryRepository
    {
        private readonly IDynamicFormDataSourceQueryRepository _dynamicFormDataSourceQueryRepository;
        public DynamicFormOdataQueryRepository(IConfiguration configuration, IDynamicFormDataSourceQueryRepository dynamicFormDataSourceQueryRepository)
            : base(configuration)
        {
            _dynamicFormDataSourceQueryRepository = dynamicFormDataSourceQueryRepository;
        }
        private async Task<IReadOnlyList<AttributeDetails>> GetAttributeDetails(List<long?> id)
        {
            try
            {
                id = id != null && id.Count > 0 ? id : new List<long?>() { -1 };
                var query = "select  CONCAT('Attr_',AttributeDetailID),AttributeId,AttributeDetailID,AttributeDetailName,Description from AttributeDetails WHERE (Disabled=0 OR Disabled IS NULL) AND AttributeID in(" + string.Join(',', id) + ");";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<object>> GetDropdownList(long? dynamicFormID, string? uniqueDynamicAttributeName)
        {
            try
            {
                List<long?> applicationMasterIds = new List<long?>(); List<long?> ApplicationMasterParentIds = new List<long?>(); List<string?> dataSourceTableIds = new List<string?>();
                List<string> controlTypes = new List<string>() { "ComboBox", "Radio", "TagBox", "RadioGroup", "ListBox" };
                List<object> attributeDetails = new List<object>();
                if (!string.IsNullOrEmpty(uniqueDynamicAttributeName))
                {
                    var result = await GetDynamicFormSectionAttribute(dynamicFormID);
                    if (result != null && result.Count() > 0)
                    {
                        var res = result.ToList();
                        var attrData = result.Where(w => w.UniqueDynamicAttributeName.ToLower() == uniqueDynamicAttributeName.ToLower()).FirstOrDefault();
                        if (attrData != null && !string.IsNullOrEmpty(attrData.ControlType))
                        {
                            if (controlTypes.Contains(attrData.ControlType))
                            {
                                if (!string.IsNullOrEmpty(attrData.AppCodeId))
                                {
                                    if (attrData.AppCodeName == "DataSource")
                                    {
                                        dataSourceTableIds.Add(attrData.AppCodeId);
                                    }
                                    if (attrData.AppCodeName == "ApplicationMaster")
                                    {
                                        var ids = Convert.ToInt64(attrData.AppCodeId);
                                        if (ids > 0)
                                        {
                                            dataSourceTableIds.Add(attrData.AppCodeName);
                                            applicationMasterIds.Add(ids);
                                        }
                                    }
                                    if (attrData.AppCodeName == "ApplicationMasterParent")
                                    {
                                        var ids = Convert.ToInt64(attrData.AppCodeId);
                                        if (ids > 0)
                                        {
                                            dataSourceTableIds.Add(attrData.AppCodeName);
                                            ApplicationMasterParentIds.Add(ids);
                                        }
                                    }
                                    var dataSourceList = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(null, dataSourceTableIds, null, applicationMasterIds, ApplicationMasterParentIds);
                                    if (dataSourceList != null && dataSourceList.Count > 0)
                                    {
                                        if (attrData.AppCodeName == "ApplicationMasterParent")
                                        {
                                            var ids = Convert.ToInt64(attrData.AppCodeId);
                                            if (ids > 0)
                                            {
                                                attributeDetails.AddRange(dataSourceList.Where(q => q.ApplicationMasterParentCodeId == ids).ToList());
                                            }
                                        }
                                        else
                                        {
                                            attributeDetails.AddRange(dataSourceList.ToList());
                                        }
                                    }
                                }
                                else
                                {
                                    List<long?> id = new List<long?>() { attrData.AttributeId };
                                    var resultData = await GetAttributeDetails(id);
                                    if (resultData != null && resultData.Count > 0)
                                    {
                                        attributeDetails.AddRange(resultData.ToList());
                                    }
                                }
                            }
                        }
                    }
                }
                return attributeDetails;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormSectionAttributesList>> GetDynamicFormSectionAttributeList(long? dynamicFormID)
        {
            try
            {
                List<DynamicFormSectionAttributesList> dynamicFormSectionAttributes = new List<DynamicFormSectionAttributesList>();
                var result = await GetDynamicFormSectionAttribute(dynamicFormID);
                if (result != null && result.Count() > 0)
                {
                    result.ToList().ForEach(s =>
                    {
                        DynamicFormSectionAttributesList dynamicFormSectionAttributes1 = new DynamicFormSectionAttributesList();
                        dynamicFormSectionAttributes1.DisplayName = s.DisplayName;
                        dynamicFormSectionAttributes1.ControlType = s.ControlType;
                        dynamicFormSectionAttributes1.SectionName = s.SectionName;
                        dynamicFormSectionAttributes1.UniqueDynamicAttributeName = s.UniqueDynamicAttributeName;
                        dynamicFormSectionAttributes.Add(dynamicFormSectionAttributes1);
                    });
                }
                return dynamicFormSectionAttributes;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<DynamicFormSectionAttributes>> GetDynamicFormSectionAttribute(long? dynamicFormID)
        {
            try
            {
                List<DynamicFormSectionAttributes> dynamicFormSectionAttributes = new List<DynamicFormSectionAttributes>();
                var query = "";
                query += "select  t1.GridDisplaySeqNo,(case when t1.IsDynamicFormGridDropdownMultiple is NULL then  0 ELSE t1.IsDynamicFormGridDropdownMultiple END) as IsDynamicFormGridDropdownMultiple,t1.IsDynamicFormGridDropdown,t1.GridDropDownDynamicFormID,t12.Name as GridDropDownDynamicFormName,t1.DynamicFormSectionAttributeID,t1.DynamicFormSectionID,t1.SessionID,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.AttributeID,t1.SortOrderBy,t1.ColSpan,t1.DisplayName,t1.IsMultiple,t1.IsRequired,t1.RequiredMessage,t1.IsSpinEditType,t1.FormUsedCount,t1.IsDisplayTableHeader,t1.FormToolTips,t1.IsVisible,t1.RadioLayout,t1.IsRadioCheckRemarks,t1.RemarksLabelName,t1.IsDeleted,t1.IsPlantLoadDependency,t1.PlantDropDownWithOtherDataSourceID,t1.PlantDropDownWithOtherDataSourceLabelName,t1.PlantDropDownWithOtherDataSourceIDs,t1.IsSetDefaultValue,t1.IsDefaultReadOnly,t1.ApplicationMasterID,t1.ApplicationMasterIDs,t1.IsDisplayDropDownHeader\n\r" +
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
                       "Where \r";
                query += "\r(t9.IsDeleted=0 OR t9.IsDeleted IS NULL) AND (t6.IsDeleted=0 OR t6.IsDeleted IS NULL) AND (t6.AttributeIsVisible=1 OR t6.AttributeIsVisible IS NULL) AND (t10.IsDeleted=0 or t10.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t1.IsVisible= 1 OR t1.IsVisible is null) AND \r";
                query += "\rt5.DynamicFormID=" + dynamicFormID + " order by t5.SortOrderBy,t1.SortOrderBy asc;";
                query += "Select plantId,PlantCode,Description from Plant;";
                query += "Select t1.HeaderDataSourceId,t1.AttributeHeaderDataSourceId,t1.DisplayName,t1.DataSourceTable,(Select COUNT(*) as IsDynamicFormFilterBy from DynamicFormFilterBy t2 where t2.AttributeHeaderDataSourceID=t1.AttributeHeaderDataSourceID)as IsDynamicFormFilterBy from AttributeHeaderDataSource t1;";
                query += "Select ApplicationMasterId,ApplicationMasterName,ApplicationMasterDescription,ApplicationMasterCodeId from ApplicationMaster;";
                query += "Select ApplicationMasterParentId,ApplicationMasterParentCodeId,ApplicationMasterName,Description,ParentId from ApplicationMasterParent;";

                var DynamicFormSectionAttribute = new List<DynamicFormSectionAttribute>();
                var Plant = new List<Plant>(); var AttributeHeaderDataSource = new List<AttributeHeaderDataSource>(); var ApplicationMaster = new List<ApplicationMaster>(); var ApplicationMasterParent = new List<ApplicationMasterParent>();
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query);
                    DynamicFormSectionAttribute = results.ReadAsync<DynamicFormSectionAttribute>().Result.ToList();
                    Plant = results.ReadAsync<Plant>().Result.ToList();
                    AttributeHeaderDataSource = results.ReadAsync<AttributeHeaderDataSource>().Result.ToList();
                    ApplicationMaster = results.ReadAsync<ApplicationMaster>().Result.ToList();
                    ApplicationMasterParent = results.ReadAsync<ApplicationMasterParent>().Result.ToList();
                }
                if (DynamicFormSectionAttribute != null && DynamicFormSectionAttribute.Count() > 0)
                {
                    DynamicFormSectionAttribute.ForEach(s =>
                    {
                        DynamicFormSectionAttributes dynamicFormSectionAttributes1 = new DynamicFormSectionAttributes();
                        dynamicFormSectionAttributes1.DynamicFormSectionAttributeId = s.DynamicFormSectionAttributeId;
                        dynamicFormSectionAttributes1.DisplayName = s.DisplayName;
                        dynamicFormSectionAttributes1.ControlType = s.ControlType;
                        dynamicFormSectionAttributes1.SectionName = s.SectionName;
                        dynamicFormSectionAttributes1.AttributeId = s.AttributeId;
                        if (s.DataSourceTable != null && s.DropDownTypeId == "Data Source")
                        {
                            dynamicFormSectionAttributes1.AppCodeId = s.DataSourceTable;
                            dynamicFormSectionAttributes1.AppCodeName = "DataSource";
                        }
                        if (s.IsPlantLoadDependency == true)
                        {
                            dynamicFormSectionAttributes1.AppCodeId = "Plant";
                            dynamicFormSectionAttributes1.AppCodeName = "DataSource";
                        }
                        dynamicFormSectionAttributes1.UniqueDynamicAttributeName = s.DynamicFormSectionAttributeId + "_" + s.AttributeName;
                        if (s.DataSourceTable != "ApplicationMaster" && s.DataSourceTable != "ApplicationMasterParent")
                        {
                            dynamicFormSectionAttributes.Add(dynamicFormSectionAttributes1);
                        }
                        if (s.IsPlantLoadDependency == true && !string.IsNullOrEmpty(s.PlantDropDownWithOtherDataSourceIds))
                        {
                            var PlantDropDownWithOtherDataSourceListIds = s.PlantDropDownWithOtherDataSourceIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (PlantDropDownWithOtherDataSourceListIds.Count > 0)
                            {
                                PlantDropDownWithOtherDataSourceListIds.ForEach(dd =>
                                {
                                    var attHdr = AttributeHeaderDataSource.Where(w => w.AttributeHeaderDataSourceId == dd).FirstOrDefault();
                                    if (attHdr != null)
                                    {
                                        var nameData = s.DynamicFormSectionAttributeId + "_" + attHdr.AttributeHeaderDataSourceId + "_" + attHdr.DataSourceTable + "_Dependency";
                                        DynamicFormSectionAttributes dynamicFormSectionAttributes2 = new DynamicFormSectionAttributes();
                                        dynamicFormSectionAttributes2.DynamicFormSectionAttributeId = s.DynamicFormSectionAttributeId;
                                        dynamicFormSectionAttributes2.DisplayName = attHdr.DisplayName;
                                        dynamicFormSectionAttributes2.ControlType = s.ControlType;
                                        dynamicFormSectionAttributes2.SectionName = s.SectionName;
                                        dynamicFormSectionAttributes2.AppCodeId = attHdr.DataSourceTable;
                                        dynamicFormSectionAttributes2.UniqueDynamicAttributeName = nameData;
                                        dynamicFormSectionAttributes2.AppCodeName = "DataSource";
                                        dynamicFormSectionAttributes.Add(dynamicFormSectionAttributes2);
                                    }
                                });
                            }
                        }
                        if (s.DataSourceTable == "ApplicationMaster" && !string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                applicationMasterIds.ForEach(dd =>
                                {
                                    var attHdr = ApplicationMaster.Where(w => w.ApplicationMasterId == dd).FirstOrDefault();
                                    if (attHdr != null)
                                    {
                                        var nameData = s.DynamicFormSectionAttributeId + "_" + attHdr.ApplicationMasterCodeId + "_AppMaster";
                                        DynamicFormSectionAttributes dynamicFormSectionAttributes2 = new DynamicFormSectionAttributes();
                                        dynamicFormSectionAttributes2.DynamicFormSectionAttributeId = s.DynamicFormSectionAttributeId;
                                        dynamicFormSectionAttributes2.DisplayName = attHdr.ApplicationMasterName;
                                        dynamicFormSectionAttributes2.ControlType = s.ControlType;
                                        dynamicFormSectionAttributes2.SectionName = s.SectionName;
                                        dynamicFormSectionAttributes2.UniqueDynamicAttributeName = nameData;
                                        dynamicFormSectionAttributes2.AppCodeId = attHdr.ApplicationMasterId.ToString();
                                        dynamicFormSectionAttributes2.AppCodeName = "ApplicationMaster";
                                        dynamicFormSectionAttributes.Add(dynamicFormSectionAttributes2);
                                    }
                                });
                            }
                        }
                        if (s.DataSourceTable == "ApplicationMasterParent" && !string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                applicationMasterIds.ForEach(dd =>
                                {
                                    var attHdr = ApplicationMasterParent.Where(w => w.ApplicationMasterParentCodeId == dd).FirstOrDefault();
                                    if (attHdr != null)
                                    {
                                        var nameData = s.DynamicFormSectionAttributeId + "_" + attHdr.ApplicationMasterParentCodeId + "_AppMasterPar";
                                        DynamicFormSectionAttributes dynamicFormSectionAttributes2 = new DynamicFormSectionAttributes();
                                        dynamicFormSectionAttributes2.DynamicFormSectionAttributeId = s.DynamicFormSectionAttributeId;
                                        dynamicFormSectionAttributes2.DisplayName = attHdr.ApplicationMasterName;
                                        dynamicFormSectionAttributes2.ControlType = s.ControlType;
                                        dynamicFormSectionAttributes2.SectionName = s.SectionName;
                                        dynamicFormSectionAttributes2.UniqueDynamicAttributeName = nameData;
                                        dynamicFormSectionAttributes2.AppCodeId = attHdr.ApplicationMasterParentCodeId.ToString();
                                        dynamicFormSectionAttributes2.AppCodeName = "ApplicationMasterParent";
                                        dynamicFormSectionAttributes.Add(dynamicFormSectionAttributes2);
                                        loadApplicationMasterParentDataList(s, attHdr, ApplicationMasterParent, dynamicFormSectionAttributes);
                                    }
                                });
                            }
                        }
                    });
                }
                return dynamicFormSectionAttributes;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        void loadApplicationMasterParentDataList(DynamicFormSectionAttribute dynamicFormSectionAttribute, ApplicationMasterParent applicationMasterParent, List<ApplicationMasterParent?> ApplicationMasterParents, List<DynamicFormSectionAttributes> dynamicFormSectionAttributes)
        {
            var dataList = ApplicationMasterParents.FirstOrDefault(f => f.ParentId == applicationMasterParent.ApplicationMasterParentCodeId);
            if (dataList != null)
            {
                var nameData = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + dataList.ApplicationMasterParentCodeId + "_AppMasterPar";
                DynamicFormSectionAttributes dynamicFormSectionAttributes2 = new DynamicFormSectionAttributes();
                dynamicFormSectionAttributes2.DynamicFormSectionAttributeId = dynamicFormSectionAttribute.DynamicFormSectionAttributeId;
                dynamicFormSectionAttributes2.DisplayName = dataList.ApplicationMasterName;
                dynamicFormSectionAttributes2.ControlType = dynamicFormSectionAttribute.ControlType;
                dynamicFormSectionAttributes2.UniqueDynamicAttributeName = nameData;
                dynamicFormSectionAttributes2.SectionName = dynamicFormSectionAttribute.SectionName;
                dynamicFormSectionAttributes2.AppCodeId = dataList.ApplicationMasterParentCodeId.ToString();
                dynamicFormSectionAttributes2.AppCodeName = "ApplicationMasterParent";
                dynamicFormSectionAttributes.Add(dynamicFormSectionAttributes2);
                loadApplicationMasterParentDataList(dynamicFormSectionAttribute, dataList, ApplicationMasterParents, dynamicFormSectionAttributes);
            }
        }

    }

}
