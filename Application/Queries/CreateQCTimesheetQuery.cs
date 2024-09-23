using Application.Queries.Base;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class CreateQCTimesheetQuery : TimeSheetForQC, IRequest<long>
    {
    }
    public class UpdateQCTimesheetQuery : TimeSheetForQC, IRequest<long>
    {

    }
    public class GetQCTimeSheetQuery : PagedRequest, IRequest<List<TimeSheetForQC>>
    {
        public string SearchString { get; set; }
    }
    public class UpdateStatusQuery : TimeSheetForQC, IRequest<long>
    {
        public long ID { get; private set; }
        public long StatusID { get; private set; }
        public long ModifiedByUserID { get; private set; }

        public UpdateStatusQuery(long id, long statusID, long modifiedByUserID)
        {
            this.ID = id;
            StatusID = statusID;
            ModifiedByUserID = modifiedByUserID;
        }
    }
    public class GetActivityQuery : PagedRequest, IRequest<List<TimeSheetForQC>>
    {
        public long QCID { get; set; }
        public GetActivityQuery(long QCID)
        {
            this.QCID = QCID;
        }
    }
    
}
