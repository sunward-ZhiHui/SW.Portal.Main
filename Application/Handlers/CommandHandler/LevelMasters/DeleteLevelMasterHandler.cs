using Application.Command.LeveMasters;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteLevelMasterHandler : IRequestHandler<DeleteLevelMasterCommand, String>
    {
        private readonly ILevelMasterCommandRepository _commandRepository;
        private readonly ILevelMasterQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteLevelMasterHandler(ILevelMasterCommandRepository customerRepository, ILevelMasterQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteLevelMasterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _queryRepository.GetByIdAsync(request.Id);
                var data = new LevelMaster
                {
                    LevelId= result.LevelID,
                    CompanyId = result.CompanyID,
                    Name= result.Name,
                    AddedByUserId= result.AddedByUserID,
                    StatusCodeId= result.StatusCodeID.Value,
                };

                await _commandRepository.DeleteAsync(data);
                if (result != null)
                {
                    var guid = Guid.NewGuid();
                    var uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Delete", result.Name, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Name", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Delete", result.Name, result.Name, request?.Id, guid, request?.UserId, DateTime.Now, true, "DisplayName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Delete", result.CompanyID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "CompanyId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Delete", result.CompanyName?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "CompanyName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Delete", result.DivisionID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "DivisionID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Delete", result.DivisionName?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "DivisionName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Delete", result.StatusCodeID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCodeID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Delete", result.StatusCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Delete", result.ModifiedByUserID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedByUserID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Delete", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Delete", result?.ModifiedBy, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedBy", uid);

                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "LevelMaster information has been deleted!";
        }
    }
}
