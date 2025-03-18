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
    public class GetRegistrationRequestVariationFormtHandler : IRequestHandler<GetRegistrationRequestVariationForm, List<RegistrationRequestVariationForm>>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public GetRegistrationRequestVariationFormtHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<RegistrationRequestVariationForm>> Handle(GetRegistrationRequestVariationForm request, CancellationToken cancellationToken)
        {
            return (List<RegistrationRequestVariationForm>)await _queryRepository.GetRegistrationRequestVariationForm(request.RegistrationRequestId);
        }
    }

    public class GetRegistrationRequestAssignmentOfJobHandler : IRequestHandler<GetRegistrationRequestAssignmentOfJob, List<RegistrationRequestAssignmentOfJob>>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public GetRegistrationRequestAssignmentOfJobHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<RegistrationRequestAssignmentOfJob>> Handle(GetRegistrationRequestAssignmentOfJob request, CancellationToken cancellationToken)
        {
            return (List<RegistrationRequestAssignmentOfJob>)await _queryRepository.GetRegistrationRequestAssignmentOfJob(request.RegistrationRequestId,request.DepartmentId);
        }
    }
    public class DeleteRegistrationRequestAssignmentOfJobHandler : IRequestHandler<DeleteRegistrationRequestAssignmentOfJob, RegistrationRequestAssignmentOfJob>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public DeleteRegistrationRequestAssignmentOfJobHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequestAssignmentOfJob> Handle(DeleteRegistrationRequestAssignmentOfJob request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteRegistrationRequestAssignmentOfJob(request.RegistrationRequestAssignmentOfJob);


        }
    }
    public class InsertorUpdateRegistrationRequestAssignmentOfJobHandler : IRequestHandler<InsertorUpdateRegistrationRequestAssignmentOfJob, RegistrationRequestAssignmentOfJob>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public InsertorUpdateRegistrationRequestAssignmentOfJobHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequestAssignmentOfJob> Handle(InsertorUpdateRegistrationRequestAssignmentOfJob request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertorUpdateRegistrationRequestAssignmentOfJob(request);
        }
    }



    public class GetRegistrationRequestProgressByRegistrationDepartmentHandler : IRequestHandler<GetRegistrationRequestProgressByRegistrationDepartment, List<RegistrationRequestProgressByRegistrationDepartment>>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public GetRegistrationRequestProgressByRegistrationDepartmentHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<RegistrationRequestProgressByRegistrationDepartment>> Handle(GetRegistrationRequestProgressByRegistrationDepartment request, CancellationToken cancellationToken)
        {
            return (List<RegistrationRequestProgressByRegistrationDepartment>)await _queryRepository.GetRegistrationRequestProgressByRegistrationDepartment(request.RegistrationRequestId);
        }
    }
    public class DeleteRegistrationRequestProgressByRegistrationDepartmentHandler : IRequestHandler<DeleteRegistrationRequestProgressByRegistrationDepartment, RegistrationRequestProgressByRegistrationDepartment>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public DeleteRegistrationRequestProgressByRegistrationDepartmentHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequestProgressByRegistrationDepartment> Handle(DeleteRegistrationRequestProgressByRegistrationDepartment request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteRegistrationRequestProgressByRegistrationDepartment(request.RegistrationRequestProgressByRegistrationDepartment);


        }
    }
    public class InsertorUpdateRegistrationRequestProgressByRegistrationDepartmentHandler : IRequestHandler<InsertorUpdateRegistrationRequestProgressByRegistrationDepartment, RegistrationRequestProgressByRegistrationDepartment>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public InsertorUpdateRegistrationRequestProgressByRegistrationDepartmentHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequestProgressByRegistrationDepartment> Handle(InsertorUpdateRegistrationRequestProgressByRegistrationDepartment request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertorUpdateRegistrationRequestProgressByRegistrationDepartment(request);
        }
    }



    public class GetRegistrationRequestComittmentLetterHandler : IRequestHandler<GetRegistrationRequestComittmentLetter, List<RegistrationRequestComittmentLetter>>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public GetRegistrationRequestComittmentLetterHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<RegistrationRequestComittmentLetter>> Handle(GetRegistrationRequestComittmentLetter request, CancellationToken cancellationToken)
        {
            return (List<RegistrationRequestComittmentLetter>)await _queryRepository.GetRegistrationRequestComittmentLetter(request.RegistrationRequestProgressByRegistrationDepartmentId);
        }
    }
    public class DeleteRegistrationRequestComittmentLetterHandler : IRequestHandler<DeleteRegistrationRequestComittmentLetter, RegistrationRequestComittmentLetter>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public DeleteRegistrationRequestComittmentLetterHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequestComittmentLetter> Handle(DeleteRegistrationRequestComittmentLetter request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteRegistrationRequestComittmentLetter(request.RegistrationRequestComittmentLetter);


        }
    }
    public class InsertorUpdateRegistrationRequestComittmentLetterHandler : IRequestHandler<InsertorUpdateRegistrationRequestComittmentLetter, RegistrationRequestComittmentLetter>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public InsertorUpdateRegistrationRequestComittmentLetterHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequestComittmentLetter> Handle(InsertorUpdateRegistrationRequestComittmentLetter request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertorUpdateRegistrationRequestComittmentLetter(request);
        }
    }



    public class RegistrationRequestQueriesHandler : IRequestHandler<GetRegistrationRequestQueries, List<RegistrationRequestQueries>>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public RegistrationRequestQueriesHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<RegistrationRequestQueries>> Handle(GetRegistrationRequestQueries request, CancellationToken cancellationToken)
        {
            return (List<RegistrationRequestQueries>)await _queryRepository.GetRegistrationRequestQueries(request.RegistrationRequestProgressByRegistrationDepartmentId);
        }
    }
    public class DeleteRegistrationRequestQueriesHandler : IRequestHandler<DeleteRegistrationRequestQueries, RegistrationRequestQueries>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public DeleteRegistrationRequestQueriesHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequestQueries> Handle(DeleteRegistrationRequestQueries request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteRegistrationRequestQueries(request.RegistrationRequestQueries);


        }
    }
    public class InsertorUpdateRegistrationRequestQueriesHandler : IRequestHandler<InsertorUpdateRegistrationRequestQueries, RegistrationRequestQueries>
    {
        private readonly IRegistrationRequestQueryRepository _queryRepository;
        public InsertorUpdateRegistrationRequestQueriesHandler(IRegistrationRequestQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RegistrationRequestQueries> Handle(InsertorUpdateRegistrationRequestQueries request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertorUpdateRegistrationRequestQueries(request);
        }
    }

}
