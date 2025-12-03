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
                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, ColumnName = "Name" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, CurrentValue = result?.Name, ColumnName = "DisplayName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Code, ColumnName = "Code" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, ColumnName = "Description" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.CompanyId?.ToString(), ColumnName = "CompanyId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.PlantCode?.ToString(), ColumnName = "CompanyName" });
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
                            Type = "Division",
                            FormType = "Delete",
                            HRMasterSetId = result?.DivisionID,
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

            return "Plant information has been deleted!";
        }
    }
}
