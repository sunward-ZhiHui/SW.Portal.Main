using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeletePlantHandler : IRequestHandler<DeletePlantCommand, String>
    {
        private readonly IPlantCommandRepository _commandRepository;
        private readonly IPlantQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeletePlantHandler(IPlantCommandRepository customerRepository, IPlantQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeletePlantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _queryRepository.GetByIdAsync(request.Id);
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var PlantData = new Plant
                {
                    PlantID = queryEntity.PlantID,
                    CompanyID = queryEntity.CompanyID,
                    PlantCode = queryEntity.PlantCode,
                    Description = queryEntity.Description,
                };

                await _commandRepository.DeleteAsync(PlantData);
                if (result != null)
                {
                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.PlantCode, ColumnName = "PlantCode" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.PlantCode, CurrentValue = result?.PlantCode, ColumnName = "DisplayName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, ColumnName = "Description" });
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
                            Type = "Plant",
                            FormType = "Delete",
                            HRMasterSetId = result?.PlantID,
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
