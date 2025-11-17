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
    public class GetDynamicFormScreenNameCheckValidation : PagedRequest, IRequest<DynamicForm>
    {
        public string? ScreenName { get; set; }
        public GetDynamicFormScreenNameCheckValidation(string? ScreenName)
        {
            this.ScreenName = ScreenName;
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
    public class DeleteDynamicForm : DynamicForm, IRequest<DynamicForm>
    {
        public DynamicForm DynamicForm { get; set; }
        public DeleteDynamicForm(DynamicForm dynamicForm)
        {
            this.DynamicForm = dynamicForm;
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
    public class GetDynamicFormSectionIds : PagedRequest, IRequest<List<DynamicFormSection>>
    {
        public List<long> DynamicFormSectionIds { get; set; }
        public GetDynamicFormSectionIds(List<long> id)
        {
            this.DynamicFormSectionIds = id;
        }
    }
    public class InsertOrUpdateDynamicFormSectionAttribute : DynamicFormSectionAttribute, IRequest<DynamicFormSectionAttribute>
    {

    }
    public class UpdateFormulaTextBox : PagedRequest, IRequest<DynamicFormSectionAttribute>
    {
        public DynamicFormSectionAttribute DynamicFormSectionAttribute { get; set; }
        public UpdateFormulaTextBox(DynamicFormSectionAttribute dynamicFormSectionAttribute)
        {
            this.DynamicFormSectionAttribute = dynamicFormSectionAttribute;
        }
    }

    public class GetDynamicFormSectionAttribute : PagedRequest, IRequest<List<DynamicFormSectionAttribute>>
    {
        public long? Id { get; set; }
        public GetDynamicFormSectionAttribute(long? id)
        {
            this.Id = id;
        }
    }
    public class GetDynamicFormSectionAttributeByIds : PagedRequest, IRequest<List<DynamicFormSectionAttribute>>
    {
        public List<long> Id { get; set; }
        public GetDynamicFormSectionAttributeByIds(List<long> id)
        {
            this.Id = id;
        }
    }
    public class GetDynamicFormSectionAttributeAll : PagedRequest, IRequest<List<DynamicFormSectionAttribute>>
    {
        public long? Id { get; set; }
        public GetDynamicFormSectionAttributeAll(long? id)
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
    public class GetDynamicFormDataBySessionAllId : PagedRequest, IRequest<DynamicFormData>
    {
        public string? SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetDynamicFormDataBySessionAllId(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }
    
    public class GetDynamicFormDataBySessionOne : PagedRequest, IRequest<DynamicFormData>
    {
        public Guid? SesionId { get; set; }
        public GetDynamicFormDataBySessionOne(Guid? SessionId)
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
        public Guid? DynamicFormDataSessionId { get; set; }
        public DynamicFormSearch DynamicFormSearch { get; set; }
        public GetDynamicFormDataById(long? id, long? userId, long? dynamicFormDataGridId, long? dynamicFormSectionGridAttributeId, Guid? dynamicFormDataSessionId, DynamicFormSearch dynamicFormSearch)
        {
            this.Id = id;
            this.UserId = userId;
            this.DynamicFormDataGridId = dynamicFormDataGridId;
            this.DynamicFormSectionGridAttributeId = dynamicFormSectionGridAttributeId;
            this.DynamicFormDataSessionId = dynamicFormDataSessionId;
            this.DynamicFormSearch = dynamicFormSearch;
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
    public class InsertOrUpdateDynamicFormDataApproved : DynamicFormApproved, IRequest<DynamicFormApproved>
    {

    }
    public class DeleteDynamicFormDataApprovedQuery : DynamicFormApproval, IRequest<DynamicFormApproved>
    {
        public DynamicFormApproved DynamicFormApproved { get; set; }
        public DeleteDynamicFormDataApprovedQuery(DynamicFormApproved dynamicFormApproved)
        {
            this.DynamicFormApproved = dynamicFormApproved;
        }
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
        public long? UserId { get; set; }
        public DeleteDynamicFormSectionSecurity(long? id, List<long?> ids, long? userId)
        {
            this.Id = id;
            this.Ids = ids;
            UserId = userId;
        }
    }
    public class DeleteDynamicFormDataAssignUser : DynamicFormDataAssignUser, IRequest<long>
    {
        public long? Id { get; set; }
        public List<long?> Ids { get; set; }
        public DeleteDynamicFormDataAssignUser(long? id, List<long?> ids)
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
        public long? UserId { get; set; }
        public InsertDynamicFormSectionAttributeSecurity(DynamicFormSectionAttributeSecurity dynamicFormSectionAttributeSecurity, long? userId)
        {
            this.DynamicFormSectionAttributeSecurity = dynamicFormSectionAttributeSecurity;
            UserId = userId;
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
    public class GetDynamicFormDataList : PagedRequest, IRequest<List<DynamicFormData>>
    {
        public List<long?> DyamicFormIds { get; set; }
        public long? DynamicFormDataGridId { get; set; }
        public GetDynamicFormDataList(List<long?> dyamicFormIds, long? dynamicFormDataGridId)
        {
            this.DyamicFormIds = dyamicFormIds;
            DynamicFormDataGridId = dynamicFormDataGridId;
        }
    }

    public class DeleteDynamicFormSectionAttributeSecurity : DynamicFormSectionAttributeSecurity, IRequest<long>
    {
        public long? Id { get; set; }
        public List<long?> Ids { get; set; }
        public long? DynamicFormId { get; set; }
        public DeleteDynamicFormSectionAttributeSecurity(long? id, List<long?> ids, long? dynamicFormId, long? userId)
        {
            this.Id = id;
            this.Ids = ids;
            DynamicFormId = dynamicFormId;
            UserId = userId;
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
    public class InsertDynamicFormReport : DynamicFormReport, IRequest<DynamicFormReport>
    {

    }
    public class GetDynamicFormReportList : PagedRequest, IRequest<List<DynamicFormReport>>
    {
        public long? DynamicFormId { get; set; }
        public GetDynamicFormReportList(long? dynamicFormId)
        {
            this.DynamicFormId = dynamicFormId;
        }
    }
    public class DeleteDynamicFormReport : DynamicFormReport, IRequest<DynamicFormReport>
    {
        public DynamicFormReport DynamicFormReport { get; set; }
        public DeleteDynamicFormReport(DynamicFormReport dynamicFormReport)
        {
            this.DynamicFormReport = dynamicFormReport;
        }
    }
    public class GetDynamicFormReportOneData : PagedRequest, IRequest<DynamicFormReport>
    {
        public Guid? SessionId { get; set; }
        public GetDynamicFormReportOneData(Guid? sessionId)
        {
            this.SessionId = sessionId;
        }
    }
    public class GetDynamicFormApplicationMasterParent : PagedRequest, IRequest<List<ApplicationMasterParent>>
    {
        public long? DynamicFormId { get; set; }
        public GetDynamicFormApplicationMasterParent(long? dynamicFormId)
        {
            this.DynamicFormId = dynamicFormId;
        }
    }
    public class GetDynamicFormApplicationMaster : PagedRequest, IRequest<List<ApplicationMaster>>
    {
        public long? DynamicFormId { get; set; }
        public GetDynamicFormApplicationMaster(long? dynamicFormId)
        {
            this.DynamicFormId = dynamicFormId;
        }
    }

    public class GetDynamicFormSectionAttributeSectionParent : PagedRequest, IRequest<List<DynamicFormSectionAttributeSectionParent>>
    {
        public long? DynamicFormSectionAttributeId { get; set; }
        public GetDynamicFormSectionAttributeSectionParent(long? dynamicFormSectionAttributeId)
        {
            this.DynamicFormSectionAttributeId = dynamicFormSectionAttributeId;
        }
    }
    public class InsertOrUpdateDynamicFormSectionAttributeSectionParent : DynamicFormSectionAttributeSectionParent, IRequest<DynamicFormSectionAttributeSectionParent>
    {

    }
    public class DeleteDynamicFormSectionAttributeParent : DynamicFormSectionAttributeSectionParent, IRequest<DynamicFormSectionAttributeSectionParent>
    {
        public DynamicFormSectionAttributeSectionParent DynamicFormSectionAttributeSectionParent { get; set; }
        public DeleteDynamicFormSectionAttributeParent(DynamicFormSectionAttributeSectionParent dynamicFormSectionAttributeSectionParent)
        {
            this.DynamicFormSectionAttributeSectionParent = dynamicFormSectionAttributeSectionParent;
        }
    }
    public class UpdateDynamicFormLocked : PagedRequest, IRequest<DynamicFormData>
    {
        public DynamicFormData? DynamicFormData { get; set; }
        public UpdateDynamicFormLocked(DynamicFormData? dynamicFormData)
        {
            this.DynamicFormData = dynamicFormData;
        }
    }
    public class InsertDynamicFormApprovedChanged : PagedRequest, IRequest<DynamicFormApprovedChanged>
    {
        public DynamicFormApprovedChanged DynamicFormApprovedChanged { get; set; }
        public InsertDynamicFormApprovedChanged(DynamicFormApprovedChanged dynamicFormApprovedChanged)
        {
            this.DynamicFormApprovedChanged = dynamicFormApprovedChanged;
        }
    }
    public class InsertDynamicFormWorkFlowApproval : DynamicFormWorkFlowApproval, IRequest<DynamicFormWorkFlowApproval>
    {

    }
    public class DeleteDynamicFormWorkFlowApproval : PagedRequest, IRequest<DynamicFormWorkFlowApproval>
    {
        public DynamicFormWorkFlowApproval DynamicFormWorkFlowApproval { get; set; }
        public DeleteDynamicFormWorkFlowApproval(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval)
        {
            this.DynamicFormWorkFlowApproval = dynamicFormWorkFlowApproval;
        }
    }
    public class GetDynamicFormWorkFlowApprovalList : PagedRequest, IRequest<List<DynamicFormWorkFlowApproval>>
    {
        public long? DynamicFormWorkFlowId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public GetDynamicFormWorkFlowApprovalList(long? dynamicFormWorkFlowId, long? dynamicFormDataId)
        {
            this.DynamicFormWorkFlowId = dynamicFormWorkFlowId;
            this.DynamicFormDataId = dynamicFormDataId;
        }
    }
    public class UpdateDynamicFormWorkFlowApprovalSortOrder : PagedRequest, IRequest<DynamicFormWorkFlowApproval>
    {
        public DynamicFormWorkFlowApproval DynamicFormWorkFlowApproval { get; private set; }
        public UpdateDynamicFormWorkFlowApprovalSortOrder(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval)
        {
            this.DynamicFormWorkFlowApproval = dynamicFormWorkFlowApproval;
        }
    }
    public class GetDynamicFormWorkFlowApprovedFormList : PagedRequest, IRequest<List<DynamicFormWorkFlowApprovedForm>>
    {
        public long? DynamicFormDataId { get; set; }
        public GetDynamicFormWorkFlowApprovedFormList(long? dynamicFormDataId)
        {
            this.DynamicFormDataId = dynamicFormDataId;
        }
    }
    public class UpdateDynamicFormWorkFlowApprovedFormByUser : PagedRequest, IRequest<DynamicFormWorkFlowApprovedForm>
    {
        public DynamicFormWorkFlowApprovedForm DynamicFormWorkFlowApprovedForm { get; set; }
        public UpdateDynamicFormWorkFlowApprovedFormByUser(DynamicFormWorkFlowApprovedForm dynamicFormWorkFlowApprovedForm)
        {
            this.DynamicFormWorkFlowApprovedForm = dynamicFormWorkFlowApprovedForm;
        }
    }
    public class InsertDynamicFormWorkFlowApprovedFormChanged : PagedRequest, IRequest<DynamicFormWorkFlowApprovedFormChanged>
    {
        public DynamicFormWorkFlowApprovedFormChanged DynamicFormWorkFlowApprovedFormChanged { get; set; }
        public InsertDynamicFormWorkFlowApprovedFormChanged(DynamicFormWorkFlowApprovedFormChanged dynamicFormWorkFlowApprovedFormChanged)
        {
            this.DynamicFormWorkFlowApprovedFormChanged = dynamicFormWorkFlowApprovedFormChanged;
        }
    }
    public class InsertDynamicFormDataWorkFlowApproval : DynamicFormWorkFlowApproval, IRequest<DynamicFormWorkFlowApproval>
    {

    }
    public class DeleteDynamicFormDataWorkFlowApproval : PagedRequest, IRequest<DynamicFormWorkFlowApproval>
    {
        public DynamicFormWorkFlowApproval DynamicFormWorkFlowApproval { get; set; }
        public DeleteDynamicFormDataWorkFlowApproval(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval)
        {
            this.DynamicFormWorkFlowApproval = dynamicFormWorkFlowApproval;
        }
    }
    public class GetDynamicFormWorkFlowApprovedFormByList : PagedRequest, IRequest<List<DynamicFormWorkFlowApprovedForm>>
    {
        public long? UserId { get; set; }
        public int? FlowStatusID { get; set; }
        public GetDynamicFormWorkFlowApprovedFormByList(long? userId, int? flowStatusID)
        {
            this.UserId = userId;
            this.FlowStatusID = flowStatusID;
        }
    }
    public class GetDynamicFormWorkFlowFormDelegateList : PagedRequest, IRequest<List<DynamicFormWorkFlowFormDelegate>>
    {
        public long? DynamicFormWorkFlowFormID { get; set; }
        public GetDynamicFormWorkFlowFormDelegateList(long? dynamicFormWorkFlowFormID)
        {
            this.DynamicFormWorkFlowFormID = dynamicFormWorkFlowFormID;
        }
    }
    public class InsertDynamicFormWorkFlowFormDelegate : PagedRequest, IRequest<DynamicFormWorkFlowFormDelegate>
    {
        public DynamicFormWorkFlowFormDelegate DynamicFormWorkFlowFormDelegate { get; set; }
        public InsertDynamicFormWorkFlowFormDelegate(DynamicFormWorkFlowFormDelegate dynamicFormWorkFlowFormDelegate)
        {
            this.DynamicFormWorkFlowFormDelegate = dynamicFormWorkFlowFormDelegate;
        }
    }
    public class InsertDynamicFormWorkFlowForm : DynamicFormWorkFlowForm, IRequest<DynamicFormWorkFlowForm>
    {
        public DynamicFormWorkFlowForm DynamicFormWorkFlowForm { get; set; }
        public InsertDynamicFormWorkFlowForm(DynamicFormWorkFlowForm dynamicFormWorkFlowForm)
        {
            this.DynamicFormWorkFlowForm = dynamicFormWorkFlowForm;
        }

    }
    public class DeleteDynamicFormWorkFlowForm : DynamicFormWorkFlowForm, IRequest<DynamicFormWorkFlowForm>
    {
        public DynamicFormWorkFlowForm DynamicFormWorkFlowForm { get; set; }
        public DeleteDynamicFormWorkFlowForm(DynamicFormWorkFlowForm dynamicFormWorkFlowForm)
        {
            this.DynamicFormWorkFlowForm = dynamicFormWorkFlowForm;
        }
    }
    public class UpdateDynamicFormDataSectionLock : DynamicFormDataSectionLock, IRequest<DynamicFormDataSectionLock>
    {
        public DynamicFormDataSectionLock DynamicFormDataSectionLock { get; set; }
        public UpdateDynamicFormDataSectionLock(DynamicFormDataSectionLock dynamicFormDataSectionLock)
        {
            this.DynamicFormDataSectionLock = dynamicFormDataSectionLock;
        }

    }
    public class InsertCloneDynamicForm : PagedRequest, IRequest<DynamicForm>
    {
        public DynamicForm DynamicForm { get; set; }
        public bool? IsWithoutForm { get; set; }
        public long? UserId { get; set; }
        public InsertCloneDynamicForm(DynamicForm dynamicForm, bool? isWithoutForm, long? userId)
        {
            this.DynamicForm = dynamicForm;
            this.IsWithoutForm = isWithoutForm;
            this.UserId = userId;
        }

    }
    public class GetDynamicFormSectionAttributeForSpinEdit : PagedRequest, IRequest<List<DynamicFormSectionAttribute>>
    {
        public long? Id { get; set; }
        public GetDynamicFormSectionAttributeForSpinEdit(long? id)
        {
            this.Id = id;
        }
    }
    public class InsertDynamicFormEmailSubCont : PagedRequest, IRequest<DynamicFormReportItems>
    {
        public Guid? SessionId { get; set; }
        public IEnumerable<DynamicFormReportItems> SubjectData { get; set; }
        public IEnumerable<DynamicFormReportItems> ContentData { get; set; }
        public InsertDynamicFormEmailSubCont(Guid? sessionId, IEnumerable<DynamicFormReportItems> subjectData, IEnumerable<DynamicFormReportItems> contentData)
        {
            this.SessionId = sessionId;
            this.SubjectData = subjectData;
            this.ContentData = contentData;
        }
    }
    public class GetDynamicFormEmailSubCont : PagedRequest, IRequest<List<DynamicFormEmailSubCont>>
    {
        public Guid? SessionId { get; set; }
        public GetDynamicFormEmailSubCont(Guid? sessionId)
        {
            this.SessionId = sessionId;
        }
    }
    public class DeleteDynamicFormEmailSubCont : PagedRequest, IRequest<DynamicFormEmailSubCont>
    {
        public Guid? SessionId { get; set; }
        public DeleteDynamicFormEmailSubCont(Guid? sessionId)
        {
            this.SessionId = sessionId;
        }
    }
    public class InsertDynamicFormPermissionPermission : PagedRequest, IRequest<DynamicForm>
    {
        public DynamicForm DynamicForm { get; set; }
        public InsertDynamicFormPermissionPermission(DynamicForm dynamicForm)
        {
            this.DynamicForm = dynamicForm;
        }
    }
    public class GetDynamicFormMenuList : PagedRequest, IRequest<List<ApplicationPermission>>
    {
    }
    public class UpdateDynamicFormMenuSortOrder : PagedRequest, IRequest<ApplicationPermission>
    {
        public ApplicationPermission ApplicationPermission { get; private set; }
        public UpdateDynamicFormMenuSortOrder(ApplicationPermission applicationPermission)
        {
            this.ApplicationPermission = applicationPermission;
        }
    }
    public class UpdateDynamicFormSectionAttributeGridSequenceSortOrder : PagedRequest, IRequest<DynamicFormSectionAttribute>
    {
        public DynamicFormSectionAttribute DynamicFormSectionAttribute { get; private set; }
        public UpdateDynamicFormSectionAttributeGridSequenceSortOrder(DynamicFormSectionAttribute dynamicFormSectionAttribute)
        {
            this.DynamicFormSectionAttribute = dynamicFormSectionAttribute;
        }
    }
    public class UpdateDynamicFormSectionAttributeAllByCheckBox : PagedRequest, IRequest<DynamicFormSectionAttribute>
    {
        public DynamicFormSectionAttribute DynamicFormSectionAttribute { get; private set; }
        public UpdateDynamicFormSectionAttributeAllByCheckBox(DynamicFormSectionAttribute dynamicFormSectionAttribute)
        {
            this.DynamicFormSectionAttribute = dynamicFormSectionAttribute;
        }
    }
    public class DeleteDynamicFormMenu : PagedRequest, IRequest<DynamicForm>
    {
        public DynamicForm DynamicForm { get; private set; }
        public DeleteDynamicFormMenu(DynamicForm dynamicForm)
        {
            this.DynamicForm = dynamicForm;
        }
    }
    public class GetDynamicFormSectionAttrFormulaMasterFunction : PagedRequest, IRequest<List<DynamicFormSectionAttrFormulaMasterFunction>>
    {
    }
    public class GetDynamicFormSectionAttrFormulaFunction : PagedRequest, IRequest<List<DynamicFormSectionAttrFormulaFunction>>
    {
        public long? DynamicFormSectionAttributeId { get; private set; }
        public GetDynamicFormSectionAttrFormulaFunction(long? dynamicFormSectionAttributeId)
        {
            this.DynamicFormSectionAttributeId = dynamicFormSectionAttributeId;
        }
    }
    public class InsertOrUpdateDynamicFormSectionAttrFormulaFunction : DynamicFormSectionAttrFormulaFunction, IRequest<DynamicFormSectionAttrFormulaFunction>
    {
    }
    public class DeleteDynamicFormSectionAttrFormulaFunction : PagedRequest, IRequest<DynamicFormSectionAttrFormulaFunction>
    {
        public DynamicFormSectionAttrFormulaFunction DynamicFormSectionAttrFormulaFunction { get; private set; }
        public DeleteDynamicFormSectionAttrFormulaFunction(DynamicFormSectionAttrFormulaFunction dynamicFormSectionAttrFormulaFunction)
        {
            this.DynamicFormSectionAttrFormulaFunction = dynamicFormSectionAttrFormulaFunction;
        }
    }
    public class InsertDynamicFormDataAssignUser : PagedRequest, IRequest<DynamicFormDataAssignUser>
    {
        public DynamicFormDataAssignUser DynamicFormDataAssignUser { get; private set; }
        public InsertDynamicFormDataAssignUser(DynamicFormDataAssignUser dynamicFormDataAssignUser)
        {
            this.DynamicFormDataAssignUser = dynamicFormDataAssignUser;
        }
    }
    public class GetDynamicFormDataAssignUserList : PagedRequest, IRequest<List<DynamicFormDataAssignUser>>
    {
        public long? DynamicFormId { get; private set; }
        public GetDynamicFormDataAssignUserList(long? dynamicFormId)
        {
            this.DynamicFormId = dynamicFormId;
        }
    }
    public class GetDynamicFormDataAssignUserAllList : PagedRequest, IRequest<List<DynamicFormDataAssignUser>>
    {
    }
    public class GetDynamicFormDataAuditList : PagedRequest, IRequest<List<DynamicFormDataAudit>>
    {
        public Guid? SessionId { get; private set; }
        public GetDynamicFormDataAuditList(Guid? sessionId)
        {
            this.SessionId = sessionId;
        }
    }
    public class GetDynamicFormDataAuditBySessionList : PagedRequest, IRequest<List<DynamicFormDataAudit>>
    {
        public Guid? SessionId { get; private set; }
        public GetDynamicFormDataAuditBySessionList(Guid? sessionId)
        {
            this.SessionId = sessionId;
        }
    }
    public class GetDynamicFormDataAuditBySessionMultipleList : PagedRequest, IRequest<List<DynamicFormDataAudit>>
    {
        public List<Guid?> SessionId { get; private set; }
        public GetDynamicFormDataAuditBySessionMultipleList(List<Guid?> sessionId)
        {
            this.SessionId = sessionId;
        }
    }
    public class GetDynamicFormDataAuditMultipleList : PagedRequest, IRequest<List<DynamicFormDataAudit>>
    {
        public List<Guid?> SessionId { get; private set; }
        public GetDynamicFormDataAuditMultipleList(List<Guid?> sessionId)
        {
            this.SessionId = sessionId;
        }
    }

    public class GetDynamicFormFormulaMathFunList : PagedRequest, IRequest<List<DynamicFormFormulaMathFun>>
    {

    }
    public class GetDynamicFormSectionSecuritySettingList : PagedRequest, IRequest<List<DynamicFormSectionSecurity>>
    {
        public long? Id { get; set; }
        public GetDynamicFormSectionSecuritySettingList(long? id)
        {
            this.Id = id;
        }
    }
    public class GetDynamicFormWorkFlowApprovalSettingList : PagedRequest, IRequest<List<DynamicFormWorkFlowApproval>>
    {
        public long? Id { get; set; }
        public GetDynamicFormWorkFlowApprovalSettingList(long? id)
        {
            this.Id = id;
        }
    }
    public class GetDynamicFormSectionAttributeSecuritySettingList : PagedRequest, IRequest<List<DynamicFormSectionAttributeSecurity>>
    {
        public long? Id { get; set; }
        public GetDynamicFormSectionAttributeSecuritySettingList(long? id)
        {
            this.Id = id;
        }
    }
    public class GetDynamicFormSectionAttributeSectionParentSettings : PagedRequest, IRequest<List<DynamicFormSectionAttributeSectionParent>>
    {
        public long? Id { get; set; }
        public GetDynamicFormSectionAttributeSectionParentSettings(long? id)
        {
            this.Id = id;
        }
    }
    public class GetDynamicFormDataUploadCheckValidation : PagedRequest, IRequest<DynamicFormDataUpload>
    {
        public long? DynamicFormDataId { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public GetDynamicFormDataUploadCheckValidation(long? dynamicFormDataId, long? dynamicFormSectionId)
        {
            this.DynamicFormDataId = dynamicFormDataId;
            this.DynamicFormSectionId = dynamicFormSectionId;
        }
    }

    public class InsertDynamicFormDataAttrUpload : PagedRequest, IRequest<DynamicFormDataAttrUpload>
    {
        public List<DynamicFormDataAttrUpload> DynamicFormDataAttrUpload { get; set; }
        public InsertDynamicFormDataAttrUpload(List<DynamicFormDataAttrUpload> dynamicFormDataAttrUploads)
        {
            this.DynamicFormDataAttrUpload = dynamicFormDataAttrUploads;
        }
    }
    public class GetDynamicFormDataAttrUpload : PagedRequest, IRequest<List<DynamicFormDataAttrUpload>>
    {
        public long? Id { get; set; }
        public long? DynamicFormDataId { get; set; }
        public GetDynamicFormDataAttrUpload(long? id, long? dynamicFormDataId)
        {
            this.Id = id;
            this.DynamicFormDataId = dynamicFormDataId;
        }
    }
    public class GetDynamicFormAttributeItemList : PagedRequest, IRequest<List<DynamicFormSectionAttributesList>>
    {
        public long? ID { get; set; }
        public GetDynamicFormAttributeItemList(long? ID)
        {
            this.ID = ID;
        }
    }
    public class DeleteDynamicFormDataAttrUpload : PagedRequest, IRequest<DynamicFormDataAttrUpload>
    {
        public DynamicFormDataAttrUpload DynamicFormDataAttrUpload { get; private set; }
        public DeleteDynamicFormDataAttrUpload(DynamicFormDataAttrUpload dynamicFormDataAttrUpload)
        {
            this.DynamicFormDataAttrUpload = dynamicFormDataAttrUpload;
        }
    }
    public class GetDynamicFormDataOneBySessionId : PagedRequest, IRequest<DynamicFormData>
    {
        public Guid? DynamicFormDataSessionId { get; set; }
        public GetDynamicFormDataOneBySessionId(Guid? dynamicFormDataId)
        {
            this.DynamicFormDataSessionId = dynamicFormDataId;
        }
    }
    public class GetDynamicFormDataAuditMasterList : PagedRequest, IRequest<List<DynamicFormDataAudit>>
    {
        public DynamicFormDataAudit DynamicFormDataAudit { get; set; }
        public GetDynamicFormDataAuditMasterList(DynamicFormDataAudit dynamicFormDataAudit)
        {
            this.DynamicFormDataAudit = dynamicFormDataAudit;
        }
    }

    public class GetDynamicFormDataOneByDataId : PagedRequest, IRequest<DynamicFormData>
    {
        public long? DynamicFormId { get; set; }
        public GetDynamicFormDataOneByDataId(long? dynamicFormId)
        {
            this.DynamicFormId = dynamicFormId;
        }
    }

    public class GetDynamicFormAuditList : PagedRequest, IRequest<List<DynamicFormAudit>>
    {
        public long? DynamicFormId { get; set; }
        public Guid? SessionId { get; set; }
        public GetDynamicFormAuditList(long? dynamicFormId, Guid? sessionId)
        {
            this.DynamicFormId = dynamicFormId;
            SessionId = sessionId;
        }
    }
    public class GetDynamicFormAuditDynamicFormSectionList : PagedRequest, IRequest<List<DynamicFormAudit>>
    {
        public List<Guid?> SessionIds { get; set; }
        public string? FormType { get; set; }
        public GetDynamicFormAuditDynamicFormSectionList(List<Guid?> dynamicFormId, string? formType)
        {
            this.SessionIds = dynamicFormId;
            FormType = formType;
        }
    }
    public class GeDynamicFormWorkFlowListIds : PagedRequest, IRequest<List<DynamicFormWorkFlow>>
    {
        public List<long?> Ids { get; set; }
        public GeDynamicFormWorkFlowListIds(List<long?> ids)
        {
            this.Ids = ids;
        }
    }
    public class GeDynamicFormPermissionListIds : PagedRequest, IRequest<List<DynamicFormPermission>>
    {
        public List<long?> Ids { get; set; }
        public GeDynamicFormPermissionListIds(List<long?> ids)
        {
            this.Ids = ids;
        }
    }
    public class GetDynamicFormDataTableSync : PagedRequest, IRequest<DynamicForm>
    {
        public List<DropDownOptionsModel> DropDownOptionsModel { get; set; }
        public List<object> DynamicformObjectDataList { get; set; }
        public AttributeHeaderListModel AttributeHeader { get; set; }
        public DynamicForm DynamicForm { get; set; }
        public GetDynamicFormDataTableSync(List<DropDownOptionsModel> dropDownOptionsModels, List<object> _dynamicformObjectDataList, AttributeHeaderListModel attributeHeaderListModel, DynamicForm dynamicForm)
        {
            this.DropDownOptionsModel = dropDownOptionsModels;
            this.DynamicformObjectDataList = _dynamicformObjectDataList;
            this.AttributeHeader = attributeHeaderListModel;
            this.DynamicForm = dynamicForm;
        }
    }
    public class GetOnDynamicFormSyncTables : PagedRequest, IRequest<DynamicForm>
    {
        public DynamicForm? DynamicForm { get; set; }
        public long? UserId { get; set; }
        public GetOnDynamicFormSyncTables(DynamicForm? dynamicForm,long? userId)
        {
            this.DynamicForm = dynamicForm;
            this.UserId = userId;
        }
    }
    public class InsertDynamicFormPermission : PagedRequest, IRequest<DynamicFormPermission>
    {
        public DynamicFormPermission DynamicFormPermission { get; private set; }
        public InsertDynamicFormPermission(DynamicFormPermission dynamicFormSectionSecurity)
        {
            this.DynamicFormPermission = dynamicFormSectionSecurity;
        }
    }
    public class GetDynamicFormPermissionList : PagedRequest, IRequest<List<DynamicFormPermission>>
    {
        public long? Id { get; set; }
        public GetDynamicFormPermissionList(long? id)
        {
            this.Id = id;
        }
    }
    public class DeleteDynamicFormPermission : DynamicFormPermission, IRequest<long>
    {
        public long? Id { get; set; }
        public List<long?> Ids { get; set; }
        public long? UserId { get; set; }
        public DeleteDynamicFormPermission(long? id, List<long?> ids, long? userId)
        {
            this.Id = id;
            this.Ids = ids;
            UserId = userId;
        }
    }
    public class GetDynamicFormPermissionCheck : PagedRequest, IRequest<DynamicFormPermission>
    {
        public long? DynamicFormId { get; set; }
        public long? UserId { get; set; }
        public GetDynamicFormPermissionCheck(long? dynamicFormId, long? userId)
        {
            this.DynamicFormId = dynamicFormId;
            UserId = userId;
        }
    }
}

