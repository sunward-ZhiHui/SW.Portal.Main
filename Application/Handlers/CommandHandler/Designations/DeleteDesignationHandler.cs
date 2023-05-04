using Application.Command.designations;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteDesignationHandler : IRequestHandler<DeleteDesignationCommand, String>
    {
        private readonly IDesignationCommandRepository _commandRepository;
        private readonly IDesignationQueryRepository _queryRepository;
        public DeleteDesignationHandler(IDesignationCommandRepository customerRepository, IDesignationQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteDesignationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new Designation
                {
                    DesignationId= request.Id,
                    CompanyId = queryEntity.CompanyID,
                    Name=queryEntity.Name,
                    AddedByUserId= queryEntity.AddedByUserID,
                    StatusCodeId= queryEntity.StatusCodeID.Value,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Designation information has been deleted!";
        }
    }
}
