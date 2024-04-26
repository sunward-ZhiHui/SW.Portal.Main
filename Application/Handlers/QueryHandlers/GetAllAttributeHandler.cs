using Application.Common.Mapper;
using Application.Queries;
using Core.Entities;
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
    public class GetAllAttributeHandler : IRequestHandler<GetAllAttributeHeader, List<DynamicForm>>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetAllAttributeHandler(IQueryRepository<AttributeHeader> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<DynamicForm>> Handle(GetAllAttributeHeader request, CancellationToken cancellationToken)
        {
            return (List<DynamicForm>)await _attrubutequeryRepository.GetComboBoxList();
        }
    }
    public class GetAllAttributeNameHandler : IRequestHandler<GetAllAttributeNameHeader, List<AttributeHeader>>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetAllAttributeNameHandler(IQueryRepository<AttributeHeader> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeHeader>> Handle(GetAllAttributeNameHeader request, CancellationToken cancellationToken)
        {
            return (List<AttributeHeader>)await _attrubutequeryRepository.GetAllAttributeName(request.IsSubForm, request.Type, request.SubId);
        }
    }
    public class GetAllAttributeNameNotInDynamicFormHandler : IRequestHandler<GetAllAttributeNameNotInDynamicForm, List<AttributeHeader>>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetAllAttributeNameNotInDynamicFormHandler(IQueryRepository<AttributeHeader> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeHeader>> Handle(GetAllAttributeNameNotInDynamicForm request, CancellationToken cancellationToken)
        {
            return (List<AttributeHeader>)await _attrubutequeryRepository.GetAllAttributeNameNotInDynamicForm(request.DynamicFormSectionId, request.AttributeID);
        }
    }
    public class CreateAttributeHandler : IRequestHandler<CreateAttributeHeader, long>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public CreateAttributeHandler(IQueryRepository<AttributeHeader> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<long> Handle(CreateAttributeHeader request, CancellationToken cancellationToken)
        {
            // return (long)await _attrubutequeryRepository.Insert(request);
            var lst = await _attrubutequeryRepository.Insert(request);
            return lst;

        }
    }
    public class EditAttributeHandler : IRequestHandler<EditAttributeHeader, long>
    {
        private readonly IAttributeQueryRepository _conversationQueryRepository;

        public EditAttributeHandler(IAttributeQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(EditAttributeHeader request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.UpdateAsync(request);
            return req;
        }
    }

    public class DeleteAttributeHandler : IRequestHandler<DeleteAttributeHeader, long>
    {
        private readonly IAttributeQueryRepository _conversationQueryRepository;

        public DeleteAttributeHandler(IAttributeQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(DeleteAttributeHeader request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.DeleteAsync(request.AttributeHeader);
            return req;
        }
    }

    public class GetAllAttributeNameListHandler : IRequestHandler<GetAllAttributeNameList, AttributeHeaderListModel>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetAllAttributeNameListHandler(IAttributeQueryRepository attrubutequeryRepository)
        {

            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<AttributeHeaderListModel> Handle(GetAllAttributeNameList request, CancellationToken cancellationToken)
        {
            return await _attrubutequeryRepository.GetAllAttributeNameAsync(request.DynamicForm, request.UserId);
        }
    }
    public class GetAttributeHeaderDataSourceHandler : IRequestHandler<GetAttributeHeaderDataSource, List<AttributeHeaderDataSource>>
    {

        private readonly IQueryRepository<AttributeHeaderDataSource> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetAttributeHeaderDataSourceHandler(IQueryRepository<AttributeHeaderDataSource> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeHeaderDataSource>> Handle(GetAttributeHeaderDataSource request, CancellationToken cancellationToken)
        {
            return (List<AttributeHeaderDataSource>)await _attrubutequeryRepository.GetAttributeHeaderDataSource();
        }
    }
    public class GetFilterDataSourceHandler : IRequestHandler<GetFilterDataSource, List<DynamicFormFilter>>
    {

        private readonly IQueryRepository<DynamicFormFilter> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetFilterDataSourceHandler(IQueryRepository<DynamicFormFilter> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<DynamicFormFilter>> Handle(GetFilterDataSource request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormFilter>)await _attrubutequeryRepository.GetFilterDataSource();
        }
    }
    public class GetAllBySessionAttributeNameHandler : IRequestHandler<GetAllBySessionAttributeName, AttributeHeader>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetAllBySessionAttributeNameHandler(IQueryRepository<AttributeHeader> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<AttributeHeader> Handle(GetAllBySessionAttributeName request, CancellationToken cancellationToken)
        {
            return await _attrubutequeryRepository.GetAllBySessionAttributeName(request.SessionId);

        }
    }
    public class GetDataSourceDropDownListHandler : IRequestHandler<GetDataSourceDropDownList, List<AttributeDetails>>
    {

        private readonly IDynamicFormDataSourceQueryRepository _attrubutequeryRepository;
        public GetDataSourceDropDownListHandler(IDynamicFormDataSourceQueryRepository attrubutequeryRepository)
        {
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeDetails>> Handle(GetDataSourceDropDownList request, CancellationToken cancellationToken)
        {
            return (List<AttributeDetails>)await _attrubutequeryRepository.GetDataSourceDropDownList(request.CompanyId, request.DataSourceTableIds, request.PlantCode, request.ApplicationMasterIds, request.ApplicationMasterParentIds);
        }
    }
    public class GetAllDropDownDataSourcesListHandler : IRequestHandler<GetAllDropDownDataSourcesList, DataSourceAttributeDetails>
    {

        private readonly IDynamicFormDataSourceQueryRepository _attrubutequeryRepository;
        public GetAllDropDownDataSourcesListHandler(IDynamicFormDataSourceQueryRepository attrubutequeryRepository)
        {

            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<DataSourceAttributeDetails> Handle(GetAllDropDownDataSourcesList request, CancellationToken cancellationToken)
        {
            return await _attrubutequeryRepository.GetAllDropDownDataSources();
        }
    }
    public class UpdateAttributeHeaderSortOrderHandler : IRequestHandler<UpdateAttributeHeaderSortOrder, AttributeHeader>
    {
        private readonly IAttributeQueryRepository _queryRepository;
        public UpdateAttributeHeaderSortOrderHandler(IAttributeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<AttributeHeader> Handle(UpdateAttributeHeaderSortOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateAttributeHeaderSortOrder(request.AttributeHeader);

        }
    }

    public class GetApplicationMasterParentByListHandler : IRequestHandler<GetApplicationMasterParentByList, List<DropDownOptionsListModel>>
    {

        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetApplicationMasterParentByListHandler(IAttributeQueryRepository attrubutequeryRepository)
        {
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<DropDownOptionsListModel>> Handle(GetApplicationMasterParentByList request, CancellationToken cancellationToken)
        {
            return (List<DropDownOptionsListModel>)await _attrubutequeryRepository.GetApplicationMasterParentByList(request.DynamicMasterParentIds, request.ApplicationMasterParentId);
        }
    }
    public class GetAllFilterDropDownDataSourcesHandler : IRequestHandler<GetAllFilterDropDownDataSources, DynamicFormFilterDataSoureList>
    {

        private readonly IDynamicFormDataSourceQueryRepository _attrubutequeryRepository;
        public GetAllFilterDropDownDataSourcesHandler(IDynamicFormDataSourceQueryRepository attrubutequeryRepository)
        {

            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<DynamicFormFilterDataSoureList> Handle(GetAllFilterDropDownDataSources request, CancellationToken cancellationToken)
        {
            return await _attrubutequeryRepository.GetAllFilterDropDownDataSources();
        }
    }

    public class GetDynamicFormFilterByDataSourceHandler : IRequestHandler<GetDynamicFormFilterByDataSource, List<DynamicFormFilterBy>>
    {

        private readonly IDynamicFormDataSourceQueryRepository _attrubutequeryRepository;
        public GetDynamicFormFilterByDataSourceHandler(IDynamicFormDataSourceQueryRepository attrubutequeryRepository)
        {
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<DynamicFormFilterBy>> Handle(GetDynamicFormFilterByDataSource request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormFilterBy>)await _attrubutequeryRepository.GetDynamicFormFilterByDataSource(request.DataSourceTableIds);
        }
    }
    public class GetFilterDataSourceListsHandler : IRequestHandler<GetFilterDataSourceLists, List<AttributeDetails>>
    {

        private readonly IDynamicFormDataSourceQueryRepository _attrubutequeryRepository;
        public GetFilterDataSourceListsHandler(IDynamicFormDataSourceQueryRepository attrubutequeryRepository)
        {
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeDetails>> Handle(GetFilterDataSourceLists request, CancellationToken cancellationToken)
        {
            return (List<AttributeDetails>)await _attrubutequeryRepository.GetFilterDataSourceLists(request.DynamicFormFilterBy);
        }
    }
    public class GetFilterByDataSourceListsHandler : IRequestHandler<GetFilterByDataSourceLists, List<AttributeDetails>>
    {

        private readonly IDynamicFormDataSourceQueryRepository _attrubutequeryRepository;
        public GetFilterByDataSourceListsHandler(IDynamicFormDataSourceQueryRepository attrubutequeryRepository)
        {
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeDetails>> Handle(GetFilterByDataSourceLists request, CancellationToken cancellationToken)
        {
            return (List<AttributeDetails>)await _attrubutequeryRepository.GetFilterByDataSourceLists(request.DynamicFormFilterBy,request.Data,request.DataSource);
        }
    }
}
