using Application.Command.LeveMasters;
using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
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
    public class EditNavitemsHandler : IRequestHandler<EditNavItemsCommand, NavitemsResponse>
    {
        private readonly INavItemsCommandRepository _commandRepository;
        private readonly INavItemsQueryRepository _navItemsQueryRepository;
        public EditNavitemsHandler(INavItemsCommandRepository customerRepository, INavItemsQueryRepository navItemsQueryRepository)
        {
            _commandRepository = customerRepository;
            _navItemsQueryRepository = navItemsQueryRepository;
        }
        public async Task<NavitemsResponse> Handle(EditNavItemsCommand request, CancellationToken cancellationToken)
        {
            View_NavItems queryrEntity = new View_NavItems();
            queryrEntity.ItemId = request.ItemId;
            queryrEntity.ItemSerialNo = request.ItemSerialNo;
            queryrEntity.PackSizeId = request.PackSizeId;
            queryrEntity.UomId = request.UomId;
            queryrEntity.CompanyId = request.CompanyId;
            queryrEntity.SupplyToId = request.SupplyToId;
            queryrEntity.ModifiedByUserId = request.ModifiedByUserId;
            queryrEntity.ModifiedDate = request.ModifiedDate;
            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }
            try
            {
                var ifExits = await _navItemsQueryRepository.GetByItemSerialNoExitsAsync(queryrEntity);
                if (ifExits is null)
                {
                    await _navItemsQueryRepository.Update(queryrEntity);
                }
                else
                {
                    if (ifExits.ItemId == queryrEntity.ItemId)
                    {
                        await _navItemsQueryRepository.Update(queryrEntity);
                    }
                    else
                    {
                        queryrEntity.ItemId = -1;
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new NavitemsResponse
            {
                ItemId = queryrEntity.ItemId,
                CompanyId = queryrEntity.CompanyId,
            };

            return response;
        }
    }
    public class CreateNavProductionInformationHandler : IRequestHandler<CreateNavProductionInformationCommand, NavProductionInformationResponse>
    {
        private readonly INavProductionInformationCommandRepository _commandRepository;
        public CreateNavProductionInformationHandler(INavProductionInformationCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<NavProductionInformationResponse> Handle(CreateNavProductionInformationCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<NavProductionInformation>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new NavProductionInformationResponse
            {
                ItemId = queryEntity.ItemId,
                NavProductionInformationId = (long)data,
            };
            return response;
        }
    }
    public class EditNavProductionInformationHandler : IRequestHandler<EditNavProductionInformationCommand, NavProductionInformationResponse>
    {
        private readonly INavProductionInformationCommandRepository _commandRepository;
        public EditNavProductionInformationHandler(INavProductionInformationCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<NavProductionInformationResponse> Handle(EditNavProductionInformationCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<NavProductionInformation>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            await _commandRepository.UpdateAsync(queryEntity);
            var response = new NavProductionInformationResponse
            {
                ItemId = queryEntity.ItemId,
                NavProductionInformationId = request.NavProductionInformationId,
            };
            return response;
        }
    }


    public class CreateNavCrossReferenceHandler : IRequestHandler<CreateNavCrossReferenceCommand, NavCrossReferenceResponse>
    {
        private readonly INavCrossReferenceCommandRepository _commandRepository;
        public CreateNavCrossReferenceHandler(INavCrossReferenceCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<NavCrossReferenceResponse> Handle(CreateNavCrossReferenceCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<NavCrossReference>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new NavCrossReferenceResponse
            {
                ItemId = queryEntity.ItemId,
                NavCrossReferenceId = (long)data,
            };
            return response;
        }
    }
    public class EditNavCrossReferenceHandler : IRequestHandler<EditNavCrossReferenceCommand, NavCrossReferenceResponse>
    {
        private readonly INavCrossReferenceCommandRepository _commandRepository;
        public EditNavCrossReferenceHandler(INavCrossReferenceCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<NavCrossReferenceResponse> Handle(EditNavCrossReferenceCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<NavCrossReference>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            await _commandRepository.UpdateAsync(queryEntity);
            var response = new NavCrossReferenceResponse
            {
                ItemId = queryEntity.ItemId,
                NavCrossReferenceId = request.NavCrossReferenceId,
            };
            return response;
        }
    }
    public class DeletNavProductionInformationHandler : IRequestHandler<DeleteNavItemsCommand, String>
    {
        private readonly INavProductionInformationCommandRepository _commandRepository;
        public DeletNavProductionInformationHandler(INavProductionInformationCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }

        public async Task<string> Handle(DeleteNavItemsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = new NavProductionInformation
                {
                    NavProductionInformationId = request.Id,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "NavProductionInformation information has been deleted!";
        }
    }
    public class DeleteNavCrossReferenceHandler : IRequestHandler<DeleteNavCrossReference, String>
    {
        private readonly INavCrossReferenceCommandRepository _commandRepository;
        public DeleteNavCrossReferenceHandler(INavCrossReferenceCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }

        public async Task<string> Handle(DeleteNavCrossReference request, CancellationToken cancellationToken)
        {
            try
            {
                var data = new NavCrossReference
                {
                    NavCrossReferenceId = request.Id,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "NavCrossReference information has been deleted!";
        }
    }
}
