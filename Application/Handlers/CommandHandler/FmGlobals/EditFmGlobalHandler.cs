using Application.Command.designations;
using Application.Command.FmGlobals;
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
    public class EditFmGlobalHandler : IRequestHandler<EditFmGlobalCommand, FmglobalResponse>
    {
        private readonly IFmGlobalCommandRepository _commandRepository;
        public EditFmGlobalHandler(IFmGlobalCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }
        public async Task<FmglobalResponse> Handle(EditFmGlobalCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<Fmglobal>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new FmglobalResponse
            {
                FmglobalId = queryrEntity.FmglobalId,
            };

            return response;
        }
    }

    public class EditFmGlobalLineHandler : IRequestHandler<EditFmGlobalLineCommand, FmglobalLineResponse>
    {
        private readonly IFmGlobalLineCommandRepository _commandRepository;
        public EditFmGlobalLineHandler(IFmGlobalLineCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }
        public async Task<FmglobalLineResponse> Handle(EditFmGlobalLineCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<FmglobalLine>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new FmglobalLineResponse
            {
                FmglobalLineId = queryrEntity.FmglobalLineId,
            };

            return response;
        }
    }
    public class EditFmGlobalLineItemHandler : IRequestHandler<EditFmGlobalLineItemCommand, FmglobalLineItemResponse>
    {
        private readonly IFmGlobalLineItemCommandRepository _commandRepository;
        public EditFmGlobalLineItemHandler(IFmGlobalLineItemCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }
        public async Task<FmglobalLineItemResponse> Handle(EditFmGlobalLineItemCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<FmglobalLineItem>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new FmglobalLineItemResponse
            {
                FmglobalLineItemId = queryrEntity.FmglobalLineItemId,
            };

            return response;
        }
    }
}
