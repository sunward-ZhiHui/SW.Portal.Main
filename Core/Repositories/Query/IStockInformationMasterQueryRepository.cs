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
using static Core.EntityModels.SyrupReportDtos;

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
        Task<SyrupPlanning> InsertOrUpdateSyrupPlanningAsync(SyrupPlanning model);
        Task SaveSyrupOtherProcessesAsync(long syrupPlanningId, IEnumerable<SyrupOtherProcess> items);
        Task SaveSyrupFillingAsync(long syrupPlanningId, IEnumerable<SyrupFilling> items);
        Task<IReadOnlyList<ProcessStepDto>> GetProcessFlowByProfileNoAsync(string profileNo, DateTime productionDay, TimeSpan shiftStart);
        Task<IReadOnlyList<TimingDetailDto>> GetTimingDetailsByProfileNoAsync(string profileNo);
        Task<IReadOnlyList<MachineInfoDto>> GetMachineInfoByProfileNoAsync(string profileNo);
        Task<IReadOnlyList<ProductItemDto>> GetProductItemsByMethodCodeAsync(int methodCodeId);
        Task<SyrupPlanningDto?> GetSyrupPlanningByIdAsync(long syrupPlanningId);
        Task<ProductItemDto?> GetMasterByMethodCodeAsync(int methodCodeId);
        Task<TimingOverviewDto?> GetTimingOverviewByProfileNoAsync(string profileNo);
        Task<IReadOnlyList<TaskData>> GetProductionGanttRowsAsync(string profileNo, DateTime productionDay, TimeSpan shiftStart);
        Task<IReadOnlyList<TaskData>> GetProductionGanttAsyncList(string profileNo, DateTime productionDay, TimeSpan shiftStart);

    }
}
