using AC.SD.Core.Pages.Attribute;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Helpers;
using DevExpress.Blazor;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting.Native;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static Google.Cloud.Firestore.V1.StructuredAggregationQuery.Types.Aggregation.Types;
using static Google.Cloud.Firestore.V1.TransactionOptions.Types;
using static iText.Svg.SvgConstants;
namespace AC.SD.Core.Helpers
{
    public class DataGridDynamicForm
    {
        public int DropDownBoxRenderForm(string DataDynamicAttributeName, string DisplayName, int j, Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder editor, DynamicFormSectionAttribute s, object? valueData, List<AttributeDetails> attributeDetailss, ApplicationUser applicationUser, PropertyInfo property, object Data,
            object DataGridObject, ViewEmployee _userEmployee, AttributeHeaderListModel _AttributeHeader, bool? isAccessOnly, bool isReadonly, List<DropDownBoxGridModel> _DropDownBoxGridModel, bool? isSubmit)
        {
            int isRequiredCount = 0;
            var propertyMain = Data.GetType().GetProperty(DataDynamicAttributeName);
            if (propertyMain != null)
            {
                var values = (long?)propertyMain.GetValue(Data);
                List<DropDownOptionsModel?> gridlistss = new List<DropDownOptionsModel?>();
                List<ExpandoObject>? _dynamicformObjectDataList = new List<ExpandoObject>();
                DataGridObject.GetType().GetProperty(DataDynamicAttributeName).SetValue(DataGridObject, null);
                gridlistss = _AttributeHeader.DropDownOptionsGridListModel.DropDownGridOptionsModel.Where(ax => ax.DynamicFormId == s.DynamicFormGridDropDownId).FirstOrDefault()?.DropDownOptionsModels;
                if (s.DataSourceTable == "DynamicGrid")
                {
                    var collection = _AttributeHeader.DropDownOptionsGridListModel.ObjectData;
                    if (collection != null)
                    {
                        foreach (var v in (collection as IEnumerable<ExpandoObject>))
                        {
                            dynamic eod = v;
                            var vv = eod.DynamicFormId;
                            var AttributeDetailIds = eod.AttributeDetailID;
                            if ((long?)vv == s.DynamicFormGridDropDownId)
                            {
                                _dynamicformObjectDataList.Add(v);
                            }
                            if (values > 0)
                            {
                                if ((long?)values == (long?)AttributeDetailIds)
                                {
                                    DataGridObject.GetType().GetProperty(DataDynamicAttributeName).SetValue(DataGridObject, v);
                                }
                            }
                        }

                    }
                }
                else
                {
                    if (values > 0)
                    {
                        var NameList = attributeDetailss.Where(mm => mm.AttributeDetailID == values && mm.DropDownTypeId == s.DataSourceTable).FirstOrDefault();
                        DataGridObject.GetType().GetProperty(DataDynamicAttributeName).SetValue(DataGridObject, NameList);
                    }
                }

                var Gridaccess = Expression.Property(Expression.Constant(DataGridObject), DataDynamicAttributeName);
                var Gridlambda = Expression.Lambda(typeof(Func<>).MakeGenericType(typeof(object)), Gridaccess);
                var Gridproperty = DataGridObject.GetType().GetProperty(DataDynamicAttributeName);
                var GridvalueData = Gridproperty.GetValue(DataGridObject);
                var GridDropBoxData = _DropDownBoxGridModel.FirstOrDefault(f => f.AttrName == DataDynamicAttributeName);

                if (valueData == null && s.IsSetDefaultValue == true)
                {
                    if (s.DataSourceTable == "Employee")
                    {
                        var exitsDefaultData = attributeDetailss.FirstOrDefault(f => f.AttributeDetailID == applicationUser.UserID);
                        if (exitsDefaultData != null)
                        {
                            valueData = applicationUser.UserID;
                            property.SetValue(Data, valueData);
                            Gridproperty.SetValue(DataGridObject, exitsDefaultData);
                        }
                    }
                    if (s.DataSourceTable == "Plant" && _userEmployee != null && _userEmployee.PlantID > 0)
                    {
                        var exitsDefaultData = attributeDetailss.FirstOrDefault(f => f.AttributeDetailID == _userEmployee.PlantID);
                        if (exitsDefaultData != null)
                        {
                            valueData = _userEmployee.PlantID;
                            property.SetValue(Data, valueData);
                            Gridproperty.SetValue(DataGridObject, exitsDefaultData);
                        }
                    }
                }

                editor.OpenComponent<DxDropDownBox>(0);
                editor.AddAttribute(1, "DropDownWidthMode", DropDownWidthMode.EditorWidth);
                editor.AddAttribute(2, "NullText", "Select " + DisplayName + "..");
                editor.AddAttribute(3, "ClearButtonDisplayMode", DataEditorClearButtonDisplayMode.Auto);
                editor.AddAttribute(4, "QueryDisplayText", s.DataSourceTable == "DynamicGrid" ? QueryDisplayDynamicComboBoxText : QueryDisplayComboBoxText);
                editor.AddAttribute(5, "CssClass", "cw-480");
                editor.AddAttribute(6, "Value", GridvalueData);
                if (isAccessOnly == true)
                {
                    editor.AddAttribute(7, "Enabled", s.IsDefaultReadOnly == true ? false : isReadonly);
                }
                else
                {
                    editor.AddAttribute(8, "Enabled", isAccessOnly);
                }
                editor.AddAttribute(9, "DropDownVisible", false);
                editor.AddAttribute(10, "ValueExpression", Gridlambda);
                editor.AddAttribute(11, "ValueChanged", EventCallback.Factory.Create<object>(this, str => OnGridValueChanged(str, DataDynamicAttributeName, s.DataSourceTable, Data, DataGridObject)));
                if (s.IsDefaultReadOnly == false)
                {
                    if (s.IsRequired == true && isReadonly == true && isAccessOnly == true)
                    {
                        if (valueData == null)
                        {
                            isRequiredCount += 1;
                            if (isSubmit == true)
                            {
                            }
                        }
                    }
                }
                CreateDropDownBodyTemplateGrid(editor, _DropDownBoxGridModel, GridDropBoxData, s.DataSourceTable, DataDynamicAttributeName, gridlistss, _dynamicformObjectDataList, attributeDetailss, false);
                editor.CloseComponent();
                if (s.IsDefaultReadOnly == false)
                {
                    if (s.IsRequired == true && isReadonly == true && isAccessOnly == true)
                    {
                        if (valueData == null && isSubmit == true)
                        {
                            editor.OpenElement(13, "span");
                            editor.AddAttribute(14, "class", "text-danger");
                            editor.AddContent(15, !string.IsNullOrEmpty(s.RequiredMessage) ? s.RequiredMessage : "This field is Required");
                            editor.CloseElement();
                        }
                    }
                }
            }
            return isRequiredCount;
        }
        public void CreateDropDownBodyTemplateGrid(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder editor, List<DropDownBoxGridModel> _DropDownBoxGridModel, DropDownBoxGridModel GridDropBoxData, string DataSourceTable, string DynamicAttributeName, List<DropDownOptionsModel?> gridlistss, List<ExpandoObject>? _dynamicformObjectDataList, List<AttributeDetails> attributeDetailss, bool isMultiple)
        {
            editor.AddAttribute(12, "DropDownBodyTemplate", (RenderFragment<DropDownBoxTemplateContext>)((context) => (b) =>
            {
                b.OpenComponent<DxGrid>(0);
                b.AddAttribute(1, "Data", DataSourceTable == "DynamicGrid" ? _dynamicformObjectDataList : attributeDetailss);
                b.AddAttribute(2, "KeyFieldName", "AttributeDetailID");
                b.AddAttribute(3, "HighlightRowOnHover", true);
                b.AddAttribute(4, "SearchTextParseMode", GridSearchTextParseMode.GroupWordsByAnd);
                b.AddAttribute(5, "SelectionMode", isMultiple == true ? GridSelectionMode.Multiple : GridSelectionMode.Single);
                b.AddAttribute(6, "AllowSelectRowByClick", true);
                b.AddAttribute(7, "RowClick", EventCallback.Factory.Create<GridRowClickEventArgs>(this, item => OnRowclick(item, context.DropDownBox)));
                b.AddAttribute(8, "FocusedRowEnabled", true);
                b.AddAttribute(9, "TextWrapEnabled", false);
                b.AddAttribute(10, "CssClass", "templateGrid");
                b.AddAttribute(11, "PagerNavigationMode", PagerNavigationMode.InputBox);
                b.AddAttribute(12, "PageSizeSelectorVisible", true);
                b.AddAttribute(13, "PageSizeSelectorAllRowsItemVisible", true);
                b.AddAttribute(14, "PageSizeSelectorItems", new int[] { 5, 10, 20 });
                b.AddAttribute(15, "PageSize", GridDropBoxData != null ? GridDropBoxData.PageSize : 10);
                b.AddAttribute(16, "PageSizeChanged", EventCallback.Factory.Create<int>(this, item => OnPageSizeChanged(item, DynamicAttributeName, _DropDownBoxGridModel)));
                b.AddAttribute(17, "searchText", GridDropBoxData != null ? GridDropBoxData.SearchText : null);
                b.AddAttribute(18, "SearchTextChanged", EventCallback.Factory.Create<string>(this, item => OnSearchTextChanged(item, DynamicAttributeName, _DropDownBoxGridModel)));
                b.AddAttribute(19, "PageIndex", GridDropBoxData != null ? GridDropBoxData.GridPageIndex : 0);
                b.AddAttribute(20, "PageIndexChanged", EventCallback.Factory.Create<int>(this, item => OnPageIndexChanged(item, DynamicAttributeName, _DropDownBoxGridModel)));
                b.AddAttribute(21, "ShowSearchBox", true);
                b.AddAttribute(22, "ShowAllRows", false);
                if (DataSourceTable == "DynamicGrid")
                {
                    b.AddAttribute(23, isMultiple == true ? "SelectedDataItems" : "SelectedDataItem", isMultiple == true ? context.DropDownBox.Value as IEnumerable<ExpandoObject> : context.DropDownBox.Value as ExpandoObject);

                }
                else
                {
                    b.AddAttribute(23, isMultiple == true ? "SelectedDataItems" : "SelectedDataItem", isMultiple == true ? context.DropDownBox.Value as IEnumerable<AttributeDetails> : context.DropDownBox.Value as AttributeDetails);

                }
                if (isMultiple == true)
                {
                    b.AddAttribute(24, "SelectedDataItemsChanged", EventCallback.Factory.Create<IReadOnlyList<object>>(this, item => GridSelectedDataItemsChanged(item, context.DropDownBox, DataSourceTable)));
                }
                else
                {
                    b.AddAttribute(24, "SelectedDataItemChanged", EventCallback.Factory.Create<object>(this, item => GridDropBoxSelectedDataItemChanged(item, context.DropDownBox)));
                }
                b.AddAttribute(25, "Columns", (RenderFragment)((layoutItemBuilders) =>
                {
                    layoutItemBuilders.OpenComponent<DxGridSelectionColumn>(0);
                    layoutItemBuilders.AddAttribute(0, "AllowSelectAll", true);
                    layoutItemBuilders.AddAttribute(1, "Width", "60px");
                    layoutItemBuilders.CloseComponent();
                    if (DataSourceTable == "DynamicGrid")
                    {
                        if (gridlistss != null)
                        {
                            LoadDataGridDynamicForms(layoutItemBuilders, _DropDownBoxGridModel, DynamicAttributeName, gridlistss);
                        }
                    }
                    else
                    {
                        layoutItemBuilders.OpenComponent<DxGridDataColumn>(1);
                        layoutItemBuilders.AddAttribute(0, "FieldName", "AttributeDetailName");
                        layoutItemBuilders.AddAttribute(1, "Caption", "Name");
                        layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                        layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == 1 ? GridDropBoxData.SortIndex : -1);
                        layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, 1, _DropDownBoxGridModel)));
                        layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == 1 ? GridDropBoxData.SortOrder : 0);
                        layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == 1 ? GridDropBoxData.FilterRowValue : null);
                        layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, 1, _DropDownBoxGridModel)));
                        layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                        layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, 1, _DropDownBoxGridModel)));
                        layoutItemBuilders.CloseComponent();
                        layoutItemBuilders.OpenComponent<DxGridDataColumn>(2);
                        layoutItemBuilders.AddAttribute(0, "FieldName", "Description");
                        layoutItemBuilders.AddAttribute(1, "Caption", "Description");
                        layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                        layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == 2 ? GridDropBoxData.SortIndex : -1);
                        layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, 2, _DropDownBoxGridModel)));
                        layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == 2 ? GridDropBoxData.SortOrder : 0);
                        layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == 2 ? GridDropBoxData.FilterRowValue : null);
                        layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, 2, _DropDownBoxGridModel)));
                        layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                        layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, 2, _DropDownBoxGridModel)));
                        layoutItemBuilders.CloseComponent();
                    }
                }));
                b.CloseComponent();
            }));
        }
        public int TagBoxRenderForm(string DataDynamicAttributeName, string DisplayName, int j, Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder editor, DynamicFormSectionAttribute s, object? valueData, List<AttributeDetails> attributeDetailss, ApplicationUser applicationUser, PropertyInfo property, object Data,
            object DataGridObject, ViewEmployee _userEmployee, AttributeHeaderListModel _AttributeHeader, bool? isAccessOnly, bool isReadonly, List<DropDownBoxGridModel> _DropDownBoxGridModel, bool? isSubmit)
        {
            int isRequiredCount = 0;
            var propertyMain = Data.GetType().GetProperty(DataDynamicAttributeName);
            if (propertyMain != null)
            {
                DataGridObject.GetType().GetProperty(DataDynamicAttributeName).SetValue(DataGridObject, null);
                var listData = propertyMain.GetValue(Data);
                IEnumerable<long?> valueDatas1 = (IEnumerable<long?>)listData;
                List<DropDownOptionsModel?> gridlistss = new List<DropDownOptionsModel?>();
                List<ExpandoObject>? _dynamicformObjectDataList = new List<ExpandoObject>();
                gridlistss = _AttributeHeader.DropDownOptionsGridListModel.DropDownGridOptionsModel.Where(ax => ax.DynamicFormId == s.DynamicFormGridDropDownId).FirstOrDefault()?.DropDownOptionsModels;
                if (s.DataSourceTable == "DynamicGrid")
                {
                    var collection = _AttributeHeader.DropDownOptionsGridListModel.ObjectData;
                    List<long?> listDatas = new List<long?>();
                    if (valueDatas1 != null)
                    {
                        listDatas = valueDatas1.ToList();
                    }
                    if (collection != null)
                    {
                        List<ExpandoObject>? _selecteddynamicformObjectDataList = new List<ExpandoObject>();
                        foreach (var v in (collection as IEnumerable<ExpandoObject>))
                        {
                            dynamic eod = v;
                            var vv = eod.DynamicFormId;
                            if ((long?)vv == s.DynamicFormGridDropDownId)
                            {
                                _dynamicformObjectDataList.Add(v);
                            }
                            var AttributeDetailIds = eod.AttributeDetailID;
                            if (listDatas.Contains((long?)AttributeDetailIds))
                            {
                                _selecteddynamicformObjectDataList.Add(v);
                            }
                        }

                        DataGridObject.GetType().GetProperty(DataDynamicAttributeName).SetValue(DataGridObject, _selecteddynamicformObjectDataList.Count() > 0 ? _selecteddynamicformObjectDataList : null);
                    }

                }
                else
                {
                    if (valueDatas1 != null)
                    {
                        List<long?> listDatas = valueDatas1.ToList();
                        if (listDatas.Count() > 0)
                        {
                            var NameList = attributeDetailss.Where(mm => listDatas.Contains(mm.AttributeDetailID) && mm.DropDownTypeId == s.DataSourceTable).ToList();
                            DataGridObject.GetType().GetProperty(DataDynamicAttributeName).SetValue(DataGridObject, NameList);
                        }
                    }
                }
                var Gridaccess = Expression.Property(Expression.Constant(DataGridObject), DataDynamicAttributeName);
                var Gridlambda = Expression.Lambda(typeof(Func<>).MakeGenericType(typeof(object)), Gridaccess);
                var Gridproperty = DataGridObject.GetType().GetProperty(DataDynamicAttributeName);
                var GridvalueData = Gridproperty.GetValue(DataGridObject);
                var GridDropBoxData = _DropDownBoxGridModel.FirstOrDefault(f => f.AttrName == DataDynamicAttributeName);



                if (valueData == null && s.IsSetDefaultValue == true)
                {
                    if (s.DataSourceTable == "Employee")
                    {
                        var exitsDefaultData = attributeDetailss.FirstOrDefault(f => f.AttributeDetailID == applicationUser.UserID);
                        if (exitsDefaultData != null)
                        {
                            List<long?> valueDatads = new List<long?>();
                            List<AttributeDetails> AttributeDetailsList = new List<AttributeDetails>();
                            AttributeDetailsList.Add(exitsDefaultData);
                            valueDatads.Add(applicationUser.UserID);
                            valueData = valueDatads;
                            property.SetValue(Data, valueData);
                            Gridproperty.SetValue(DataGridObject, AttributeDetailsList);
                        }
                    }
                    if (s.DataSourceTable == "Plant" && _userEmployee != null && _userEmployee.PlantID > 0)
                    {
                        var exitsDefaultData = attributeDetailss.FirstOrDefault(f => f.AttributeDetailID == _userEmployee.PlantID);
                        if (exitsDefaultData != null)
                        {
                            List<long?> valueDatads = new List<long?>();
                            List<AttributeDetails> AttributeDetailsList = new List<AttributeDetails>();
                            AttributeDetailsList.Add(exitsDefaultData);
                            valueDatads.Add(_userEmployee.PlantID);
                            valueData = valueDatads;
                            property.SetValue(Data, valueData);
                            Gridproperty.SetValue(DataGridObject, AttributeDetailsList);
                        }
                    }
                }

                editor.OpenComponent<DxDropDownBox>(0);
                editor.AddAttribute(1, "DropDownWidthMode", DropDownWidthMode.EditorWidth);
                editor.AddAttribute(2, "NullText", "Select " + DisplayName + "..");
                editor.AddAttribute(3, "ClearButtonDisplayMode", DataEditorClearButtonDisplayMode.Auto);
                editor.AddAttribute(4, "QueryDisplayText", s.DataSourceTable == "DynamicGrid" ? QueryMultipleDynamicFormText : QueryMultipleText);
                editor.AddAttribute(5, "CssClass", "cw-480");
                editor.AddAttribute(6, "Value", GridvalueData);
                if (isAccessOnly == true)
                {
                    editor.AddAttribute(7, "Enabled", s.IsDefaultReadOnly == true ? false : isReadonly);
                }
                else
                {
                    editor.AddAttribute(8, "Enabled", isAccessOnly);
                }
                editor.AddAttribute(9, "DropDownVisible", false);
                editor.AddAttribute(10, "ValueExpression", Gridlambda);
                editor.AddAttribute(11, "ValueChanged", EventCallback.Factory.Create<object>(this, str => OnMultipleValueChanged(str, DataDynamicAttributeName, s.DataSourceTable, Data, DataGridObject)));

                if (s.IsDefaultReadOnly == false)
                {
                    if (s.IsRequired == true && isReadonly == true)
                    {
                        if (valueData == null)
                        {
                            isRequiredCount += 1;
                            if (isSubmit == true)
                            {
                            }
                        }
                        else
                        {
                            IEnumerable<long?> valueDatas = (IEnumerable<long?>)valueData;
                            if (valueDatas != null)
                            {
                                List<long?> listData1 = valueDatas.ToList();
                                if (listData1.Count() == 0)
                                {
                                    isRequiredCount += 1;
                                    if (isSubmit == true)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
                CreateDropDownBodyTemplateGrid(editor, _DropDownBoxGridModel, GridDropBoxData, s.DataSourceTable, DataDynamicAttributeName, gridlistss, _dynamicformObjectDataList, attributeDetailss, true);

                editor.CloseComponent();
                if (s.IsDefaultReadOnly == false)
                {
                    if (s.IsRequired == true && isReadonly == true && isAccessOnly == true)
                    {
                        if (valueData == null && isSubmit == true)
                        {
                            editor.OpenElement(12, "span");
                            editor.AddAttribute(13, "class", "text-danger");
                            editor.AddContent(14, !string.IsNullOrEmpty(s.RequiredMessage) ? s.RequiredMessage : "This field is Required");
                            editor.CloseElement();
                        }
                        else
                        {
                            IEnumerable<long?> valueDatas2 = (IEnumerable<long?>)valueData;
                            if (valueDatas2 != null)
                            {
                                List<long?> listData2 = valueDatas2.ToList();
                                if (listData2.Count() == 0 && isSubmit == true)
                                {
                                    editor.OpenElement(15, "span");
                                    editor.AddAttribute(16, "class", "text-danger");
                                    editor.AddContent(17, !string.IsNullOrEmpty(s.RequiredMessage) ? s.RequiredMessage : "This field is Required");
                                    editor.CloseElement();
                                }
                            }
                        }
                    }
                }
            }
            return isRequiredCount;
        }
        void OnMultipleValueChanged(object? newValue, string AttrName, string DataSource, object Data, object DataGridObject)
        {
            List<long?> ids = new List<long?>();
            Data.GetType().GetProperty(AttrName).SetValue(Data, null);
            if (DataSource == "DynamicGrid")
            {
                List<ExpandoObject> collection = (List<ExpandoObject>)newValue;

                if (collection != null && collection.Count() > 0)
                {
                    DataGridObject.GetType().GetProperty(AttrName).SetValue(DataGridObject, newValue);
                    foreach (var item in collection)
                    {
                        dynamic eod = item;
                        ids.Add((long)eod.AttributeDetailID);
                    }

                }
            }
            else
            {
                if (newValue != null)
                {
                    List<AttributeDetails> collection = (List<AttributeDetails>)newValue;
                    if (collection != null && collection.Count() > 0)
                    {
                        DataGridObject.GetType().GetProperty(AttrName).SetValue(DataGridObject, newValue);
                        foreach (var item in collection)
                        {
                            ids.Add(item.AttributeDetailID);
                        }
                    }
                }
            }
            Data.GetType().GetProperty(AttrName).SetValue(Data, ids);
        }
        void OnGridValueChanged(object? newValue, string AttrName, string? DataSource, object Data, object DataGridObject)
        {
            long? Id = null;
            if (DataSource == "DynamicGrid")
            {
                if (newValue is ExpandoObject value)
                {
                    if (value != null)
                    {
                        dynamic eod = value;
                        Id = (long?)eod.AttributeDetailID;
                    }
                }
            }
            else
            {
                if (newValue is AttributeDetails value)
                {
                    if (value != null)
                    {
                        Id = value?.AttributeDetailID;
                    }
                }
            }
            Data.GetType().GetProperty(AttrName).SetValue(Data, Id);
            DataGridObject.GetType().GetProperty(AttrName).SetValue(DataGridObject, newValue);
            //OnRadioChangedItem(Id, PropertyInfo property, List<AttributeDetails> attributeDetails, DynamicFormSection dynamicFormSection, DynamicFormSectionAttribute dynamicFormSectionAttribute, object Data, AttributeHeaderListModel _AttributeHeader, List<DynamicFormSectionAttributeSection?> dynamicFormSectionAttributeSectionSelected, DynamicForm _dynamicform, List<Type?> DataClassTypeList, List<string?> DataClassObjectNameList, List<AddTempSectionAttribute> addTempSectionAttributes)

        }
       void OnPageIndexChanged(int newPageIndex, string AttrName, List<DropDownBoxGridModel> _DropDownBoxGridModel)
        {
            var exits = _DropDownBoxGridModel.Where(f => f.AttrName == AttrName).FirstOrDefault();
            if (exits != null)
            {
                exits.GridPageIndex = newPageIndex;
            }
            else
            {
                DropDownBoxGridModel dropDownBoxGridModel = new DropDownBoxGridModel();
                dropDownBoxGridModel.GridPageIndex = newPageIndex;
                dropDownBoxGridModel.AttrName = AttrName;
                _DropDownBoxGridModel.Add(dropDownBoxGridModel);
            }
        }
        void OnPageSizeChanged(int newPageSize, string AttrName, List<DropDownBoxGridModel> _DropDownBoxGridModel)
        {
            var exits = _DropDownBoxGridModel.Where(f => f.AttrName == AttrName).FirstOrDefault();
            if (exits != null)
            {
                exits.PageSize = newPageSize;
            }
            else
            {
                DropDownBoxGridModel dropDownBoxGridModel = new DropDownBoxGridModel();
                dropDownBoxGridModel.PageSize = newPageSize;
                dropDownBoxGridModel.AttrName = AttrName;
                _DropDownBoxGridModel.Add(dropDownBoxGridModel);
            }
        }
        void OnSearchTextChanged(string newSearchText, string AttrName, List<DropDownBoxGridModel> _DropDownBoxGridModel)
        {
            var exits = _DropDownBoxGridModel.Where(f => f.AttrName == AttrName).FirstOrDefault();
            if (exits != null)
            {
                exits.SearchText = newSearchText;
            }
            else
            {
                DropDownBoxGridModel dropDownBoxGridModel = new DropDownBoxGridModel();
                dropDownBoxGridModel.SearchText = newSearchText;
                dropDownBoxGridModel.AttrName = AttrName;
                _DropDownBoxGridModel.Add(dropDownBoxGridModel);
            }
        }
        void OnRowclick(GridRowClickEventArgs e, IDropDownBox dropDownBox)
        {
        }
        void GridSelectedDataItemsChanged(IReadOnlyList<object> selectedItems, IDropDownBox dropDownBox, string DataSource)
        {
            if (DataSource == "DynamicGrid")
            {
                List<ExpandoObject> selectedAllItems = new List<ExpandoObject>();
                if (selectedItems != null)
                {
                    foreach (object item in selectedItems)
                    {
                        if (item is ExpandoObject dataselectItems)
                        {
                            selectedAllItems.Add(dataselectItems);
                        }
                    }
                }
                dropDownBox.BeginUpdate();
                dropDownBox.Value = selectedAllItems;
                dropDownBox.EndUpdate();
            }
            else
            {
                List<AttributeDetails> selectedAllItems = new List<AttributeDetails>();
                if (selectedItems != null)
                {
                    foreach (object item in selectedItems)
                    {
                        if (item is AttributeDetails dataselectItems)
                        {
                            selectedAllItems.Add(dataselectItems);
                        }
                    }
                }
                dropDownBox.BeginUpdate();
                dropDownBox.Value = selectedAllItems;
                dropDownBox.EndUpdate();
            }
        }
        void GridDropBoxSelectedDataItemChanged(object selectedItems, IDropDownBox dropDownBox)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = selectedItems;
            dropDownBox.EndUpdate();
        }
        string? QueryDisplayComboBoxText(DropDownBoxQueryDisplayTextContext arg)
        {
            string? returnData = string.Empty;
            if (arg.Value is AttributeDetails value)
            {
                if (value != null)
                {
                    returnData = value?.AttributeDetailName;
                }
            }
            return returnData.TrimEnd(',');
        }
        string? QueryMultipleText(DropDownBoxQueryDisplayTextContext arg)
        {
            List<long?> ids = new List<long?>();
            string? returnData = string.Empty;
            if (arg.Value != null)
            {
                List<AttributeDetails> collection = (List<AttributeDetails>)arg.Value;

                if (collection != null && collection.Count() > 0)
                {
                    foreach (var item in collection)
                    {
                        returnData += item?.AttributeDetailName + ",";
                    }

                }
            }

            return returnData.TrimEnd(',');
        }
        string? QueryMultipleDynamicFormText(DropDownBoxQueryDisplayTextContext arg)
        {
            List<long?> ids = new List<long?>();
            string? returnData = string.Empty;
            if (arg.Value != null)
            {
                List<ExpandoObject> collection = (List<ExpandoObject>)arg.Value;

                if (collection != null && collection.Count() > 0)
                {
                    foreach (var item in collection)
                    {
                        dynamic eod = item;
                        returnData += (string)eod.AttributeDetailName + ",";
                    }

                }
            }

            return returnData.TrimEnd(',');
        }
        string? QueryDisplayDynamicComboBoxText(DropDownBoxQueryDisplayTextContext arg)
        {
            string? returnData = string.Empty;
            if (arg.Value is ExpandoObject value)
            {
                if (value != null)
                {
                    dynamic eod = value;
                    returnData = (string)eod.AttributeDetailName;
                }
            }
            return returnData.TrimEnd(',');
        }
        void FilterCriteriaChangeds(object args, string AttrName, int? DataGridColumn, List<DropDownBoxGridModel> _DropDownBoxGridModel)
        {
            var exits = _DropDownBoxGridModel.Where(f => f.AttrName == AttrName).FirstOrDefault();
            if (exits != null)
            {
                exits.DataGridColumn = DataGridColumn;
                exits.FilterRowValue = (string)args;
            }
            else
            {
                DropDownBoxGridModel dropDownBoxGridModel = new DropDownBoxGridModel();
                dropDownBoxGridModel.FilterRowValue = (string)args;
                dropDownBoxGridModel.AttrName = AttrName;
                dropDownBoxGridModel.DataGridColumn = DataGridColumn;
                _DropDownBoxGridModel.Add(dropDownBoxGridModel);
            }
        }
        void OnSortIndexChangeds(int newSortIndex, string AttrName, int? DataGridColumn, List<DropDownBoxGridModel> _DropDownBoxGridModel)
        {
            var exits = _DropDownBoxGridModel.Where(f => f.AttrName == AttrName).FirstOrDefault();
            if (exits != null)
            {
                exits.DataGridColumn = DataGridColumn;
                exits.SortIndex = newSortIndex;
            }
            else
            {
                DropDownBoxGridModel dropDownBoxGridModel = new DropDownBoxGridModel();
                dropDownBoxGridModel.DataGridColumn = DataGridColumn;
                dropDownBoxGridModel.SortIndex = newSortIndex;
                dropDownBoxGridModel.AttrName = AttrName;
                _DropDownBoxGridModel.Add(dropDownBoxGridModel);
            }
        }
        void OnSortOrderChangeds(GridColumnSortOrder gridColumnSortOrder, string AttrName, int? DataGridColumn, List<DropDownBoxGridModel> _DropDownBoxGridModel)
        {
            var sortOrder = Enum.GetName(gridColumnSortOrder.GetType(), gridColumnSortOrder);
            var exits = _DropDownBoxGridModel.Where(f => f.AttrName == AttrName).FirstOrDefault();
            if (exits != null)
            {
                exits.SortOrder = sortOrder == "Ascending" ? 1 : 2;
                exits.DataGridColumn = DataGridColumn;
            }
            else
            {
                DropDownBoxGridModel dropDownBoxGridModel = new DropDownBoxGridModel();
                dropDownBoxGridModel.SortOrder = sortOrder == "Ascending" ? 1 : 2;
                dropDownBoxGridModel.AttrName = AttrName;
                _DropDownBoxGridModel.Add(dropDownBoxGridModel);
            }
        }
        public void LoadDataGridDynamicForms(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder layoutItemBuilders, List<DropDownBoxGridModel> _DropDownBoxGridModel, string DynamicAttributeName, List<DropDownOptionsModel> dropDownOptionsModels)
        {
            int i = 0;
            var GridDropBoxData = _DropDownBoxGridModel.FirstOrDefault(f => f.AttrName == DynamicAttributeName);
            if (dropDownOptionsModels != null && dropDownOptionsModels.Count() > 0)
            {
                var dropDownOptionsModel = dropDownOptionsModels.OrderBy(o => o.OrderBy).ToList();

                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
                i += 1;
                if (dropDownOptionsModel.ElementAtOrDefault(i - 1) != null)
                {
                    int j = i + 1;
                    layoutItemBuilders.OpenComponent<DxGridDataColumn>(i - 1);
                    layoutItemBuilders.AddAttribute(0, "FieldName", dropDownOptionsModel[i - 1].Value);
                    layoutItemBuilders.AddAttribute(1, "Caption", dropDownOptionsModel[i - 1].Text);
                    layoutItemBuilders.AddAttribute(2, "AllowSort", true);
                    layoutItemBuilders.AddAttribute(3, "SortIndex", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortIndex : -1);
                    layoutItemBuilders.AddAttribute(4, "SortIndexChanged", EventCallback.Factory.Create<int>(this, item => OnSortIndexChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(5, "SortOrder", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.SortOrder : 0);
                    layoutItemBuilders.AddAttribute(6, "FilterRowValue", GridDropBoxData != null && GridDropBoxData.DataGridColumn == j ? GridDropBoxData.FilterRowValue : null);
                    layoutItemBuilders.AddAttribute(7, "SortOrderChanged", EventCallback.Factory.Create<GridColumnSortOrder>(this, item => OnSortOrderChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(8, "FilterMenuButtonDisplayMode", GridFilterMenuButtonDisplayMode.Always);
                    layoutItemBuilders.AddAttribute(9, "FilterRowValueChanged", EventCallback.Factory.Create<object>(this, item => FilterCriteriaChangeds(item, DynamicAttributeName, j, _DropDownBoxGridModel)));
                    layoutItemBuilders.AddAttribute(10, "SortMode", GridColumnSortMode.Default);
                    layoutItemBuilders.CloseComponent();
                }
            }
        }

    }
}
