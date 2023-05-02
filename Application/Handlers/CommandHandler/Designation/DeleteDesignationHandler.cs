using Application.Command.Designation;
using Application.Commands;
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
    public class DeleteDesignationHandler : IRequestHandler<DeleteDesignationCommand, String>
    {
       
    
        private readonly IDesignationCommandRepository _commandRepository;
        private readonly IDesignationQueryRepository _queryRepository;
        public DeleteDesignationHandler(IDesignationCommandRepository designationRepository, IDesignationQueryRepository designationQueryRepository)
        {
            _commandRepository = designationRepository;
            _queryRepository = designationQueryRepository;
        }

        public async Task<string> Handle(DeleteDesignationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new Designation
                {
                    DesignationId = request.Id,
                    CompanyId = queryEntity.CompanyID,
                    Name = queryEntity.CompanyName,
                    AddedByUserId = queryEntity.AddedByUserID,
                    StatusCodeId = queryEntity.StatusCodeID,
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

