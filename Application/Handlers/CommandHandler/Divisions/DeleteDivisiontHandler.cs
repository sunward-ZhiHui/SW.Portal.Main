using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteDivisionHandler : IRequestHandler<DeleteDivisionCommand, String>
    {
        private readonly IDivisionCommandRepository _commandRepository;
        private readonly IDivisionQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;

        public DeleteDivisionHandler(IDivisionCommandRepository customerRepository, IDivisionQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _queryRepository.GetByIdAsync(request.Id);
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new Division
                {
                    DivisionId = request.Id,
                    CompanyId = queryEntity.CompanyId,
                    Name = queryEntity.Name,
                    AddedByUserId = queryEntity.AddedByUserID,
                    StatusCodeId = queryEntity.StatusCodeID,
                };

                await _commandRepository.DeleteAsync(data);
                if (result != null)
                {
                    var guid = Guid.NewGuid();
                    var uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Delete", result.Name, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Name", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Delete", result.Name, result.Name, request?.Id, guid, request?.UserId, DateTime.Now, true, "DisplayName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Delete", result.Code, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Code", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Delete", result.CompanyId?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "CompanyId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Delete", result.PlantCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "PlantCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Delete", result.Description, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Description", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Delete", result.StatusCodeID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCodeID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Delete", result.StatusCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Delete", result.ModifiedByUserID?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedByUserID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Delete", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Division", "Delete", result?.ModifiedBy, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedBy", uid);

                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Plant information has been deleted!";
        }
    }
}
