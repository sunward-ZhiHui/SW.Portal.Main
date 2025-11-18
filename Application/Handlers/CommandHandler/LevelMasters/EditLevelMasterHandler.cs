using Application.Command.LayoutPlanType;
using Application.Command.LeveMasters;
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
    public class EditLevelMasterHandler : IRequestHandler<EditLevelMasterCommand, LevelMasterResponse>
    {
        private readonly ILevelMasterCommandRepository _commandRepository;
        private readonly ILevelMasterQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditLevelMasterHandler(ILevelMasterCommandRepository customerRepository, ILevelMasterQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<LevelMasterResponse> Handle(EditLevelMasterCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.LevelId);
            var queryrEntity = RoleMapper.Mapper.Map<LevelMaster>(request);

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
                    if (result.Name != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Update", result.Name, queryrEntity?.Name, queryrEntity?.LevelId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Name", uid);
                    }
                    if (result.CompanyID != queryrEntity?.CompanyId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Update", result.CompanyID?.ToString(), queryrEntity?.CompanyId.ToString(), queryrEntity?.LevelId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "CompanyId", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Update", result.CompanyName, queryrEntity?.CompanyName, queryrEntity?.LevelId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "CompanyName", uid);
                    }
                    if (result.DivisionID != queryrEntity?.DivisionID)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Update", result.DivisionID?.ToString(), queryrEntity?.DivisionID.ToString(), queryrEntity?.LevelId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DivisionID", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Update", result.DivisionName, queryrEntity?.DivisionName, queryrEntity?.LevelId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DivisionName", uid);
                    }
                    if (result.StatusCodeID != queryrEntity?.StatusCodeId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Update", result.StatusCodeID?.ToString(), queryrEntity?.StatusCodeId.ToString(), queryrEntity?.LevelId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCodeID", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Update", result.StatusCode, queryrEntity?.StatusCode, queryrEntity?.LevelId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCode", uid);
                    }
                    if (isUpdate)
                    {
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Update", queryrEntity?.Name, queryrEntity?.Name, queryrEntity?.LevelId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DisplayName", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Update", result.ModifiedByUserID?.ToString(), queryrEntity?.ModifiedByUserId?.ToString(), queryrEntity?.LevelId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedByUserID", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Update", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.LevelId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedDate", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Update", result?.ModifiedBy, queryrEntity?.ModifiedBy?.ToString(), queryrEntity?.LevelId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedBy", uid);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new LevelMasterResponse
            {
                LevelID = queryrEntity.LevelId,
                AddedByUserID = queryrEntity.StatusCodeId,
                Name = queryrEntity.Name,
            };

            return response;
        }
    }
}
