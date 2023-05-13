using Application.Command.Departments;
using Application.Command.EmployeeEmailInfos;
using Application.Command.EmployeeOtherDutyInformations;
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
        public DeleteEmployeeEmailInfoHandler(IEmployeeEmailInfoCommandRepository customerRepository, IEmployeeEmailInfoQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteEmployeeEmailInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
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
                    EmployeeEmailInfoForwardID= queryEntity.EmployeeEmailInfoForwardID,
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
