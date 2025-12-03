using Application.Command.Departments;
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
    public class EditDepartmentHandler : IRequestHandler<EditDepartmentCommand, DepartmentResponse>
    {
        private readonly IDepartmentCommandRepository _commandRepository;
        private readonly IDepartmentQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditDepartmentHandler(IDepartmentCommandRepository customerRepository, IDepartmentQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<DepartmentResponse> Handle(EditDepartmentCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.DepartmentId);
            var queryrEntity = RoleMapper.Mapper.Map<Department>(request);

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
                    if (result.DepartmentCode != queryrEntity.Code)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.DepartmentCode, CurrentValue = queryrEntity?.Code, ColumnName = "Code" });
                    }
                    if (result?.DepartmentName != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.DepartmentName, CurrentValue = queryrEntity?.Name, ColumnName = "Name" });
                    }
                    if (result?.Description != queryrEntity?.Description)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, CurrentValue = queryrEntity?.Description, ColumnName = "Description" });
                    }
                    if (result?.CompanyId != queryrEntity?.CompanyId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.CompanyId?.ToString(), CurrentValue = queryrEntity?.CompanyId?.ToString(), ColumnName = "CompanyId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.PlantCode, CurrentValue = queryrEntity?.CompanyName, ColumnName = "CompanyName" });
                    }
                    if (result?.DivisionId != queryrEntity?.DivisionId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.DivisionId?.ToString(), CurrentValue = queryrEntity?.DivisionId?.ToString(), ColumnName = "DivisionId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.Division, CurrentValue = queryrEntity?.DivisionName, ColumnName = "DivisionName" });
                    }
                    if (result?.ProfileCode != queryrEntity?.ProfileCode)
                    {

                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ProfileCode, CurrentValue = queryrEntity?.ProfileCode, ColumnName = "ProfileCode" });
                    }
                    if (result?.StatusCodeId != queryrEntity?.StatusCodeId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeId?.ToString(), CurrentValue = queryrEntity?.StatusCodeId.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, CurrentValue = queryrEntity?.StatusCode, ColumnName = "StatusCode" });
                    }
                    if (isUpdate)
                    {
                        auditList.Add(new HRMasterAuditTrail { CurrentValue = queryrEntity?.Name, PreValue = queryrEntity?.Name, ColumnName = "DisplayName" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserId?.ToString(), CurrentValue = queryrEntity?.ModifiedByUserId?.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = queryrEntity?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                    }
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "Department",
                            FormType = "Update",
                            HRMasterSetId = result?.DepartmentId,
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
            var response = new DepartmentResponse
            {
                DepartmentId = queryrEntity.DepartmentId,
                CompanyId = queryrEntity.CompanyId,
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
                DepartmentName = queryrEntity.Name,
            };

            return response;
        }
    }
}
