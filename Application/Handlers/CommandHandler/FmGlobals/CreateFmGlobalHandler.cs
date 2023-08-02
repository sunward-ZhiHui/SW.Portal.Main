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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Handlers.CommandHandler.FmGlobals
{
    public class CreateFmGlobalHandler : IRequestHandler<CreateFmGlobalCommand, FmglobalResponse>
    {
        private readonly IFmGlobalCommandRepository _commandRepository;
        public CreateFmGlobalHandler(IFmGlobalCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<FmglobalResponse> Handle(CreateFmGlobalCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<Fmglobal>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new FmglobalResponse
            {
                FmglobalId = (long)data,
                StatusCodeId = queryEntity.StatusCodeId,
            };
            return response;
        }
    }
    public class CreateFmGlobalLineHandler : IRequestHandler<CreateFmGlobalLineCommand, FmglobalLineResponse>
    {
        private readonly IFmGlobalLineCommandRepository _commandRepository;
        private readonly IFmGlobalLineQueryRepository _fmGlobalLineQueryRepository;
        private readonly IFmGlobalLineItemQueryRepository _queryRepository;
        private readonly IFmGlobalLineItemCommandRepository _commandLineRepository;
        public CreateFmGlobalLineHandler(IFmGlobalLineCommandRepository commandRepository, IFmGlobalLineQueryRepository fmGlobalLineQueryRepository, IFmGlobalLineItemQueryRepository queryRepository, IFmGlobalLineItemCommandRepository commandLineRepository)
        {
            _commandRepository = commandRepository;
            _fmGlobalLineQueryRepository = fmGlobalLineQueryRepository;
            _queryRepository = queryRepository;
            _commandLineRepository = commandLineRepository;
        }
        public async Task<FmglobalLineResponse> Handle(CreateFmGlobalLineCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<FmglobalLine>(request);
            if (request.PalletEntryNoId > 0)
            {
            }
            else
            {
                queryEntity.PalletNoYear = DateTime.Now.ToString("yyyy");
                var result = await _fmGlobalLineQueryRepository.GetByPalletNoAsync(queryEntity.PalletNoYear, request.CompanyId);
                int PalletNo = 1;
                if (result != null)
                {
                    PalletNo = (int)(result.PalletNoAuto + 1);
                }
                queryEntity.PalletNoAuto = PalletNo;
                queryEntity.PalletNo = queryEntity.PalletNoYear + "/" + System.String.Format("{0:D6}", PalletNo);
            }

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            if (request.PalletEntryNoId > 0)
            {
                queryEntity.FmglobalLineId = (long)data;
                queryEntity.PalletNo = data + "_" + queryEntity.PalletNo;
                await _commandRepository.UpdateAsync(queryEntity);
                var results = await _queryRepository.GetAllByIdAsync(request.PalletEntryNoId.Value);
                results.ToList().ForEach(async a =>
                {
                    var fmglobalLineItem = new FmglobalLineItem();
                    fmglobalLineItem.FmglobalLineId = data;
                    fmglobalLineItem.ItemId = a.ItemId;
                    fmglobalLineItem.BatchInfoId = a.BatchInfoId;
                    fmglobalLineItem.CartonPackingId = a.CartonPackingId;
                    fmglobalLineItem.NoOfCarton = a.NoOfCarton;
                    fmglobalLineItem.NoOfUnitsPerCarton = a.NoOfUnitsPerCarton;
                    fmglobalLineItem.StatusCodeId = a.StatusCodeId;
                    fmglobalLineItem.AddedByUserId = request.AddedByUserId;
                    fmglobalLineItem.AddedDate = request.AddedDate;
                    fmglobalLineItem.ModifiedByUserId = request.ModifiedByUserId;
                    fmglobalLineItem.ModifiedDate = request.ModifiedDate;
                    fmglobalLineItem.SessionId = Guid.NewGuid();
                    fmglobalLineItem.ItemBatchInfoId = a.ItemBatchInfoId;
                    var datas = await _commandLineRepository.AddAsync(fmglobalLineItem);
                });
            }
            var response = new FmglobalLineResponse
            {
                FmglobalLineId = (long)data,
                FmglobalId = queryEntity.FmglobalId,
                StatusCodeId = queryEntity.StatusCodeId,
            };
            return response;
        }
    }
    public class CreateFmGlobalLineItemHandler : IRequestHandler<CreateGlobalLineItemCommand, FmglobalLineItemResponse>
    {
        private readonly IFmGlobalLineItemCommandRepository _commandRepository;

        public CreateFmGlobalLineItemHandler(IFmGlobalLineItemCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;

        }
        public async Task<FmglobalLineItemResponse> Handle(CreateGlobalLineItemCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<FmglobalLineItem>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new FmglobalLineItemResponse
            {
                FmglobalLineItemId = (long)data,
                FmglobalLineId = queryEntity.FmglobalLineId,
                StatusCodeId = queryEntity.StatusCodeId,
            };
            return response;
        }
    }
}
