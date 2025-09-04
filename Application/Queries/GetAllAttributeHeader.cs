using Application.Queries.Base;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using DevExpress.XtraReports.Design;
using MediatR;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllAttributeHeader : PagedRequest, IRequest<List<DynamicForm>>
    {


    }
    public class GetAllAttributeNameHeader : PagedRequest, IRequest<List<AttributeHeader>>
    {
        public bool? IsSubForm { get; set; }
        public string? Type { get; set; }
        public long? SubId { get; set; }
        public GetAllAttributeNameHeader(bool? isSubForm, string? type, long? subId)
        {
            this.IsSubForm = isSubForm;
            this.Type = type;
            this.SubId = subId;
        }
    }
    public class GetAttributeHeaderDataSource : PagedRequest, IRequest<List<AttributeHeaderDataSource>>
    {


    }
    public class GetFilterDataSource : PagedRequest, IRequest<List<DynamicFormFilter>>
    {


    }
    public class GetAllAttributeNameNotInDynamicForm : PagedRequest, IRequest<List<AttributeHeader>>
    {
        public long? AttributeID { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public GetAllAttributeNameNotInDynamicForm(long? dynamicFormSectionId, long? attributeID)
        {
            this.DynamicFormSectionId = dynamicFormSectionId;
            this.AttributeID = attributeID;
        }
    }

    public class GetAllAttributeNameNotInDynamicFormList : PagedRequest, IRequest<List<AttributeHeader>>
    {
        public List<long?> AttributeID { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public GetAllAttributeNameNotInDynamicFormList(long? dynamicFormSectionId, List<long?> attributeID)
        {
            this.DynamicFormSectionId = dynamicFormSectionId;
            this.AttributeID = attributeID;
        }
    }
    public class CreateAttributeHeader : AttributeHeader, IRequest<long>
    {
    }
    public class EditAttributeHeader : AttributeHeader, IRequest<long>
    {
        public AttributeHeader AttributeDetailsItem { get; set; }
        public EditAttributeHeader(AttributeHeader AttributeDetailsItem)
        {
            this.AttributeDetailsItem = AttributeDetailsItem;
        }
    }

    public class DeleteAttributeHeader : PagedRequest, IRequest<long>
    {
        public AttributeHeader AttributeHeader { get; set; }
        public DeleteAttributeHeader(AttributeHeader attributeHeader)
        {
            this.AttributeHeader = attributeHeader;
        }
    }

    public class GetAllAttributeNameById : PagedRequest, IRequest<AttributeHeaderListModel>
    {

        public List<long?> DynamicFormIds { get; set; }
        public long? UserId { get; set; }

        public GetAllAttributeNameById(List<long?> dynamicFormIds, long? userId)
        {
            this.DynamicFormIds = dynamicFormIds;
            this.UserId = userId;
        }
    }
    public class GetAllAttributeNameList : PagedRequest, IRequest<AttributeHeaderListModel>
    {

        public DynamicForm DynamicForm { get; set; }
        public long? UserId { get; set; }
        public bool? IsSubFormLoad { get; set; }
        public bool? IsNoDelete { get; set; } = true;
        public bool? IsTableHeader { get; set; } = false;
        public GetAllAttributeNameList(DynamicForm dynamicForm, long? userId, bool? isSubFormLoad, bool? IsNoDelete, bool? isTableHeader)
        {
            this.DynamicForm = dynamicForm;
            this.UserId = userId;
            this.IsSubFormLoad = isSubFormLoad;
            this.IsNoDelete = IsNoDelete;
            this.IsTableHeader = isTableHeader;
        }
    }
    public class GetAllBySessionAttributeName : PagedRequest, IRequest<AttributeHeader>
    {

        public Guid? SessionId { get; set; }
        public GetAllBySessionAttributeName(Guid? sessionId)
        {
            this.SessionId = sessionId;
        }
    }
    public class GetDataSourceDropDownList : PagedRequest, IRequest<List<AttributeDetails>>
    {
        public long? CompanyId { get; set; }
        public List<string?> DataSourceTableIds { get; set; }
        public string? PlantCode { get; set; }
        public List<long?> ApplicationMasterIds { get; set; }
        public List<long?> ApplicationMasterParentIds { get; set; }
        public GetDataSourceDropDownList(long? companyId, List<string?> dataSourceTableIds, string? plantCode, List<long?> applicationMasterIds, List<long?> applicationMasterParentIds)
        {
            this.CompanyId = companyId;
            this.DataSourceTableIds = dataSourceTableIds;
            this.PlantCode = plantCode;
            this.ApplicationMasterIds = applicationMasterIds;
            this.ApplicationMasterParentIds = applicationMasterParentIds;
        }
    }
    public class GetAllDropDownDataSourcesList : PagedRequest, IRequest<DataSourceAttributeDetails>
    {

    }
    public class UpdateAttributeHeaderSortOrder : PagedRequest, IRequest<AttributeHeader>
    {
        public AttributeHeader AttributeHeader { get; private set; }
        public UpdateAttributeHeaderSortOrder(AttributeHeader attributeHeader)
        {
            this.AttributeHeader = attributeHeader;
        }
    }
    public class GetApplicationMasterParentByList : PagedRequest, IRequest<List<DropDownOptionsListModel>>
    {
        public IDictionary<string, object> DynamicMasterParentIds { get; set; }
        public long? ApplicationMasterParentId { get; set; }
        public GetApplicationMasterParentByList(IDictionary<string, object> dynamicMasterParentIds, long? applicationMasterParentId)
        {
            this.DynamicMasterParentIds = dynamicMasterParentIds;
            this.ApplicationMasterParentId = applicationMasterParentId;
        }
    }
    public class GetAllFilterDropDownDataSources : PagedRequest, IRequest<DynamicFormFilterDataSoureList>
    {

    }
    public class GetDynamicFormFilterByDataSource : PagedRequest, IRequest<List<DynamicFormFilterBy>>
    {
        public List<string?> DataSourceTableIds { get; set; }
        public GetDynamicFormFilterByDataSource(List<string?> dataSourceTableIds)
        {
            this.DataSourceTableIds = dataSourceTableIds;
        }
    }
    public class GetFilterDataSourceLists : PagedRequest, IRequest<List<AttributeDetails>>
    {
        public List<DynamicFormFilterBy> DynamicFormFilterBy { get; set; }
        public GetFilterDataSourceLists(List<DynamicFormFilterBy> dynamicFormFilterBy)
        {
            this.DynamicFormFilterBy = dynamicFormFilterBy;
        }
    }
    public class GetFilterByDataSourceLists : PagedRequest, IRequest<List<AttributeDetails>>
    {
        public List<DynamicFormFilterBy> DynamicFormFilterBy { get; set; }
        public object Data { get; set; }
        public string DataSource { get; set; }
        public GetFilterByDataSourceLists(List<DynamicFormFilterBy> dynamicFormFilterBy, object data, string dataSource)
        {
            this.DynamicFormFilterBy = dynamicFormFilterBy;
            this.Data = data;
            this.DataSource = dataSource;
        }
    }
    public class GetApplicationMasterParentMobileByList : PagedRequest, IRequest<List<DropDownOptionsListModel>>
    {
        public IDictionary<string, JsonElement> DynamicMasterParentIds { get; set; }
        public long? ApplicationMasterParentId { get; set; }
        public GetApplicationMasterParentMobileByList(IDictionary<string, JsonElement> dynamicMasterParentIds, long? applicationMasterParentId)
        {
            this.DynamicMasterParentIds = dynamicMasterParentIds;
            this.ApplicationMasterParentId = applicationMasterParentId;
        }
    }
    public class GetDynamicGridNested : PagedRequest, IRequest<DropDownOptionsGridListModel>
    {
        public List<long?> DynamicFormDataId { get; set; }
        public long? UserId { get; set; }
        public GetDynamicGridNested(List<long?> dynamicFormDataId, long? userId)
        {
            this.DynamicFormDataId = dynamicFormDataId;
            this.UserId = userId;
        }
    }
    public class GetDynamicFormApi : PagedRequest, IRequest<List<DynamicFormDataResponse>>
    {
        public Guid? DynamicFormSessionId { get; set; }
        public Guid? DynamicFormDataSessionId { get; set; }
        public Guid? DynamicFormDataGridSessionId { get; set; }
        public Guid? DynamicFormSectionGridAttributeSessionId { get; set; }
        public string? BaseUrl { get; set; }
        public bool? IsAll { get; set; }
        public int? PageNo { get; set; }
        public int? PageSizes { get; set; }
        public List<DynamicFormFilterOdata> DynamicFormFilterOdatas { get; set; }
        public GetDynamicFormApi(Guid? dynamicFormSessionId, Guid? dynamicFormDataSessionId, Guid? dynamicFormDataGridSessionId, Guid? dynamicFormSectionGridAttributeSessionId, string? baseUrl, bool? isAll, int? pageNo, int? pageSize, List<DynamicFormFilterOdata> dynamicFormFilterOdatas)
        {
            this.DynamicFormSessionId = dynamicFormSessionId;
            this.DynamicFormDataSessionId = dynamicFormDataSessionId;
            this.DynamicFormDataGridSessionId = dynamicFormDataGridSessionId;
            this.DynamicFormSectionGridAttributeSessionId = dynamicFormSectionGridAttributeSessionId;
            this.BaseUrl = baseUrl;
            this.IsAll = isAll;
            this.PageNo = pageNo;
            this.PageSizes = pageSize;
            this.DynamicFormFilterOdatas = dynamicFormFilterOdatas;
        }
    }
    public class GetDynamicFormAttributeApi : PagedRequest, IRequest<List<DynamicFormDataResponse>>
    {
        public Guid? DynamicFormSessionId { get; set; }
        public Guid? DynamicFormDataSessionId { get; set; }
        public Guid? DynamicFormDataGridSessionId { get; set; }
        public Guid? DynamicFormSectionGridAttributeSessionId { get; set; }
        public string? BaseUrl { get; set; }
        public bool? IsAll { get; set; }
        public GetDynamicFormAttributeApi(Guid? dynamicFormSessionId, Guid? dynamicFormDataSessionId, Guid? dynamicFormDataGridSessionId, Guid? dynamicFormSectionGridAttributeSessionId, string? baseUrl, bool? isAll)
        {
            this.DynamicFormSessionId = dynamicFormSessionId;
            this.DynamicFormDataSessionId = dynamicFormDataSessionId;
            this.DynamicFormDataGridSessionId = dynamicFormDataGridSessionId;
            this.DynamicFormSectionGridAttributeSessionId = dynamicFormSectionGridAttributeSessionId;
            this.BaseUrl = baseUrl;
            this.IsAll = isAll;
        }
    }
    public class GetDynamicGridDropDownById : PagedRequest, IRequest<DropDownOptionsGridListModel>
    {
        public List<long?> DynamicFormId { get; set; }
        public long? UserId { get; set; }
        public List<long?> DynamicFormDataId { get; set; }
        public GetDynamicGridDropDownById(List<long?> dynamicFormId, long? userId, List<long?> dynamicFormDataId)
        {
            this.DynamicFormId = dynamicFormId;
            this.UserId = userId;
            this.DynamicFormDataId = dynamicFormDataId;
        }
    }
    public class GetAttributeDetailsDataSource : PagedRequest, IRequest<List<AttributeDetails>>
    {
        public long? AttributeId { get; set; }
        public string? BaseUrl { get; set; }
        public GetAttributeDetailsDataSource(long? attributeId)
        {
            this.AttributeId = attributeId;
        }
    }
    public class GetQcTestRequirementSummery : PagedRequest, IRequest<List<QCTestRequirement>>
    {
    }
    public class GetAllDynamicFormDataOneApi : PagedRequest, IRequest<List<DynamicFormDataResponse>>
    {
        public Guid? DynamicFormDataSessionId { get; set; }

        public GetAllDynamicFormDataOneApi(Guid? dynamicFormDataSessionId)
        {
            this.DynamicFormDataSessionId = dynamicFormDataSessionId;
        }
    }
    public class GetDynamicFormObjectsApi : PagedRequest, IRequest<List<ExpandoObject>>
    {
        public Guid? DynamicFormSessionId { get; set; }
        public Guid? DynamicFormDataSessionId { get; set; }
        public Guid? DynamicFormDataGridSessionId { get; set; }
        public Guid? DynamicFormSectionGridAttributeSessionId { get; set; }
        public string? BaseUrl { get; set; }
        public bool? IsAll { get; set; }
        public GetDynamicFormObjectsApi(Guid? dynamicFormSessionId, Guid? dynamicFormDataSessionId, Guid? dynamicFormDataGridSessionId, Guid? dynamicFormSectionGridAttributeSessionId, string? baseUrl, bool? isAll)
        {
            this.DynamicFormSessionId = dynamicFormSessionId;
            this.DynamicFormDataSessionId = dynamicFormDataSessionId;
            this.DynamicFormDataGridSessionId = dynamicFormDataGridSessionId;
            this.DynamicFormSectionGridAttributeSessionId = dynamicFormSectionGridAttributeSessionId;
            this.BaseUrl = baseUrl;
            this.IsAll = isAll;
        }
    }
    public class GetAttributeHeaderDataSource1 : PagedRequest, IRequest<List<object>>
    {
    }

}
