using Application.Command.Departments;
using Application.Command.EmployeeICTInformations;
using Application.Command.EmployeeOtherDutyInformations;
using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class EditEmployeeICTInformationHandler : IRequestHandler<EditEmployeeICTInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeICTInformationCommandRepository _commandRepository;
        private readonly IEmployeeICTInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditEmployeeICTInformationHandler(IEmployeeICTInformationCommandRepository customerRepository, IEmployeeICTInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(EditEmployeeICTInformationCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.EmployeeIctinformationId);
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeICTInformation>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);

                if (result != null)
                {
                    bool isUpdate = false; List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    if (result.SoftwareId != queryrEntity.SoftwareId)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.SoftwareId?.ToString(), CurrentValue = queryrEntity?.SoftwareId?.ToString(), ColumnName = "SoftwareId" });
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.SoftwareName, CurrentValue = queryrEntity?.SoftwareName, ColumnName = "SoftwareName" });
                    }
                    if (result.LoginId != queryrEntity?.LoginId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.LoginId, CurrentValue = queryrEntity?.LoginId, ColumnName = "LoginId" });
                    }
                    if (result.Password != queryrEntity?.Password)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.Password, CurrentValue = queryrEntity?.Password, ColumnName = "Password" });
                    }
                    if (result.RoleId != queryrEntity?.RoleId)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.RoleId?.ToString(), CurrentValue = queryrEntity?.RoleId?.ToString(), ColumnName = "RoleId" });
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.RoleName, CurrentValue = queryrEntity?.RoleName, ColumnName = "RoleName" });
                    }
                    if (result.StatusCodeId != queryrEntity?.StatusCodeId)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.StatusCodeId?.ToString(), CurrentValue = queryrEntity?.StatusCodeId?.ToString(), ColumnName = "StatusCodeId" });
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.StatusCode, CurrentValue = queryrEntity?.StatusCode, ColumnName = "StatusCode" });
                    }
                    if (isUpdate)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserId?.ToString(), CurrentValue = queryrEntity?.ModifiedByUserId?.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = queryrEntity?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.EmployeeId?.ToString(), CurrentValue = queryrEntity?.EmployeeId?.ToString(), ColumnName = "EmployeeId" });
                    }
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "EmployeeICTInformation",
                            FormType = "Update",
                            HRMasterSetId = result?.EmployeeIctinformationId,
                            AuditUserId = queryrEntity?.ModifiedByUserId,
                        };
                        await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new EmployeeOtherDutyInformationResponse
            {
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
            };

            return response;
        }
    }
    public class EditEmployeeICTHardInformationHandler : IRequestHandler<EditEmployeeICTHardInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeICTHardInformationCommandRepository _commandRepository;
        private readonly IEmployeeICTHardInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditEmployeeICTHardInformationHandler(IEmployeeICTHardInformationCommandRepository customerRepository, IEmployeeICTHardInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(EditEmployeeICTHardInformationCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.EmployeeIctHardinformationId);
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeICTHardInformation>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);

                if (result != null)
                {
                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    bool isUpdate = false;
                    if (result.CompanyId != queryrEntity.CompanyId)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.CompanyId?.ToString(), CurrentValue = queryrEntity?.CompanyId?.ToString(), ColumnName = "CompanyId" });
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.PlantCode?.ToString(), CurrentValue = queryrEntity?.CompanyName?.ToString(), ColumnName = "PlantCode" });
                    }
                    if (result.LoginId != queryrEntity?.LoginId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.LoginId?.ToString(), CurrentValue = queryrEntity?.LoginId?.ToString(), ColumnName = "LoginId" });
                    }
                    if (result.Name != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.Name?.ToString(), CurrentValue = queryrEntity?.Name?.ToString(), ColumnName = "Name" });
                    }
                    if (result.Password != queryrEntity?.Password)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.Password?.ToString(), CurrentValue = queryrEntity?.Password?.ToString(), ColumnName = "Password" });
                    }
                    if (result.Instruction != queryrEntity?.Instruction)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.Instruction?.ToString(), CurrentValue = queryrEntity?.Instruction?.ToString(), ColumnName = "Instruction" });
                    }
                    if (result.StatusCodeId != queryrEntity?.StatusCodeId)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.StatusCodeId?.ToString()?.ToString(), CurrentValue = queryrEntity?.StatusCodeId?.ToString()?.ToString(), ColumnName = "StatusCodeId" });
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.StatusCode?.ToString(), CurrentValue = queryrEntity?.StatusCode?.ToString(), ColumnName = "StatusCode" });
                    }
                    if (isUpdate)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserId?.ToString(), CurrentValue = queryrEntity?.ModifiedByUserId?.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = queryrEntity?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.EmployeeId?.ToString(), CurrentValue = queryrEntity?.EmployeeId?.ToString(), ColumnName = "EmployeeId" });
                    }
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "EmployeeICTHardInformation",
                            FormType = "Update",
                            HRMasterSetId = result?.EmployeeIctHardinformationId,
                            AuditUserId = queryrEntity?.ModifiedByUserId,
                        };
                        await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new EmployeeOtherDutyInformationResponse
            {
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
            };

            return response;
        }
    }
}
