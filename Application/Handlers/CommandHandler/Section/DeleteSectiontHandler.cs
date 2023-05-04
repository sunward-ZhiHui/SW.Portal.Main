using Application.Command.Sections;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteSectionHandler : IRequestHandler<DeleteSectionCommand, String>
    {
        private readonly ISectionCommandRepository _commandRepository;
        private readonly ISectionQueryRepository _queryRepository;
        public DeleteSectionHandler(ISectionCommandRepository customerRepository, ISectionQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new Section
                {
                    SectionId= request.Id,
                    Name=queryEntity.SectionName,
                    AddedByUserId= queryEntity.AddedByUserId,
                    StatusCodeId= queryEntity.StatusCodeId.Value,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Section information has been deleted!";
        }
    }
}
