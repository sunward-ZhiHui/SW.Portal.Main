using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllProductGroupingHandler : IRequestHandler<GetAllProductGroupingQuery, List<GenericCodesModel>>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public GetAllProductGroupingHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<GenericCodesModel>> Handle(GetAllProductGroupingQuery request, CancellationToken cancellationToken)
        {
            return (List<GenericCodesModel>)await _queryRepository.GetAllByAsync();
        }
    }
    public class GetCompanyListingListHandler : IRequestHandler<GetCompanyListingListQuery, List<CompanyListingModel>>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public GetCompanyListingListHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<CompanyListingModel>> Handle(GetCompanyListingListQuery request, CancellationToken cancellationToken)
        {
            return (List<CompanyListingModel>)await _queryRepository.GetCompanyListingList();
        }
    }
    public class GetCompanyListingForProductGroupingManufactureHandler : IRequestHandler<GetCompanyListingForProductGroupingManufacture, List<CompanyListingModel>>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public GetCompanyListingForProductGroupingManufactureHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<CompanyListingModel>> Handle(GetCompanyListingForProductGroupingManufacture request, CancellationToken cancellationToken)
        {
            return (List<CompanyListingModel>)await _queryRepository.GetCompanyListingForProductGroupingManufacture();
        }
    }
    public class InsertOrUpdateProductGroupingHandler : IRequestHandler<InsertOrUpdateProductGrouping, GenericCodesModel>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public InsertOrUpdateProductGroupingHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<GenericCodesModel> Handle(InsertOrUpdateProductGrouping request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateProductGrouping(request);

        }
    }

    public class DeleteProductGroupingHandler : IRequestHandler<DeleteProductGrouping, GenericCodesModel>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public DeleteProductGroupingHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<GenericCodesModel> Handle(DeleteProductGrouping request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteProductGrouping(request.GenericCodesModel);
        }

    }


    public class GetProductGroupingManufactureListHandler : IRequestHandler<GetProductGroupingManufactureList, List<ProductGroupingManufactureModel>>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public GetProductGroupingManufactureListHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ProductGroupingManufactureModel>> Handle(GetProductGroupingManufactureList request, CancellationToken cancellationToken)
        {
            return (List<ProductGroupingManufactureModel>)await _queryRepository.GetProductGroupingManufactureList(request.ProductGroupingId);
        }
    }
    public class InsertOrUpdateProductGroupingManufactureHandler : IRequestHandler<InsertOrUpdateProductGroupingManufacture, ProductGroupingManufactureModel>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public InsertOrUpdateProductGroupingManufactureHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<ProductGroupingManufactureModel> Handle(InsertOrUpdateProductGroupingManufacture request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateProductGroupingManufacture(request);

        }
    }

    public class DeleteProductGroupingManufactureHandler : IRequestHandler<DeleteProductGroupingManufacture, ProductGroupingManufactureModel>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public DeleteProductGroupingManufactureHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ProductGroupingManufactureModel> Handle(DeleteProductGroupingManufacture request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteProductGroupingManufacture(request.ProductGroupingManufactureModel);
        }

    }
    public class GetProductGroupingNavHandler : IRequestHandler<GetProductGroupingNavList, List<ProductGroupingNavModel>>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public GetProductGroupingNavHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ProductGroupingNavModel>> Handle(GetProductGroupingNavList request, CancellationToken cancellationToken)
        {
            return (List<ProductGroupingNavModel>)await _queryRepository.GetProductGroupingNavList(request.ProductGroupingManufactureId);
        }
    }
    public class InsertOrUpdateProductGroupingNavHandler : IRequestHandler<InsertOrUpdateProductGroupingNav, ProductGroupingNavModel>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public InsertOrUpdateProductGroupingNavHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<ProductGroupingNavModel> Handle(InsertOrUpdateProductGroupingNav request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateProductGroupingNav(request);

        }
    }

    public class DeleteProductGroupingNavHandler : IRequestHandler<DeleteProductGroupingNav, ProductGroupingNavModel>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public DeleteProductGroupingNavHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ProductGroupingNavModel> Handle(DeleteProductGroupingNav request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteProductGroupingNav(request.ProductGroupingNavModel);
        }

    }


    public class GetProductGroupingNavDifferenceListHandler : IRequestHandler<GetProductGroupingNavDifferenceList, List<ProductGroupingNavDifferenceModel>>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public GetProductGroupingNavDifferenceListHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ProductGroupingNavDifferenceModel>> Handle(GetProductGroupingNavDifferenceList request, CancellationToken cancellationToken)
        {
            return (List<ProductGroupingNavDifferenceModel>)await _queryRepository.GetProductGroupingNavDifferenceList(request.ProductGroupingNavId);
        }
    }
    public class InsertOrUpdateProductGroupingNavDifferenceHandler : IRequestHandler<InsertOrUpdateProductGroupingNavDifference, ProductGroupingNavDifferenceModel>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public InsertOrUpdateProductGroupingNavDifferenceHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<ProductGroupingNavDifferenceModel> Handle(InsertOrUpdateProductGroupingNavDifference request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateProductGroupingNavDifference(request);

        }
    }

    public class DeleteProductGroupingNavDifferenceHandler : IRequestHandler<DeleteProductGroupingNavDifference, ProductGroupingNavDifferenceModel>
    {
        private readonly IProductGroupingListQueryRepository _queryRepository;
        public DeleteProductGroupingNavDifferenceHandler(IProductGroupingListQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ProductGroupingNavDifferenceModel> Handle(DeleteProductGroupingNavDifference request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteProductGroupingNavDifference(request.ProductGroupingNavDifferenceModel);
        }

    }
}
