using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.EntityModel;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler.Plants
{
    public class CreateDivisionHandler : IRequestHandler<CreateDivisionCommand, DivisionResponse>
    {
        private readonly IDivisionCommandRepository _commandRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public CreateDivisionHandler(IDivisionCommandRepository commandRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<DivisionResponse> Handle(CreateDivisionCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<Division>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var plantData = await _commandRepository.AddAsync(queryEntity);
            var guid = Guid.NewGuid();
            var uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Add", queryEntity?.Name, queryEntity?.Name, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DisplayName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Add", null, queryEntity?.Name, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "Name", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Add", null, queryEntity?.Code, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "Code", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Add", null, queryEntity?.Description, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "Description", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Add", null, queryEntity?.CompanyId.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "CompanyId", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Add", null, queryEntity?.CompanyName, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "CompanyName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Add", null, queryEntity?.StatusCodeId?.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "StatusCodeID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Add", null, queryEntity?.StatusCode, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "StatusCode", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Add", null, queryEntity?.AddedByUserId?.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedByUserID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Add", null, queryEntity?.AddedDate != null ? queryEntity.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedDate", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Add", null, queryEntity?.AddedBy?.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedBy", uid);

            var response = new DivisionResponse
            {
                DivisionID = (long)plantData,
                StatusCodeID = queryEntity.StatusCodeId,
                AddedByUserID = queryEntity.AddedByUserId,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
