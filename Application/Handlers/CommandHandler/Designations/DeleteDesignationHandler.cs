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
                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, ColumnName = "Name" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, CurrentValue = result?.Name, ColumnName = "DisplayName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Code, ColumnName = "Code" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, ColumnName = "Description" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionID?.ToString(), ColumnName = "SectionId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionName?.ToString(), ColumnName = "SectionName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.SubSectionID?.ToString(), ColumnName = "SubSectionID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.SubSectionName?.ToString(), ColumnName = "SubSectionName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.LevelID?.ToString(), ColumnName = "LevelID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.LevelName?.ToString(), ColumnName = "LevelName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.HeadCount?.ToString(), ColumnName = "HeadCount" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeID?.ToString(), ColumnName = "StatusCodeID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, ColumnName = "StatusCode" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserID.ToString(), ColumnName = "ModifiedByUserID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "Designation",
                            FormType = "Delete",
                            HRMasterSetId = result?.DesignationID,
                            IsDeleted = true,
                            AuditUserId = request?.UserId,
                        };
                        await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                    }
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
