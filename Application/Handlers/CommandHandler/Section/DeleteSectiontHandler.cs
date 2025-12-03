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
                    SectionId = request.Id,
                    Name = queryEntity.SectionName,
                    AddedByUserId = queryEntity.AddedByUserId,
                    StatusCodeId = queryEntity.StatusCodeId.Value,
                };
                if (result != null)
                {
                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionName, ColumnName = "Name" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionName, CurrentValue = result?.SectionName, ColumnName = "DisplayName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DepartmentCode, ColumnName = "Code" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, ColumnName = "Description" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DepartmentId?.ToString(), ColumnName = "DepartmentId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DepartmentName?.ToString(), ColumnName = "DepartmentName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ProfileCode?.ToString(), ColumnName = "ProfileCode" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeId?.ToString(), ColumnName = "StatusCodeID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, ColumnName = "StatusCode" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserId.ToString(), ColumnName = "ModifiedByUserID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "Section",
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

            return "Section information has been deleted!";
        }
    }
}
