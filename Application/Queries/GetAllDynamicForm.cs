using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllDynamicForm : PagedRequest, IRequest<List<DynamicForm>>
    {
        public long? UserId { get; set; }
        public GetAllDynamicForm(long? userId)
        {
            this.UserId = userId;
        }
    }
    public class GetAllByGridForm : PagedRequest, IRequest<List<DynamicForm>>
    {
        public long? UserId { get; set; }
        public GetAllByGridForm(long? userId)
        {
            this.UserId = userId;
        }
    }

    public class GetAllByGridNoForm : PagedRequest, IRequest<List<DynamicForm>>
    {
        public long? UserId { get; set; }
        public GetAllByGridNoForm(long? userId)
        {
            this.UserId = userId;
        }
    }

    public class GetAllDynamicFormList : PagedRequest, IRequest<DynamicForm>
    {
        public Guid? ID { get; set; }
        public long? DynamicFormDataId { get; set; }
        public GetAllDynamicFormList(Guid? ID, long? dynamicFormDataId)
        {
            this.ID = ID;
            this.DynamicFormDataId = dynamicFormDataId;
        }
    }
    public class CreateDynamicForm : DynamicForm, IRequest<long>
    {

    }
    public class EditDynamicForm : DynamicForm, IRequest<long>
    {

    }
    public class DeleteDynamicForm : DynamicForm, IRequest<long>
    {
        public long ID { get; set; }
        public DeleteDynamicForm(long ID)
        {
            this.ID = ID;
        }
    }



    public class InsertOrUpdateDynamicFormSection : DynamicFormSection, IRequest<DynamicFormSection>
    {

    }

    public class GetDynamicFormSection : PagedRequest, IRequest<List<DynamicFormSection>>
    {
        public long? Id { get; set; }
        public GetDynamicFormSection(long? id)
        {
            this.Id = id;
        }
    }

    public class InsertOrUpdateDynamicFormSectionAttribute : DynamicFormSectionAttribute, IRequest<DynamicFormSectionAttribute>
    {

    }


    public class GetDynamicFormSectionAttribute : PagedRequest, IRequest<List<DynamicFormSectionAttribute>>
    {
        public long? Id { get; set; }
        public GetDynamicFormSectionAttribute(long? id)
        {
            this.Id = id;
        }
    }
    public class GetDynamicFormBySession : PagedRequest, IRequest<DynamicForm>
    {
        public string? SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetDynamicFormBySession(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }
    public class GetDynamicFormById : PagedRequest, IRequest<DynamicForm>
    {
        public long? Id { get; set; }
        public GetDynamicFormById(long? id)
        {
            this.Id = id;
        }
    }

    public class DeleteDynamicFormSection : DynamicFormSection, IRequest<long>
    {
        public DynamicFormSection DynamicFormSection { get; set; }
        public DeleteDynamicFormSection(DynamicFormSection dynamicFormSection)
        {
            this.DynamicFormSection = dynamicFormSection;
        }
    }
    public class UpdateDynamicFormSectionSortOrder : PagedRequest, IRequest<DynamicFormSection>
    {
        public DynamicFormSection DynamicFormSection { get; private set; }
        public UpdateDynamicFormSectionSortOrder(DynamicFormSection dynamicFormSection)
        {
            this.DynamicFormSection = dynamicFormSection;
        }
    }
    public class DeleteDynamicFormSectionAttribute : DynamicFormSectionAttribute, IRequest<long>
    {
        public DynamicFormSectionAttribute DynamicFormSectionAttribute { get; set; }
        public DeleteDynamicFormSectionAttribute(DynamicFormSectionAttribute dynamicFormSectionAttribute)
        {
            this.DynamicFormSectionAttribute = dynamicFormSectionAttribute;
        }
    }

    public class UpdateDynamicFormSectionAttributeSortOrder : PagedRequest, IRequest<DynamicFormSectionAttribute>
    {
        public DynamicFormSectionAttribute DynamicFormSectionAttribute { get; private set; }
        public UpdateDynamicFormSectionAttributeSortOrder(DynamicFormSectionAttribute dynamicFormSectionAttribute)
        {
            this.DynamicFormSectionAttribute = dynamicFormSectionAttribute;
        }
    }
    public class InsertDynamicFormAttributeSection : DynamicForm, IRequest<long>
    {
        public long DynamicFormSectionId { get; set; }
        public long? UserId { get; set; }
        public IEnumerable<AttributeHeader> AttributeIds { get; set; }
        public InsertDynamicFormAttributeSection(long dynamicFormSectionId, IEnumerable<AttributeHeader> attributeIds, long? userId)
        {
            this.DynamicFormSectionId = dynamicFormSectionId;
            this.AttributeIds = attributeIds;
            this.UserId = userId;
        }
    }
    public class InsertOrUpdateDynamicFormData : DynamicFormData, IRequest<DynamicFormData>
    {

    }
    public class GetDynamicFormDataBySessionId : PagedRequest, IRequest<DynamicFormData>
    {
        public string? SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetDynamicFormDataBySessionId(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }
    public class GetDynamicFormDataBySessionIdForDMS : PagedRequest, IRequest<DocumentsModel>
    {
        public string? SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetDynamicFormDataBySessionIdForDMS(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }
    public class GetDynamicFormDataById : PagedRequest, IRequest<List<DynamicFormData>>
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public long? DynamicFormDataGridId { get; set; }
        public long? DynamicFormSectionGridAttributeId { get; set; }
        public GetDynamicFormDataById(long? id, long? userId, long? dynamicFormDataGridId, long? dynamicFormSectionGridAttributeId)
        {
            this.Id = id;
            this.UserId = userId;
            this.DynamicFormDataGridId = dynamicFormDataGridId;
            this.DynamicFormSectionGridAttributeId = dynamicFormSectionGridAttributeId;
        }
    }
    public class DeleteDynamicFormData : DynamicFormData, IRequest<DynamicFormData>
    {
        public DynamicFormData DynamicFormData { get; set; }
        public DeleteDynamicFormData(DynamicFormData dynamicFormData)
        {
            this.DynamicFormData = dynamicFormData;
        }
    }
    public class GetDynamicFormApproval : PagedRequest, IRequest<List<DynamicFormApproval>>
    {
        public long? Id { get; set; }
        public GetDynamicFormApproval(long? id)
        {
            this.Id = id;
        }
    }
    public class InsertOrUpdateDynamicFormApproval : DynamicFormApproval, IRequest<DynamicFormApproval>
    {

    }
    public class DeleteDynamicFormApproval : DynamicFormApproval, IRequest<DynamicFormApproval>
    {
        public DynamicFormApproval DynamicFormApproval { get; set; }
        public DeleteDynamicFormApproval(DynamicFormApproval dynamicFormApproval)
        {
            this.DynamicFormApproval = dynamicFormApproval;
        }
    }
    public class UpdateDynamicFormApprovalSortOrder : PagedRequest, IRequest<DynamicFormApproval>
    {
        public DynamicFormApproval DynamicFormApproval { get; private set; }
        public UpdateDynamicFormApprovalSortOrder(DynamicFormApproval dynamicFormApproval)
        {
            this.DynamicFormApproval = dynamicFormApproval;
        }
    }
    public class GetDynamicFormApprovalUpdateDescriptionField : PagedRequest, IRequest<DynamicFormApproval>
    {
        public DynamicFormApproval DynamicFormApproval { get; private set; }
        public GetDynamicFormApprovalUpdateDescriptionField(DynamicFormApproval dynamicFormApproval)
        {
            this.DynamicFormApproval = dynamicFormApproval;
        }
    }
    public class InsertDynamicFormSectionSecurity : PagedRequest, IRequest<DynamicFormSectionSecurity>
    {
        public DynamicFormSectionSecurity DynamicFormSectionSecurity { get; private set; }
        public InsertDynamicFormSectionSecurity(DynamicFormSectionSecurity dynamicFormSectionSecurity)
        {
            this.DynamicFormSectionSecurity = dynamicFormSectionSecurity;
        }
    }
    public class GetDynamicFormSectionSecurityList : PagedRequest, IRequest<List<DynamicFormSectionSecurity>>
    {
        public long? Id { get; set; }
        public GetDynamicFormSectionSecurityList(long? id)
        {
            this.Id = id;
        }
    }
    public class DeleteDynamicFormSectionSecurity : DynamicFormSectionSecurity, IRequest<long>
    {
        public long? Id { get; set; }
        public List<long?> Ids { get; set; }
        public DeleteDynamicFormSectionSecurity(long? id, List<long?> ids)
        {
            this.Id = id;
            this.Ids = ids;
        }
    }
    public class InsertDynamicFormApproved : DynamicFormApproved, IRequest<DynamicFormApproved>
    {

    }
    public class GetDynamicFormApprovedByID : PagedRequest, IRequest<DynamicFormApproved>
    {
        public long? DynamicFormDataId { get; set; }
        public long? ApprovalUserId { get; set; }
        public GetDynamicFormApprovedByID(long? dynamicFormDataId, long? approvalUserId)
        {
            this.DynamicFormDataId = dynamicFormDataId;
            this.ApprovalUserId = approvalUserId;
        }
    }
    public class InsertOrUpdateDynamicFormApproved : DynamicFormData, IRequest<DynamicFormData>
    {
        public DynamicFormData DynamicFormData { get; set; }
        public InsertOrUpdateDynamicFormApproved(DynamicFormData dynamicFormData)
        {
            this.DynamicFormData = dynamicFormData;
        }
    }
    public class GetDynamicFormApprovedList : PagedRequest, IRequest<List<DynamicFormApproved>>
    {
        public long? Id { get; set; }
        public GetDynamicFormApprovedList(long? id)
        {
            this.Id = id;
        }
    }
    public class UpdateDynamicFormApprovedByStaus : DynamicFormApproved, IRequest<DynamicFormApproved>
    {
        public DynamicFormApproved DynamicFormApproved { get; set; }
        public UpdateDynamicFormApprovedByStaus(DynamicFormApproved dynamicFormApproved)
        {
            this.DynamicFormApproved = dynamicFormApproved;
        }
    }
    public class GetDynamicFormSectionById : PagedRequest, IRequest<List<DynamicFormSection>>
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public GetDynamicFormSectionById(long? id, long? userId, long? dynamicFormDataId)
        {
            this.Id = id;
            this.UserId = userId;
            this.DynamicFormDataId = dynamicFormDataId;
        }
    }
    public class InsertDynamicFormDataUpload : DynamicFormDataUpload, IRequest<DynamicFormDataUpload>
    {
        public DynamicFormDataUpload DynamicFormDataUpload { get; set; }
        public InsertDynamicFormDataUpload(DynamicFormDataUpload dynamicFormDataUpload)
        {
            this.DynamicFormDataUpload = dynamicFormDataUpload;
        }
    }

    public class GetDynamicFormSectionWorkFlowByID : PagedRequest, IRequest<List<DynamicFormSectionWorkFlow>>
    {
        public long? DynamicFormSectionId { get; set; }
        public long? UserId { get; set; }
        public GetDynamicFormSectionWorkFlowByID(long? userId, long? dynamicFormSectionId)
        {
            this.UserId = userId;
            this.DynamicFormSectionId = dynamicFormSectionId;
        }
    }

    public class InsertDynamicFormWorkFlow : DynamicFormWorkFlow, IRequest<DynamicFormWorkFlow>
    {
        public DynamicFormWorkFlow DynamicFormWorkFlow { get; set; }
        public InsertDynamicFormWorkFlow(DynamicFormWorkFlow dynamicFormWorkFlow)
        {
            this.DynamicFormWorkFlow = dynamicFormWorkFlow;
        }

    }
    public class GetDynamicFormWorkFlow : PagedRequest, IRequest<List<DynamicFormWorkFlow>>
    {
        public long? DynamicFormId { get; set; }
        public GetDynamicFormWorkFlow(long? dynamicFormId)
        {
            this.DynamicFormId = dynamicFormId;
        }
    }

    public class DeleteDynamicFormWorkFlow : DynamicFormWorkFlow, IRequest<DynamicFormWorkFlow>
    {
        public DynamicFormWorkFlow DynamicFormWorkFlow { get; set; }
        public DeleteDynamicFormWorkFlow(DynamicFormWorkFlow dynamicFormWorkFlow)
        {
            this.DynamicFormWorkFlow = dynamicFormWorkFlow;
        }
    }

    public class GetDynamicFormWorkFlowExits : PagedRequest, IRequest<List<DynamicFormWorkFlowSection>>
    {
        public long? DynamicFormId { get; set; }
        public long? UserId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public GetDynamicFormWorkFlowExits(long? dynamicFormId, long? userId, long? dynamicFormDataId)
        {
            this.DynamicFormId = dynamicFormId;
            this.UserId = userId;
            this.DynamicFormDataId = dynamicFormDataId;
        }
    }
    public class InsertOrUpdateFormWorkFlowSectionNoWorkFlow : PagedRequest, IRequest<DynamicFormWorkFlowSection>
    {
        public List<DynamicFormWorkFlowSection> DynamicFormWorkFlowSection { get; set; }
        public long? UserId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public InsertOrUpdateFormWorkFlowSectionNoWorkFlow(List<DynamicFormWorkFlowSection> dynamicFormWorkFlowSection, long? dynamicFormDataId, long? userId)
        {
            this.DynamicFormWorkFlowSection = dynamicFormWorkFlowSection;
            this.UserId = userId;
            this.DynamicFormDataId = dynamicFormDataId;
        }
    }
    public class GetDynamicFormWorkFlowFormList : PagedRequest, IRequest<List<DynamicFormWorkFlowForm>>
    {
        public long? DynamicFormDataId { get; set; }
        public long? DynamicFormId { get; set; }
        public GetDynamicFormWorkFlowFormList(long? dynamicFormId, long? dynamicFormDataId)
        {
            this.DynamicFormDataId = dynamicFormDataId;
            this.DynamicFormId = dynamicFormId;
        }
    }
    public class GetDynamicFormWorkFlowFormExits : PagedRequest, IRequest<DynamicFormWorkFlowForm>
    {
        public long? DynamicFormDataId { get; set; }
        public long? UserId { get; set; }
        public long? DynamicFormWorkFlowSectionId { get; set; }
        public GetDynamicFormWorkFlowFormExits(long? dynamicFormWorkFlowSectionId, long? userId, long? dynamicFormDataId)
        {
            this.DynamicFormDataId = dynamicFormDataId;
            this.UserId = userId;
            this.DynamicFormWorkFlowSectionId = dynamicFormWorkFlowSectionId;
        }
    }

    public class GetDynamicFormWorkFlowListByUser : PagedRequest, IRequest<List<DynamicFormDataWrokFlow>>
    {
        public long? DynamicFormDataId { get; set; }
        public long? UserId { get; set; }
        public GetDynamicFormWorkFlowListByUser(long? userId, long? dynamicFormDataId)
        {
            this.DynamicFormDataId = dynamicFormDataId;
            this.UserId = userId;
        }
    }
    public class InsertCreateEmailFormData : DynamicFormWorkFlow, IRequest<DynamicFormData>
    {
        public DynamicFormData DynamicFormData { get; set; }
        public InsertCreateEmailFormData(DynamicFormData dynamicFormData)
        {
            this.DynamicFormData = dynamicFormData;
        }
    }
    public class GetEmployeeFormByUserId : PagedRequest, IRequest<ViewEmployee>
    {
        public long? UserId { get; set; }
        public GetEmployeeFormByUserId(long? userId)
        {
            this.UserId = userId;
        }
    }
    public class InsertDynamicFormSectionAttributeSecurity : PagedRequest, IRequest<DynamicFormSectionAttributeSecurity>
    {
        public DynamicFormSectionAttributeSecurity DynamicFormSectionAttributeSecurity { get; private set; }
        public InsertDynamicFormSectionAttributeSecurity(DynamicFormSectionAttributeSecurity dynamicFormSectionAttributeSecurity)
        {
            this.DynamicFormSectionAttributeSecurity = dynamicFormSectionAttributeSecurity;
        }
    }
    public class GetDynamicFormSectionAttributeSecurityList : PagedRequest, IRequest<List<DynamicFormSectionAttributeSecurity>>
    {
        public long? Id { get; set; }
        public GetDynamicFormSectionAttributeSecurityList(long? id)
        {
            this.Id = id;
        }
    }
    public class DeleteDynamicFormSectionAttributeSecurity : DynamicFormSectionAttributeSecurity, IRequest<long>
    {
        public long? Id { get; set; }
        public List<long?> Ids { get; set; }
        public DeleteDynamicFormSectionAttributeSecurity(long? id, List<long?> ids)
        {
            this.Id = id;
            this.Ids = ids;
        }
    }
    public class GetDynamicFormDataApprovalList : PagedRequest, IRequest<List<DynamicFormData>>
    {
        public long? UserId { get; set; }
        public GetDynamicFormDataApprovalList(long? userId)
        {
            this.UserId = userId;
        }
    }
    public class UpdateDynamicFormDataSortOrder : PagedRequest, IRequest<DynamicFormData>
    {
        public DynamicFormData? DynamicFormData { get; set; }
        public UpdateDynamicFormDataSortOrder(DynamicFormData? dynamicFormData)
        {
            this.DynamicFormData = dynamicFormData;
        }
    }
    public class InsertDmsDocumentDynamicFormData : PagedRequest, IRequest<DynamicFormDataUpload>
    {
        public DynamicFormDataUpload? DynamicFormDataUpload { get; set; }
        public InsertDmsDocumentDynamicFormData(DynamicFormDataUpload? dynamicFormDataUpload)
        {
            this.DynamicFormDataUpload = dynamicFormDataUpload;
        }
    }
    

}

