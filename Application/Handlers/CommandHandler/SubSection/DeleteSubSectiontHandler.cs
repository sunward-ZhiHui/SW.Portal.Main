using Application.Command.SubSections;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteSubSectionHandler : IRequestHandler<DeleteSubSectionCommand, String>
    {
        private readonly ISubSectionCommandRepository _commandRepository;
        private readonly ISubSectionQueryRepository _queryRepository;
        public DeleteSubSectionHandler(ISubSectionCommandRepository customerRepository, ISubSectionQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteSubSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new SubSection
                {
                    SubSectionId= request.Id,
                    Name=queryEntity.SubSectionName,
                    AddedByUserId= queryEntity.AddedByUserId,
                    StatusCodeId= queryEntity.StatusCodeId,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "SubSection information has been deleted!";
        }
    }
}
