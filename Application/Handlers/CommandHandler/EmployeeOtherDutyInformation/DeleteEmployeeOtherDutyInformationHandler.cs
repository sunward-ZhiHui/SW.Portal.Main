using Application.Command.Departments;
using Application.Command.EmployeeOtherDutyInformations;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteEmployeeOtherDutyInformationHandler : IRequestHandler<DeleteEmployeeOtherDutyInformationCommand, String>
    {
        private readonly IEmployeeOtherDutyInformationCommandRepository _commandRepository;
        private readonly IEmployeeOtherDutyInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteEmployeeOtherDutyInformationHandler(IEmployeeOtherDutyInformationCommandRepository customerRepository, IEmployeeOtherDutyInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteEmployeeOtherDutyInformationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _queryRepository.GetByIdAsync(request.Id);
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new EmployeeOtherDutyInformation
                {
                    EmployeeId = queryEntity.EmployeeId,
                    EmployeeOtherDutyInformationId = request.Id,
                    AddedByUserId = queryEntity.AddedByUserId,
                };

                if (result != null)
                {
                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DesignationId?.ToString(), ColumnName = "DesignationId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DesignationName?.ToString(), ColumnName = "DesignationName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StartDate != null ? result?.StartDate.Value.ToString("dd-MMM-yyyy") : null, ColumnName = "StartDate" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.EndDate != null ? result?.EndDate.Value.ToString("dd-MMM-yyyy") : null, ColumnName = "EndDate" });

                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DutyTypeId?.ToString(), ColumnName = "DutyTypeId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.DutyType?.ToString(), ColumnName = "DutyType" });

                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, ColumnName = "StatusCode" });

                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserId.ToString(), ColumnName = "ModifiedByUserId" });

                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt"), ColumnName = "ModifiedDate" });

                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, ColumnName = "ModifiedBy" });

                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.EmployeeId?.ToString(), ColumnName = "EmployeeId" });
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "EmployeeOtherDutyInformation",
                            FormType = "Delete",
                            HRMasterSetId = result?.EmployeeOtherDutyInformationId,
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

            return "Department information has been deleted!";
        }
    }
}
