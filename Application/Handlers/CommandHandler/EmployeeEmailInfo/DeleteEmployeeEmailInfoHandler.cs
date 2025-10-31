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
                var guid = Guid.NewGuid();
                var uid = Guid.NewGuid();

                if (result != null)
                {
                    var date = DateTime.Now;
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Delete", result.SubscriptionID?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "SubscriptionID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Delete", result.Subscription, null, request.Id, guid, request?.UserId, DateTime.Now, true, "Subscription", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Delete", result?.EmailGuideID?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "EmailGuideID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Delete", result?.EmailGuide, null, request.Id, guid, request?.UserId, DateTime.Now, true, "EmailGuide", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Delete", request.UserId.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedByUserId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Delete", date.ToString("dd-MMM-yyyy hh:mm:ss tt"), null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Delete", request.UserName, null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedBy", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeEmailInfo", "Delete", result?.EmployeeID?.ToString(), result?.EmployeeID?.ToString(), request.Id, guid, request?.UserId, DateTime.Now, true, "EmployeeId", uid);

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
