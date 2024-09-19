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
        Task<long> Insert(TimeSheetForQC timeSheetForQC);
        Task<long> Update(TimeSheetForQC timeSheetForQC);
    }
}
