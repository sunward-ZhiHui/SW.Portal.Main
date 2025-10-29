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
    public class CreatePlantHandler : IRequestHandler<CreatePlantCommand, PlantResponse>
    {
        private readonly IPlantCommandRepository _commandRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public CreatePlantHandler(IPlantCommandRepository commandRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<PlantResponse> Handle(CreatePlantCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<Plant>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var plantData = await _commandRepository.AddAsync(customerEntity);
            var guid = Guid.NewGuid();
            var uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Add", customerEntity?.PlantCode, null, plantData, guid, customerEntity?.AddedByUserID, DateTime.Now, false, "DisplayName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Add", null, customerEntity?.PlantCode, plantData, guid, customerEntity?.AddedByUserID, DateTime.Now, false, "PlantCode", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Add", null, customerEntity?.Description, plantData, guid, customerEntity?.AddedByUserID, DateTime.Now, false, "Description", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Add", null, customerEntity?.StatusCodeID?.ToString(), plantData, guid, customerEntity?.AddedByUserID, DateTime.Now, false, "StatusCodeID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Add", null, customerEntity?.StatusCode, plantData, guid, customerEntity?.AddedByUserID, DateTime.Now, false, "StatusCode", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Add", null, customerEntity?.AddedByUserID?.ToString(), plantData, guid, customerEntity?.AddedByUserID, DateTime.Now, false, "AddedByUserID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Add", null, customerEntity?.AddedDate != null ? customerEntity.AddedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, plantData, guid, customerEntity?.AddedByUserID, DateTime.Now, false, "AddedDate", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Add", null, customerEntity?.AddedBy?.ToString(), plantData, guid, customerEntity?.AddedByUserID, DateTime.Now, false, "AddedBy", uid);

            var response = new PlantResponse
            {
                PlantID = (long)plantData,
                CompanyID = customerEntity.CompanyID,
                PlantCode = customerEntity.PlantCode,
                Description = customerEntity.Description,
            };
            return response;
        }
    }
}
