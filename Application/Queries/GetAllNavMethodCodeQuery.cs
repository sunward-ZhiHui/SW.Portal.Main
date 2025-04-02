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
    public class GetAllNavMethodCodeQuery : PagedRequest, IRequest<List<NavMethodCodeModel>>
    {
        public string? SearchString { get; set; }
    }

    public class InsertOrUpdateNavMethodCode : NavMethodCodeModel, IRequest<NavMethodCodeModel>
    {

    }
    public class DeleteNavMethodCode : NavMethodCodeModel, IRequest<NavMethodCodeModel>
    {
        public NavMethodCodeModel NavMethodCodeModel { get; private set; }
        public DeleteNavMethodCode(NavMethodCodeModel navMethodCodeModel)
        {
            this.NavMethodCodeModel = navMethodCodeModel;
        }
    }
    public class GetAllNAVINPCategory : PagedRequest, IRequest<List<NAVINPCategoryModel>>
    {
        public string? SearchString { get; set; }
    }
    public class GetAllNavMethodCodeLineQuery : PagedRequest, IRequest<List<NavMethodCodeLines>>
    {
        public string? SearchString { get; set; }
        public long? MethodCodeId { get; set; }
        public GetAllNavMethodCodeLineQuery(long? methodCodeId)
        {
            this.MethodCodeId = methodCodeId;
        }
    }

    public class InsertOrUpdateNavMethodCodeLine : NavMethodCodeLines, IRequest<NavMethodCodeLines>
    {

    }
    public class DeleteNavMethodCodeLines : NavMethodCodeModel, IRequest<NavMethodCodeLines>
    {
        public NavMethodCodeLines NavMethodCodeLines { get; private set; }
        public DeleteNavMethodCodeLines(NavMethodCodeLines navMethodCodeModel)
        {
            this.NavMethodCodeLines = navMethodCodeModel;
        }
    }

    public class GetAllProductionForecastQuery : PagedRequest, IRequest<List<ProductionForecastModel>>
    {
        public string? SearchString { get; set; }
        public long? MethodCodeId { get; set; }
        public GetAllProductionForecastQuery(long? methodCodeId)
        {
            this.MethodCodeId = methodCodeId;
        }
    }

    public class InsertOrUpdateProductionForecast : ProductionForecastModel, IRequest<ProductionForecastModel>
    {

    }
    public class DeleteProductionForecast : NavMethodCodeModel, IRequest<ProductionForecastModel>
    {
        public ProductionForecastModel ProductionForecastModel { get; private set; }
        public DeleteProductionForecast(ProductionForecastModel navMethodCodeModel)
        {
            this.ProductionForecastModel = navMethodCodeModel;
        }
    }
}
