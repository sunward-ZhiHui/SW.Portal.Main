using Application.Command.Departments;
using Application.Command.EmployeeICTInformations;
using Application.Command.EmployeeOtherDutyInformations;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteEmployeeICTInformationHandler : IRequestHandler<DeleteEmployeeICTInformationCommand, String>
    {
        private readonly IEmployeeICTInformationCommandRepository _commandRepository;
        private readonly IEmployeeICTInformationQueryRepository _queryRepository;
        public DeleteEmployeeICTInformationHandler(IEmployeeICTInformationCommandRepository customerRepository, IEmployeeICTInformationQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
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

                await _commandRepository.DeleteAsync(data);
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
        public DeleteEmployeeICTHardInformationHandler(IEmployeeICTHardInformationCommandRepository customerRepository, IEmployeeICTHardInformationQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
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
