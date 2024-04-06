using Application.Command.AssetCatalogMasters;
using Application.Command.Departments;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler.AssetCatalogMasters
{
    public class CreateAssetCatalogMasterHandler : IRequestHandler<CreateAssetCatalogMasterCommand, AssetCatalogMasterResponse>
    {
        private readonly IAssetCatalogMasterCommandRepository _commandRepository;
        private readonly IAssetCatalogMasterQueryRepository _queryRepository;
        public CreateAssetCatalogMasterHandler(IAssetCatalogMasterCommandRepository commandRepository, IAssetCatalogMasterQueryRepository queryRepository)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
        }
        public async Task<AssetCatalogMasterResponse> Handle(CreateAssetCatalogMasterCommand request, CancellationToken cancellationToken)
        {

            var queryEntity = RoleMapper.Mapper.Map<AssetCatalogMaster>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }
            queryEntity.AssetCatalogNo = await _queryRepository.GenerateAssetCatalogNo();
            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new AssetCatalogMasterResponse
            {
                AssetCatalogMasterId = (long)data,
                StatusCodeId = queryEntity.StatusCodeId,
                AssetDescription = queryEntity.AssetDescription,
                AssetCatalogNo = queryEntity.AssetCatalogNo,
            };
            return response;
        }
    }
    public class EditAssetCatalogMasterHandler : IRequestHandler<EditAssetCatalogMasterCommand, AssetCatalogMasterResponse>
    {
        private readonly IAssetCatalogMasterCommandRepository _commandRepository;
        private readonly IAssetCatalogMasterQueryRepository _queryRepository;
        public EditAssetCatalogMasterHandler(IAssetCatalogMasterCommandRepository customerRepository, IAssetCatalogMasterQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<AssetCatalogMasterResponse> Handle(EditAssetCatalogMasterCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<AssetCatalogMaster>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                if (string.IsNullOrEmpty(queryrEntity.AssetCatalogNo))
                {
                    queryrEntity.AssetCatalogNo = await _queryRepository.GenerateAssetCatalogNo();
                }
                await _commandRepository.UpdateAsync(queryrEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new AssetCatalogMasterResponse
            {
                AssetCatalogMasterId = queryrEntity.AssetCatalogMasterId,
                CompanyId = queryrEntity.CompanyId,
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
                AssetDescription = queryrEntity.AssetDescription,
                AssetCatalogNo = queryrEntity.AssetCatalogNo,
            };

            return response;
        }
    }
    public class DeleteAssetCatalogMasterHandler : IRequestHandler<DeleteAssetCatalogMasterCommand, String>
    {
        private readonly IAssetCatalogMasterCommandRepository _commandRepository;
        private readonly IAssetCatalogMasterQueryRepository _queryRepository;
        public DeleteAssetCatalogMasterHandler(IAssetCatalogMasterCommandRepository customerRepository, IAssetCatalogMasterQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteAssetCatalogMasterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new AssetCatalogMaster
                {
                    AssetCatalogMasterId = request.Id,
                    CompanyId = queryEntity.CompanyId,
                    AssetDescription = queryEntity.AssetDescription,
                    AddedByUserId = queryEntity.AddedByUserId,
                    StatusCodeId = queryEntity.StatusCodeId,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "AssetCatalogMaster has been deleted!";
        }
    }
}
