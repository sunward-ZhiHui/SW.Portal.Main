using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllDynamicFormHandler : IRequestHandler<GetAllDynamicForm, List<DynamicForm>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetAllDynamicFormHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicForm>> Handle(GetAllDynamicForm request, CancellationToken cancellationToken)
        {
            return (List<DynamicForm>)await _dynamicFormQueryRepository.GetAllAsync(request.UserId);
        }


    }
    public class GetAllByGridFormHandler : IRequestHandler<GetAllByGridForm, List<DynamicForm>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetAllByGridFormHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicForm>> Handle(GetAllByGridForm request, CancellationToken cancellationToken)
        {
            return (List<DynamicForm>)await _dynamicFormQueryRepository.GetAllByGridFormAsync(request.UserId);
        }


    }
    public class GetAllByGridNoFormHandler : IRequestHandler<GetAllByGridNoForm, List<DynamicForm>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetAllByGridNoFormHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicForm>> Handle(GetAllByGridNoForm request, CancellationToken cancellationToken)
        {
            return (List<DynamicForm>)await _dynamicFormQueryRepository.GetAllByNoGridFormAsync(request.UserId);
        }


    }


    public class GetDynamicFormBySessionHandler : IRequestHandler<GetDynamicFormBySession, DynamicForm>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormBySessionHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicForm> Handle(GetDynamicFormBySession request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormBySessionIdAsync(request.SesionId);
        }
    }
    public class GetDynamicFormByIdHandler : IRequestHandler<GetDynamicFormById, DynamicForm>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormByIdHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicForm> Handle(GetDynamicFormById request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormByIdAsync(request.Id);
        }
    }
    public class CreateDynamicFormHandler : IRequestHandler<CreateDynamicForm, long>
    {


        private readonly IDynamicFormQueryRepository _queryRepository;
        public CreateDynamicFormHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<long> Handle(CreateDynamicForm request, CancellationToken cancellationToken)
        {
            return (long)await _queryRepository.Insert(request);

        }
    }

    public class GetAllDynamicFormLstHandler : IRequestHandler<GetAllDynamicFormList, DynamicForm>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;
        public GetAllDynamicFormLstHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {

            _DynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicForm> Handle(GetAllDynamicFormList request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.GetAllSelectedList(request.ID, request.DynamicFormDataId);
        }
    }

    public class EditDynamicFormHandler : IRequestHandler<EditDynamicForm, long>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;
        public EditDynamicFormHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<long> Handle(EditDynamicForm request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.Update(request);
        }
    }
    public class DeleteDynamicFormHandler : IRequestHandler<DeleteDynamicForm, long>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<long> Handle(DeleteDynamicForm request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.Delete(request.ID);
        }
    }



    public class InsertOrUpdateDynamicFormSectionHandler : IRequestHandler<InsertOrUpdateDynamicFormSection, DynamicFormSection>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertOrUpdateDynamicFormSectionHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormSection> Handle(InsertOrUpdateDynamicFormSection request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateDynamicFormSection(request);

        }
    }

    public class GetDynamicFormSectionHandler : IRequestHandler<GetDynamicFormSection, List<DynamicFormSection>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSection>> Handle(GetDynamicFormSection request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSection>)await _dynamicFormQueryRepository.GetDynamicFormSectionAsync(request.Id);
        }


    }


    public class InsertOrUpdateDynamicFormSectionAttributeHandler : IRequestHandler<InsertOrUpdateDynamicFormSectionAttribute, DynamicFormSectionAttribute>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertOrUpdateDynamicFormSectionAttributeHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormSectionAttribute> Handle(InsertOrUpdateDynamicFormSectionAttribute request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateDynamicFormSectionAttribute(request);

        }
    }

    public class GetDynamicFormSectionAttributeHandler : IRequestHandler<GetDynamicFormSectionAttribute, List<DynamicFormSectionAttribute>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionAttributeHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionAttribute>> Handle(GetDynamicFormSectionAttribute request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttribute>)await _dynamicFormQueryRepository.GetDynamicFormSectionAttributeAsync(request.Id);
        }


    }
    public class DeleteDynamicFormSectionHandler : IRequestHandler<DeleteDynamicFormSection, long>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormSectionHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<long> Handle(DeleteDynamicFormSection request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.DeleteDynamicFormSection(request.DynamicFormSection);
        }
    }
    public class UpdateDynamicFormSectionSortOrderHandler : IRequestHandler<UpdateDynamicFormSectionSortOrder, DynamicFormSection>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormSectionSortOrderHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormSection> Handle(UpdateDynamicFormSectionSortOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormSectionSortOrder(request.DynamicFormSection);

        }
    }



    public class DeleteDynamicFormSectionAttributeHandler : IRequestHandler<DeleteDynamicFormSectionAttribute, long>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormSectionAttributeHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<long> Handle(DeleteDynamicFormSectionAttribute request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.DeleteDynamicFormSectionAttribute(request.DynamicFormSectionAttribute);
        }
    }
    public class UpdateDynamicFormSectionAttributeSortOrderHandler : IRequestHandler<UpdateDynamicFormSectionAttributeSortOrder, DynamicFormSectionAttribute>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormSectionAttributeSortOrderHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormSectionAttribute> Handle(UpdateDynamicFormSectionAttributeSortOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormSectionAttributeSortOrder(request.DynamicFormSectionAttribute);

        }
    }
    public class InsertDynamicFormAttributeSectionHandler : IRequestHandler<InsertDynamicFormAttributeSection, long>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public InsertDynamicFormAttributeSectionHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<long> Handle(InsertDynamicFormAttributeSection request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.InsertDynamicFormAttributeSection(request.DynamicFormSectionId, request.AttributeIds, request.UserId);
        }
    }
    public class InsertOrUpdateDynamicFormDataHandler : IRequestHandler<InsertOrUpdateDynamicFormData, DynamicFormData>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertOrUpdateDynamicFormDataHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormData> Handle(InsertOrUpdateDynamicFormData request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateDynamicFormData(request);

        }
    }
    public class GetDynamicFormDataBySessionIdHandler : IRequestHandler<GetDynamicFormDataBySessionId, DynamicFormData>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataBySessionIdHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicFormData> Handle(GetDynamicFormDataBySessionId request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormDataBySessionIdAsync(request.SesionId);
        }


    }
    public class GetDynamicFormDataBySessionIdForDMSAsyncHandler : IRequestHandler<GetDynamicFormDataBySessionIdForDMS, DocumentsModel>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataBySessionIdForDMSAsyncHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DocumentsModel> Handle(GetDynamicFormDataBySessionIdForDMS request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormDataBySessionIdForDMSAsync(request.SesionId);
        }


    }
    public class GetDynamicFormDataByIdHandler : IRequestHandler<GetDynamicFormDataById, List<DynamicFormData>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataByIdHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormData>> Handle(GetDynamicFormDataById request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormData>)await _dynamicFormQueryRepository.GetDynamicFormDataByIdAsync(request.Id, request.UserId, request.DynamicFormDataGridId,request.DynamicFormSectionGridAttributeId);
        }


    }
    public class DeleteDynamicFormDataHandler : IRequestHandler<DeleteDynamicFormData, DynamicFormData>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormDataHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<DynamicFormData> Handle(DeleteDynamicFormData request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.DeleteDynamicFormData(request.DynamicFormData);
        }
    }
    public class GetDynamicFormApprovalHandler : IRequestHandler<GetDynamicFormApproval, List<DynamicFormApproval>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormApprovalHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormApproval>> Handle(GetDynamicFormApproval request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormApproval>)await _dynamicFormQueryRepository.GetDynamicFormApprovalAsync(request.Id);
        }


    }
    public class InsertOrUpdateDynamicFormApprovalHandler : IRequestHandler<InsertOrUpdateDynamicFormApproval, DynamicFormApproval>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertOrUpdateDynamicFormApprovalHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormApproval> Handle(InsertOrUpdateDynamicFormApproval request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateDynamicFormApproval(request);


        }
    }
    public class DeleteDynamicFormApprovalHandler : IRequestHandler<DeleteDynamicFormApproval, DynamicFormApproval>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormApprovalHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<DynamicFormApproval> Handle(DeleteDynamicFormApproval request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.DeleteDynamicFormApproval(request.DynamicFormApproval);
        }
    }
    public class UpdateDynamicFormApprovalSortOrderHandler : IRequestHandler<UpdateDynamicFormApprovalSortOrder, DynamicFormApproval>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormApprovalSortOrderHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormApproval> Handle(UpdateDynamicFormApprovalSortOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormApprovalSortOrder(request.DynamicFormApproval);

        }
    }
    public class GetDynamicFormApprovalUpdateDescriptionFieldHandler : IRequestHandler<GetDynamicFormApprovalUpdateDescriptionField, DynamicFormApproval>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public GetDynamicFormApprovalUpdateDescriptionFieldHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormApproval> Handle(GetDynamicFormApprovalUpdateDescriptionField request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDescriptionDynamicFormApprovalField(request.DynamicFormApproval);

        }
    }
    public class InsertDynamicFormSectionSecuritysHandler : IRequestHandler<InsertDynamicFormSectionSecurity, DynamicFormSectionSecurity>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormSectionSecuritysHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<DynamicFormSectionSecurity> Handle(InsertDynamicFormSectionSecurity request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormSectionSecurity(request.DynamicFormSectionSecurity);
        }

    }
    public class GetDynamicFormSectionSecurityListHandler : IRequestHandler<GetDynamicFormSectionSecurityList, List<DynamicFormSectionSecurity>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionSecurityListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionSecurity>> Handle(GetDynamicFormSectionSecurityList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionSecurity>)await _dynamicFormQueryRepository.GetDynamicFormSectionSecurityList(request.Id);
        }


    }
    public class DeleteDynamicFormSectionSecurityHandler : IRequestHandler<DeleteDynamicFormSectionSecurity, long>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormSectionSecurityHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<long> Handle(DeleteDynamicFormSectionSecurity request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.DeleteDynamicFormSectionSecurity(request.Id, request.Ids);
        }
    }
    public class InsertDynamicFormApprovedHandler : IRequestHandler<InsertDynamicFormApproved, DynamicFormApproved>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormApprovedHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormApproved> Handle(InsertDynamicFormApproved request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormApproved(request);


        }
    }
    public class GetDynamicFormApprovedByIDHandler : IRequestHandler<GetDynamicFormApprovedByID, DynamicFormApproved>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormApprovedByIDHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicFormApproved> Handle(GetDynamicFormApprovedByID request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormApprovedByID(request.DynamicFormDataId, request.ApprovalUserId);
        }


    }
    public class InsertOrUpdateDynamicFormApprovedHandler : IRequestHandler<InsertOrUpdateDynamicFormApproved, DynamicFormData>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertOrUpdateDynamicFormApprovedHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormData> Handle(InsertOrUpdateDynamicFormApproved request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateDynamicFormApproved(request.DynamicFormData);


        }
    }
    public class GetDynamicFormApprovedListHandler : IRequestHandler<GetDynamicFormApprovedList, List<DynamicFormApproved>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormApprovedListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormApproved>> Handle(GetDynamicFormApprovedList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormApproved>)await _dynamicFormQueryRepository.GetDynamicFormApprovedList(request.Id);
        }


    }
    public class UpdateDynamicFormApprovedByStausHandler : IRequestHandler<UpdateDynamicFormApprovedByStaus, DynamicFormApproved>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormApprovedByStausHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormApproved> Handle(UpdateDynamicFormApprovedByStaus request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormApprovedByStaus(request.DynamicFormApproved);


        }
    }
    public class GetDynamicFormSectionByIdHandler : IRequestHandler<GetDynamicFormSectionById, List<DynamicFormSection>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionByIdHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSection>> Handle(GetDynamicFormSectionById request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSection>)await _dynamicFormQueryRepository.GetDynamicFormSectionByIdAsync(request.Id, request.UserId, request.DynamicFormDataId);
        }


    }
    public class InsertDynamicFormDataUploadHandler : IRequestHandler<InsertDynamicFormDataUpload, DynamicFormDataUpload>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormDataUploadHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormDataUpload> Handle(InsertDynamicFormDataUpload request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormDataUpload(request.DynamicFormDataUpload);


        }
    }
    public class GetDynamicFormSectionWorkFlowByIDHandler : IRequestHandler<GetDynamicFormSectionWorkFlowByID, List<DynamicFormSectionWorkFlow>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionWorkFlowByIDHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionWorkFlow>> Handle(GetDynamicFormSectionWorkFlowByID request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionWorkFlow>)await _dynamicFormQueryRepository.GetDynamicFormSectionWorkFlowByID(request.DynamicFormSectionId, request.UserId);
        }
    }
    public class InsertDynamicFormWorkFlowHandler : IRequestHandler<InsertDynamicFormWorkFlow, DynamicFormWorkFlow>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormWorkFlowHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlow> Handle(InsertDynamicFormWorkFlow request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormWorkFlow(request.DynamicFormWorkFlow);


        }
    }

    public class DynamicFormWorkFlowHandler : IRequestHandler<GetDynamicFormWorkFlow, List<DynamicFormWorkFlow>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public DynamicFormWorkFlowHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormWorkFlow>> Handle(GetDynamicFormWorkFlow request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlow>)await _dynamicFormQueryRepository.GetDynamicFormWorkFlowAsync(request.DynamicFormId);
        }
    }


    public class DeleteDynamicFormWorkFlowHandler : IRequestHandler<DeleteDynamicFormWorkFlow, DynamicFormWorkFlow>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public DeleteDynamicFormWorkFlowHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlow> Handle(DeleteDynamicFormWorkFlow request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDynamicFormWorkFlow(request.DynamicFormWorkFlow);


        }
    }
    public class GetDynamicFormWorkFlowExitsHandler : IRequestHandler<GetDynamicFormWorkFlowExits, List<DynamicFormWorkFlowSection>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormWorkFlowExitsHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormWorkFlowSection>> Handle(GetDynamicFormWorkFlowExits request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlowSection>)await _dynamicFormQueryRepository.GetDynamicFormWorkFlowExits(request.DynamicFormId, request.UserId, request.DynamicFormDataId);
        }
    }

    public class InsertOrUpdateFormWorkFlowSectionNoWorkFlowHandler : IRequestHandler<InsertOrUpdateFormWorkFlowSectionNoWorkFlow, DynamicFormWorkFlowSection>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public InsertOrUpdateFormWorkFlowSectionNoWorkFlowHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicFormWorkFlowSection> Handle(InsertOrUpdateFormWorkFlowSectionNoWorkFlow request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.InsertOrUpdateFormWorkFlowSectionNoWorkFlow(request.DynamicFormWorkFlowSection, request.DynamicFormDataId, request.UserId);
        }
    }
    public class GetDynamicFormWorkFlowFormListHandler : IRequestHandler<GetDynamicFormWorkFlowFormList, List<DynamicFormWorkFlowForm>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormWorkFlowFormListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormWorkFlowForm>> Handle(GetDynamicFormWorkFlowFormList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlowForm>)await _dynamicFormQueryRepository.GetDynamicFormWorkFlowFormList(request.DynamicFormDataId, request.DynamicFormId);
        }
    }

    public class GetDynamicFormWorkFlowFormExitsHandler : IRequestHandler<GetDynamicFormWorkFlowFormExits, DynamicFormWorkFlowForm>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormWorkFlowFormExitsHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicFormWorkFlowForm> Handle(GetDynamicFormWorkFlowFormExits request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormWorkFlowFormExits(request.DynamicFormWorkFlowSectionId, request.UserId, request.DynamicFormDataId);
        }
    }

    public class GetDynamicFormWorkFlowListByUserHandler : IRequestHandler<GetDynamicFormWorkFlowListByUser, List<DynamicFormDataWrokFlow>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormWorkFlowListByUserHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormDataWrokFlow>> Handle(GetDynamicFormWorkFlowListByUser request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormDataWrokFlow>)await _dynamicFormQueryRepository.GetDynamicFormWorkFlowListByUser(request.UserId,request.DynamicFormDataId);
        }
    }
    public class InsertCreateEmailFormDataHandler : IRequestHandler<InsertCreateEmailFormData, DynamicFormData>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public InsertCreateEmailFormDataHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicFormData> Handle(InsertCreateEmailFormData request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.InsertCreateEmailFormData(request.DynamicFormData);
        }
    }
    public class GetEmployeeByUserIdIdHandler : IRequestHandler<GetEmployeeFormByUserId, ViewEmployee>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetEmployeeByUserIdIdHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<ViewEmployee> Handle(GetEmployeeFormByUserId request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetEmployeeByUserIdIdAsync(request.UserId);
        }
    }

    public class InsertDynamicFormSectionAttributeSecurityHandler : IRequestHandler<InsertDynamicFormSectionAttributeSecurity, DynamicFormSectionAttributeSecurity>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormSectionAttributeSecurityHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<DynamicFormSectionAttributeSecurity> Handle(InsertDynamicFormSectionAttributeSecurity request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormSectionAttributeSecurity(request.DynamicFormSectionAttributeSecurity);
        }

    }
    public class GetDynamicFormSectionAttributeSecurityListHandler : IRequestHandler<GetDynamicFormSectionAttributeSecurityList, List<DynamicFormSectionAttributeSecurity>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionAttributeSecurityListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionAttributeSecurity>> Handle(GetDynamicFormSectionAttributeSecurityList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttributeSecurity>)await _dynamicFormQueryRepository.GetDynamicFormSectionAttributeSecurityList(request.Id);
        }


    }
    public class DeleteDynamicFormSectionAttributeSecurityHandler : IRequestHandler<DeleteDynamicFormSectionAttributeSecurity, long>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormSectionAttributeSecurityHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<long> Handle(DeleteDynamicFormSectionAttributeSecurity request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.DeleteDynamicFormSectionAttributeSecurity(request.Id, request.Ids);
        }
    }

    public class GetDynamicFormDataApprovalListHandler : IRequestHandler<GetDynamicFormDataApprovalList, List<DynamicFormData>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataApprovalListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormData>> Handle(GetDynamicFormDataApprovalList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormData>)await _dynamicFormQueryRepository.GetDynamicFormDataApprovalList(request.UserId);
        }


    }
    public class UpdateDynamicFormDataSortOrderHandler : IRequestHandler<UpdateDynamicFormDataSortOrder, DynamicFormData>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormDataSortOrderHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormData> Handle(UpdateDynamicFormDataSortOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormDataSortOrder(request.DynamicFormData);

        }
    }
    public class InsertDmsDocumentDynamicFormDataHandler : IRequestHandler<InsertDmsDocumentDynamicFormData, DynamicFormDataUpload>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public InsertDmsDocumentDynamicFormDataHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<DynamicFormDataUpload> Handle(InsertDmsDocumentDynamicFormData request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.InsertDmsDocumentDynamicFormData(request.DynamicFormDataUpload);
        }
    }
    public class InsertDynamicFormReportHandler : IRequestHandler<InsertDynamicFormReport, DynamicFormReport>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormReportHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<DynamicFormReport> Handle(InsertDynamicFormReport request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormReport(request);
        }

    }
    public class GetDynamicFormReportListHandler : IRequestHandler<GetDynamicFormReportList, List<DynamicFormReport>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormReportListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormReport>> Handle(GetDynamicFormReportList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormReport>)await _dynamicFormQueryRepository.GetDynamicFormReportList(request.DynamicFormId);
        }


    }
    public class DeleteDynamicFormReportHandler : IRequestHandler<DeleteDynamicFormReport, DynamicFormReport>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public DeleteDynamicFormReportHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormReport> Handle(DeleteDynamicFormReport request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDynamicFormReport(request.DynamicFormReport);


        }
    }
    public class GetDynamicFormReportOneDataHandler : IRequestHandler<GetDynamicFormReportOneData, DynamicFormReport>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormReportOneDataHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicFormReport> Handle(GetDynamicFormReportOneData request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormReportOneData(request.SessionId);
        }


    }
    public class GetDynamicFormApplicationMasterParentHandler : IRequestHandler<GetDynamicFormApplicationMasterParent, List<ApplicationMasterParent>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormApplicationMasterParentHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<ApplicationMasterParent>> Handle(GetDynamicFormApplicationMasterParent request, CancellationToken cancellationToken)
        {
            return (List<ApplicationMasterParent>)await _dynamicFormQueryRepository.GetDynamicFormApplicationMasterParentAsync(request.DynamicFormId);
        }


    }
}
