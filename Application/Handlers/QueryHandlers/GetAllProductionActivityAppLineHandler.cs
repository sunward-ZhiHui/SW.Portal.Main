using Application.Queries;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllProductionActivityAppLineHandler : IRequestHandler<GetAllProductionActivityAppLineQuery, List<ProductActivityAppModel>>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public GetAllProductionActivityAppLineHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<ProductActivityAppModel>> Handle(GetAllProductionActivityAppLineQuery request, CancellationToken cancellationToken)
        {
            return (List<ProductActivityAppModel>)await _productionactivityQueryRepository.GetAllAsync(request.CompanyID,request.ProdorderNo,request.UserId,request.LocationID);
        }

    }
   
    public class GetAllProductionActivitylocHandler : IRequestHandler<GetAllProductionActivitylocQuery, List<ProductionActivityApp>>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public GetAllProductionActivitylocHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<ProductionActivityApp>> Handle(GetAllProductionActivitylocQuery request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityApp>)await _productionactivityQueryRepository.GetAlllocAsync(request.ProductionActivityAppID);
        }

    }
    public class GetProductActivityAppLineOneItemHandler : IRequestHandler<GetProductActivityAppLineOneItem, ProductActivityAppModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public GetProductActivityAppLineOneItemHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductActivityAppModel> Handle(GetProductActivityAppLineOneItem request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.GetProductActivityAppLineOneItem(request.Id);
        }
    }
    public class GetUpdateproductActivityAppLineCommentFieldHandler : IRequestHandler<GetUpdateproductActivityAppLineCommentField, ProductActivityAppModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public GetUpdateproductActivityAppLineCommentFieldHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductActivityAppModel> Handle(GetUpdateproductActivityAppLineCommentField request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.UpdateproductActivityAppLineCommentField(request.ProductActivityAppModel);
        }
    }
    public class DeleteproductActivityAppLineHandler : IRequestHandler<DeleteproductActivityAppLine, ProductActivityAppModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public DeleteproductActivityAppLineHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductActivityAppModel> Handle(DeleteproductActivityAppLine request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.DeleteproductActivityAppLine(request.ProductActivityAppModel);
        }
    }
    public class DeleteSupportingDocumentsHandler : IRequestHandler<DeleteSupportingDocuments, DocumentsModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public DeleteSupportingDocumentsHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<DocumentsModel> Handle(DeleteSupportingDocuments request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.DeleteSupportingDocuments(request.DocumentsModel);
        }
    }
    public class GetProductionActivityNonComplianceHandler : IRequestHandler<GetProductionActivityNonCompliance, ProductionActivityNonComplianceModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public GetProductionActivityNonComplianceHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityNonComplianceModel> Handle(GetProductionActivityNonCompliance request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.GetProductionActivityNonComplianceAsync(request.Type,request.Id,request.ActionType);
        }
    }
    public class InsertProductionActivityNonComplianceHandler : IRequestHandler<InsertProductionActivityNonCompliance, ProductionActivityNonComplianceModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public InsertProductionActivityNonComplianceHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityNonComplianceModel> Handle(InsertProductionActivityNonCompliance request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.InsertProductionActivityNonCompliance(request.ProductionActivityNonComplianceModel);
        }
    }
    public class DeleteProductionActivityNonComplianceHandler : IRequestHandler<DeleteProductionActivityNonCompliance, ProductionActivityNonComplianceUserModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public DeleteProductionActivityNonComplianceHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityNonComplianceUserModel> Handle(DeleteProductionActivityNonCompliance request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.DeleteProductionActivityNonCompliance(request.ProductionActivityNonComplianceUserModel);
        }
    }
}