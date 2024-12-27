using Core.Entities;
using Core.EntityModels;
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
        Task<IReadOnlyList<view_QCAssignmentRM>> GetAllListByQRAsync(string Date,string Company);
        Task<IReadOnlyList<view_QCAssignmentRM>> GetAllQCListByQRAsync(string ItemName, string QCRefNo,string TestName);
    }
}
