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
    public interface IRoutineQueryRepository : IQueryRepository<ProductionActivityRoutineAppLine>
    {
      
        Task<IReadOnlyList<ProductionActivityRoutineAppLine>> GetAllAsync();

        Task<IReadOnlyList<ProductionActivityRoutineAppModel>> GetAllProductionActivityRoutineAsync(ProductionActivityRoutineAppModel value);
        Task<ProductionActivityRoutineAppModel> DeleteproductActivityRoutineAppLine(ProductionActivityRoutineAppModel productionActivityRoutineAppModel);
        Task<ProductionActivityRoutineAppModel> UpdateproductActivityRoutineAppLineCommentField(ProductionActivityRoutineAppModel value);
        Task<ProductionActivityRoutineAppModel> UpdateActivityRoutineMaster(ProductionActivityRoutineAppModel value);
        Task<ProductionActivityRoutineAppModel> UpdateActivityRoutineStatus(ProductionActivityRoutineAppModel value);
        Task<ProductionActivityRoutineAppModel> GetProductActivityRoutineAppLineOneItem(long? ProductionActivityAppLineID);
        Task<ProductionActivityRoutineAppModel> UpdateRoutineChecker(ProductionActivityRoutineAppModel value);

        Task<IReadOnlyList<ProductionActivityRoutineCheckedDetailsModel>> GetProductionActivityRoutineCheckedDetails(long? value);

        Task<ProductionActivityRoutineCheckedDetailsModel> DeleteProductionActivityRoutineCheckedDetails(ProductionActivityRoutineCheckedDetailsModel value);
        Task<ProductionActivityRoutineCheckedDetailsModel> InsertProductionActivityRoutineCheckedDetails(ProductionActivityRoutineCheckedDetailsModel value);

        Task<IReadOnlyList<ProductionActivityRoutineEmailModel>> GetProductionActivityRoutineEmailList(long? ProductionActivityRoutineAppLineID);
    }
}
