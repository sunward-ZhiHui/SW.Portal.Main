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
    public class EditForumTypeHandler : IRequestHandler<EditForumTypeCommand, ForumTypeResponse>
    {
        private readonly IForumTypeCommandRepository _customerCommandRepository;
        private readonly IForumTypeQueryRepository _forumTypeQueryRepository;
     
        public EditForumTypeHandler(IForumTypeCommandRepository customerRepository, IForumTypeQueryRepository forumTypeQueryRepository)
        {
            _customerCommandRepository = customerRepository;
            _forumTypeQueryRepository = forumTypeQueryRepository;
        }
        public async Task<ForumTypeResponse> Handle(EditForumTypeCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<ForumTypes>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _customerCommandRepository.UpdateAsync(customerEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedCustomer = await _forumTypeQueryRepository.GetByIdAsync(request.ID);
            var customerResponse = RoleMapper.Mapper.Map<ForumTypeResponse>(modifiedCustomer);

            return customerResponse;
        }
    }
}
