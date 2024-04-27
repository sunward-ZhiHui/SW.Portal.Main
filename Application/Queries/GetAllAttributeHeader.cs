using Application.Queries.Base;
using Core.Entities;
using Core.Repositories.Query;
using DevExpress.XtraReports.Design;
using MediatR;
using System;
using System.Collections.Generic;
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

    public class GetAllAttributeNameList : PagedRequest, IRequest<AttributeHeaderListModel>
    {

        public DynamicForm DynamicForm { get; set; }
        public long? UserId { get; set; }
        public GetAllAttributeNameList(DynamicForm dynamicForm, long? userId)
        {
            this.DynamicForm = dynamicForm;
            this.UserId = userId;
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
        public GetDynamicFormFilterByDataSource( List<string?> dataSourceTableIds)
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
}
