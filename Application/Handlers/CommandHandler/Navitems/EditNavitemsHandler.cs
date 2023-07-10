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
            var queryrEntity = RoleMapper.Mapper.Map<Navitems>(request);

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
}
