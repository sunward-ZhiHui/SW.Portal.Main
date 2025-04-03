using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllProductGroupingQuery : PagedRequest, IRequest<List<GenericCodesModel>>
    {
        public string? SearchString { get; set; }
    }
    public class InsertOrUpdateProductGrouping : GenericCodesModel, IRequest<GenericCodesModel>
    {

    }
    public class GetCompanyListingListQuery : PagedRequest, IRequest<List<CompanyListingModel>>
    {
        public string? SearchString { get; set; }
    }
    public class GetCompanyListingForProductGroupingManufacture : PagedRequest, IRequest<List<CompanyListingModel>>
    {
        public string? SearchString { get; set; }
    }

    public class DeleteProductGrouping : ACEntryModel, IRequest<GenericCodesModel>
    {
        public GenericCodesModel GenericCodesModel { get; private set; }
        public DeleteProductGrouping(GenericCodesModel aCEntryModel)
        {
            this.GenericCodesModel = aCEntryModel;
        }
    }

    public class InsertOrUpdateProductGroupingManufacture : ProductGroupingManufactureModel, IRequest<ProductGroupingManufactureModel>
    {

    }
    public class GetProductGroupingManufactureList : PagedRequest, IRequest<List<ProductGroupingManufactureModel>>
    {
        public long? ProductGroupingId { get; set; }
        public GetProductGroupingManufactureList(long? productGroupingId)
        {
            this.ProductGroupingId = productGroupingId;
        }
    }

    public class DeleteProductGroupingManufacture : ACEntryModel, IRequest<ProductGroupingManufactureModel>
    {
        public ProductGroupingManufactureModel ProductGroupingManufactureModel { get; private set; }
        public DeleteProductGroupingManufacture(ProductGroupingManufactureModel aCEntryModel)
        {
            this.ProductGroupingManufactureModel = aCEntryModel;
        }
    }


    public class InsertOrUpdateProductGroupingNav : ProductGroupingNavModel, IRequest<ProductGroupingNavModel>
    {

    }
    public class GetProductGroupingNavList : PagedRequest, IRequest<List<ProductGroupingNavModel>>
    {
        public long? ProductGroupingManufactureId { get; set; }
        public GetProductGroupingNavList(long? productGroupingManufactureId)
        {
            this.ProductGroupingManufactureId = productGroupingManufactureId;
        }
    }

    public class DeleteProductGroupingNav : ACEntryModel, IRequest<ProductGroupingNavModel>
    {
        public ProductGroupingNavModel ProductGroupingNavModel { get; private set; }
        public DeleteProductGroupingNav(ProductGroupingNavModel aCEntryModel)
        {
            this.ProductGroupingNavModel = aCEntryModel;
        }
    }

    public class InsertOrUpdateProductGroupingNavDifference : ProductGroupingNavDifferenceModel, IRequest<ProductGroupingNavDifferenceModel>
    {

    }
    public class GetProductGroupingNavDifferenceList : PagedRequest, IRequest<List<ProductGroupingNavDifferenceModel>>
    {
        public long? ProductGroupingNavId { get; set; }
        public GetProductGroupingNavDifferenceList(long? productGroupingNavId)
        {
            this.ProductGroupingNavId = productGroupingNavId;
        }
    }

    public class DeleteProductGroupingNavDifference : ACEntryModel, IRequest<ProductGroupingNavDifferenceModel>
    {
        public ProductGroupingNavDifferenceModel ProductGroupingNavDifferenceModel { get; private set; }
        public DeleteProductGroupingNavDifference(ProductGroupingNavDifferenceModel aCEntryModel)
        {
            this.ProductGroupingNavDifferenceModel = aCEntryModel;
        }
    }
}
