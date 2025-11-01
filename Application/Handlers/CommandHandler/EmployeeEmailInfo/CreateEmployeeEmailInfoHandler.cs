using Application.Command.Departments;
using Application.Command.EmployeeEmailInfos;
using Application.Command.EmployeeOtherDutyInformations;
using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class CreateEmployeeEmailInfoHandler : IRequestHandler<CreateEmployeeEmailInfoCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoCommandRepository _commandRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public CreateEmployeeEmailInfoHandler(IEmployeeEmailInfoCommandRepository commandRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(CreateEmployeeEmailInfoCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeEmailInfo>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var guid = Guid.NewGuid();
            var uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Add", null, queryEntity?.EmailGuideID?.ToString(),data, guid, request?.AddedByUserId, DateTime.Now, false, "EmailGuideID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Add", null, queryEntity?.EmailGuide,data, guid, request?.AddedByUserId, DateTime.Now, false, "EmailGuide", uid);

            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Add", null, queryEntity?.SubscriptionID?.ToString(),data, guid, request?.AddedByUserId, DateTime.Now, false, "EmailGuideID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Add", null, queryEntity?.Subscription,data, guid, request?.AddedByUserId, DateTime.Now, false, "EmailGuide", uid);

            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Add", null, queryEntity?.AddedByUserId?.ToString(),data, guid, request?.AddedByUserId, DateTime.Now, false, "AddedByUserId", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Add", null, queryEntity?.AddedDate != null ? queryEntity.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null,data, guid, request?.AddedByUserId, DateTime.Now, false, "AddedDate", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Add", null, queryEntity?.AddedBy?.ToString(),data, guid, request?.AddedByUserId, DateTime.Now, false, "AddedBy", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Add", queryEntity?.EmployeeID?.ToString(), queryEntity?.EmployeeID?.ToString(),data, guid, request?.AddedByUserId, DateTime.Now, false, "EmployeeId", uid);

            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID = (long)data,
                EmployeeID = request.EmployeeID,
            };
            return response;
        }
    }
    public class CreateEmployeeEmailInfoForwardHandler : IRequestHandler<CreateEmployeeEmailInfoForwardCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoForwardCommandRepository _commandRepository;
        public CreateEmployeeEmailInfoForwardHandler(IEmployeeEmailInfoForwardCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(CreateEmployeeEmailInfoForwardCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeEmailInfoForward>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID = (long)data,
            };
            return response;
        }
    }

    public class CreateEmployeeEmailInfoAuthorityHandler : IRequestHandler<CreateEmployeeEmailInfoAuthorityCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoAuthorityCommandRepository _commandRepository;
        public CreateEmployeeEmailInfoAuthorityHandler(IEmployeeEmailInfoAuthorityCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(CreateEmployeeEmailInfoAuthorityCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeEmailInfoAuthority>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID = (long)data,
            };
            return response;
        }
    }
}
