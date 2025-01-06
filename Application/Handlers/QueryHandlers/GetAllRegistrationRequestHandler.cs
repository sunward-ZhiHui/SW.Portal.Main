using Application.Queries;
using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllRegistrationRequestHandler : IRequestHandler<GetAllRegistrationRequestQuery, List<RegistrationRequest>>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public GetAllRegistrationRequestHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<RegistrationRequest>> Handle(GetAllRegistrationRequestQuery request, CancellationToken cancellationToken)
        {
            return (List<RegistrationRequest>)await _queryRepository.GetRegistrationRequest();
        }
    }
    public class DeleteRegistrationRequestHandler : IRequestHandler<DeleteRegistrationRequest, RegistrationRequest>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public DeleteRegistrationRequestHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequest> Handle(DeleteRegistrationRequest request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteRegistrationRequest(request.RegistrationRequest);


        }
    }
    public class InsertorUpdateRegistrationRequestHandler : IRequestHandler<InsertorUpdateRegistrationRequest, RegistrationRequest>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public InsertorUpdateRegistrationRequestHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequest> Handle(InsertorUpdateRegistrationRequest request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertorUpdateRegistrationRequest(request.RegistrationRequest);


        }
    }
    public class GetRegistrationRequestBySessionHandler : IRequestHandler<GetRegistrationRequestBySession, RegistrationRequest>
    {
        private readonly IRegistrationRequestQueryRepository _dynamicFormQueryRepository;
        public GetRegistrationRequestBySessionHandler(IRegistrationRequestQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<RegistrationRequest> Handle(GetRegistrationRequestBySession request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetRegistrationRequestBySession(request.SessionId);
        }
    }



    public class GetRegistrationRequestDueDateAssignmentHandler : IRequestHandler<GetRegistrationRequestDueDateAssignment, List<RegistrationRequestDueDateAssignment>>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public GetRegistrationRequestDueDateAssignmentHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<RegistrationRequestDueDateAssignment>> Handle(GetRegistrationRequestDueDateAssignment request, CancellationToken cancellationToken)
        {
            return (List<RegistrationRequestDueDateAssignment>)await _queryRepository.GetRegistrationRequestDueDateAssignment(request.RegistrationRequestId);
        }
    }
    public class DeleteRegistrationRequestDueDateAssignmentHandler : IRequestHandler<DeleteRegistrationRequestDueDateAssignment, RegistrationRequestDueDateAssignment>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public DeleteRegistrationRequestDueDateAssignmentHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequestDueDateAssignment> Handle(DeleteRegistrationRequestDueDateAssignment request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteRegistrationRequestDueDateAssignment(request.RegistrationRequestDueDateAssignment);


        }
    }
    public class InsertorUpdateRegistrationRequestDueDateAssignmentHandler : IRequestHandler<InsertorUpdateRegistrationRequestDueDateAssignment, RegistrationRequestDueDateAssignment>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public InsertorUpdateRegistrationRequestDueDateAssignmentHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequestDueDateAssignment> Handle(InsertorUpdateRegistrationRequestDueDateAssignment request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertorUpdateRegistrationRequestDueDateAssignment(request);
        }
    }
}
