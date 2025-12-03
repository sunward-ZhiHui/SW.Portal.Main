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
                    LevelId = result.LevelID,
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
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.CompanyID?.ToString(), ColumnName = "CompanyID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.CompanyName?.ToString(), ColumnName = "CompanyName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DivisionID?.ToString(), ColumnName = "DivisionID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DivisionName?.ToString(), ColumnName = "DivisionName" });
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
                            Type = "LevelMaster",
                            FormType = "Delete",
                            HRMasterSetId = result?.LevelID,
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

            return "LevelMaster information has been deleted!";
        }
    }
}
