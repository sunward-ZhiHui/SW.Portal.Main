using Application.Command.Departments;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentCommand, String>
    {
        private readonly IDepartmentCommandRepository _commandRepository;
        private readonly IDepartmentQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteDepartmentHandler(IDepartmentCommandRepository customerRepository, IDepartmentQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _queryRepository.GetByIdAsync(request.Id);
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new Department
                {
                    DepartmentId = request.Id,
                    CompanyId = queryEntity.CompanyId,
                    Name = queryEntity.DepartmentName,
                    AddedByUserId = queryEntity.AddedByUserId,
                    StatusCodeId = queryEntity.StatusCodeId.Value,
                };

                await _commandRepository.DeleteAsync(data);
                if (result != null)
                {
                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DepartmentName, ColumnName = "Name" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DepartmentName, CurrentValue = result?.DepartmentName, ColumnName = "DisplayName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DepartmentCode, ColumnName = "Code" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, ColumnName = "Description" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.CompanyId?.ToString(), ColumnName = "CompanyId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.PlantCode?.ToString(), ColumnName = "CompanyName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DivisionId?.ToString(), ColumnName = "DivisionId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Division?.ToString(), ColumnName = "DivisionName" });
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
                            Type = "Department",
                            FormType = "Delete",
                            HRMasterSetId = result?.DepartmentId,
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

            return "Department information has been deleted!";
        }
    }
}
