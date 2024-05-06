using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
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
            return (List<ProductActivityAppModel>)await _productionactivityQueryRepository.GetAllAsync(request.ProductionActivityModel);
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
            return await _productionactivityQueryRepository.GetProductionActivityNonComplianceAsync(request.Type, request.Id, request.ActionType);
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
    public class UpdateActivityMasterHandler : IRequestHandler<UpdateActivityMaster, ProductActivityAppModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public UpdateActivityMasterHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductActivityAppModel> Handle(UpdateActivityMaster request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.UpdateActivityMaster(request.ProductActivityAppModel);
        }
    }
    public class UpdateActivityStatusHandler : IRequestHandler<UpdateActivityStatus, ProductActivityAppModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public UpdateActivityStatusHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductActivityAppModel> Handle(UpdateActivityStatus request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.UpdateActivityStatus(request.ProductActivityAppModel);
        }
    }
    public class GetProductActivityEmailActivitySubjectsHandler : IRequestHandler<GetProductActivityEmailActivitySubjects, List<view_ActivityEmailSubjects>>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public GetProductActivityEmailActivitySubjectsHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<view_ActivityEmailSubjects>> Handle(GetProductActivityEmailActivitySubjects request, CancellationToken cancellationToken)
        {
            return (List<view_ActivityEmailSubjects>)await _productionactivityQueryRepository.GetProductActivityEmailActivitySubjects(request.ActivityMasterId, request.ActivityType, request.UserId);
        }

    }
    public class UpdateActivityCheckerHandler : IRequestHandler<UpdateActivityChecker, ProductActivityAppModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public UpdateActivityCheckerHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductActivityAppModel> Handle(UpdateActivityChecker request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.UpdateActivityChecker(request.ProductActivityAppModel);
        }
    }
    public class InsertProductionActivityCheckedDetailsHandler : IRequestHandler<InsertProductionActivityCheckedDetails, ProductionActivityCheckedDetailsModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public InsertProductionActivityCheckedDetailsHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityCheckedDetailsModel> Handle(InsertProductionActivityCheckedDetails request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.InsertProductionActivityCheckedDetails(request.ProductionActivityCheckedDetailsModel);
        }
    }
    public class DeleteProductionActivityCheckedDetailsHandler : IRequestHandler<DeleteProductionActivityCheckedDetails, ProductionActivityCheckedDetailsModel>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public DeleteProductionActivityCheckedDetailsHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityCheckedDetailsModel> Handle(DeleteProductionActivityCheckedDetails request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.DeleteProductionActivityCheckedDetails(request.ProductionActivityCheckedDetailsModel);
        }
    }

    public class ProductionActivityCheckedDetailsModelHandler : IRequestHandler<GetProductionActivityCheckedDetails, List<ProductionActivityCheckedDetailsModel>>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public ProductionActivityCheckedDetailsModelHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<ProductionActivityCheckedDetailsModel>> Handle(GetProductionActivityCheckedDetails request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityCheckedDetailsModel>)await _productionactivityQueryRepository.GetProductionActivityCheckedDetails(request.ProductionActivityCheckedDetailsModel);
        }

    }

   

    public class GetAllProductionActivityRoutineAppLineHandler : IRequestHandler<GetAllProductionActivityRoutineAppLineQuery, List<ProductionActivityRoutineAppModel>>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public GetAllProductionActivityRoutineAppLineHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<ProductionActivityRoutineAppModel>> Handle(GetAllProductionActivityRoutineAppLineQuery request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityRoutineAppModel>)await _productionactivityQueryRepository.GetAllProductionActivityRoutineAsync(request.ProductionActivityRoutineAppModel);
        }

    }
    public class DeleteproductActivityRoutineAppLineHandler : IRequestHandler<DeleteproductActivityRoutineAppLine, ProductionActivityRoutineAppModel>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public DeleteproductActivityRoutineAppLineHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityRoutineAppModel> Handle(DeleteproductActivityRoutineAppLine request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.DeleteproductActivityRoutineAppLine(request.ProductionActivityRoutineAppModel);
        }
    }
    public class GetUpdateproductActivityRoutineAppLineCommentFieldHandler : IRequestHandler<GetUpdateproductActivityRoutineAppLineCommentField, ProductionActivityRoutineAppModel>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public GetUpdateproductActivityRoutineAppLineCommentFieldHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityRoutineAppModel> Handle(GetUpdateproductActivityRoutineAppLineCommentField request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.UpdateproductActivityRoutineAppLineCommentField(request.ProductionActivityRoutineAppModel);
        }
    }
    public class UpdateActivityRoutineMasterHandler : IRequestHandler<UpdateActivityRoutineMaster, ProductionActivityRoutineAppModel>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public UpdateActivityRoutineMasterHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityRoutineAppModel> Handle(UpdateActivityRoutineMaster request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.UpdateActivityRoutineMaster(request.ProductionActivityRoutineAppModel);
        }
    }
    public class UpdateActivityRoutineStatusHandler : IRequestHandler<UpdateActivityRoutineStatus, ProductionActivityRoutineAppModel>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public UpdateActivityRoutineStatusHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityRoutineAppModel> Handle(UpdateActivityRoutineStatus request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.UpdateActivityRoutineStatus(request.ProductionActivityRoutineAppModel);
        }
    }
    public class GetProductActivityRoutineAppLineOneItemHandler : IRequestHandler<GetProductActivityRoutineAppLineOneItem, ProductionActivityRoutineAppModel>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public GetProductActivityRoutineAppLineOneItemHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityRoutineAppModel> Handle(GetProductActivityRoutineAppLineOneItem request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.GetProductActivityRoutineAppLineOneItem(request.Id);
        }
    }
    public class UpdateRoutineCheckerHandler : IRequestHandler<UpdateRoutineChecker, ProductionActivityRoutineAppModel>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public UpdateRoutineCheckerHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityRoutineAppModel> Handle(UpdateRoutineChecker request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.UpdateRoutineChecker(request.ProductionActivityRoutineAppModel);
        }
    }




    public class InsertProductionRoutineCheckedDetailsHandler : IRequestHandler<InsertProductionRoutineCheckedDetails, ProductionActivityRoutineCheckedDetailsModel>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public InsertProductionRoutineCheckedDetailsHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityRoutineCheckedDetailsModel> Handle(InsertProductionRoutineCheckedDetails request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.InsertProductionActivityRoutineCheckedDetails(request.ProductionActivityRoutineCheckedDetailsModel);
        }
    }
    public class DeleteProductionRoutineCheckedDetailsHandler : IRequestHandler<DeleteProductionRoutineCheckedDetails, ProductionActivityRoutineCheckedDetailsModel>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public DeleteProductionRoutineCheckedDetailsHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ProductionActivityRoutineCheckedDetailsModel> Handle(DeleteProductionRoutineCheckedDetails request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.DeleteProductionActivityRoutineCheckedDetails(request.ProductionActivityRoutineCheckedDetailsModel);
        }
    }

    public class ProductionRoutineCheckedDetailsModelHandler : IRequestHandler<GetProductionRoutineCheckedDetails, List<ProductionActivityRoutineCheckedDetailsModel>>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public ProductionRoutineCheckedDetailsModelHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<ProductionActivityRoutineCheckedDetailsModel>> Handle(GetProductionRoutineCheckedDetails request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityRoutineCheckedDetailsModel>)await _productionactivityQueryRepository.GetProductionActivityRoutineCheckedDetails(request.ProductionActivityRoutineCheckedDetailsModel);
        }

    }
    public class ProductionActivityReportHandler : IRequestHandler<GetProductionActivityReportList, List<View_ProductionActivityReport>>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public ProductionActivityReportHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<View_ProductionActivityReport>> Handle(GetProductionActivityReportList request, CancellationToken cancellationToken)
        {
            return (List<View_ProductionActivityReport>)await _productionactivityQueryRepository.GetProductionActivityReportList();
        }

    }
    public class ProductionActivityReportDocumentHandler : IRequestHandler<GetProductionActivityReportDocumentList, List<imgDocList>>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
        public ProductionActivityReportDocumentHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<imgDocList>> Handle(GetProductionActivityReportDocumentList request, CancellationToken cancellationToken)
        {
            return (List<imgDocList>)await _productionactivityQueryRepository.GetProductionActivityReportDocList(request.ProductionActivityAppLineID);
        }

    }
    

}