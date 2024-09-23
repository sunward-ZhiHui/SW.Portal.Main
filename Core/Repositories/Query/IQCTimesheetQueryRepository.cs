using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IQCTimesheetQueryRepository
    {
        Task<IReadOnlyList<TimeSheetForQC>> GetAllAsync();
        Task<long> Insert(TimeSheetForQC timeSheetForQC);
        Task<long> Update(TimeSheetForQC timeSheetForQC);
        Task<IReadOnlyList<TimeSheetForQC>> GetAllQCTimeSheetAsync(long QCTimeSheetID);
        Task<long> UpdateStatus(long ID ,long StatusID,long ModifiedByUserID);
        Task<IReadOnlyList<TimeSheetForQC>> GetMultipleQueryAsync(long? QCTimesheetID);
    }
}
