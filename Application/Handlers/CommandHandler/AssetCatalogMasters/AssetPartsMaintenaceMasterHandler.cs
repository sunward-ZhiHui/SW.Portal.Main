using Application.Command.AssetPartsMaintenaceMasters;
using Application.Command.Departments;
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

namespace Application.Handlers.CommandHandler.AssetCatalogMasters
{
    public class CreateAssetPartsMaintenaceMasterHandler : IRequestHandler<CreateAssetPartsMaintenaceMasterCommand, AssetPartsMaintenaceMasterResponse>
    {
        private readonly IAssetPartsMaintenaceMasterCommandRepository _commandRepository;
        public CreateAssetPartsMaintenaceMasterHandler(IAssetPartsMaintenaceMasterCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<AssetPartsMaintenaceMasterResponse> Handle(CreateAssetPartsMaintenaceMasterCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<AssetPartsMaintenaceMaster>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new AssetPartsMaintenaceMasterResponse
            {
                AssetPartsMaintenaceMasterId = (long)data,
                AssetCatalogMasterId= queryEntity.AssetCatalogMasterId,
                StatusCodeId = queryEntity.StatusCodeId,
            };
            return response;
        }
    }
    public class EditAssetPartsMaintenaceMasterHandler : IRequestHandler<EditAssetPartsMaintenaceMasterCommand, AssetPartsMaintenaceMasterResponse>
    {
        private readonly IAssetPartsMaintenaceMasterCommandRepository _commandRepository;
        private readonly IAssetPartsMaintenaceMasterQueryRepository _queryRepository;
        public EditAssetPartsMaintenaceMasterHandler(IAssetPartsMaintenaceMasterCommandRepository customerRepository, IAssetPartsMaintenaceMasterQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<AssetPartsMaintenaceMasterResponse> Handle(EditAssetPartsMaintenaceMasterCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<AssetPartsMaintenaceMaster>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new AssetPartsMaintenaceMasterResponse
            {
                AssetPartsMaintenaceMasterId = queryEntity.AssetPartsMaintenaceMasterId,
                AssetCatalogMasterId = queryEntity.AssetCatalogMasterId,
                StatusCodeId = queryEntity.StatusCodeId,
            };

            return response;
        }
    }
    public class DeleteAssetPartsMaintenaceMasterHandler : IRequestHandler<DeleteAssetPartsMaintenaceMasterCommand, String>
    {
        private readonly IAssetPartsMaintenaceMasterCommandRepository _commandRepository;
        private readonly IAssetPartsMaintenaceMasterQueryRepository _queryRepository;
        public DeleteAssetPartsMaintenaceMasterHandler(IAssetPartsMaintenaceMasterCommandRepository customerRepository, IAssetPartsMaintenaceMasterQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteAssetPartsMaintenaceMasterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new AssetPartsMaintenaceMaster
                {
                    AssetPartsMaintenaceMasterId = request.Id,
                    AssetCatalogMasterId= queryEntity.AssetCatalogMasterId,
                    AddedByUserId = queryEntity.AddedByUserId,
                    StatusCodeId = queryEntity.StatusCodeId,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "AssetPartsMaintenaceMaster has been deleted!";
        }
    }
}
