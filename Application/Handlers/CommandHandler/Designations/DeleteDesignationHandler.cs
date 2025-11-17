using Application.Command.designations;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteDesignationHandler : IRequestHandler<DeleteDesignationCommand, String>
    {
        private readonly IDesignationCommandRepository _commandRepository;
        private readonly IDesignationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteDesignationHandler(IDesignationCommandRepository customerRepository, IDesignationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteDesignationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _queryRepository.GetByIdAsync(request.Id);
                var data = new Designation
                {
                    DesignationId = request.Id,
                    CompanyId = result.CompanyID,
                    Name = result.Name,
                    AddedByUserId = result.AddedByUserID,
                    StatusCodeId = result.StatusCodeID.Value,
                };

                await _commandRepository.DeleteAsync(data);
                if (result != null)
                {
                    var guid = Guid.NewGuid();
                    var uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.Code, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Code", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.Name, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Name", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.Name, result.Name, request?.Id, guid, request?.UserId, DateTime.Now, true, "DisplayName", uid);

                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.Description, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Description", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.CompanyID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "CompanyId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.CompanyName?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "CompanyName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.SectionName, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "SectionName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.SectionID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "SectionId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.SubSectionID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "SubSectionID", uid);

                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.LevelID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "LevelID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.LevelName?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "LevelName", uid);

                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.SubSectionName, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "SubSectionName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.HeadCount?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "HeadCount", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.StatusCodeID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCodeID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.StatusCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result.ModifiedByUserID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedByUserID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Delete", result?.ModifiedBy, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedBy", uid);

                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Designation information has been deleted!";
        }
    }
}
