
using Application.Queries.Base;
using Core.Entities;
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
}
