using Application.Command.SoSalesOrderLine;
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
    public class EditSoSalesOrderLineHandler : IRequestHandler<EditSoSalesOrderLineCommand, SoSalesOrderLineResponse>
    {
        private readonly ISoSalesOrderLineCommandRepository _commandRepository;
        private readonly ISoSalesOrderLineQueryRepository _queryRepository;
        public EditSoSalesOrderLineHandler(ISoSalesOrderLineCommandRepository customerRepository, ISoSalesOrderLineQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<SoSalesOrderLineResponse> Handle(EditSoSalesOrderLineCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<SoSalesOrderLine>(request);

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
            var response = new SoSalesOrderLineResponse
            {
                SoSalesOrderLineId = queryrEntity.SoSalesOrderLineId,
               
            };

            return response;
        }
    }
}
