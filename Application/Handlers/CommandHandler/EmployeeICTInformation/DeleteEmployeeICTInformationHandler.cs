using Application.Command.Departments;
using Application.Command.EmployeeICTInformations;
using Application.Command.EmployeeOtherDutyInformations;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;


namespace Application.Handlers.CommandHandler
{
    public class DeleteEmployeeICTInformationHandler : IRequestHandler<DeleteEmployeeICTInformationCommand, String>
    {
        private readonly IEmployeeICTInformationCommandRepository _commandRepository;
        private readonly IEmployeeICTInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteEmployeeICTInformationHandler(IEmployeeICTInformationCommandRepository customerRepository, IEmployeeICTInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteEmployeeICTInformationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new EmployeeICTInformation
                {
                    EmployeeId = queryEntity.EmployeeId,
                    EmployeeIctinformationId = queryEntity.EmployeeIctinformationId,
                    AddedByUserId = queryEntity.AddedByUserId,
                };
                var result = queryEntity;
                await _commandRepository.DeleteAsync(data);

                if (result != null)
                {
                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    auditList.Add(new HRMasterAuditTrail { PreValue = result.SoftwareId?.ToString(), ColumnName = "SoftwareId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result.SoftwareName, ColumnName = "SoftwareName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result.Password, ColumnName = "Password" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.RoleId?.ToString(), ColumnName = "RoleId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.RoleName, ColumnName = "RoleName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeId?.ToString(), ColumnName = "StatusCodeId" });
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
                            Type = "EmployeeICTInformation",
                            FormType = "Delete",
                            HRMasterSetId = result?.EmployeeIctinformationId,
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
    public class DeleteEmployeeICTHardInformationHandler : IRequestHandler<DeleteEmployeeICTHardInformationCommand, String>
    {
        private readonly IEmployeeICTHardInformationCommandRepository _commandRepository;
        private readonly IEmployeeICTHardInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteEmployeeICTHardInformationHandler(IEmployeeICTHardInformationCommandRepository customerRepository, IEmployeeICTHardInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteEmployeeICTHardInformationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new EmployeeICTHardInformation
                {
                    EmployeeId = queryEntity.EmployeeId,
                    EmployeeIctHardinformationId = queryEntity.EmployeeIctHardinformationId,
                    AddedByUserId = queryEntity.AddedByUserId,
                };
                var result = queryEntity;
                await _commandRepository.DeleteAsync(data);
                var guid = Guid.NewGuid();

                if (result != null)
                {
                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    var date = DateTime.Now;
                    auditList.Add(new HRMasterAuditTrail { PreValue = result.Name?.ToString(), ColumnName = "Name" });

                    auditList.Add(new HRMasterAuditTrail { PreValue = result.Password, ColumnName = "Password" });

                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.CompanyId?.ToString(), ColumnName = "CompanyId" });

                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.PlantCode, ColumnName = "PlantCode" });

                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeId?.ToString(), ColumnName = "StatusCodeId" });

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
                            Type = "EmployeeICTHardInformation",
                            FormType = "Delete",
                            HRMasterSetId = result?.EmployeeIctHardinformationId,
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
