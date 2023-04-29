using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Entities.Base;
using Core.Repositories.Command;
using Core.Repositories.Command.Base;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class CreateHandler<T> : IRequestHandler<CreateCommand<T>, T> where T : class
    {
        private readonly ICommandRepository<T> _repository;  

        public CreateHandler(ICommandRepository<T> repository)       
        {
            _repository = repository;
        }

        public async Task<T> Handle(CreateCommand<T> request, CancellationToken cancellationToken)
        {
            //return await _repository.AddAsync(request.Entity);

            var customerEntity = RoleMapper.Mapper.Map<T>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var newCustomer = await _repository.AddAsync(customerEntity);
            var customerResponse = RoleMapper.Mapper.Map<T>(newCustomer);
            return customerResponse;

        }
    }
}
