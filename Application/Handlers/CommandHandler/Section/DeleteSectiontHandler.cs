using Application.Command.Sections;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteSectionHandler : IRequestHandler<DeleteSectionCommand, String>
    {
        private readonly ISectionCommandRepository _commandRepository;
        private readonly ISectionQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteSectionHandler(ISectionCommandRepository customerRepository, ISectionQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _queryRepository.GetByIdAsync(request.Id);
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new Section
                {
                    SectionId= request.Id,
                    Name=queryEntity.SectionName,
                    AddedByUserId= queryEntity.AddedByUserId,
                    StatusCodeId= queryEntity.StatusCodeId.Value,
                };
                if (result != null)
                {
                    var guid = Guid.NewGuid();
                    var uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result.SectionName, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Name", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result.SectionName, result.SectionName, request?.Id, guid, request?.UserId, DateTime.Now, true, "DisplayName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result.SectionCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Code", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result.Description, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Description", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result.DepartmentId?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "DepartmentId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result.DepartmentName, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "DepartmentName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result.ProfileCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ProfileCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result.StatusCodeId?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCodeID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result.StatusCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result.ModifiedByUserId?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedByUserID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Delete", result?.ModifiedBy, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedBy", uid);

                }
                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Section information has been deleted!";
        }
    }
}
