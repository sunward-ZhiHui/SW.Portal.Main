using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IProductionActivityQueryRepository : IQueryRepository<ProductionActivityAppLine>
    {
        Task<IReadOnlyList<ProductActivityAppModel>> GetAllAsync(ProductActivityAppModel productActivityAppModel);
        Task<IReadOnlyList<ProductionActivityApp>> GetAlllocAsync(long? location);
        Task<ProductActivityAppModel> GetProductActivityAppLineOneItem(long? ProductionActivityAppLineID);
        Task<ProductActivityAppModel> UpdateproductActivityAppLineCommentField(ProductActivityAppModel productActivityAppModel);
        Task<ProductActivityAppModel> DeleteproductActivityAppLine(ProductActivityAppModel productActivityAppModel);
        Task<DocumentsModel> DeleteSupportingDocuments(DocumentsModel value);
        Task<ProductionActivityNonComplianceModel> GetProductionActivityNonComplianceAsync(string type, long? id, string actionType);
        Task<ProductionActivityNonComplianceModel> InsertProductionActivityNonCompliance(ProductionActivityNonComplianceModel value);
        Task<ProductionActivityNonComplianceUserModel> DeleteProductionActivityNonCompliance(ProductionActivityNonComplianceUserModel value);
        Task<ProductActivityAppModel> UpdateActivityMaster(ProductActivityAppModel value);
        Task<ProductActivityAppModel> UpdateActivityStatus(ProductActivityAppModel value);
        Task<IReadOnlyList<view_ActivityEmailSubjects>> GetProductActivityEmailActivitySubjects(long? ActivityMasterId, string? ActivityType, long? UserId);

        Task<ProductActivityAppModel> UpdateActivityChecker(ProductActivityAppModel value);

        Task<IReadOnlyList<ProductionActivityCheckedDetailsModel>> GetProductionActivityCheckedDetails(long? value);
        Task<ProductionActivityCheckedDetailsModel> InsertProductionActivityCheckedDetails(ProductionActivityCheckedDetailsModel value);

        Task<ProductionActivityCheckedDetailsModel> DeleteProductionActivityCheckedDetails(ProductionActivityCheckedDetailsModel value);
    }
}
