using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.EntityModel;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler.Plants
{
    public class CreatePlantHandler : IRequestHandler<CreatePlantCommand, PlantResponse>
    {
        private readonly IPlantCommandRepository _commandRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public CreatePlantHandler(IPlantCommandRepository commandRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<PlantResponse> Handle(CreatePlantCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<Plant>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var plantData = await _commandRepository.AddAsync(customerEntity);
            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
            auditList.Add(new HRMasterAuditTrail { CurrentValue = customerEntity?.PlantCode, ColumnName = "PlantCode" });
            auditList.Add(new HRMasterAuditTrail { PreValue = customerEntity?.PlantCode, CurrentValue = customerEntity?.PlantCode, ColumnName = "DisplayName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = customerEntity?.Description, ColumnName = "Description" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = customerEntity?.StatusCodeID?.ToString(), ColumnName = "StatusCodeID" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = customerEntity?.StatusCode, ColumnName = "StatusCode" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = customerEntity?.AddedByUserID?.ToString(), ColumnName = "AddedByUserId" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = customerEntity?.AddedDate != null ? customerEntity.AddedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "AddedDate" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = customerEntity?.AddedBy?.ToString(), ColumnName = "AddedBy" });
            if (auditList.Count() > 0)
            {
                HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                {
                    HRMasterAuditTrailItems = auditList,
                    Type = "Plant",
                    FormType = "Add",
                    HRMasterSetId = plantData,
                    AuditUserId = customerEntity?.AddedByUserID,
                };
                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
            }
            var response = new PlantResponse
            {
                PlantID = (long)plantData,
                CompanyID = customerEntity.CompanyID,
                PlantCode = customerEntity.PlantCode,
                Description = customerEntity.Description,
            };
            return response;
        }
    }
}
