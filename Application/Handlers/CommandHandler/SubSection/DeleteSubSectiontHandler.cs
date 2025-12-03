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
                    SubSectionId = request.Id,
                    Name = queryEntity.SubSectionName,
                    AddedByUserId = queryEntity.AddedByUserId,
                    StatusCodeId = queryEntity.StatusCodeId.Value,
                };
                if (result != null)
                {
                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.SubSectionName, ColumnName = "Name" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.SubSectionName, CurrentValue = result?.SubSectionName, ColumnName = "DisplayName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionCode, ColumnName = "Code" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, ColumnName = "Description" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionId?.ToString(), ColumnName = "SectionId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionName?.ToString(), ColumnName = "SectionName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ProfileCode?.ToString(), ColumnName = "ProfileCode" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeId?.ToString(), ColumnName = "StatusCodeID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, ColumnName = "StatusCode" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserId.ToString(), ColumnName = "ModifiedByUserID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "SubSection",
                            FormType = "Delete",
                            HRMasterSetId = result?.SectionId,
                            IsDeleted = true,
                            AuditUserId = request?.UserId,
                        };
                        await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                    }
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
