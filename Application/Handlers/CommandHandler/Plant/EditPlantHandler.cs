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
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class EditPlantHandler : IRequestHandler<EditPlantCommand, PlantResponse>
    {
        private readonly IPlantCommandRepository _commandRepository;
        private readonly IPlantQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditPlantHandler(IPlantCommandRepository customerRepository, IPlantQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<PlantResponse> Handle(EditPlantCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.PlantID);
            var queryrEntity = RoleMapper.Mapper.Map<Plant>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            if (result != null)
            {
                bool isUpdate = false;
                var guid = Guid.NewGuid();
                var uid = Guid.NewGuid();
                if (result.PlantCode != queryrEntity.PlantCode)
                {
                    isUpdate = true;
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Update", result.PlantCode, queryrEntity?.PlantCode, queryrEntity?.PlantID, guid, queryrEntity?.ModifiedByUserID, DateTime.Now, false, "PlantCode", uid);
                }
                if (result.Description != queryrEntity?.Description)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Update", result.Description, queryrEntity?.Description, queryrEntity?.PlantID, guid, queryrEntity?.ModifiedByUserID, DateTime.Now, false, "Description", uid);
                }
                if (result.StatusCodeID != queryrEntity?.StatusCodeID)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Update", result.StatusCodeID?.ToString(), queryrEntity?.StatusCodeID?.ToString(), queryrEntity?.PlantID, guid, queryrEntity?.ModifiedByUserID, DateTime.Now, false, "StatusCodeID", uid);

                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Update", result.StatusCode, queryrEntity?.StatusCode, queryrEntity?.PlantID, guid, queryrEntity?.ModifiedByUserID, DateTime.Now, false, "StatusCode", uid);
                }
                if (isUpdate)
                {
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Update", queryrEntity?.PlantCode, queryrEntity?.PlantCode, queryrEntity?.PlantID, guid, queryrEntity?.ModifiedByUserID, DateTime.Now, false, "DisplayName", uid);

                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Update", result.ModifiedByUserID?.ToString(), queryrEntity?.ModifiedByUserID?.ToString(), queryrEntity?.PlantID, guid, queryrEntity?.ModifiedByUserID, DateTime.Now, false, "ModifiedByUserID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Update", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.PlantID, guid, queryrEntity?.ModifiedByUserID, DateTime.Now, false, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Plant", "Update", result?.ModifiedBy, queryrEntity?.ModifiedBy?.ToString(), queryrEntity?.PlantID, guid, queryrEntity?.ModifiedByUserID, DateTime.Now, false, "ModifiedBy", uid);
                }
            }
            var response = new PlantResponse
            {
                PlantID = queryrEntity.PlantID,
                CompanyID = queryrEntity.CompanyID,
                PlantCode = queryrEntity.PlantCode,
                Description = queryrEntity.Description,
            };

            return response;
        }
    }
}
