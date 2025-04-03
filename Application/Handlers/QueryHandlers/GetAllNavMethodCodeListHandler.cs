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
    public class GetAllNavMethodCodeListHandler : IRequestHandler<GetAllNavMethodCodeQuery, List<NavMethodCodeModel>>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public GetAllNavMethodCodeListHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<NavMethodCodeModel>> Handle(GetAllNavMethodCodeQuery request, CancellationToken cancellationToken)
        {
            return (List<NavMethodCodeModel>)await _queryRepository.GetNavMethodCodeAsync();
        }
    }
    public class GetAllNAVINPCategoryHandler : IRequestHandler<GetAllNAVINPCategory, List<NAVINPCategoryModel>>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public GetAllNAVINPCategoryHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<NAVINPCategoryModel>> Handle(GetAllNAVINPCategory request, CancellationToken cancellationToken)
        {
            return (List<NAVINPCategoryModel>)await _queryRepository.GetNAVINPCategorysync();
        }
    }
    public class InsertOrUpdateNavMethodCodeHandler : IRequestHandler<InsertOrUpdateNavMethodCode, NavMethodCodeModel>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public InsertOrUpdateNavMethodCodeHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<NavMethodCodeModel> Handle(InsertOrUpdateNavMethodCode request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateNavMethodCode(request);

        }
    }

    public class DeleteNavMethodCodeHandler : IRequestHandler<DeleteNavMethodCode, NavMethodCodeModel>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public DeleteNavMethodCodeHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<NavMethodCodeModel> Handle(DeleteNavMethodCode request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteNavMethodCode(request.NavMethodCodeModel);
        }

    }
    public class GetAllNavMethodCodeLinesListHandler : IRequestHandler<GetAllNavMethodCodeLineQuery, List<NavMethodCodeLines>>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public GetAllNavMethodCodeLinesListHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<NavMethodCodeLines>> Handle(GetAllNavMethodCodeLineQuery request, CancellationToken cancellationToken)
        {
            return (List<NavMethodCodeLines>)await _queryRepository.GetNavMethodCodeLinesById(request.MethodCodeId);
        }
    }
    public class InsertOrUpdateNavMethodCodeLineHandler : IRequestHandler<InsertOrUpdateNavMethodCodeLine, NavMethodCodeLines>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public InsertOrUpdateNavMethodCodeLineHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<NavMethodCodeLines> Handle(InsertOrUpdateNavMethodCodeLine request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateNavMethodCodeLines(request);

        }
    }
    public class DeleteNavMethodCodeLinesHandler : IRequestHandler<DeleteNavMethodCodeLines, NavMethodCodeLines>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public DeleteNavMethodCodeLinesHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<NavMethodCodeLines> Handle(DeleteNavMethodCodeLines request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteNavMethodCodeLines(request.NavMethodCodeLines);
        }

    }


    public class GetAllProductionForecastHandler : IRequestHandler<GetAllProductionForecastQuery, List<ProductionForecastModel>>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public GetAllProductionForecastHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ProductionForecastModel>> Handle(GetAllProductionForecastQuery request, CancellationToken cancellationToken)
        {
            return (List<ProductionForecastModel>)await _queryRepository.GetProductionForecastById(request.MethodCodeId);
        }
    }
    public class InsertOrUpdateInsertOrUpdateProductionForecastHandler : IRequestHandler<InsertOrUpdateProductionForecast, ProductionForecastModel>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public InsertOrUpdateInsertOrUpdateProductionForecastHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<ProductionForecastModel> Handle(InsertOrUpdateProductionForecast request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateProductionForecast(request);

        }
    }
    public class DeleteProductionForecastHandler : IRequestHandler<DeleteProductionForecast, ProductionForecastModel>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public DeleteProductionForecastHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ProductionForecastModel> Handle(DeleteProductionForecast request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteProductionForecast(request.ProductionForecastModel);
        }

    }
}
