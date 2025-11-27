using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
using Infrastructure.Data;
using Infrastructure.Repository.Query.Base;
using MediatR;
using Microsoft.Extensions.Configuration;
using NAV;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iText.IO.Image.Jpeg2000ImageData;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Repository.Query
{
    public class DynamicFormSyncQueryRepository : QueryRepository<DynamicForm>, IDynamicFormSyncQueryRepository
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        private readonly IDynamicFormDataSourceQueryRepository _dynamicFormDataSourceQueryRepository;
        public DynamicFormSyncQueryRepository(IConfiguration configuration, IDynamicFormQueryRepository dynamicFormQueryRepository, IAttributeQueryRepository attrubutequeryRepository, IDynamicFormDataSourceQueryRepository dynamicFormDataSourceQueryRepository)
            : base(configuration)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
            _dynamicFormDataSourceQueryRepository = dynamicFormDataSourceQueryRepository;
        }
        public async Task<DynamicForm> OnDynamicFormSyncTables(DynamicForm? _dynamicForm, long? UserId)
        {
            try
            {
                if (_dynamicForm != null && _dynamicForm?.ID > 0)
                {
                    DynamicFormSearch dynamicFormSearch1 = new DynamicFormSearch(); List<DropDownOptionsModel> dataColumnNamesSync = new List<DropDownOptionsModel>();
                    var _dynamicformDataListItems = await _dynamicFormQueryRepository.GetDynamicFormDataByIdAsync(_dynamicForm.ID, 0, -1, null, null, dynamicFormSearch1,false);
                    var _AttributeHeadersDetails = await _attrubutequeryRepository.GetAllAttributeNameAsync(_dynamicForm, UserId, false, true, false);
                    List<string?> dataSourceTableIds = new List<string?>() { "Plant" };
                    var DataSourceTablelists = _AttributeHeadersDetails.DynamicFormSectionAttribute.Where(w => w.AttributeHeaderDataSource.Count > 0).SelectMany(a => a.AttributeHeaderDataSource.Select(s => s.DataSourceTable)).ToList().Distinct().ToList();
                    dataSourceTableIds.AddRange(DataSourceTablelists); List<long?> ApplicationMasterIds = new List<long?>(); List<long?> ApplicationMasterParentIds = new List<long?>();
                    ApplicationMasterIds = _AttributeHeadersDetails.DynamicFormSectionAttribute.Where(w => w.ApplicationMaster.Count > 0).SelectMany(a => a.ApplicationMaster.Select(s => (long?)s.ApplicationMasterId)).ToList().Distinct().ToList();
                    ApplicationMasterParentIds = _AttributeHeadersDetails.DynamicFormSectionAttribute.Where(w => w.ApplicationMasterParents.Count > 0).SelectMany(a => a.ApplicationMasterParents.Select(s => (long?)s.ApplicationMasterParentCodeId)).ToList().Distinct().ToList();
                    if (ApplicationMasterIds.Count > 0)
                    {
                        dataSourceTableIds.Add("ApplicationMaster");
                    }
                    if (ApplicationMasterParentIds != null && ApplicationMasterParentIds.Count > 0)
                    {
                        dataSourceTableIds.Add("ApplicationMasterParent");
                    }
                    var PlantDependencySubAttributeDetailsItems = await _dynamicFormDataSourceQueryRepository.GetDataSourceDropDownList(null, dataSourceTableIds, null, ApplicationMasterIds, ApplicationMasterParentIds != null ? ApplicationMasterParentIds : new List<long?>());
                    var _dynamicformObjectDataLists = loadItems(_dynamicformDataListItems.ToList(), _AttributeHeadersDetails, PlantDependencySubAttributeDetailsItems.ToList());
                    dynamicFormSearch1.StartDate = DateTime.Today.AddYears(50);
                    dynamicFormSearch1.EndDate = DateTime.Now;

                    var dataColumnNameData = UpdateDataSyncDatas(_AttributeHeadersDetails, dataColumnNamesSync, _dynamicForm);
                    var results = await _dynamicFormQueryRepository.GetDynamicFormDataTableSync(dataColumnNameData, _dynamicformObjectDataLists, _AttributeHeadersDetails, _dynamicForm);
                }
                return _dynamicForm;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private List<object> loadItems(List<DynamicFormData> dynamicFormDatas, AttributeHeaderListModel attributeHeaderListModels, List<AttributeDetails> PlantDependencySubAttributeDetails)
        {
            List<object>? _dynamicformObjectDataLists = new List<object>();
            if (dynamicFormDatas != null && dynamicFormDatas.Count > 0)
            {
                dynamicFormDatas.ForEach(s =>
                {
                    dynamic jsonObj = new object();
                    if (IsValidJson(s.DynamicFormItem))
                    {
                        jsonObj = JsonConvert.DeserializeObject(s.DynamicFormItem);
                    }
                    IDictionary<string, object> objectData = new ExpandoObject();
                    objectData["SortOrderByNo"] = s.SortOrderByNo;
                    objectData["DynamicFormDataId"] = s.DynamicFormDataId;
                    objectData["ProfileNo"] = s.ProfileNo;
                    objectData["SessionId"] = s.SessionId;
                    objectData["ScreenID"] = s.ScreenID;
                    objectData["DynamicFormId"] = s.DynamicFormId;
                    objectData["Name"] = s.Name;
                    objectData["ModifiedBy"] = s.ModifiedBy;
                    objectData["ModifiedDate"] = s.ModifiedDate;
                    objectData["CurrentUserName"] = s.CurrentUserName;
                    objectData["StatusName"] = s.StatusName;
                    objectData["IsDynamicFormDataGrid"] = s.IsDynamicFormDataGrid;
                    objectData["IsFileprofileTypeDocument"] = s.IsFileprofileTypeDocument;
                    objectData["DynamicFormSectionGridAttributeId"] = s.DynamicFormSectionGridAttributeId;
                    objectData["DynamicFormDataGridId"] = s.DynamicFormDataGridId;
                    objectData["GridSortOrderByNo"] = s.GridSortOrderByNo;
                    objectData["IsDraft"] = s.IsDraft;
                    objectData["IsLocked"] = s.IsLocked;
                    objectData["LockedUserId"] = s.LockedUserId;
                    objectData["LockedUser"] = s.LockedUser;
                    if (attributeHeaderListModels != null && attributeHeaderListModels.DynamicFormSectionAttribute != null && attributeHeaderListModels.DynamicFormSectionAttribute.Count > 0)
                    {
                        attributeHeaderListModels.DynamicFormSectionAttribute.ForEach(s =>
                        {
                            string attrName = s.DynamicAttributeName;
                            var Names = jsonObj.ContainsKey(attrName);
                            if (Names == true)
                            {
                                if (s.ControlTypeId == 2712)
                                {
                                    objectData[attrName] = s.DynamicFormSessionId;
                                }
                                else
                                {
                                    if (s.DataSourceTable == "ApplicationMaster")
                                    {
                                        objectData[attrName] = string.Empty;
                                        if (s.ApplicationMaster != null && s.ApplicationMaster.Count() > 0)
                                        {
                                            s.ApplicationMaster.ForEach(ab =>
                                            {
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
                                                            objectData[nameData] = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                                                            objectData[nameData + "_UId"] = listData;
                                                        }
                                                        else
                                                        {
                                                            objectData[nameData + "_UId"] = null;
                                                            objectData[nameData] = string.Empty;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (s.ControlType == "ComboBox")
                                                        {
                                                            long? values = itemValue == null ? -1 : (long)itemValue;
                                                            var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                                                            objectData[nameData] = listss;
                                                            objectData[nameData + "_UId"] = values;
                                                        }
                                                        else
                                                        {
                                                            if (s.ControlType == "ListBox" && s.IsMultiple == false)
                                                            {
                                                                long? values = itemValue == null ? -1 : (long)itemValue;
                                                                var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                                                                objectData[nameData] = listss;
                                                                objectData[nameData + "_UId"] = values;
                                                            }
                                                            else
                                                            {
                                                                objectData[nameData] = string.Empty;
                                                                objectData[nameData + "_UId"] = null;
                                                            }
                                                        }
                                                    }

                                                    if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMaster" && s.IsDynamicFormGridDropdown == true)
                                                    {
                                                        var collection = attributeHeaderListModels.DropDownOptionsGridListModel.ObjectData;
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
                                                                        objectData[appendDependency + "_UId"] = listData;
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
                                                                    objectData[appendDependency + "_UId"] = itemDepValue;
                                                                }
                                                            }
                                                        }

                                                        objectData[appendDependency] = displayNames;
                                                    }
                                                }
                                            });
                                        }
                                    }
                                    else if (s.DataSourceTable == "ApplicationMasterParent")
                                    {
                                        var collection = attributeHeaderListModels.DropDownOptionsGridListModel.ObjectData;
                                        objectData[attrName] = string.Empty;
                                        List<string?> nameDatas = new List<string?>();
                                        s.ApplicationMasterParents.ForEach(ab =>
                                        {
                                            nameDatas.Add(s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_AppMasterPar");
                                            RemoveApplicationMasterParentSingleNameItem(ab, s, nameDatas, attributeHeaderListModels.ApplicationMasterParent);
                                            if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMasterParent" && s.IsDynamicFormGridDropdown == true)
                                            {
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
                                                                objectData[appendDependency + "_UId"] = listData;
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
                                                            objectData[appendDependency + "_UId"] = valuesDep;
                                                        }
                                                    }
                                                }
                                                objectData[appendDependency] = displayNames;
                                            }

                                        });
                                        if (nameDatas != null && nameDatas.Count() > 0)
                                        {
                                            nameDatas.ForEach(n =>
                                            {
                                                loadApplicationMasterParentData(jsonObj, s, n, objectData, PlantDependencySubAttributeDetails);
                                            });
                                        }
                                    }
                                    else
                                    {
                                        var itemValue = jsonObj[attrName];
                                        var collection = attributeHeaderListModels.DropDownOptionsGridListModel.ObjectData;
                                        if (itemValue is JArray)
                                        {
                                            List<long?> listData = itemValue.ToObject<List<long?>>();
                                            if (s.DataSourceTable == "DynamicGrid")
                                            {
                                                List<DropDownOptionsModel?> gridlistss = new List<DropDownOptionsModel?>();
                                                gridlistss = attributeHeaderListModels.DropDownOptionsGridListModel.DropDownGridOptionsModel.Where(ax => ax.DynamicFormId == s.DynamicFormGridDropDownId).FirstOrDefault()?.DropDownOptionsModels;

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
                                                                var vv = eod.DynamicFormId;
                                                                string listone = string.Empty;
                                                                if (gridlistss != null && gridlistss.Count() > 0)
                                                                {
                                                                    var g1 = gridlistss.OrderBy(o => o.OrderBy).ToList();
                                                                    g1.ForEach(q =>
                                                                    {
                                                                        var byName = (IDictionary<string, object>)v;
                                                                        if (q.Value != null)
                                                                        {
                                                                            if (byName.ContainsKey(q.Value))
                                                                            {
                                                                                var names = (string)byName[q.Value]?.ToString();
                                                                                listone += names + "|";
                                                                            }
                                                                        }
                                                                    });
                                                                }
                                                                eod.DisplayItemsQuery = listone;
                                                                if ((long?)eod.AttributeDetailID == l)
                                                                {
                                                                    displayNames += eod.DisplayItemsQuery + ",";
                                                                }
                                                            }
                                                        }
                                                    });
                                                }
                                                objectData[attrName + "_UId"] = listData;
                                                objectData[attrName] = displayNames.TrimEnd(',');
                                            }
                                            else
                                            {
                                                List<string?> listName = new List<string?>();
                                                if (attributeHeaderListModels != null)
                                                {
                                                    var nameList = attributeHeaderListModels.AttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.DropDownTypeId == s.DataSourceTable).ToList();
                                                    if (nameList != null && nameList.Count() > 0)
                                                    {
                                                        nameList.ForEach(n =>
                                                        {
                                                            if (s.DataSourceTable == "Employee")
                                                            {
                                                                listName.Add((n?.AttributeDetailName + "|" + n?.Description + "|" + n?.DesignationName + "|" + n?.DepartmentName).TrimEnd('|'));
                                                            }
                                                            else if (s.DataSourceTable == "RawMatPurch")
                                                            {
                                                                listName.Add((n?.AttributeDetailName + "|" + n?.QcRefNo).TrimEnd('|'));
                                                            }
                                                            else if (s.DataSourceTable == "ReleaseProdOrderLine")
                                                            {
                                                                listName.Add((n?.AttributeDetailName + "|" + n?.ReplanRefNo).TrimEnd('|'));
                                                            }
                                                            else if (s.DataSourceTable == "FinishedProdOrderLine" || s.DataSourceTable == "FinishedProdOrderLineProductionInProgress" || s.DataSourceTable == "Location")
                                                            {
                                                                listName.Add(n?.NameList);
                                                            }
                                                            else
                                                            {
                                                                listName.Add((n?.AttributeDetailName + "|" + n?.Description).TrimEnd('|'));
                                                            }
                                                        });
                                                    }
                                                }
                                                objectData[attrName + "_UId"] = listData;
                                                objectData[attrName] = listName != null && listName.Count > 0 ? (string.Join(",", listName)).TrimEnd(',') : string.Empty;
                                            }
                                        }
                                        else
                                        {
                                            List<AttributeHeader> SubAttrsHeader = new List<AttributeHeader>();
                                            if (s.ControlType == "ComboBox" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                                            {
                                                long? Svalues = itemValue == null ? null : (long)itemValue;
                                                if (Svalues != null)
                                                {
                                                    if (s.DataSourceTable == "DynamicGrid")
                                                    {
                                                        List<DropDownOptionsModel?> gridlistss = new List<DropDownOptionsModel?>();
                                                        gridlistss = attributeHeaderListModels.DropDownOptionsGridListModel.DropDownGridOptionsModel.Where(ax => ax.DynamicFormId == s.DynamicFormGridDropDownId).FirstOrDefault()?.DropDownOptionsModels;
                                                        var displayNames = string.Empty;
                                                        if (collection != null)
                                                        {
                                                            foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                                            {
                                                                dynamic eod = v;
                                                                var vv = eod.DynamicFormId;
                                                                var AttributeDetailIds = eod.AttributeDetailID;
                                                                string listone = string.Empty;
                                                                if (gridlistss != null && gridlistss.Count() > 0)
                                                                {
                                                                    var g1 = gridlistss.OrderBy(o => o.OrderBy).ToList();
                                                                    g1.ForEach(q =>
                                                                    {
                                                                        var byName = (IDictionary<string, object>)v;
                                                                        if (q.Value != null)
                                                                        {
                                                                            if (byName.ContainsKey(q.Value))
                                                                            {
                                                                                var names = (string)byName[q.Value]?.ToString();
                                                                                listone += names + "|";
                                                                            }
                                                                        }
                                                                    });
                                                                }
                                                                eod.DisplayItemsQuery = listone;
                                                                if ((long?)eod.AttributeDetailID == Svalues)
                                                                {
                                                                    displayNames += eod.DisplayItemsQuery;
                                                                }
                                                            }
                                                        }
                                                        objectData[attrName + "_UId"] = Svalues;
                                                        objectData[attrName] = displayNames;
                                                    }
                                                    else
                                                    {
                                                        List<string?> listName = new List<string?>();
                                                        if (attributeHeaderListModels != null)
                                                        {
                                                            var nameList = attributeHeaderListModels.AttributeDetails.Where(a => a.AttributeDetailID == Svalues && a.AttributeDetailName != null && a.DropDownTypeId == s.DataSourceTable).ToList();
                                                            if (nameList != null && nameList.Count() > 0)
                                                            {
                                                                nameList.ForEach(n =>
                                                                {
                                                                    if (s.DataSourceTable == "Employee")
                                                                    {
                                                                        listName.Add((n?.AttributeDetailName + "|" + n?.Description + "|" + n?.DesignationName + "|" + n?.DepartmentName).TrimEnd('|'));
                                                                    }
                                                                    else if (s.DataSourceTable == "RawMatPurch")
                                                                    {
                                                                        listName.Add((n?.AttributeDetailName + "|" + n?.QcRefNo).TrimEnd('|'));
                                                                    }
                                                                    else if (s.DataSourceTable == "ReleaseProdOrderLine")
                                                                    {
                                                                        listName.Add((n?.AttributeDetailName + "|" + n?.ReplanRefNo).TrimEnd('|'));
                                                                    }
                                                                    else if (s.DataSourceTable == "FinishedProdOrderLine" || s.DataSourceTable == "FinishedProdOrderLineProductionInProgress" || s.DataSourceTable == "Location")
                                                                    {
                                                                        listName.Add(n?.NameList);
                                                                    }
                                                                    else
                                                                    {
                                                                        listName.Add((n?.AttributeDetailName + "|" + n?.Description).TrimEnd('|'));
                                                                    }
                                                                });
                                                            }
                                                        }
                                                        objectData[attrName + "_UId"] = Svalues;
                                                        objectData[attrName] = listName != null && listName.Count > 0 ? (string.Join(",", listName)).TrimEnd(',') : string.Empty;
                                                    }
                                                }
                                                else
                                                {
                                                    objectData[attrName + "_UId"] = null;
                                                    objectData[attrName] = string.Empty;
                                                }
                                                // if (Svalues > 0)
                                                // {
                                                //     var attrDetails = _AttributeHeader.AttributeDetails.Where(mm => mm.AttributeDetailID == Svalues && mm.DropDownTypeId == null).FirstOrDefault()?.SubAttributeHeaders;
                                                //     if (attrDetails != null && attrDetails.Count > 0)
                                                //     {
                                                //         SubAttrsHeader = attrDetails;
                                                //     }
                                                // }
                                                var attrDetails = attributeHeaderListModels.AttributeDetails.Where(mm => mm.AttributeID == s.AttributeId && mm.DropDownTypeId == null).ToList();
                                                if (attrDetails != null && attrDetails.Count > 0)
                                                {
                                                    attrDetails.ForEach(u =>
                                                    {
                                                        if (u.SubAttributeHeaders != null && u.SubAttributeHeaders.Count() > 0)
                                                        {
                                                            loadSubHeaders(u.SubAttributeHeaders, s, jsonObj, objectData, attributeHeaderListModels);
                                                        }
                                                    });
                                                }
                                                if (s.ControlType == "ComboBox" && s.IsPlantLoadDependency == true && s.AttributeHeaderDataSource.Count() > 0 && PlantDependencySubAttributeDetails != null && PlantDependencySubAttributeDetails.Count() > 0)
                                                {

                                                    s.AttributeHeaderDataSource.ForEach(dd =>
                                                    {
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
                                                                    List<string?> listName = new List<string?>();
                                                                    if (PlantDependencySubAttributeDetails != null)
                                                                    {
                                                                        var nameList = PlantDependencySubAttributeDetails.Where(a => listData.Contains(a.AttributeDetailID) && a.DropDownTypeId == dd.DataSourceTable).ToList();
                                                                        if (nameList != null && nameList.Count() > 0)
                                                                        {
                                                                            nameList.ForEach(n =>
                                                                            {
                                                                                if (s.DataSourceTable == "Employee")
                                                                                {
                                                                                    listName.Add((n?.AttributeDetailName + "|" + n?.Description + "|" + n?.DesignationName + "|" + n?.DepartmentName).TrimEnd('|'));
                                                                                }
                                                                                else if (s.DataSourceTable == "RawMatPurch")
                                                                                {
                                                                                    listName.Add((n?.AttributeDetailName + "|" + n?.QcRefNo).TrimEnd('|'));
                                                                                }
                                                                                else if (s.DataSourceTable == "ReleaseProdOrderLine")
                                                                                {
                                                                                    listName.Add((n?.AttributeDetailName + "|" + n?.ReplanRefNo).TrimEnd('|'));
                                                                                }
                                                                                else if (s.DataSourceTable == "FinishedProdOrderLine" || s.DataSourceTable == "FinishedProdOrderLineProductionInProgress" || s.DataSourceTable == "Location")
                                                                                {
                                                                                    listName.Add(n?.NameList);
                                                                                }
                                                                                else
                                                                                {
                                                                                    listName.Add((n?.AttributeDetailName + "|" + n?.Description).TrimEnd('|'));
                                                                                }
                                                                            });
                                                                        }
                                                                    }
                                                                    objectData[nameData + "_UId"] = listData;
                                                                    objectData[nameData] = listName != null && listName.Count > 0 ? (string.Join(",", listName)).TrimEnd(',') : string.Empty;
                                                                }
                                                                else
                                                                {
                                                                    objectData[nameData + "_UId"] = null;
                                                                    objectData[nameData] = string.Empty;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                long? valuesDep = itemDepValue == null ? -1 : (long)itemDepValue;
                                                                var nameList = PlantDependencySubAttributeDetails.Where(v => dd.DataSourceTable == v.DropDownTypeId && v.AttributeDetailID == valuesDep).ToList();
                                                                List<string?> listName = new List<string?>();
                                                                if (nameList != null && nameList.Count() > 0)
                                                                {
                                                                    nameList.ForEach(n =>
                                                                    {
                                                                        if (s.DataSourceTable == "Employee")
                                                                        {
                                                                            listName.Add((n?.AttributeDetailName + "|" + n?.Description + "|" + n?.DesignationName + "|" + n?.DepartmentName).TrimEnd('|'));
                                                                        }
                                                                        else if (s.DataSourceTable == "RawMatPurch")
                                                                        {
                                                                            listName.Add((n?.AttributeDetailName + "|" + n?.QcRefNo).TrimEnd('|'));
                                                                        }
                                                                        else if (s.DataSourceTable == "ReleaseProdOrderLine")
                                                                        {
                                                                            listName.Add((n?.AttributeDetailName + "|" + n?.ReplanRefNo).TrimEnd('|'));
                                                                        }
                                                                        else if (s.DataSourceTable == "FinishedProdOrderLine" || s.DataSourceTable == "FinishedProdOrderLineProductionInProgress" || s.DataSourceTable == "Location")
                                                                        {
                                                                            listName.Add(n?.NameList);
                                                                        }
                                                                        else
                                                                        {
                                                                            listName.Add((n?.AttributeDetailName + "|" + n?.Description).TrimEnd('|'));
                                                                        }
                                                                    });
                                                                }
                                                                objectData[nameData + "_UId"] = valuesDep;
                                                                objectData[nameData] = listName != null && listName.Count > 0 ? (string.Join(",", listName)).TrimEnd(',') : string.Empty;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            objectData[nameData + "_UId"] = null;
                                                            objectData[nameData] = string.Empty;
                                                        }

                                                    });
                                                }
                                            }
                                            else if (s.ControlType == "ListBox" && s.IsMultiple == false)
                                            {
                                                long? Svalues = itemValue == null ? null : (long)itemValue;
                                                if (Svalues != null)
                                                {
                                                    List<string?> listName = new List<string?>();
                                                    var nameList = attributeHeaderListModels.AttributeDetails.Where(a => a.AttributeDetailID == Svalues && a.DropDownTypeId == s.DataSourceTable).ToList();
                                                    if (nameList != null && nameList.Count() > 0)
                                                    {
                                                        nameList.ForEach(n =>
                                                        {
                                                            if (s.DataSourceTable == "Employee")
                                                            {
                                                                listName.Add((n?.AttributeDetailName + "|" + n?.Description + "|" + n?.DesignationName + "|" + n?.DepartmentName).TrimEnd('|'));
                                                            }
                                                            else if (s.DataSourceTable == "RawMatPurch")
                                                            {
                                                                listName.Add((n?.AttributeDetailName + "|" + n?.QcRefNo).TrimEnd('|'));
                                                            }
                                                            else if (s.DataSourceTable == "ReleaseProdOrderLine")
                                                            {
                                                                listName.Add((n?.AttributeDetailName + "|" + n?.ReplanRefNo).TrimEnd('|'));
                                                            }
                                                            else if (s.DataSourceTable == "FinishedProdOrderLine" || s.DataSourceTable == "FinishedProdOrderLineProductionInProgress" || s.DataSourceTable == "Location")
                                                            {
                                                                listName.Add(n?.NameList);
                                                            }
                                                            else
                                                            {
                                                                listName.Add((n?.AttributeDetailName + "|" + n?.Description).TrimEnd('|'));
                                                            }
                                                        });

                                                    }
                                                    objectData[attrName + "_UId"] = Svalues;
                                                    objectData[attrName] = listName != null && listName.Count > 0 ? (string.Join(",", listName)).TrimEnd(',') : string.Empty;
                                                }
                                                else
                                                {
                                                    objectData[attrName + "_UId"] = null;
                                                    objectData[attrName] = string.Empty;
                                                }
                                            }
                                            else if (s.ControlType == "DateEdit")
                                            {
                                                DateTime? values = itemValue == null ? null : (DateTime)itemValue;
                                                objectData[attrName] = values;
                                            }
                                            else if (s.ControlType == "TimeEdit")
                                            {
                                                TimeSpan? values = itemValue == null ? null : (TimeSpan)itemValue;
                                                objectData[attrName] = values;
                                            }
                                            else if (s.ControlType == "SpinEdit")
                                            {
                                                if (s.IsSpinEditType == "decimal")
                                                {
                                                    decimal? values = itemValue == null ? null : (decimal)itemValue;
                                                    objectData[attrName] = values;
                                                }
                                                else
                                                {
                                                    int? values = itemValue == null ? null : (int)itemValue;
                                                    objectData[attrName] = values;
                                                }
                                            }
                                            else if (s.ControlType == "GroupCheckBox")
                                            {
                                                string valuesData = string.Empty;
                                                valuesData += "<ul>";
                                                var _attributeGroupCheckBoxes = attributeHeaderListModels.AttributeGroupCheckBoxes.Where(g => g.AttributeId == s.AttributeId).ToList();
                                                if (_attributeGroupCheckBoxes.Count() > 0)
                                                {
                                                    _attributeGroupCheckBoxes.ForEach(r =>
                                                    {
                                                        var nameData = s.DynamicFormSectionAttributeId + "_" + s.AttributeId + "_" + r.AttributeGroupCheckBoxId + "_GroupCheckBox";
                                                        var NamesA = jsonObj.ContainsKey(nameData);
                                                        var itemValueA = jsonObj[nameData];
                                                        if (r.IsTextBox == true)
                                                        {
                                                            loadSubHeaders(r.SubAttributeHeaders, s, jsonObj, objectData, attributeHeaderListModels);
                                                        }
                                                        else
                                                        {
                                                            if (NamesA == true)
                                                            {
                                                                bool? valuesA = itemValueA == null ? false : (bool?)itemValueA;
                                                                if (valuesA == true)
                                                                {
                                                                    valuesData += "<li>" + r.Value + "</li>";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                //objectData[nameData] = r.Value.TrimEnd(',');
                                                            }

                                                        }
                                                    });
                                                    valuesData += "</ul>";
                                                    objectData[attrName] = valuesData.TrimEnd(',');
                                                }

                                            }
                                            else if (s.ControlType == "CheckBox")
                                            {
                                                bool? values = itemValue == null ? false : (bool)itemValue;
                                                objectData[attrName] = values;
                                                if (values == true)
                                                {
                                                    SubAttrsHeader = s.SubAttributeHeaders;
                                                }
                                            }
                                            else
                                            {
                                                objectData[attrName] = (string)itemValue;
                                            }
                                            loadSubHeaders(SubAttrsHeader, s, jsonObj, objectData, attributeHeaderListModels);
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (s.ControlTypeId == 2712)
                                {
                                    objectData[attrName] = s.DynamicFormSessionId;
                                }
                                else
                                {
                                    if (s.ControlType == "GroupCheckBox")
                                    {
                                        var _attributeGroupCheckBoxes = attributeHeaderListModels.AttributeGroupCheckBoxes.Where(g => g.AttributeId == s.AttributeId).ToList();
                                        if (_attributeGroupCheckBoxes.Count() > 0)
                                        {
                                            _attributeGroupCheckBoxes.ForEach(r =>
                                            {
                                                var nameData = s.DynamicFormSectionAttributeId + "_" + s.AttributeId + "_" + r.AttributeGroupCheckBoxId + "_GroupCheckBox";
                                                var NamesA = jsonObj.ContainsKey(nameData);
                                                var itemValueA = jsonObj[nameData];
                                                if (r.IsTextBox == true)
                                                {
                                                    loadSubHeaders(r.SubAttributeHeaders, s, jsonObj, objectData, attributeHeaderListModels);
                                                }
                                                else
                                                {


                                                }
                                            });
                                            objectData[attrName] = string.Empty;
                                        }
                                    }
                                    if (s.ControlType == "ComboBox" && s.IsPlantLoadDependency == true && s.AttributeHeaderDataSource.Count() > 0)
                                    {
                                        objectData[attrName] = string.Empty;
                                        s.AttributeHeaderDataSource.ForEach(dd =>
                                        {
                                            var nameData = s.DynamicFormSectionAttributeId + "_" + dd.AttributeHeaderDataSourceId + "_" + dd.DataSourceTable + "_Dependency";
                                            objectData[nameData] = string.Empty;
                                        });
                                    }
                                    else
                                    {
                                        if (s.ControlType == "CheckBox" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                                        {
                                            if (s.ControlType == "CheckBox")
                                            {
                                                objectData[attrName] = false;
                                                loadSubHeaders(s.SubAttributeHeaders, s, jsonObj, objectData, attributeHeaderListModels);
                                            }
                                            else
                                            {
                                                objectData[attrName] = string.Empty;
                                                var attrDetails = attributeHeaderListModels.AttributeDetails.Where(mm => mm.AttributeID == s.AttributeId && mm.DropDownTypeId == null).ToList();
                                                if (attrDetails != null && attrDetails.Count > 0)
                                                {
                                                    attrDetails.ForEach(u =>
                                                    {
                                                        if (u.SubAttributeHeaders != null && u.SubAttributeHeaders.Count() > 0)
                                                        {
                                                            loadSubHeaders(u.SubAttributeHeaders, s, jsonObj, objectData, attributeHeaderListModels);
                                                        }
                                                    });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (s.DataSourceTable == "ApplicationMasterParent")
                                            {
                                                objectData[attrName] = string.Empty;
                                                List<string?> nameDatas = new List<string?>();
                                                s.ApplicationMasterParents.ForEach(ab =>
                                                {
                                                    nameDatas.Add(s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_AppMasterPar");
                                                    RemoveApplicationMasterParentSingleNameItem(ab, s, nameDatas, attributeHeaderListModels.ApplicationMasterParent);
                                                    if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMasterParent" && s.IsDynamicFormGridDropdown == true)
                                                    {
                                                        var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                                        objectData[appendDependency] = string.Empty;
                                                    }
                                                });
                                                if (nameDatas != null && nameDatas.Count() > 0)
                                                {
                                                    nameDatas.ForEach(n =>
                                                    {
                                                        objectData[n] = string.Empty;
                                                    });
                                                }

                                            }
                                            else if (s.DataSourceTable == "ApplicationMaster")
                                            {
                                                objectData[attrName] = string.Empty;
                                                if (s.ApplicationMaster.Count() > 0)
                                                {
                                                    s.ApplicationMaster.ForEach(ab =>
                                                    {
                                                        var nameData = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_AppMaster";
                                                        objectData[nameData] = string.Empty;
                                                        if (s.ControlTypeId == 2702 && s.DropDownTypeId == "Data Source" && s.DataSourceTable == "ApplicationMaster" && s.IsDynamicFormGridDropdown == true)
                                                        {
                                                            var appendDependency = s.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_" + s.GridDropDownDynamicFormID + "_GridAppMaster";
                                                            objectData[appendDependency] = string.Empty;
                                                        }
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                if (s.ControlTypeId == 2710)
                                                {
                                                    objectData[attrName] = false;
                                                }
                                                else if (s.ControlTypeId == 2703)
                                                {
                                                    DateTime? values = null;
                                                    objectData[attrName] = values;
                                                }
                                                else if (s.ControlTypeId == 2709)
                                                {
                                                    objectData[attrName] = (TimeSpan?)null;
                                                }
                                                else
                                                {
                                                    objectData[attrName] = string.Empty;
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                        });
                    }
                    _dynamicformObjectDataLists.Add(objectData);
                });
            }
            return _dynamicformObjectDataLists;
        }
        void loadSubHeaders(List<AttributeHeader>? SubAttrsHeader, DynamicFormSectionAttribute dynamicFormSectionAttribute, dynamic jsonObjs, IDictionary<string, object> objectData, AttributeHeaderListModel attributeHeaderListModels)
        {
            if (SubAttrsHeader != null && SubAttrsHeader.Count > 0)
            {
                SubAttrsHeader.ForEach(d =>
                {
                    var subAttrName = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + d.SubDynamicAttributeName;
                    var SubNamess = jsonObjs.ContainsKey(subAttrName);
                    if (SubNamess == true)
                    {
                        var itemValues = jsonObjs[subAttrName];
                        if (itemValues is JArray)
                        {
                            var values = (JArray)itemValues;
                            List<long?> listDatas = values.ToObject<List<long?>>();
                            if (d.DataSourceTable == "DynamicGrid")
                            {
                                List<DropDownOptionsModel?> gridlistss = new List<DropDownOptionsModel?>();
                                gridlistss = attributeHeaderListModels.DropDownOptionsGridListModel.DropDownGridOptionsModel.Where(ax => ax.DynamicFormId == d.DynamicFormId).FirstOrDefault()?.DropDownOptionsModels;

                                string displayNames = string.Empty;

                                if (listDatas != null && listDatas.Count > 0)
                                {
                                    var collection = attributeHeaderListModels.DropDownOptionsGridListModel.ObjectData;
                                    listDatas.ForEach(l =>
                                    {
                                        if (collection != null)
                                        {
                                            foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                            {
                                                dynamic eod = v;
                                                var vv = eod.DynamicFormId;
                                                string listone = string.Empty;
                                                if (gridlistss != null && gridlistss.Count() > 0)
                                                {
                                                    var g1 = gridlistss.OrderBy(o => o.OrderBy).ToList();
                                                    g1.ForEach(q =>
                                                    {
                                                        var byName = (IDictionary<string, object>)v;
                                                        if (q.Value != null)
                                                        {
                                                            if (byName.ContainsKey(q.Value))
                                                            {
                                                                var names = (string)byName[q.Value]?.ToString();
                                                                listone += names + "|";
                                                            }
                                                        }
                                                    });
                                                }
                                                eod.DisplayItemsQuery = listone;
                                                if ((long?)eod.AttributeDetailID == l)
                                                {
                                                    displayNames += eod.DisplayItemsQuery + ",";
                                                }
                                            }
                                        }
                                    });
                                }
                                objectData[subAttrName + "_UId"] = listDatas;
                                objectData[subAttrName] = displayNames.TrimEnd(',');

                            }
                            else
                            {
                                if (d.IsMultiple == true || d.ControlType == "TagBox")
                                {
                                    var ValueSet = string.Empty;
                                    if (listDatas != null && listDatas.Count > 0 && d.SubAttributeDetails != null && d.SubAttributeDetails.Count > 0)
                                    {
                                        var nameList = d.SubAttributeDetails.Where(a => listDatas.Contains(a.AttributeDetailID) && a.AttributeDetailName != null && a.DropDownTypeId == d.DataSourceTable).ToList();
                                        List<string?> listName = new List<string?>();
                                        if (nameList != null && nameList.Count() > 0)
                                        {
                                            nameList.ForEach(n =>
                                            {
                                                if (d.DataSourceTable == "Employee")
                                                {
                                                    listName.Add((n?.AttributeDetailName + "|" + n?.Description + "|" + n?.DesignationName + "|" + n?.DepartmentName).TrimEnd('|'));
                                                }
                                                else if (d.DataSourceTable == "RawMatPurch")
                                                {
                                                    listName.Add((n?.AttributeDetailName + "|" + n?.QcRefNo).TrimEnd('|'));
                                                }
                                                else if (d.DataSourceTable == "ReleaseProdOrderLine")
                                                {
                                                    listName.Add((n?.AttributeDetailName + "|" + n?.ReplanRefNo).TrimEnd('|'));
                                                }
                                                else if (d.DataSourceTable == "FinishedProdOrderLine" || d.DataSourceTable == "FinishedProdOrderLineProductionInProgress" || d.DataSourceTable == "Location")
                                                {
                                                    listName.Add(n?.NameList);
                                                }
                                                else
                                                {
                                                    listName.Add((n?.AttributeDetailName + "|" + n?.Description).TrimEnd('|'));
                                                }
                                            });

                                        }
                                        ValueSet = listName != null && listName.Count > 0 ? (string.Join(",", listName)).TrimEnd(',') : string.Empty;
                                    }
                                    objectData[subAttrName + "_UId"] = listDatas;
                                    objectData[subAttrName] = ValueSet;
                                }
                            }
                        }
                        else if (d.ControlType == "ListBox" && d.IsMultiple == false)
                        {
                            var ValueSet = string.Empty;
                            long? values = itemValues == null ? null : (long)itemValues;
                            if (values > 0 && d.SubAttributeDetails != null && d.SubAttributeDetails.Count > 0)
                            {
                                var nameList = d.SubAttributeDetails.Where(a => a.AttributeDetailID == values && a.AttributeDetailName != null && a.DropDownTypeId == d.DataSourceTable).ToList();
                                List<string?> listName = new List<string?>();
                                if (nameList != null && nameList.Count() > 0)
                                {
                                    nameList.ForEach(n =>
                                    {
                                        if (d.DataSourceTable == "Employee")
                                        {
                                            listName.Add((n?.AttributeDetailName + "|" + n?.Description + "|" + n?.DesignationName + "|" + n?.DepartmentName).TrimEnd('|'));
                                        }
                                        else if (d.DataSourceTable == "RawMatPurch")
                                        {
                                            listName.Add((n?.AttributeDetailName + "|" + n?.QcRefNo).TrimEnd('|'));
                                        }
                                        else if (d.DataSourceTable == "ReleaseProdOrderLine")
                                        {
                                            listName.Add((n?.AttributeDetailName + "|" + n?.ReplanRefNo).TrimEnd('|'));
                                        }
                                        else if (d.DataSourceTable == "FinishedProdOrderLine" || d.DataSourceTable == "FinishedProdOrderLineProductionInProgress" || d.DataSourceTable == "Location")
                                        {
                                            listName.Add(n?.NameList);
                                        }
                                        else
                                        {
                                            listName.Add((n?.AttributeDetailName + "|" + n?.Description).TrimEnd('|'));
                                        }
                                    });

                                }
                                ValueSet = listName != null && listName.Count > 0 ? (string.Join(",", listName)).TrimEnd(',') : string.Empty;
                            }
                            objectData[subAttrName + "_UId"] = values;
                            objectData[subAttrName] = ValueSet;
                        }
                        else
                        {
                            if (d.ControlType == "TextBox" || d.ControlType == "Memo")
                            {
                                var values = (string)itemValues;
                                objectData[subAttrName] = values;
                            }
                            else if (d.ControlType == "QR Code")
                            {
                                var values = (string)itemValues;
                                objectData[subAttrName] = values;
                            }
                            else if (d.ControlType == "ComboBox" || d.ControlType == "Radio" || d.ControlType == "RadioGroup")
                            {
                                long? values = itemValues == null ? null : (long)itemValues;
                                var ValueSet = string.Empty;
                                if (values > 0)
                                {
                                    if (d.DataSourceTable == "DynamicGrid")
                                    {
                                        List<DropDownOptionsModel?> gridlistss = new List<DropDownOptionsModel?>();
                                        gridlistss = attributeHeaderListModels.DropDownOptionsGridListModel.DropDownGridOptionsModel.Where(ax => ax.DynamicFormId == d.DynamicFormId).FirstOrDefault()?.DropDownOptionsModels;
                                        var displayNames = string.Empty;
                                        var collection = attributeHeaderListModels.DropDownOptionsGridListModel.ObjectData;
                                        if (collection != null)
                                        {
                                            foreach (var v in (collection as IEnumerable<ExpandoObject>))
                                            {
                                                dynamic eod = v;
                                                var vv = eod.DynamicFormId;
                                                var AttributeDetailIds = eod.AttributeDetailID;
                                                string listone = string.Empty;
                                                if (gridlistss != null && gridlistss.Count() > 0)
                                                {
                                                    var g1 = gridlistss.OrderBy(o => o.OrderBy).ToList();
                                                    g1.ForEach(q =>
                                                    {
                                                        var byName = (IDictionary<string, object>)v;
                                                        if (q.Value != null)
                                                        {
                                                            if (byName.ContainsKey(q.Value))
                                                            {
                                                                var names = (string)byName[q.Value]?.ToString();
                                                                listone += names + "|";
                                                            }
                                                        }
                                                    });
                                                }
                                                eod.DisplayItemsQuery = listone;
                                                if ((long?)eod.AttributeDetailID == values)
                                                {
                                                    displayNames += eod.DisplayItemsQuery;
                                                }
                                            }
                                        }
                                        ValueSet = displayNames.TrimEnd(',');
                                    }
                                    else
                                    {
                                        if (d.SubAttributeDetails != null && d.SubAttributeDetails.Count > 0)
                                        {
                                            var nameList = d.SubAttributeDetails.Where(a => a.AttributeDetailID == values && a.AttributeDetailName != null && a.DropDownTypeId == d.DataSourceTable).ToList();

                                            List<string?> listName = new List<string?>();
                                            if (nameList != null && nameList.Count() > 0)
                                            {
                                                nameList.ForEach(n =>
                                                {
                                                    if (d.DataSourceTable == "Employee")
                                                    {
                                                        listName.Add((n?.AttributeDetailName + "|" + n?.Description + "|" + n?.DesignationName + "|" + n?.DepartmentName).TrimEnd('|'));
                                                    }
                                                    else if (d.DataSourceTable == "RawMatPurch")
                                                    {
                                                        listName.Add((n?.AttributeDetailName + "|" + n?.QcRefNo).TrimEnd('|'));
                                                    }
                                                    else if (d.DataSourceTable == "ReleaseProdOrderLine")
                                                    {
                                                        listName.Add((n?.AttributeDetailName + "|" + n?.ReplanRefNo).TrimEnd('|'));
                                                    }
                                                    else if (d.DataSourceTable == "FinishedProdOrderLine" || d.DataSourceTable == "FinishedProdOrderLineProductionInProgress" || d.DataSourceTable == "Location")
                                                    {
                                                        listName.Add(n?.NameList);
                                                    }
                                                    else
                                                    {
                                                        listName.Add((n?.AttributeDetailName + "|" + n?.Description).TrimEnd('|'));
                                                    }
                                                });

                                            }
                                            ValueSet = listName != null && listName.Count > 0 ? (string.Join(",", listName)).TrimEnd(',') : string.Empty;
                                        }
                                    }
                                }
                                objectData[subAttrName + "_UId"] = values;
                                objectData[subAttrName] = ValueSet;

                            }
                            else if (d.ControlType == "CheckBox")
                            {
                                bool? values = itemValues == null ? false : (bool)itemValues;
                                objectData[subAttrName] = values;

                            }
                            else if (d.ControlType == "DateEdit")
                            {
                                DateTime? values = itemValues == null ? null : (DateTime)itemValues;
                                objectData[subAttrName] = values;

                            }
                            else if (d.ControlType == "TimeEdit")
                            {
                                TimeSpan? values = itemValues == null ? null : (TimeSpan)itemValues;
                                objectData[subAttrName] = values;
                            }
                            else if (d.ControlType == "SpinEdit")
                            {
                                if (d.IsAttributeSpinEditType == "decimal")
                                {
                                    decimal? values = itemValues == null ? null : (decimal)itemValues;
                                    objectData[subAttrName] = values;
                                }
                                else
                                {
                                    int? values = itemValues == null ? null : (int)itemValues;
                                    objectData[subAttrName] = values;
                                }
                            }
                        }
                        if (d.DataSourceTable == "ApplicationMaster" && d.SubApplicationMaster != null && d.SubApplicationMaster.Count() > 0)
                        {
                            d.SubApplicationMaster.ForEach(ab =>
                            {
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
                                            objectData[appendAppMaster + "_UId"] = listDatas;
                                            objectData[appendAppMaster] = ValueSet;
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
                                            objectData[appendAppMaster + "_UId"] = values;
                                            objectData[appendAppMaster] = ValueSet;
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
                                                objectData[appendAppMaster + "_UId"] = values;
                                                objectData[appendAppMaster] = ValueSet;
                                            }
                                            else
                                            {
                                                objectData[appendAppMaster + "_UId"] = null;
                                                objectData[appendAppMaster] = string.Empty;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    objectData[appendAppMaster] = string.Empty;
                                }
                            });
                        }
                    }
                    else
                    {
                        if (d.DataSourceTable == "ApplicationMaster" && d.SubApplicationMaster != null && d.SubApplicationMaster.Count() > 0)
                        {
                            objectData[subAttrName] = string.Empty;
                            d.SubApplicationMaster.ForEach(ab =>
                            {
                                var appendAppMaster = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + d.SubDynamicAttributeName + "_" + ab.ApplicationMasterCodeId + "_SubAppMaster";
                                objectData[appendAppMaster] = string.Empty;
                            });
                        }
                        else
                        {
                            objectData[subAttrName] = d.ControlType == "CheckBox" ? false : string.Empty;
                        }
                    }

                });
            }
        }

        void loadApplicationMasterParentData(dynamic jsonObj, DynamicFormSectionAttribute s, string nameData, IDictionary<string, object> objectData, List<AttributeDetails> PlantDependencySubAttributeDetails)
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
                        objectData[nameData + "_UId"] = listData;
                        objectData[nameData] = listName != null && listName.Count > 0 ? string.Join(",", listName) : string.Empty;
                    }
                    else
                    {
                        objectData[nameData + "_UId"] = null;
                        objectData[nameData] = string.Empty;
                    }
                }
                else
                {
                    if (s.ControlType == "ComboBox")
                    {
                        long? values = itemValue == null ? -1 : (long)itemValue;
                        var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                        objectData[nameData] = listss;
                        objectData[nameData + "_UId"] = values;
                    }
                    else
                    {
                        if (s.ControlType == "ListBox" && s.IsMultiple == false)
                        {
                            long? values = itemValue == null ? -1 : (long)itemValue;
                            var listss = PlantDependencySubAttributeDetails != null ? PlantDependencySubAttributeDetails.Where(v => v.DropDownTypeId == s.DataSourceTable && v.AttributeDetailID == values).FirstOrDefault()?.AttributeDetailName : string.Empty;
                            objectData[nameData] = listss;
                            objectData[nameData + "_UId"] = values;
                        }
                        else
                        {
                            objectData[nameData + "_UId"] = null;
                            objectData[nameData] = string.Empty;
                        }
                    }
                }
            }
            else
            {
                objectData[nameData] = string.Empty;
            }


        }
        void RemoveApplicationMasterParentSingleItem(ApplicationMasterParent applicationMasterParent, DynamicFormSectionAttribute dynamicFormSectionAttribute, List<DropDownOptionsModel> dataColumnNames, List<ApplicationMasterParent> applicationMasterParents)
        {
            if (applicationMasterParent != null)
            {
                var listss = applicationMasterParents.FirstOrDefault(f => f.ParentId == applicationMasterParent.ApplicationMasterParentCodeId);
                if (listss != null)
                {
                    var nameData = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + listss.ApplicationMasterParentCodeId + "_AppMasterPar";
                    dataColumnNames.Add(new DropDownOptionsModel() { IsMultiple = dynamicFormSectionAttribute.IsMultiple, DynamicFormSectionAttributeId = dynamicFormSectionAttribute.DynamicFormSectionAttributeId, ControlType = dynamicFormSectionAttribute.ControlType, Value = nameData, Text = listss.ApplicationMasterName, Type = "DynamicForm", OrderBy = dynamicFormSectionAttribute.GridDisplaySeqNo, IsVisible = dynamicFormSectionAttribute.IsDisplayTableHeader.Value });
                    RemoveApplicationMasterParentSingleItem(listss, dynamicFormSectionAttribute, dataColumnNames, applicationMasterParents);
                }
            }
        }
        void RemoveApplicationMasterParentSingleNameItem(ApplicationMasterParent applicationMasterParent, DynamicFormSectionAttribute dynamicFormSectionAttribute, List<string?> dataColumnNames, List<ApplicationMasterParent> applicationMasterParents)
        {
            if (applicationMasterParent != null)
            {
                var listss = applicationMasterParents.FirstOrDefault(f => f.ParentId == applicationMasterParent.ApplicationMasterParentCodeId);
                if (listss != null)
                {
                    var nameData = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + listss.ApplicationMasterParentCodeId + "_AppMasterPar";
                    dataColumnNames.Add(nameData);
                    RemoveApplicationMasterParentSingleNameItem(listss, dynamicFormSectionAttribute, dataColumnNames, applicationMasterParents);
                }
            }
        }
        public List<DropDownOptionsModel> UpdateDataSyncDatas(AttributeHeaderListModel attributeHeaderListModel, List<DropDownOptionsModel> dataColumnNamesSync, DynamicForm _dynamicForm)
        {
            try
            {
                dataColumnNamesSync = new List<DropDownOptionsModel>();
                dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "DynamicFormDataId", Text = "DynamicFormDataId", Type = "Form" });
                dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "IsFileprofileTypeDocument", Text = "IsFileprofileTypeDocument", Type = "Form" });
                dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "SortOrderByNo", Text = "No", Type = "Form" });
                dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "ProfileNo", Text = "Profile No", Type = "Form" });
                if (_dynamicForm != null)
                {
                    if (attributeHeaderListModel != null && attributeHeaderListModel.DynamicFormSectionAttribute != null && attributeHeaderListModel.DynamicFormSectionAttribute.Count > 0)
                    {
                        attributeHeaderListModel.DynamicFormSectionAttribute.ForEach(a =>
                        {
                            if (a.DataSourceTable == "ApplicationMaster")
                            {
                                if (a.ApplicationMaster.Count() > 0)
                                {
                                    a.ApplicationMaster.ForEach(ab =>
                                    {
                                        var nameData = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_AppMaster";
                                        dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = a.IsMultiple, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = a.ControlType, Value = nameData, Text = ab.ApplicationMasterName, Type = "DynamicForm", OrderBy = a.GridDisplaySeqNo });
                                        if (a.ControlTypeId == 2702 && a.DropDownTypeId == "Data Source" && a.DataSourceTable == "ApplicationMaster" && a.IsDynamicFormGridDropdown == true)
                                        {
                                            var appendDependency = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterCodeId + "_" + a.GridDropDownDynamicFormID + "_GridAppMaster";
                                            dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = a.IsMultiple, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = a.ControlType, Value = appendDependency, Text = a.GridDropDownDynamicFormName, Type = "DynamicForm", OrderBy = a.GridDisplaySeqNo });
                                        }
                                    });
                                }
                            }
                            else if (a.ControlType == "GroupCheckBox")
                            {
                                dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = a.IsMultiple, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = a.ControlType, IsDynamicGrid = a.ControlTypeId == 2712 ? true : false, Value = a.DynamicAttributeName, Text = a.DisplayName, Type = "DynamicForm", Id = a.DynamicFormSectionAttributeId, OrderBy = a.GridDisplaySeqNo });

                                var _attributeGroupCheckBoxes = attributeHeaderListModel.AttributeGroupCheckBoxes.Where(s => s.AttributeId == a.AttributeId).ToList();
                                if (_attributeGroupCheckBoxes.Count() > 0)
                                {
                                    _attributeGroupCheckBoxes.ForEach(z =>
                                    {
                                        if (z.IsTextBox == true)
                                        {
                                            if (z.SubAttributeHeaders.Count() > 0)
                                            {
                                                z.SubAttributeHeaders.ForEach(w =>
                                                {
                                                    if (w.DataSourceTable == "ApplicationMaster" && w.SubApplicationMaster != null && w.SubApplicationMaster.Count() > 0)
                                                    {
                                                        w.SubApplicationMaster.ForEach(ab =>
                                                        {
                                                            var appendAppMaster = a.DynamicFormSectionAttributeId + "_" + w.SubDynamicAttributeName + "_" + ab.ApplicationMasterCodeId + "_SubAppMaster";
                                                            dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = w.IsMultiple, IsSubForm = true, AttributeDetailID = w.AttributeID, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = w.ControlType, Value = appendAppMaster, Text = ab.ApplicationMasterName, Type = "DynamicForm", OrderBy = a.GridDisplaySeqNo });

                                                        });
                                                    }
                                                    else
                                                    {
                                                        var nameData = a.DynamicFormSectionAttributeId + "_" + w.SubDynamicAttributeName;
                                                        dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = w.IsMultiple, IsSubForm = true, AttributeDetailID = w.AttributeID, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = w.ControlType, Value = nameData, Text = w.Description, Type = "DynamicForm", OrderBy = a.GridDisplaySeqNo });

                                                    }
                                                });
                                            }
                                        }
                                        else
                                        {

                                        }
                                    });

                                }

                            }
                            else if (a.DataSourceTable == "ApplicationMasterParent")
                            {
                                if (a.ApplicationMasterParents.Count() > 0)
                                {
                                    a.ApplicationMasterParents.ForEach(ab =>
                                    {
                                        var nameData = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_AppMasterPar";
                                        dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = a.IsMultiple, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = a.ControlType, Value = nameData, Text = ab.ApplicationMasterName, Type = "DynamicForm", OrderBy = a.GridDisplaySeqNo });
                                        RemoveApplicationMasterParentSingleItemSyncData(ab, a, dataColumnNamesSync, attributeHeaderListModel.ApplicationMasterParent);
                                        if (a.ControlTypeId == 2702 && a.DropDownTypeId == "Data Source" && a.DataSourceTable == "ApplicationMasterParent" && a.IsDynamicFormGridDropdown == true)
                                        {
                                            var appendDependency = a.DynamicFormSectionAttributeId + "_" + ab.ApplicationMasterParentCodeId + "_" + a.GridDropDownDynamicFormID + "_GridAppMaster";
                                            dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = a.IsMultiple, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = a.ControlType, Value = appendDependency, Text = a.GridDropDownDynamicFormName, Type = "DynamicForm", OrderBy = a.GridDisplaySeqNo });
                                        }
                                    });
                                }
                            }
                            else if (a.ControlType == "CheckBox" || a.ControlType == "Radio" || a.ControlType == "RadioGroup")
                            {

                                dataColumnNamesSync.Add(new DropDownOptionsModel() { DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = a.ControlType, IsDynamicGrid = a.ControlTypeId == 2712 ? true : false, Value = a.DynamicAttributeName, Text = a.DisplayName, Type = "DynamicForm", Id = a.DynamicFormSectionAttributeId, OrderBy = a.GridDisplaySeqNo });
                                if (a.ControlType == "Radio" || a.ControlType == "RadioGroup")
                                {
                                    var attrDetails = attributeHeaderListModel.AttributeDetails.Where(mm => mm.AttributeID == a.AttributeId && mm.DropDownTypeId == null).ToList();
                                    if (attrDetails.Count() > 0)
                                    {
                                        attrDetails.ForEach(q =>
                                        {
                                            if (q.SubAttributeHeaders.Count() > 0)
                                            {
                                                q.SubAttributeHeaders.ForEach(w =>
                                                {
                                                    var nameData = a.DynamicFormSectionAttributeId + "_" + w.SubDynamicAttributeName;
                                                    dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = w.IsMultiple, IsSubForm = true, AttributeDetailID = w.AttributeID, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = w.ControlType, Value = nameData, Text = w.Description, Type = "DynamicForm", OrderBy = a.GridDisplaySeqNo });
                                                    if (w.DataSourceTable == "ApplicationMaster" && w.SubApplicationMaster != null && w.SubApplicationMaster.Count() > 0)
                                                    {
                                                        w.SubApplicationMaster.ForEach(ab =>
                                                        {
                                                            var appendAppMaster = a.DynamicFormSectionAttributeId + "_" + w.SubDynamicAttributeName + "_" + ab.ApplicationMasterCodeId + "_SubAppMaster";
                                                            dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = w.IsMultiple, IsSubForm = true, AttributeDetailID = w.AttributeID, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = w.ControlType, Value = appendAppMaster, Text = ab.ApplicationMasterName, Type = "DynamicForm", OrderBy = a.GridDisplaySeqNo });

                                                        });
                                                    }
                                                });
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    if (a.SubAttributeHeaders.Count() > 0)
                                    {
                                        a.SubAttributeHeaders.ForEach(w =>
                                        {
                                            var nameData = a.DynamicFormSectionAttributeId + "_" + w.SubDynamicAttributeName;
                                            dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = w.IsMultiple, IsSubForm = true, AttributeDetailID = w.AttributeID, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = w.ControlType, Value = nameData, Text = w.Description, Type = "DynamicForm", OrderBy = a.GridDisplaySeqNo });
                                            if (w.DataSourceTable == "ApplicationMaster" && w.SubApplicationMaster != null && w.SubApplicationMaster.Count() > 0)
                                            {
                                                w.SubApplicationMaster.ForEach(ab =>
                                                {
                                                    var appendAppMaster = a.DynamicFormSectionAttributeId + "_" + w.SubDynamicAttributeName + "_" + ab.ApplicationMasterCodeId + "_SubAppMaster";
                                                    dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = w.IsMultiple, IsSubForm = true, AttributeDetailID = w.AttributeID, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = w.ControlType, Value = appendAppMaster, Text = ab.ApplicationMasterName, Type = "DynamicForm", OrderBy = a.GridDisplaySeqNo });

                                                });
                                            }

                                        });
                                    }
                                }
                            }
                            else
                            {
                                dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = a.IsMultiple, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = a.ControlType, IsDynamicGrid = a.ControlTypeId == 2712 ? true : false, Value = a.DynamicAttributeName, Text = a.DisplayName, Type = "DynamicForm", Id = a.DynamicFormSectionAttributeId, OrderBy = a.GridDisplaySeqNo });

                            }
                            if (a.ControlType == "ComboBox" && a.IsPlantLoadDependency == true && a.AttributeHeaderDataSource.Count() > 0)
                            {
                                a.AttributeHeaderDataSource.ForEach(dd =>
                                {
                                    var nameData = a.DynamicFormSectionAttributeId + "_" + dd.AttributeHeaderDataSourceId + "_" + dd.DataSourceTable + "_Dependency";
                                    dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = a.IsDependencyMultiple, DynamicFormSectionAttributeId = a.DynamicFormSectionAttributeId, ControlType = a.ControlType, Value = nameData, Text = dd.DisplayName, Type = "DynamicForm", OrderBy = a.GridDisplaySeqNo });
                                });
                            }

                        });
                        var isSet = dataColumnNamesSync.Where(w => w.Type == "DynamicForm" && w.OrderBy > 0).ToList();
                        if (isSet.Count() > 0)
                        {
                            dataColumnNamesSync = dataColumnNamesSync.OrderBy(o => o.OrderBy).ToList();
                        }
                        if (_dynamicForm != null)
                        {
                            if (_dynamicForm.IsApproval == true)
                            {
                                dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "CurrentUserName", Text = "Current User", Type = "Form" });
                            }
                        }
                        dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "ModifiedBy", Text = "Modified By", Type = "Form" });
                        dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "ModifiedDate", Text = "Modified Date", Type = "Form" });
                        if (_dynamicForm != null)
                        {
                            if (_dynamicForm.IsApproval == true)
                            {
                                dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "StatusName", Text = "Status", Type = "Form" });
                            }
                        }
                    }
                    else
                    {
                        if (_dynamicForm != null)
                        {
                            if (_dynamicForm.IsApproval == true)
                            {
                                dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "CurrentUserName", Text = "Current User", Type = "Form" });
                            }
                        }
                        dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "ModifiedBy", Text = "Modified By", Type = "Form" });
                        dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "ModifiedDate", Text = "Modified Date", Type = "Form" });
                        if (_dynamicForm != null)
                        {
                            if (_dynamicForm.IsApproval == true)
                            {
                                dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "StatusName", Text = "Status", Type = "Form" });
                            }
                        }
                    }
                }
                else
                {
                    dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "ModifiedBy", Text = "ModifiedBy", Type = "Form" });
                    dataColumnNamesSync.Add(new DropDownOptionsModel() { Value = "ModifiedDate", Text = "ModifiedDate", Type = "Form" });
                }
                

            }
            catch (Exception ex)
            {

            }
            return dataColumnNamesSync;
        }
        void RemoveApplicationMasterParentSingleItemSyncData(ApplicationMasterParent applicationMasterParent, DynamicFormSectionAttribute dynamicFormSectionAttribute, List<DropDownOptionsModel> dataColumnNamesSync, List<ApplicationMasterParent> applicationMasterParents)
        {
            if (applicationMasterParent != null)
            {
                var listss = applicationMasterParents.FirstOrDefault(f => f.ParentId == applicationMasterParent.ApplicationMasterParentCodeId);
                if (listss != null)
                {
                    var nameData = dynamicFormSectionAttribute.DynamicFormSectionAttributeId + "_" + listss.ApplicationMasterParentCodeId + "_AppMasterPar";
                    dataColumnNamesSync.Add(new DropDownOptionsModel() { IsMultiple = dynamicFormSectionAttribute.IsMultiple, DynamicFormSectionAttributeId = dynamicFormSectionAttribute.DynamicFormSectionAttributeId, ControlType = dynamicFormSectionAttribute.ControlType, Value = nameData, Text = listss.ApplicationMasterName, Type = "DynamicForm", OrderBy = dynamicFormSectionAttribute.GridDisplaySeqNo });
                    RemoveApplicationMasterParentSingleItemSyncData(listss, dynamicFormSectionAttribute, dataColumnNamesSync, applicationMasterParents);
                }
            }
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

    }

}
