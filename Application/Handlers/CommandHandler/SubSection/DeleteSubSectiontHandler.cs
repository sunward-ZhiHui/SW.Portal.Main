using Application.Command.SubSections;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteSubSectionHandler : IRequestHandler<DeleteSubSectionCommand, String>
    {
        private readonly ISubSectionCommandRepository _commandRepository;
        private readonly ISubSectionQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteSubSectionHandler(ISubSectionCommandRepository customerRepository, ISubSectionQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteSubSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _queryRepository.GetByIdAsync(request.Id);
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new SubSection
                {
                    SubSectionId= request.Id,
                    Name=queryEntity.SubSectionName,
                    AddedByUserId= queryEntity.AddedByUserId,
                    StatusCodeId= queryEntity.StatusCodeId.Value,
                };
                if (result != null)
                {
                    var guid = Guid.NewGuid();
                    var uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result.SectionName, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Name", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result.SectionName, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "DisplayName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result.SectionCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Code", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result.Description, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Description", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result.SectionId?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "SectionId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result.SectionName, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "SectionName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result.ProfileCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ProfileCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result.StatusCodeId?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCodeID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result.StatusCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result.ModifiedByUserId?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedByUserID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result?.ModifiedDate != null ? result.ModifiedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Delete", result?.ModifiedBy, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedBy", uid);

                }
                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "SubSection information has been deleted!";
        }
    }
}
