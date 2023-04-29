using Application.Commands;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
 

namespace Application.Handlers.CommandHandler
{
    public class DeleteForumTypeHandler : IRequestHandler<DeleteForumTypeCommand, String>
    {
        private readonly IForumTypeCommandRepository _customerCommandRepository;
        private readonly IForumTypeQueryRepository _customerQueryRepository;
        public DeleteForumTypeHandler(IForumTypeCommandRepository customerRepository, IForumTypeQueryRepository customerQueryRepository)
        {
            _customerCommandRepository = customerRepository;
            _customerQueryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteForumTypeCommand request, CancellationToken cancellationToken)
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
