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
        Task<IReadOnlyList<ProductActivityAppModel>> GetAllAsync(long? CompanyID, string? prodorderNo, long? userId, long? locationID);
        Task<IReadOnlyList<ProductionActivityApp>>GetAlllocAsync(long? location);
        Task<ProductActivityAppModel> GetProductActivityAppLineOneItem(long? ProductionActivityAppLineID);
        Task<ProductActivityAppModel> UpdateproductActivityAppLineCommentField(ProductActivityAppModel productActivityAppModel);
        Task<ProductActivityAppModel> DeleteproductActivityAppLine(ProductActivityAppModel productActivityAppModel);
        Task<DocumentsModel> DeleteSupportingDocuments(DocumentsModel value);
        Task<ProductionActivityNonComplianceModel> GetProductionActivityNonComplianceAsync(string type, long? id, string actionType);
        Task<ProductionActivityNonComplianceModel> InsertProductionActivityNonCompliance(ProductionActivityNonComplianceModel value);
        Task<ProductionActivityNonComplianceUserModel> DeleteProductionActivityNonCompliance(ProductionActivityNonComplianceUserModel value);

    }
}
