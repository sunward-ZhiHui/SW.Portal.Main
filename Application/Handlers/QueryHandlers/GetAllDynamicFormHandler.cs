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
    public class GetDynamicFormScreenIDHandler : IRequestHandler<GetDynamicFormScreenNameCheckValidation, DynamicForm>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormScreenIDHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicForm> Handle(GetDynamicFormScreenNameCheckValidation request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormScreenNameDataCheckValidation(request.ScreenName);
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
    public class DeleteDynamicFormHandler : IRequestHandler<DeleteDynamicForm, DynamicForm>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<DynamicForm> Handle(DeleteDynamicForm request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.Delete(request.DynamicForm);
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

    public class GetDynamicFormSectionIdsHandler : IRequestHandler<GetDynamicFormSectionIds, List<DynamicFormSection>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionIdsHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSection>> Handle(GetDynamicFormSectionIds request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSection>)await _dynamicFormQueryRepository.GetDynamicFormSectionIdsAsync(request.DynamicFormSectionIds);
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
    public class UpdateFormulaTextBoxHandler : IRequestHandler<UpdateFormulaTextBox, DynamicFormSectionAttribute>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateFormulaTextBoxHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormSectionAttribute> Handle(UpdateFormulaTextBox request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateFormulaTextBox(request.DynamicFormSectionAttribute);

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
    public class GetDynamicFormSectionAttributeByIdsHandler : IRequestHandler<GetDynamicFormSectionAttributeByIds, List<DynamicFormSectionAttribute>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionAttributeByIdsHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionAttribute>> Handle(GetDynamicFormSectionAttributeByIds request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttribute>)await _dynamicFormQueryRepository.GetDynamicFormSectionAttributeByIdsAsync(request.Id);
        }


    }
    public class GetDynamicFormSectionAttributeAllHandler : IRequestHandler<GetDynamicFormSectionAttributeAll, List<DynamicFormSectionAttribute>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionAttributeAllHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionAttribute>> Handle(GetDynamicFormSectionAttributeAll request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttribute>)await _dynamicFormQueryRepository.GetDynamicFormSectionAttributeAllAsync(request.Id);
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
    public class GetDynamicFormDataBySessionAllIdHandler : IRequestHandler<GetDynamicFormDataBySessionAllId, DynamicFormData>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataBySessionAllIdHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicFormData> Handle(GetDynamicFormDataBySessionAllId request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormDataBySessionAllIdAsync(request.SesionId);
        }


    }
    public class GetDynamicFormDataOneBySessionIdHandler : IRequestHandler<GetDynamicFormDataBySessionOne, DynamicFormData>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataOneBySessionIdHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicFormData> Handle(GetDynamicFormDataBySessionOne request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormDataBySessionOneAsync(request.SesionId);
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
            return (List<DynamicFormData>)await _dynamicFormQueryRepository.GetDynamicFormDataByIdAsync(request.Id, request.UserId, request.DynamicFormDataGridId, request.DynamicFormSectionGridAttributeId, request.DynamicFormDataSessionId, request.DynamicFormSearch, request.IsDelete);
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
            return await _DynamicFormQueryRepository.DeleteDynamicFormData(request.DynamicFormData, request.UserId);
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
    public class InsertOrUpdateDynamicFormDataApprovedHandler : IRequestHandler<InsertOrUpdateDynamicFormDataApproved, DynamicFormApproved>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertOrUpdateDynamicFormDataApprovedHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormApproved> Handle(InsertOrUpdateDynamicFormDataApproved request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateDynamicFormDataApproved(request);


        }
    }
    public class DeleteDynamicFormDataApprovedHandler : IRequestHandler<DeleteDynamicFormDataApprovedQuery, DynamicFormApproved>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormDataApprovedHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<DynamicFormApproved> Handle(DeleteDynamicFormDataApprovedQuery request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.DeleteDynamicFormDataApproved(request.DynamicFormApproved);
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
            return await _DynamicFormQueryRepository.DeleteDynamicFormSectionSecurity(request.Id, request.Ids, request.UserId);
        }
    }
    public class DeleteDynamicFormDataAssignUserHandler : IRequestHandler<DeleteDynamicFormDataAssignUser, long>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormDataAssignUserHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<long> Handle(DeleteDynamicFormDataAssignUser request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.DeleteDynamicFormDataAssignUser(request.Id, request.Ids);
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
            return (List<DynamicFormDataWrokFlow>)await _dynamicFormQueryRepository.GetDynamicFormWorkFlowListByUser(request.UserId, request.DynamicFormDataId);
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
            return await _queryRepository.InsertDynamicFormSectionAttributeSecurity(request.DynamicFormSectionAttributeSecurity, request.UserId);
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

    public class GetDynamicFormDataListHandler : IRequestHandler<GetDynamicFormDataList, List<DynamicFormData>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormData>> Handle(GetDynamicFormDataList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormData>)await _dynamicFormQueryRepository.GetDynamicFormDataList(request.DyamicFormIds, request.DynamicFormDataGridId);
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
            return await _DynamicFormQueryRepository.DeleteDynamicFormSectionAttributeSecurity(request.Id, request.Ids, request.DynamicFormId, request.UserId);
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
    public class GetDynamicFormApplicationMasterHandler : IRequestHandler<GetDynamicFormApplicationMaster, List<ApplicationMaster>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormApplicationMasterHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<ApplicationMaster>> Handle(GetDynamicFormApplicationMaster request, CancellationToken cancellationToken)
        {
            return (List<ApplicationMaster>)await _dynamicFormQueryRepository.GetDynamicFormApplicationMasterAsync(request.DynamicFormId);
        }


    }
    public class GetDynamicFormSectionAttributeSectionParentHandler : IRequestHandler<GetDynamicFormSectionAttributeSectionParent, List<DynamicFormSectionAttributeSectionParent>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionAttributeSectionParentHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionAttributeSectionParent>> Handle(GetDynamicFormSectionAttributeSectionParent request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttributeSectionParent>)await _dynamicFormQueryRepository.GetDynamicFormSectionAttributeSectionParentAsync(request.DynamicFormSectionAttributeId);
        }


    }
    public class InsertOrUpdateDynamicFormSectionAttributeSectionParentHandler : IRequestHandler<InsertOrUpdateDynamicFormSectionAttributeSectionParent, DynamicFormSectionAttributeSectionParent>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertOrUpdateDynamicFormSectionAttributeSectionParentHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormSectionAttributeSectionParent> Handle(InsertOrUpdateDynamicFormSectionAttributeSectionParent request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateDynamicFormSectionAttributeSectionParent(request);


        }
    }
    public class DeleteDynamicFormSectionAttributeParentHandler : IRequestHandler<DeleteDynamicFormSectionAttributeParent, DynamicFormSectionAttributeSectionParent>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public DeleteDynamicFormSectionAttributeParentHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormSectionAttributeSectionParent> Handle(DeleteDynamicFormSectionAttributeParent request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDynamicFormSectionAttributeParent(request.DynamicFormSectionAttributeSectionParent);


        }
    }
    public class UpdateDynamicFormLockedHandler : IRequestHandler<UpdateDynamicFormLocked, DynamicFormData>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormLockedHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormData> Handle(UpdateDynamicFormLocked request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormLocked(request.DynamicFormData);


        }
    }
    public class InsertDynamicFormApprovedChangedHandler : IRequestHandler<InsertDynamicFormApprovedChanged, DynamicFormApprovedChanged>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormApprovedChangedHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormApprovedChanged> Handle(InsertDynamicFormApprovedChanged request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormApprovedChanged(request.DynamicFormApprovedChanged);


        }
    }
    public class InsertDynamicFormWorkFlowApprovalHandler : IRequestHandler<InsertDynamicFormWorkFlowApproval, DynamicFormWorkFlowApproval>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormWorkFlowApprovalHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlowApproval> Handle(InsertDynamicFormWorkFlowApproval request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormWorkFlowApproval(request);


        }
    }
    public class DeleteDynamicFormWorkFlowApprovalHandler : IRequestHandler<DeleteDynamicFormWorkFlowApproval, DynamicFormWorkFlowApproval>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public DeleteDynamicFormWorkFlowApprovalHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlowApproval> Handle(DeleteDynamicFormWorkFlowApproval request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDynamicFormWorkFlowApproval(request.DynamicFormWorkFlowApproval);


        }
    }
    public class GetDynamicFormWorkFlowApprovalListHandler : IRequestHandler<GetDynamicFormWorkFlowApprovalList, List<DynamicFormWorkFlowApproval>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormWorkFlowApprovalListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormWorkFlowApproval>> Handle(GetDynamicFormWorkFlowApprovalList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlowApproval>)await _dynamicFormQueryRepository.GetDynamicFormWorkFlowApprovalList(request.DynamicFormWorkFlowId, request.DynamicFormDataId);
        }


    }
    public class UpdateDynamicFormWorkFlowApprovalSortOrderHandler : IRequestHandler<UpdateDynamicFormWorkFlowApprovalSortOrder, DynamicFormWorkFlowApproval>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormWorkFlowApprovalSortOrderHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlowApproval> Handle(UpdateDynamicFormWorkFlowApprovalSortOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormWorkFlowApprovalSortOrder(request.DynamicFormWorkFlowApproval);

        }
    }
    public class GetDynamicFormWorkFlowApprovedFormListHandler : IRequestHandler<GetDynamicFormWorkFlowApprovedFormList, List<DynamicFormWorkFlowApprovedForm>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormWorkFlowApprovedFormListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormWorkFlowApprovedForm>> Handle(GetDynamicFormWorkFlowApprovedFormList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlowApprovedForm>)await _dynamicFormQueryRepository.GetDynamicFormWorkFlowApprovedFormList(request.DynamicFormDataId);
        }


    }
    public class UpdateDynamicFormWorkFlowApprovedFormByUserHandler : IRequestHandler<UpdateDynamicFormWorkFlowApprovedFormByUser, DynamicFormWorkFlowApprovedForm>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormWorkFlowApprovedFormByUserHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlowApprovedForm> Handle(UpdateDynamicFormWorkFlowApprovedFormByUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormWorkFlowApprovedFormByUser(request.DynamicFormWorkFlowApprovedForm);


        }
    }
    public class InsertDynamicFormWorkFlowApprovedFormChangedHandler : IRequestHandler<InsertDynamicFormWorkFlowApprovedFormChanged, DynamicFormWorkFlowApprovedFormChanged>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormWorkFlowApprovedFormChangedHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlowApprovedFormChanged> Handle(InsertDynamicFormWorkFlowApprovedFormChanged request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormWorkFlowApprovedFormChanged(request.DynamicFormWorkFlowApprovedFormChanged);


        }
    }
    public class InsertDynamicFormDataWorkFlowApprovalHandler : IRequestHandler<InsertDynamicFormDataWorkFlowApproval, DynamicFormWorkFlowApproval>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormDataWorkFlowApprovalHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlowApproval> Handle(InsertDynamicFormDataWorkFlowApproval request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormDataWorkFlowApproval(request);


        }
    }
    public class DeleteDynamicFormDataWorkFlowApprovalHandler : IRequestHandler<DeleteDynamicFormDataWorkFlowApproval, DynamicFormWorkFlowApproval>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public DeleteDynamicFormDataWorkFlowApprovalHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlowApproval> Handle(DeleteDynamicFormDataWorkFlowApproval request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDynamicFormDataWorkFlowApproval(request.DynamicFormWorkFlowApproval);


        }
    }
    public class GetDynamicFormWorkFlowApprovedFormByListHandler : IRequestHandler<GetDynamicFormWorkFlowApprovedFormByList, List<DynamicFormWorkFlowApprovedForm>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormWorkFlowApprovedFormByListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormWorkFlowApprovedForm>> Handle(GetDynamicFormWorkFlowApprovedFormByList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlowApprovedForm>)await _dynamicFormQueryRepository.GetDynamicFormWorkFlowApprovedFormByList(request.UserId, request.FlowStatusID);
        }


    }
    public class GetGetDynamicFormWorkFlowFormDelegateListHandler : IRequestHandler<GetDynamicFormWorkFlowFormDelegateList, List<DynamicFormWorkFlowFormDelegate>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetGetDynamicFormWorkFlowFormDelegateListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormWorkFlowFormDelegate>> Handle(GetDynamicFormWorkFlowFormDelegateList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlowFormDelegate>)await _dynamicFormQueryRepository.GetDynamicFormWorkFlowFormDelegateList(request.DynamicFormWorkFlowFormID);
        }


    }
    public class InsertDynamicFormWorkFlowFormDelegateHandler : IRequestHandler<InsertDynamicFormWorkFlowFormDelegate, DynamicFormWorkFlowFormDelegate>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormWorkFlowFormDelegateHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlowFormDelegate> Handle(InsertDynamicFormWorkFlowFormDelegate request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormWorkFlowFormDelegate(request.DynamicFormWorkFlowFormDelegate);


        }
    }
    public class InsertDynamicFormWorkFormFlowHandler : IRequestHandler<InsertDynamicFormWorkFlowForm, DynamicFormWorkFlowForm>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormWorkFormFlowHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlowForm> Handle(InsertDynamicFormWorkFlowForm request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormWorkFlowForm(request.DynamicFormWorkFlowForm);


        }
    }
    public class DeleteDynamicFormWorkFlowFormHandler : IRequestHandler<DeleteDynamicFormWorkFlowForm, DynamicFormWorkFlowForm>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public DeleteDynamicFormWorkFlowFormHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormWorkFlowForm> Handle(DeleteDynamicFormWorkFlowForm request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDynamicFormWorkFlowForm(request.DynamicFormWorkFlowForm);


        }
    }
    public class UpdateDynamicFormDataSectionLockHandler : IRequestHandler<UpdateDynamicFormDataSectionLock, DynamicFormDataSectionLock>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormDataSectionLockHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormDataSectionLock> Handle(UpdateDynamicFormDataSectionLock request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormDataSectionLock(request.DynamicFormDataSectionLock);


        }
    }
    public class UpdateDynamicFormDataSectionSecurityReleaseHandler : IRequestHandler<UpdateDynamicFormDataSectionSecurityRelease, DynamicFormDataSectionSecurityRelease>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormDataSectionSecurityReleaseHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormDataSectionSecurityRelease> Handle(UpdateDynamicFormDataSectionSecurityRelease request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormDataSectionSecurityRelease(request.DynamicFormDataSectionSecurityRelease);


        }
    }
    public class InsertCloneDynamicFormHandler : IRequestHandler<InsertCloneDynamicForm, DynamicForm>
    {
        private readonly IDynamicFormDataQueryRepository _queryRepository;
        public InsertCloneDynamicFormHandler(IDynamicFormDataQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicForm> Handle(InsertCloneDynamicForm request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertCloneDynamicForm(request.DynamicForm, request.IsWithoutForm, request.UserId);


        }
    }
    public class GetDynamicFormSectionAttributeForSpinEditHandler : IRequestHandler<GetDynamicFormSectionAttributeForSpinEdit, List<DynamicFormSectionAttribute>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionAttributeForSpinEditHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionAttribute>> Handle(GetDynamicFormSectionAttributeForSpinEdit request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttribute>)await _dynamicFormQueryRepository.GetDynamicFormSectionAttributeForSpinEditAsync(request.Id);
        }


    }
    public class InsertDynamicFormEmailSubContHandler : IRequestHandler<InsertDynamicFormEmailSubCont, DynamicFormReportItems>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormEmailSubContHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormReportItems> Handle(InsertDynamicFormEmailSubCont request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormEmailSubCont(request.SubjectData, request.ContentData, request.SessionId);

        }
    }
    public class GetDynamicFormEmailSubContHandler : IRequestHandler<GetDynamicFormEmailSubCont, List<DynamicFormEmailSubCont>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormEmailSubContHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormEmailSubCont>> Handle(GetDynamicFormEmailSubCont request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormEmailSubCont>)await _dynamicFormQueryRepository.GetDynamicFormEmailSubCont(request.SessionId);
        }


    }
    public class DeleteDynamicFormEmailSubContContHandler : IRequestHandler<DeleteDynamicFormEmailSubCont, DynamicFormEmailSubCont>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public DeleteDynamicFormEmailSubContContHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormEmailSubCont> Handle(DeleteDynamicFormEmailSubCont request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDynamicFormEmailSubCont(request.SessionId);

        }
    }
    public class InsertDynamicFormPermissionPermissionHandler : IRequestHandler<InsertDynamicFormPermissionPermission, DynamicForm>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormPermissionPermissionHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicForm> Handle(InsertDynamicFormPermissionPermission request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormPermissionPermission(request.DynamicForm);

        }
    }
    public class GetDynamicFormMenuListHandler : IRequestHandler<GetDynamicFormMenuList, List<ApplicationPermission>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormMenuListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<ApplicationPermission>> Handle(GetDynamicFormMenuList request, CancellationToken cancellationToken)
        {
            return (List<ApplicationPermission>)await _dynamicFormQueryRepository.GetDynamicFormMenuList();
        }


    }
    public class UpdateDynamicFormMenuSortOrderHandler : IRequestHandler<UpdateDynamicFormMenuSortOrder, ApplicationPermission>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormMenuSortOrderHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<ApplicationPermission> Handle(UpdateDynamicFormMenuSortOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormMenuSortOrder(request.ApplicationPermission);

        }
    }
    public class DeleteDynamicFormMenuHandler : IRequestHandler<DeleteDynamicFormMenu, DynamicForm>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public DeleteDynamicFormMenuHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicForm> Handle(DeleteDynamicFormMenu request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDynamicFormMenu(request.DynamicForm);

        }
    }
    public class DynamicFormSectionAttrFormulaFunctionHandler : IRequestHandler<GetDynamicFormSectionAttrFormulaFunction, List<DynamicFormSectionAttrFormulaFunction>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public DynamicFormSectionAttrFormulaFunctionHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionAttrFormulaFunction>> Handle(GetDynamicFormSectionAttrFormulaFunction request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttrFormulaFunction>)await _dynamicFormQueryRepository.GetDynamicFormSectionAttrFormulaFunction(request.DynamicFormSectionAttributeId);
        }


    }
    public class GetDynamicFormSectionAttrFormulaMasterFunctionHandler : IRequestHandler<GetDynamicFormSectionAttrFormulaMasterFunction, List<DynamicFormSectionAttrFormulaMasterFunction>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionAttrFormulaMasterFunctionHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionAttrFormulaMasterFunction>> Handle(GetDynamicFormSectionAttrFormulaMasterFunction request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttrFormulaMasterFunction>)await _dynamicFormQueryRepository.GetDynamicFormSectionAttrFormulaMasterFunction();
        }


    }
    public class InsertOrUpdateDynamicFormSectionAttrFormulaFunctionHandler : IRequestHandler<InsertOrUpdateDynamicFormSectionAttrFormulaFunction, DynamicFormSectionAttrFormulaFunction>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertOrUpdateDynamicFormSectionAttrFormulaFunctionHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormSectionAttrFormulaFunction> Handle(InsertOrUpdateDynamicFormSectionAttrFormulaFunction request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateDynamicFormSectionAttrFormulaFunction(request);

        }
    }
    public class DeleteDynamicFormSectionAttrFormulaFunctionHandler : IRequestHandler<DeleteDynamicFormSectionAttrFormulaFunction, DynamicFormSectionAttrFormulaFunction>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public DeleteDynamicFormSectionAttrFormulaFunctionHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormSectionAttrFormulaFunction> Handle(DeleteDynamicFormSectionAttrFormulaFunction request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDynamicFormSectionAttrFormulaFunction(request.DynamicFormSectionAttrFormulaFunction);

        }
    }
    public class InsertDynamicFormDataAssignUserHandler : IRequestHandler<InsertDynamicFormDataAssignUser, DynamicFormDataAssignUser>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormDataAssignUserHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormDataAssignUser> Handle(InsertDynamicFormDataAssignUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormDataAssignUser(request.DynamicFormDataAssignUser);

        }
    }
    public class GetDynamicFormDataAssignUserListHandler : IRequestHandler<GetDynamicFormDataAssignUserList, List<DynamicFormDataAssignUser>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataAssignUserListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormDataAssignUser>> Handle(GetDynamicFormDataAssignUserList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormDataAssignUser>)await _dynamicFormQueryRepository.GetDynamicFormDataAssignUserList(request.DynamicFormId);
        }


    }
    public class GetDynamicFormDataAssignUserAllListHandler : IRequestHandler<GetDynamicFormDataAssignUserAllList, List<DynamicFormDataAssignUser>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataAssignUserAllListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormDataAssignUser>> Handle(GetDynamicFormDataAssignUserAllList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormDataAssignUser>)await _dynamicFormQueryRepository.GetDynamicFormDataAssignUserAllList();
        }


    }
    public class UpdateDynamicFormSectionAttributeGridSequenceSortOrderHandler : IRequestHandler<UpdateDynamicFormSectionAttributeGridSequenceSortOrder, DynamicFormSectionAttribute>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormSectionAttributeGridSequenceSortOrderHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormSectionAttribute> Handle(UpdateDynamicFormSectionAttributeGridSequenceSortOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormSectionAttributeGridSequenceSortOrder(request.DynamicFormSectionAttribute);

        }
    }
    public class UpdateDynamicFormSectionAttributeAllByCheckBoxHandler : IRequestHandler<UpdateDynamicFormSectionAttributeAllByCheckBox, DynamicFormSectionAttribute>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public UpdateDynamicFormSectionAttributeAllByCheckBoxHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormSectionAttribute> Handle(UpdateDynamicFormSectionAttributeAllByCheckBox request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateDynamicFormSectionAttributeAllByCheckBox(request.DynamicFormSectionAttribute);

        }
    }
    public class GetDynamicFormDataAuditListHandler : IRequestHandler<GetDynamicFormDataAuditList, List<DynamicFormDataAudit>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataAuditListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormDataAudit>> Handle(GetDynamicFormDataAuditList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormDataAudit>)await _dynamicFormQueryRepository.GetDynamicFormDataAuditList(request.SessionId, request.IsGridAudit, request.DynamicFormDataAuditGridData);
        }
    }
    public class GetDynamicFormDataAuditMultipleListHandler : IRequestHandler<GetDynamicFormDataAuditMultipleList, List<DynamicFormDataAudit>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataAuditMultipleListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormDataAudit>> Handle(GetDynamicFormDataAuditMultipleList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormDataAudit>)await _dynamicFormQueryRepository.GetDynamicFormDataAuditMultipleList(request.SessionId);
        }
    }
    public class GetDynamicFormDataAuditBySessionListHandler : IRequestHandler<GetDynamicFormDataAuditBySessionList, List<DynamicFormDataAudit>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataAuditBySessionListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormDataAudit>> Handle(GetDynamicFormDataAuditBySessionList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormDataAudit>)await _dynamicFormQueryRepository.GetDynamicFormDataAuditBySessionList(request.SessionId);
        }


    }

    public class GetDynamicFormDataAuditBySessionMultipleListHandler : IRequestHandler<GetDynamicFormDataAuditBySessionMultipleList, List<DynamicFormDataAudit>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataAuditBySessionMultipleListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormDataAudit>> Handle(GetDynamicFormDataAuditBySessionMultipleList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormDataAudit>)await _dynamicFormQueryRepository.GetDynamicFormDataAuditBySessionMultipleList(request.SessionId);
        }


    }
    public class GetDynamicFormFormulaMathFunListHandler : IRequestHandler<GetDynamicFormFormulaMathFunList, List<DynamicFormFormulaMathFun>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormFormulaMathFunListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormFormulaMathFun>> Handle(GetDynamicFormFormulaMathFunList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormFormulaMathFun>)await _dynamicFormQueryRepository.GetDynamicFormFormulaMathFunList();
        }


    }
    public class GetDynamicFormSectionSecuritySettingListHandler : IRequestHandler<GetDynamicFormSectionSecuritySettingList, List<DynamicFormSectionSecurity>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionSecuritySettingListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionSecurity>> Handle(GetDynamicFormSectionSecuritySettingList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionSecurity>)await _dynamicFormQueryRepository.GetDynamicFormSectionSecuritySettingList(request.Id);
        }


    }
    public class GetDynamicFormWorkFlowApprovalSettingListHandler : IRequestHandler<GetDynamicFormWorkFlowApprovalSettingList, List<DynamicFormWorkFlowApproval>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormWorkFlowApprovalSettingListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormWorkFlowApproval>> Handle(GetDynamicFormWorkFlowApprovalSettingList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlowApproval>)await _dynamicFormQueryRepository.GetDynamicFormWorkFlowApprovalSettingList(request.Id);
        }
    }
    public class GetDynamicFormSectionAttributeSecuritySettingListHandler : IRequestHandler<GetDynamicFormSectionAttributeSecuritySettingList, List<DynamicFormSectionAttributeSecurity>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionAttributeSecuritySettingListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionAttributeSecurity>> Handle(GetDynamicFormSectionAttributeSecuritySettingList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttributeSecurity>)await _dynamicFormQueryRepository.GetDynamicFormSectionAttributeSecuritySettingList(request.Id);
        }
    }
    public class GetDynamicFormSectionAttributeSectionParentSettingsHandler : IRequestHandler<GetDynamicFormSectionAttributeSectionParentSettings, List<DynamicFormSectionAttributeSectionParent>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormSectionAttributeSectionParentSettingsHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionAttributeSectionParent>> Handle(GetDynamicFormSectionAttributeSectionParentSettings request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttributeSectionParent>)await _dynamicFormQueryRepository.GetDynamicFormSectionAttributeSectionParentSettings(request.Id);
        }
    }

    public class GetDynamicFormDataAttrUploadHandler : IRequestHandler<GetDynamicFormDataAttrUpload, List<DynamicFormDataAttrUpload>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataAttrUploadHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormDataAttrUpload>> Handle(GetDynamicFormDataAttrUpload request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormDataAttrUpload>)await _dynamicFormQueryRepository.GetDynamicFormDataAttrUpload(request.Id, request.DynamicFormDataId);
        }
    }

    public class InsertDynamicFormDataAttrUploadHandler : IRequestHandler<InsertDynamicFormDataAttrUpload, DynamicFormDataAttrUpload>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormDataAttrUploadHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormDataAttrUpload> Handle(InsertDynamicFormDataAttrUpload request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormDataAttrUpload(request.DynamicFormDataAttrUpload);

        }
    }
    public class GetDynamicFormDataUploadCheckValidationHandler : IRequestHandler<GetDynamicFormDataUploadCheckValidation, DynamicFormDataUpload>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public GetDynamicFormDataUploadCheckValidationHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormDataUpload> Handle(GetDynamicFormDataUploadCheckValidation request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetDynamicFormDataUploadCheckValidation(request.DynamicFormDataId, request.DynamicFormSectionId);

        }
    }

    public class GetDynamicFormAttributeItemListHandler : IRequestHandler<GetDynamicFormAttributeItemList, List<DynamicFormSectionAttributesList>>
    {
        private readonly IDynamicFormOdataQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormAttributeItemListHandler(IDynamicFormOdataQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormSectionAttributesList>> Handle(GetDynamicFormAttributeItemList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormSectionAttributesList>)await _dynamicFormQueryRepository.GetDynamicFormSectionAttributeList(request.ID);
        }
    }
    public class DeleteDynamicFormDataAttrUploadHandler : IRequestHandler<DeleteDynamicFormDataAttrUpload, DynamicFormDataAttrUpload>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public DeleteDynamicFormDataAttrUploadHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DynamicFormDataAttrUpload> Handle(DeleteDynamicFormDataAttrUpload request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDynamicFormDataAttrUpload(request.DynamicFormDataAttrUpload);

        }
    }
    public class GetDynamicFormDataOneBySessionIdsHandler : IRequestHandler<GetDynamicFormDataOneBySessionId, DynamicFormData>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataOneBySessionIdsHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicFormData> Handle(GetDynamicFormDataOneBySessionId request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormDataOneBySessionIdAsync(request.DynamicFormDataSessionId);
        }


    }
    public class GetDynamicFormDataAuditMasterListHandler : IRequestHandler<GetDynamicFormDataAuditMasterList, List<DynamicFormDataAudit>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataAuditMasterListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormDataAudit>> Handle(GetDynamicFormDataAuditMasterList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormDataAudit>)await _dynamicFormQueryRepository.GetDynamicFormDataAuditMasterList(request.DynamicFormDataAudit);
        }


    }
    public class GetDynamicFormDataOneByDataIdHandler : IRequestHandler<GetDynamicFormDataOneByDataId, DynamicFormData>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataOneByDataIdHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicFormData> Handle(GetDynamicFormDataOneByDataId request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormDataOneByDataIdAsync(request.DynamicFormId);
        }


    }
    public class GetDynamicFormDataTableSyncHandler : IRequestHandler<GetDynamicFormDataTableSync, DynamicForm>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormDataTableSyncHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicForm> Handle(GetDynamicFormDataTableSync request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetDynamicFormDataTableSync(request.DropDownOptionsModel, request.DynamicformObjectDataList, request.AttributeHeader, request.DynamicForm);
        }


    }
    public class GetDynamicFormAuditListHandler : IRequestHandler<GetDynamicFormAuditList, List<DynamicFormAudit>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormAuditListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormAudit>> Handle(GetDynamicFormAuditList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormAudit>)await _dynamicFormQueryRepository.GetDynamicFormAuditList(request.DynamicFormId, request.SessionId);
        }


    }
    public class GetDynamicFormAuditDynamicFormSectionListHandler : IRequestHandler<GetDynamicFormAuditDynamicFormSectionList, List<DynamicFormAudit>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormAuditDynamicFormSectionListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormAudit>> Handle(GetDynamicFormAuditDynamicFormSectionList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormAudit>)await _dynamicFormQueryRepository.GetDynamicFormAuditDynamicFormSectionList(request.SessionIds, request.FormType);
        }


    }
    public class GeDynamicFormWorkFlowListIdsHandler : IRequestHandler<GeDynamicFormWorkFlowListIds, List<DynamicFormWorkFlow>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GeDynamicFormWorkFlowListIdsHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormWorkFlow>> Handle(GeDynamicFormWorkFlowListIds request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormWorkFlow>)await _dynamicFormQueryRepository.GeDynamicFormWorkFlowListIds(request.Ids);
        }
    }
    public class GeDynamicFormPermissionListIdsHandler : IRequestHandler<GeDynamicFormPermissionListIds, List<DynamicFormPermission>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GeDynamicFormPermissionListIdsHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormPermission>> Handle(GeDynamicFormPermissionListIds request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormPermission>)await _dynamicFormQueryRepository.GeDynamicFormPermissionListIds(request.Ids);
        }
    }
    public class GetOnDynamicFormSyncTablesHandler : IRequestHandler<GetOnDynamicFormSyncTables, DynamicForm>
    {
        private readonly IDynamicFormSyncQueryRepository _dynamicFormQueryRepository;
        public GetOnDynamicFormSyncTablesHandler(IDynamicFormSyncQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<DynamicForm> Handle(GetOnDynamicFormSyncTables request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.OnDynamicFormSyncTables(request.DynamicForm, request.UserId);
        }


    }
    public class DeleteDynamicFormPermissionHandler : IRequestHandler<DeleteDynamicFormPermission, long>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public DeleteDynamicFormPermissionHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<long> Handle(DeleteDynamicFormPermission request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.DeleteDynamicFormPermission(request.Id, request.Ids, request.UserId);
        }
    }
    public class InsertDynamicFormPermissionHandler : IRequestHandler<InsertDynamicFormPermission, DynamicFormPermission>
    {
        private readonly IDynamicFormQueryRepository _queryRepository;
        public InsertDynamicFormPermissionHandler(IDynamicFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<DynamicFormPermission> Handle(InsertDynamicFormPermission request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertDynamicFormPermission(request.DynamicFormPermission);
        }

    }
    public class GetDynamicFormPermissionListHandler : IRequestHandler<GetDynamicFormPermissionList, List<DynamicFormPermission>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetDynamicFormPermissionListHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<DynamicFormPermission>> Handle(GetDynamicFormPermissionList request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormPermission>)await _dynamicFormQueryRepository.GetDynamicFormPermissionList(request.Id);
        }


    }
    public class GetDynamicFormPermissionCheckHandler : IRequestHandler<GetDynamicFormPermissionCheck, DynamicFormPermission>
    {
        private readonly IDynamicFormQueryRepository _DynamicFormQueryRepository;

        public GetDynamicFormPermissionCheckHandler(IDynamicFormQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<DynamicFormPermission> Handle(GetDynamicFormPermissionCheck request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.GetDynamicFormPermissionCheckAsync(request.DynamicFormId, request.UserId);
        }
    }

}
