using Application.Common.Mapper;
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
    public class GetAllCCFImplementationQueryHandler : IRequestHandler<GetAllImplementationQuery, List<View_GetCCFImplementation>>
    {

        private readonly ICCDFImplementationQueryRepository _queryRepository;
        public GetAllCCFImplementationQueryHandler(ICCDFImplementationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_GetCCFImplementation>> Handle(GetAllImplementationQuery request, CancellationToken cancellationToken)
        {
            return (List<View_GetCCFImplementation>)await _queryRepository.GetAllAsync(request.SesionId);
        }
    }
    public class GetAllSoOrderBySessionHandler : IRequestHandler<GetAllChangeControlBySession, CCFInformationModels>
    {
        private readonly ICCDFImplementationQueryRepository _queryRepository;
        public GetAllSoOrderBySessionHandler(ICCDFImplementationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<CCFInformationModels> Handle(GetAllChangeControlBySession request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetAllBySessionAsync(request.SesionId);
        }
    }
    public class EditCCFImplementationQueryHandler : IRequestHandler<EditImplementationQuery, long>
    {
        private readonly ICCDFImplementationQueryRepository _topicTodoListQueryRepository;
        public EditCCFImplementationQueryHandler(ICCDFImplementationQueryRepository topicTodoListQueryRepository)
        {
            _topicTodoListQueryRepository = topicTodoListQueryRepository;
        }

        public async Task<long> Handle(EditImplementationQuery request, CancellationToken cancellationToken)
        {
            var req = await _topicTodoListQueryRepository.InsertDetail(request);
            return req;
        }
    }

    public class CreateCCFInformationHandler : IRequestHandler<CreateCCFInformationModels, long>
    {
        private readonly ICCDFImplementationQueryRepository _emailTopicsQueryRepository;

        public CreateCCFInformationHandler(ICCDFImplementationQueryRepository emailTopicsQueryRepository)
        {
            _emailTopicsQueryRepository = emailTopicsQueryRepository;
        }
        public async Task<long> Handle(CreateCCFInformationModels request, CancellationToken cancellationToken)
        {
           

            var newTopics = _emailTopicsQueryRepository.Insert(request);
          
            return newTopics;
        }
    }

    public class EditCCFInformationHandler : IRequestHandler<SaveImplementationQuery, long>
    {
        private readonly ICCDFImplementationQueryRepository _impelementationQueryRepository;
        public EditCCFInformationHandler(ICCDFImplementationQueryRepository impelementationQueryRepository, IQueryRepository<TopicToDoList> queryRepository)
        {
            _impelementationQueryRepository = impelementationQueryRepository;
        }

        public async Task<long> Handle(SaveImplementationQuery request, CancellationToken cancellationToken)
        {
            var req = await _impelementationQueryRepository.UpdateDetail(request);
            return req;
        }
    }
}
