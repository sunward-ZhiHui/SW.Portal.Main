using Application.Command.Departments;
using Application.Command.EmployeeEmailInfos;
using Application.Command.EmployeeEmailInfos;
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
    public class EditEmployeeEmailInfoHandler : IRequestHandler<EditEmployeeEmailInfoCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoCommandRepository _commandRepository;
        private readonly IEmployeeEmailInfoQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditEmployeeEmailInfoHandler(IEmployeeEmailInfoCommandRepository customerRepository, IEmployeeEmailInfoQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(EditEmployeeEmailInfoCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.EmployeeEmailInfoID);
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeEmailInfo>(request);

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
                    if (result.EmailGuideID != queryrEntity.EmailGuideID)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.EmailGuideID?.ToString(), CurrentValue = queryrEntity?.EmailGuideID?.ToString(), ColumnName = "EmailGuideID" });
                        isUpdate = true;

                        auditList.Add(new HRMasterAuditTrail { PreValue = result.EmailGuide, CurrentValue = queryrEntity?.EmailGuide, ColumnName = "EmailGuide" });
                    }
                    if (result.SubscriptionID != queryrEntity?.SubscriptionID)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.SubscriptionID?.ToString(), CurrentValue = queryrEntity?.SubscriptionID?.ToString(), ColumnName = "EmailGuideID" });
                        isUpdate = true;

                        auditList.Add(new HRMasterAuditTrail { PreValue = result.Subscription, CurrentValue = queryrEntity?.Subscription, ColumnName = "EmailGuide" });
                    }
                    if (isUpdate)
                    {

                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserId?.ToString(), CurrentValue = queryrEntity?.ModifiedByUserId?.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });

                        auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryrEntity?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });

                        auditList.Add(new HRMasterAuditTrail { PreValue = queryrEntity?.EmployeeID?.ToString(), CurrentValue = queryrEntity?.EmployeeID?.ToString(), ColumnName = "EmployeeId" });
                    }
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "EmployeeEmailInfo",
                            FormType = "Update",
                            HRMasterSetId = request?.EmployeeEmailInfoID,
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
            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID = request.EmployeeEmailInfoID,
                EmployeeID = request.EmployeeID,
            };

            return response;
        }
    }
    public class EditEmployeeEmailInfoForwardHandler : IRequestHandler<EditEmployeeEmailInfoForwardCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoForwardCommandRepository _commandRepository;
        private readonly IEmployeeEmailInfoForwardQueryRepository _queryRepository;
        public EditEmployeeEmailInfoForwardHandler(IEmployeeEmailInfoForwardCommandRepository customerRepository, IEmployeeEmailInfoForwardQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(EditEmployeeEmailInfoForwardCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeEmailInfoForward>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID = 1,
            };

            return response;
        }
    }

    public class EditEmployeeEmailInfoAuthorityHandler : IRequestHandler<EditEmployeeEmailInfoAuthorityCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoAuthorityCommandRepository _commandRepository;
        private readonly IEmployeeEmailInfoAuthorityQueryRepository _queryRepository;
        public EditEmployeeEmailInfoAuthorityHandler(IEmployeeEmailInfoAuthorityCommandRepository customerRepository, IEmployeeEmailInfoAuthorityQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(EditEmployeeEmailInfoAuthorityCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeEmailInfoAuthority>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID = 1,
            };

            return response;
        }
    }
}
