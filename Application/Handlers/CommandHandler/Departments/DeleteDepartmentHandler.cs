using Application.Command.Departments;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentCommand, String>
    {
        private readonly IDepartmentCommandRepository _commandRepository;
        private readonly IDepartmentQueryRepository _queryRepository;
        public DeleteDepartmentHandler(IDepartmentCommandRepository customerRepository, IDepartmentQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new Department
                {
                    DepartmentId= request.Id,
                    CompanyId = queryEntity.CompanyId,
                    Name=queryEntity.DepartmentName,
                    AddedByUserId= queryEntity.AddedByUserId,
                    StatusCodeId= queryEntity.StatusCodeId,
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
