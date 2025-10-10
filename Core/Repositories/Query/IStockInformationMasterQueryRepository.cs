using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.EntityModels.SyrupPlanning;

namespace Core.Repositories.Query
{
    public interface IStockInformationMasterQueryRepository
    {        
        Task<IReadOnlyList<StockInformationMaster>> GetAllByAsync();
        Task<StockInformationMaster> DeleteStockInformationMaster(StockInformationMaster value);
        Task<StockInformationMaster> InsertOrUpdateStockInformationMaster(StockInformationMaster value);
        Task<IReadOnlyList<SyrupPlanning>> GetSyrupPlannings();
        Task<IReadOnlyList<SyrupProcessNameList>> GetSyrupProcessNameList();        
        Task<SyrupPlanning> SelectSyrupSimplexDataList(long methodCodeId);
        Task<SyrupPlanning> SelectSyruppreparationDataList(long methodCodeId);
        Task<IReadOnlyList<SyrupFilling>> GetSyrupFillingList();
        Task<IReadOnlyList<SyrupOtherProcess>> GetSyrupOtherProcessList();
    }
}
