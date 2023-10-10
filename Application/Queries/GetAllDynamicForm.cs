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
    public class GetDynamicFormDataById : PagedRequest, IRequest<List<DynamicFormData>>
    {
        public long? Id { get; set; }
        public GetDynamicFormDataById(long? id)
        {
            this.Id = id;
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
}

