using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
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
        public GetAllDynamicFormList(Guid? ID)
        {
            this.ID = ID;
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
        public InsertDynamicFormAttributeSection(long dynamicFormSectionId, IEnumerable<AttributeHeader> attributeIds,long? userId)
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
    
}

