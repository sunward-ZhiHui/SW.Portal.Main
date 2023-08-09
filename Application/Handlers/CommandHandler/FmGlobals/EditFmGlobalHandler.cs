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
        private readonly IFmGlobalLineQueryRepository _fmGlobalLineQueryRepository;
        private readonly IFmGlobalMoveCommandRepository _fmGlobalMoveCommandRepository;
        public EditFmGlobalHandler(IFmGlobalCommandRepository commandRepository, IFmGlobalLineQueryRepository fmGlobalLineQueryRepository, IFmGlobalMoveCommandRepository fmGlobalMoveCommandRepository)
        {
            _commandRepository = commandRepository;
            _fmGlobalLineQueryRepository = fmGlobalLineQueryRepository;
            _fmGlobalMoveCommandRepository = fmGlobalMoveCommandRepository;
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
                if (!string.IsNullOrEmpty(request.FmglobalStaus) && request.FmglobalStaus.ToLower() == "released")
                {
                    var result = await _fmGlobalLineQueryRepository.GetAllbyIdsAsync(request.FmglobalId);
                    if (result != null && result.Count > 0)
                    {
                        result.ToList().ForEach(async s =>
                        {
                            var Exits = await _fmGlobalLineQueryRepository.GetFMGlobalMoveCheckExits(request.LocationFromId, request.LocationToId,s.FmglobalId, s.FmglobalLineId, 1, 0);
                            if (Exits == null)
                            {
                                var lastInsertedRecordId = await _fmGlobalLineQueryRepository.UpdatePreviosMoveQty(s.FmglobalLineId, request.LocationFromId,request.ModifiedByUserId);
                                if (lastInsertedRecordId == 0)
                                {
                                    FmglobalMove fmglobalMove = new FmglobalMove();
                                    fmglobalMove.LocationID = request.LocationFromId;
                                    fmglobalMove.AddedDate = DateTime.Now;
                                    fmglobalMove.AddedByUserId = request.ModifiedByUserId;
                                    fmglobalMove.FMGlobalID = request.FmglobalId;
                                    fmglobalMove.LocationToID = request.LocationToId;
                                    fmglobalMove.FmglobalLineId = s.FmglobalLineId;
                                    fmglobalMove.IsHandQty = 1;
                                    fmglobalMove.TransactionQty = 0;
                                    await _fmGlobalMoveCommandRepository.AddAsync(fmglobalMove);
                                }
                            }
                        });
                    }
                }
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
