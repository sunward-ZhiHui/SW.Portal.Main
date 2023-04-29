using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class CreateForumTypeHandler : IRequestHandler<CreateForumTypeCommand, ForumTypeResponse>
    {
        private readonly IForumTypeCommandRepository _typeCommandRepository;
        public CreateForumTypeHandler(IForumTypeCommandRepository typeCommandRepository)
        {
            _typeCommandRepository = typeCommandRepository;
        }
        public async Task<ForumTypeResponse> Handle(CreateForumTypeCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<ForumTypes>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var newCustomer = await _typeCommandRepository.AddAsync(customerEntity);
            var customerResponse = RoleMapper.Mapper.Map<ForumTypeResponse>(newCustomer);
            return customerResponse;
        }
    }
}
