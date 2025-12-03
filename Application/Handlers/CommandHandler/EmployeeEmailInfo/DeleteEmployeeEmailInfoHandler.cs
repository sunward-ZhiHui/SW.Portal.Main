using Application.Command.Departments;
using Application.Command.EmployeeEmailInfos;
using Application.Command.EmployeeEmailInfos;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteEmployeeEmailInfoHandler : IRequestHandler<DeleteEmployeeEmailInfoCommand, String>
    {
        private readonly IEmployeeEmailInfoCommandRepository _commandRepository;
        private readonly IEmployeeEmailInfoQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteEmployeeEmailInfoHandler(IEmployeeEmailInfoCommandRepository customerRepository, IEmployeeEmailInfoQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteEmployeeEmailInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _queryRepository.GetByIdAsync(request.Id);
                var queryEntity = result;

                if (result != null)
                {
                    List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    var date = DateTime.Now;
                    auditList.Add(new HRMasterAuditTrail { PreValue = result.EmailGuideID?.ToString(), ColumnName = "EmailGuideID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result.Subscription?.ToString(), ColumnName = "Subscription" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result.EmailGuideID?.ToString(), ColumnName = "EmailGuideID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result.EmailGuide?.ToString(), ColumnName = "EmailGuide" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = request.UserId?.ToString(), ColumnName = "ModifiedByUserId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result.EmployeeID?.ToString(), ColumnName = "EmployeeID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = date != null ? date.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = request.UserName?.ToString(), ColumnName = "ModifiedBy" });
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "EmployeeEmailInfo",
                            FormType = "Delete",
                            HRMasterSetId = result?.EmployeeEmailInfoID,
                            IsDeleted = true,
                            AuditUserId = request?.UserId,
                        };
                        await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                    }
                }
                var data = new EmployeeEmailInfo
                {
                    EmployeeID = queryEntity.EmployeeID,
                    EmployeeEmailInfoID = queryEntity.EmployeeEmailInfoID,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Department information has been deleted!";
        }
    }
    public class DeleteEmployeeEmailInfoForwardHandler : IRequestHandler<DeleteEmployeeEmailInfoForwardCommand, String>
    {
        private readonly IEmployeeEmailInfoForwardCommandRepository _commandRepository;
        private readonly IEmployeeEmailInfoForwardQueryRepository _queryRepository;
        public DeleteEmployeeEmailInfoForwardHandler(IEmployeeEmailInfoForwardCommandRepository customerRepository, IEmployeeEmailInfoForwardQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteEmployeeEmailInfoForwardCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new EmployeeEmailInfoForward
                {
                    EmployeeEmailInfoForwardID = queryEntity.EmployeeEmailInfoForwardID,
                    EmployeeEmailInfoID = queryEntity.EmployeeEmailInfoID,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Department information has been deleted!";
        }
    }

    public class DeleteEmployeeEmailInfoAuthorityHandler : IRequestHandler<DeleteEmployeeEmailInfoAuthorityCommand, String>
    {
        private readonly IEmployeeEmailInfoAuthorityCommandRepository _commandRepository;
        private readonly IEmployeeEmailInfoAuthorityQueryRepository _queryRepository;
        public DeleteEmployeeEmailInfoAuthorityHandler(IEmployeeEmailInfoAuthorityCommandRepository customerRepository, IEmployeeEmailInfoAuthorityQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteEmployeeEmailInfoAuthorityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new EmployeeEmailInfoAuthority
                {
                    EmployeeEmailInfoAuthorityID = queryEntity.EmployeeEmailInfoAuthorityID,
                    EmployeeEmailInfoID = queryEntity.EmployeeEmailInfoID,
                };

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
