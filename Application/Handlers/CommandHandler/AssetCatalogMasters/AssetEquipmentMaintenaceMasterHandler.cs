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
    public class CreateAssetEquipmentMaintenaceMasterHandler : IRequestHandler<CreateAssetEquipmentMaintenaceMasterCommand, AssetEquipmentMaintenaceMasterResponse>
    {
        private readonly IAssetEquipmentMaintenaceMasterCommandRepository _commandRepository;
        private readonly IAssetEquipmentMaintenaceMasterAssetDocumentCommandRepository _assetEquipmentMaintenaceMasterAssetDocumentCommandRepository;
        public CreateAssetEquipmentMaintenaceMasterHandler(IAssetEquipmentMaintenaceMasterCommandRepository commandRepository, IAssetEquipmentMaintenaceMasterAssetDocumentCommandRepository assetEquipmentMaintenaceMasterAssetDocumentCommandRepository)
        {
            _commandRepository = commandRepository;
            _assetEquipmentMaintenaceMasterAssetDocumentCommandRepository = assetEquipmentMaintenaceMasterAssetDocumentCommandRepository;
        }
        public async Task<AssetEquipmentMaintenaceMasterResponse> Handle(CreateAssetEquipmentMaintenaceMasterCommand request, CancellationToken cancellationToken)
        {
            var assetEquipmentMaintenaceMaster = new AssetEquipmentMaintenaceMaster
            {
                AssetPartsMaintenaceMasterId = request.AssetPartsMaintenaceMasterId,
                IsCalibarion = request.IsCalibarion,
                PreventiveMaintenaceId = request.PreventiveMaintenaceId,
                StatusCodeId = request.StatusCodeId,
                AddedByUserId = request.AddedByUserId,
                ModifiedByUserId = request.AddedByUserId,
                AddedDate = request.AddedDate,
                ModifiedDate = request.ModifiedDate,
            };
            var queryEntity = RoleMapper.Mapper.Map<AssetEquipmentMaintenaceMaster>(assetEquipmentMaintenaceMaster);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            request.AssetEquipmentMaintenaceMasterId = data.Value;
            if (request.AssetDocumentationIds != null)
            {
                var listData = request.AssetDocumentationIds.ToList();
                if (listData.Count > 0)
                {
                    request.AssetDocumentationIds.ToList().ForEach(a =>
                    {
                        var employeeReportTo = new AssetEquipmentMaintenaceMasterAssetDocument
                        {
                            AssetEquipmentMaintenaceMasterId = data,
                            AssetDocumentId = a
                        };
                        _assetEquipmentMaintenaceMasterAssetDocumentCommandRepository.AddAsync(employeeReportTo);
                    });
                }
            }
            var response = new AssetEquipmentMaintenaceMasterResponse
            {
                AssetEquipmentMaintenaceMasterId = (long)data,
                AssetPartsMaintenaceMasterId = queryEntity.AssetPartsMaintenaceMasterId,
                StatusCodeId = queryEntity.StatusCodeId,
            };
            return response;
        }
    }
    public class EditAssetEquipmentMaintenaceMasterHandler : IRequestHandler<EditAssetEquipmentMaintenaceMasterCommand, AssetEquipmentMaintenaceMasterResponse>
    {
        private readonly IAssetEquipmentMaintenaceMasterCommandRepository _commandRepository;
        private readonly IAssetEquipmentMaintenaceMasterQueryRepository _queryRepository;
        private readonly IAssetEquipmentMaintenaceMasterAssetDocumentCommandRepository _assetEquipmentMaintenaceMasterAssetDocumentCommandRepository;
        public EditAssetEquipmentMaintenaceMasterHandler(IAssetEquipmentMaintenaceMasterCommandRepository customerRepository, IAssetEquipmentMaintenaceMasterQueryRepository customerQueryRepository, IAssetEquipmentMaintenaceMasterAssetDocumentCommandRepository assetEquipmentMaintenaceMasterAssetDocumentCommandRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _assetEquipmentMaintenaceMasterAssetDocumentCommandRepository = assetEquipmentMaintenaceMasterAssetDocumentCommandRepository;
        }
        public async Task<AssetEquipmentMaintenaceMasterResponse> Handle(EditAssetEquipmentMaintenaceMasterCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<AssetEquipmentMaintenaceMaster>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryEntity);
                await _queryRepository.DeleteAssetEquipmentMaintenaceMasterAssetDocument(request.AssetEquipmentMaintenaceMasterId);
                if (request.AssetDocumentationIds != null)
                {
                    var listData = request.AssetDocumentationIds.ToList();
                    if (listData.Count > 0)
                    {
                        request.AssetDocumentationIds.ToList().ForEach(a =>
                        {
                            var employeeReportTo = new AssetEquipmentMaintenaceMasterAssetDocument();
                            employeeReportTo.AssetEquipmentMaintenaceMasterId = request.AssetEquipmentMaintenaceMasterId;
                            employeeReportTo.AssetDocumentId = a;
                            _assetEquipmentMaintenaceMasterAssetDocumentCommandRepository.AddAsync(employeeReportTo);
                        });
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new AssetEquipmentMaintenaceMasterResponse
            {
                AssetPartsMaintenaceMasterId = queryEntity.AssetPartsMaintenaceMasterId,
                AssetEquipmentMaintenaceMasterId = queryEntity.AssetEquipmentMaintenaceMasterId,
                StatusCodeId = queryEntity.StatusCodeId,
            };

            return response;
        }
    }
    public class DeleteAssetEquipmentMaintenaceMasterHandler : IRequestHandler<DeleteAssetEquipmentMaintenaceMasterCommand, String>
    {
        private readonly IAssetEquipmentMaintenaceMasterCommandRepository _commandRepository;
        private readonly IAssetEquipmentMaintenaceMasterQueryRepository _queryRepository;
        public DeleteAssetEquipmentMaintenaceMasterHandler(IAssetEquipmentMaintenaceMasterCommandRepository customerRepository, IAssetEquipmentMaintenaceMasterQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteAssetEquipmentMaintenaceMasterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new AssetEquipmentMaintenaceMaster
                {
                    AssetEquipmentMaintenaceMasterId = request.Id,
                    AssetPartsMaintenaceMasterId = queryEntity.AssetPartsMaintenaceMasterId,
                    AddedByUserId = queryEntity.AddedByUserId,
                    StatusCodeId = queryEntity.StatusCodeId,
                };
                await _queryRepository.DeleteAssetEquipmentMaintenaceMasterAssetDocument(request.Id);
                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "AssetEquipmentMaintenaceMaster has been deleted!";
        }
    }
}
