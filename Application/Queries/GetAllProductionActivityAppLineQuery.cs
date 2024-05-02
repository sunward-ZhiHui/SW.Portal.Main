
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

    public class GetAllProductionActivityAppLineQuery : PagedRequest, IRequest<List<ProductActivityAppModel>>
    {
        public ProductActivityAppModel? ProductionActivityModel { get; set; }

        public GetAllProductionActivityAppLineQuery(ProductActivityAppModel productionActivityModel)
        {
            this.ProductionActivityModel = productionActivityModel;
        }
    }


    public class GetAllProductionActivitylocQuery : PagedRequest, IRequest<List<ProductionActivityApp>>
    {
        public long? ProductionActivityAppID { get; set; }
        public GetAllProductionActivitylocQuery(long? location)
        {
            this.ProductionActivityAppID = location;

        }
    }
    public class GetProductActivityAppLineOneItem : PagedRequest, IRequest<ProductActivityAppModel>
    {
        public long? Id { get; set; }
        public GetProductActivityAppLineOneItem(long? id)
        {
            this.Id = id;
        }
    }
    public class GetUpdateproductActivityAppLineCommentField : PagedRequest, IRequest<ProductActivityAppModel>
    {
        public ProductActivityAppModel ProductActivityAppModel { get; private set; }
        public GetUpdateproductActivityAppLineCommentField(ProductActivityAppModel productActivityAppModel)
        {
            this.ProductActivityAppModel = productActivityAppModel;
        }
    }
    public class DeleteproductActivityAppLine : PagedRequest, IRequest<ProductActivityAppModel>
    {
        public ProductActivityAppModel ProductActivityAppModel { get; private set; }
        public DeleteproductActivityAppLine(ProductActivityAppModel productActivityAppModel)
        {
            this.ProductActivityAppModel = productActivityAppModel;
        }
    }
    public class DeleteSupportingDocuments : PagedRequest, IRequest<DocumentsModel>
    {
        public DocumentsModel DocumentsModel { get; private set; }
        public DeleteSupportingDocuments(DocumentsModel documentsModel)
        {
            this.DocumentsModel = documentsModel;
        }
    }
    public class DeleteProductionActivityNonCompliance : PagedRequest, IRequest<ProductionActivityNonComplianceUserModel>
    {
        public ProductionActivityNonComplianceUserModel ProductionActivityNonComplianceUserModel { get; private set; }
        public DeleteProductionActivityNonCompliance(ProductionActivityNonComplianceUserModel productionActivityNonComplianceUserModel)
        {
            this.ProductionActivityNonComplianceUserModel = productionActivityNonComplianceUserModel;
        }
    }

    public class GetProductionActivityNonCompliance : PagedRequest, IRequest<ProductionActivityNonComplianceModel>
    {
        public long? Id { get; set; }
        public string? Type { get; set; }
        public string? ActionType { get; set; }
        public GetProductionActivityNonCompliance(string type, long? id, string? actionType)
        {
            this.Id = id;
            this.Type = type;
            this.ActionType = actionType;
        }
    }
    public class InsertProductionActivityNonCompliance : PagedRequest, IRequest<ProductionActivityNonComplianceModel>
    {
        public ProductionActivityNonComplianceModel ProductionActivityNonComplianceModel { get; private set; }
        public InsertProductionActivityNonCompliance(ProductionActivityNonComplianceModel productionActivityNonComplianceModel)
        {
            this.ProductionActivityNonComplianceModel = productionActivityNonComplianceModel;
        }
    }
    public class UpdateActivityMaster : PagedRequest, IRequest<ProductActivityAppModel>
    {
        public ProductActivityAppModel ProductActivityAppModel { get; private set; }
        public UpdateActivityMaster(ProductActivityAppModel productActivityAppModel)
        {
            this.ProductActivityAppModel = productActivityAppModel;

        }
    }
    public class UpdateActivityStatus : PagedRequest, IRequest<ProductActivityAppModel>
    {
        public ProductActivityAppModel ProductActivityAppModel { get; private set; }
        public UpdateActivityStatus(ProductActivityAppModel productActivityAppModel)
        {
            this.ProductActivityAppModel = productActivityAppModel;

        }
    }
    public class GetProductActivityEmailActivitySubjects : PagedRequest, IRequest<List<view_ActivityEmailSubjects>>
    {
        public long? ActivityMasterId { get; set; }
        public string? ActivityType { get; set; }
        public long? UserId { get; set; }
        public GetProductActivityEmailActivitySubjects(long? activityMasterId, string? activityType, long? userId)
        {
            this.ActivityMasterId = activityMasterId;
            this.ActivityType = activityType;
            this.UserId = userId;
        }
    }
    public class UpdateActivityChecker : PagedRequest, IRequest<ProductActivityAppModel>
    {
        public ProductActivityAppModel ProductActivityAppModel { get; private set; }
        public UpdateActivityChecker(ProductActivityAppModel productActivityAppModel)
        {
            this.ProductActivityAppModel = productActivityAppModel;

        }
    }
    public class InsertProductionActivityCheckedDetails : PagedRequest, IRequest<ProductionActivityCheckedDetailsModel>
    {
        public ProductionActivityCheckedDetailsModel ProductionActivityCheckedDetailsModel { get; private set; }
        public InsertProductionActivityCheckedDetails(ProductionActivityCheckedDetailsModel productionActivityCheckedDetailsModel)
        {
            this.ProductionActivityCheckedDetailsModel = productionActivityCheckedDetailsModel;
        }
    }
    public class DeleteProductionActivityCheckedDetails : PagedRequest, IRequest<ProductionActivityCheckedDetailsModel>
    {
        public ProductionActivityCheckedDetailsModel ProductionActivityCheckedDetailsModel { get; private set; }
        public DeleteProductionActivityCheckedDetails(ProductionActivityCheckedDetailsModel productionActivityCheckedDetailsModel)
        {
            this.ProductionActivityCheckedDetailsModel = productionActivityCheckedDetailsModel;
        }
    }
    public class GetProductionActivityCheckedDetails : PagedRequest, IRequest<List<ProductionActivityCheckedDetailsModel>>
    {
        public long? ProductionActivityCheckedDetailsModel { get; private set; }
        public GetProductionActivityCheckedDetails(long? productionActivityCheckedDetailsModel)
        {
            this.ProductionActivityCheckedDetailsModel = productionActivityCheckedDetailsModel;
        }
    }












    public class GetAllProductionActivityRoutineAppLineQuery : PagedRequest, IRequest<List<ProductionActivityRoutineAppModel>>
    {
        public ProductionActivityRoutineAppModel? ProductionActivityRoutineAppModel { get; set; }

        public GetAllProductionActivityRoutineAppLineQuery(ProductionActivityRoutineAppModel productionActivityRoutineAppModel)
        {
            this.ProductionActivityRoutineAppModel = productionActivityRoutineAppModel;
        }
    }
    public class DeleteproductActivityRoutineAppLine : PagedRequest, IRequest<ProductionActivityRoutineAppModel>
    {
        public ProductionActivityRoutineAppModel ProductionActivityRoutineAppModel { get; private set; }
        public DeleteproductActivityRoutineAppLine(ProductionActivityRoutineAppModel productionActivityRoutineAppModel)
        {
            this.ProductionActivityRoutineAppModel = productionActivityRoutineAppModel;
        }
    }
    public class GetUpdateproductActivityRoutineAppLineCommentField : PagedRequest, IRequest<ProductionActivityRoutineAppModel>
    {
        public ProductionActivityRoutineAppModel ProductionActivityRoutineAppModel { get; private set; }
        public GetUpdateproductActivityRoutineAppLineCommentField(ProductionActivityRoutineAppModel productionActivityRoutineAppModel)
        {
            this.ProductionActivityRoutineAppModel = productionActivityRoutineAppModel;
        }
    }
    public class UpdateActivityRoutineStatus : PagedRequest, IRequest<ProductionActivityRoutineAppModel>
    {
        public ProductionActivityRoutineAppModel ProductionActivityRoutineAppModel { get; private set; }
        public UpdateActivityRoutineStatus(ProductionActivityRoutineAppModel productionActivityRoutineAppModel)
        {
            this.ProductionActivityRoutineAppModel = productionActivityRoutineAppModel;

        }
    }
    public class UpdateActivityRoutineMaster : PagedRequest, IRequest<ProductionActivityRoutineAppModel>
    {
        public ProductionActivityRoutineAppModel ProductionActivityRoutineAppModel { get; private set; }
        public UpdateActivityRoutineMaster(ProductionActivityRoutineAppModel productionActivityRoutineAppModel)
        {
            this.ProductionActivityRoutineAppModel = productionActivityRoutineAppModel;

        }
    }
    public class GetProductActivityRoutineAppLineOneItem : PagedRequest, IRequest<ProductionActivityRoutineAppModel>
    {
        public long? Id { get; set; }
        public GetProductActivityRoutineAppLineOneItem(long? id)
        {
            this.Id = id;
        }
    }
    public class UpdateRoutineChecker : PagedRequest, IRequest<ProductionActivityRoutineAppModel>
    {
        public ProductionActivityRoutineAppModel ProductionActivityRoutineAppModel { get; private set; }
        public UpdateRoutineChecker(ProductionActivityRoutineAppModel productionActivityRoutineAppModel)
        {
            this.ProductionActivityRoutineAppModel = productionActivityRoutineAppModel;

        }
    }

    public class InsertProductionRoutineCheckedDetails : PagedRequest, IRequest<ProductionActivityRoutineCheckedDetailsModel>
    {
        public ProductionActivityRoutineCheckedDetailsModel ProductionActivityRoutineCheckedDetailsModel { get; private set; }
        public InsertProductionRoutineCheckedDetails(ProductionActivityRoutineCheckedDetailsModel productionActivityRoutineCheckedDetailsModel)
        {
            this.ProductionActivityRoutineCheckedDetailsModel = productionActivityRoutineCheckedDetailsModel;
        }
    }
    public class DeleteProductionRoutineCheckedDetails : PagedRequest, IRequest<ProductionActivityRoutineCheckedDetailsModel>
    {
        public ProductionActivityRoutineCheckedDetailsModel ProductionActivityRoutineCheckedDetailsModel { get; private set; }
        public DeleteProductionRoutineCheckedDetails(ProductionActivityRoutineCheckedDetailsModel productionActivityRoutineCheckedDetailsModel)
        {
            this.ProductionActivityRoutineCheckedDetailsModel = productionActivityRoutineCheckedDetailsModel;
        }
    }
    public class GetProductionRoutineCheckedDetails : PagedRequest, IRequest<List<ProductionActivityRoutineCheckedDetailsModel>>
    {
        public long? ProductionActivityRoutineCheckedDetailsModel { get; private set; }
        public GetProductionRoutineCheckedDetails(long? productionActivityRoutineCheckedDetailsModel)
        {
            this.ProductionActivityRoutineCheckedDetailsModel = productionActivityRoutineCheckedDetailsModel;
        }
    }

    public class GetProductionActivityReportList : PagedRequest, IRequest<List<View_ProductionActivityReport>>
    {
      
     
    }
    public class GetProductionActivityReportDocumentList : PagedRequest, IRequest<List<Documents>>
    {

        public long ProductionActivityAppLineID { get;  set; }
        public GetProductionActivityReportDocumentList(long ProductionActivityAppLineID)
        {
            this.ProductionActivityAppLineID = ProductionActivityAppLineID;

        }
    }
}
