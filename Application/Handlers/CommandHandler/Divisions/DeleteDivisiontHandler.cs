using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteDivisionHandler : IRequestHandler<DeleteDivisionCommand, String>
    {
        private readonly IDivisionCommandRepository _commandRepository;
        private readonly IDivisionQueryRepository _queryRepository;
        public DeleteDivisionHandler(IDivisionCommandRepository customerRepository, IDivisionQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new Division
                {
                    DivisionId= request.Id,
                    CompanyId = queryEntity.CompanyId,
                    Name=queryEntity.Name,
                    AddedByUserId= queryEntity.AddedByUserID,
                    StatusCodeId= queryEntity.StatusCodeID,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Plant information has been deleted!";
        }
    }
}
