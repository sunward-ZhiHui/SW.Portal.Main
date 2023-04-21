using Application.Commands;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
 

namespace Application.Handlers.CommandHandler
{
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, String>
    {
        private readonly IRoleCommandRepository _customerCommandRepository;
        private readonly IRoleQueryRepository _customerQueryRepository;
        public DeleteRoleHandler(IRoleCommandRepository customerRepository, IRoleQueryRepository customerQueryRepository)
        {
            _customerCommandRepository = customerRepository;
            _customerQueryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customerEntity = await _customerQueryRepository.GetByIdAsync(request.Id);

                await _customerCommandRepository.DeleteAsync(customerEntity);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Customer information has been deleted!";
        }
    }
}
