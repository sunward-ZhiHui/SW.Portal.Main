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
            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.EmailGuideID?.ToString(), ColumnName = "EmailGuideID" });

            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.EmailGuide, ColumnName = "EmailGuide" });

            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.SubscriptionID?.ToString(), ColumnName = "EmailGuideID" });

            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.Subscription, ColumnName = "EmailGuide" });


            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.AddedByUserId?.ToString(), ColumnName = "AddedByUserId" });

            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.AddedDate != null ? queryEntity.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "AddedDate" });

            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.AddedBy?.ToString(), ColumnName = "AddedBy" });

            auditList.Add(new HRMasterAuditTrail { PreValue = queryEntity?.EmployeeID?.ToString(), CurrentValue = queryEntity?.EmployeeID?.ToString(), ColumnName = "EmployeeId" });
            if (auditList.Count() > 0)
            {
                HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                {
                    HRMasterAuditTrailItems = auditList,
                    Type = "EmployeeEmailInfo",
                    FormType = "Add",
                    HRMasterSetId = data,
                    AuditUserId = request?.AddedByUserId,
                };
                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
            }
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
