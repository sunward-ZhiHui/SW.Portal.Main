using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IProductGroupingListQueryRepository
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<GenericCodesModel>> GetAllByAsync();
        Task<GenericCodesModel> InsertOrUpdateProductGrouping(GenericCodesModel value);
        Task<GenericCodesModel> DeleteProductGrouping(GenericCodesModel value);
        Task<IReadOnlyList<CompanyListingModel>> GetCompanyListingList();
        Task<IReadOnlyList<CompanyListingModel>> GetCompanyListingForProductGroupingManufacture();
        Task<IReadOnlyList<ProductGroupingManufactureModel>> GetProductGroupingManufactureList(long? ProductGroupingId);
        Task<ProductGroupingManufactureModel> InsertOrUpdateProductGroupingManufacture(ProductGroupingManufactureModel value);
        Task<ProductGroupingManufactureModel> DeleteProductGroupingManufacture(ProductGroupingManufactureModel value);

        Task<IReadOnlyList<ProductGroupingNavModel>> GetProductGroupingNavList(long? ProductGroupingManufactureId);
        Task<ProductGroupingNavModel> InsertOrUpdateProductGroupingNav(ProductGroupingNavModel value);
        Task<ProductGroupingNavModel> DeleteProductGroupingNav(ProductGroupingNavModel value);
        Task<IReadOnlyList<ProductGroupingNavDifferenceModel>> GetProductGroupingNavDifferenceList(long? ProductGroupingNavId);
        Task<ProductGroupingNavDifferenceModel> InsertOrUpdateProductGroupingNavDifference(ProductGroupingNavDifferenceModel value);
        Task<ProductGroupingNavDifferenceModel> DeleteProductGroupingNavDifference(ProductGroupingNavDifferenceModel value);
    }
}
