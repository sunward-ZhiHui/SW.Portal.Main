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
    public class EditDivisionHandler : IRequestHandler<EditDivisionCommand, DivisionResponse>
    {
        private readonly IDivisionCommandRepository _commandRepository;
        private readonly IDivisionQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditDivisionHandler(IDivisionCommandRepository customerRepository, IDivisionQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<DivisionResponse> Handle(EditDivisionCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.DivisionId);
            var queryrEntity = RoleMapper.Mapper.Map<Division>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
                if (result != null)
                {
                    bool isUpdate = false;
                    var guid = Guid.NewGuid();
                    var uid = Guid.NewGuid();
                    if (result.Code != queryrEntity.Code)
                    {
                        isUpdate = true;
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Update", result.Code, queryrEntity?.Code, queryrEntity?.DivisionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Code", uid);
                    }
                    if (result.Name != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Update", result.Name, queryrEntity?.Name, queryrEntity?.DivisionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Name", uid);
                    }
                    if (result.Description != queryrEntity?.Description)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Update", result.Description, queryrEntity?.Description, queryrEntity?.DivisionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Description", uid);
                    }
                    if (result.CompanyId != queryrEntity?.CompanyId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Update", result.CompanyId?.ToString(), queryrEntity?.CompanyId?.ToString(), queryrEntity?.DivisionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "CompanyId", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Update", result.PlantCode, queryrEntity?.CompanyName, queryrEntity?.DivisionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "CompanyName", uid);
                    }
                    if (result.StatusCodeID != queryrEntity?.StatusCodeId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Update", result.StatusCodeID?.ToString(), queryrEntity?.StatusCodeId?.ToString(), queryrEntity?.DivisionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCodeID", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Update", result.StatusCode, queryrEntity?.StatusCode, queryrEntity?.DivisionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCode", uid);
                    }
                    if (isUpdate)
                    {
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Update", queryrEntity?.Name, queryrEntity?.Name, queryrEntity?.DivisionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DisplayName", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Update", result.ModifiedByUserID?.ToString(), queryrEntity?.ModifiedByUserId?.ToString(), queryrEntity?.DivisionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedByUserID", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Update", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.DivisionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedDate", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Update", result?.ModifiedBy, queryrEntity?.ModifiedBy?.ToString(), queryrEntity?.DivisionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedBy", uid);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new DivisionResponse
            {
                DivisionID = queryrEntity.DivisionId,
                CompanyId = queryrEntity.CompanyId,
                AddedByUserID = queryrEntity.AddedByUserId,
                StatusCodeID = queryrEntity.StatusCodeId,
            };

            return response;
        }
    }
}
