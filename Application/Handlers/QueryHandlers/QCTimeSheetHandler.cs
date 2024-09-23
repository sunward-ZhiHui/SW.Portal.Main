using Application.Queries;
using Application.Queries.Base;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
     public class QCTimeSheetHandler: IRequestHandler<CreateQCTimesheetQuery, long>
     {
            private readonly IQCTimesheetQueryRepository _qcqueryRepository;
            public QCTimeSheetHandler(IQCTimesheetQueryRepository qcqueryRepository)
            {
            _qcqueryRepository = qcqueryRepository;
            }

            public async Task<long> Handle(CreateQCTimesheetQuery request, CancellationToken cancellationToken)
            {
                var newlist = await _qcqueryRepository.Insert(request);
                return newlist;

            }
     }
    public class EditQCTimeSheetHandler : IRequestHandler<UpdateQCTimesheetQuery, long>
    {
        private readonly IQCTimesheetQueryRepository _qcqueryRepository;
        public EditQCTimeSheetHandler(IQCTimesheetQueryRepository qcqueryRepository)
        {
            _qcqueryRepository = qcqueryRepository;
        }

        public async Task<long> Handle(UpdateQCTimesheetQuery request, CancellationToken cancellationToken)
        {
            var req = await _qcqueryRepository.Update(request);
            return req;
        }
    }
    public class GetQCListHandler : IRequestHandler<GetQCTimeSheetQuery, List<TimeSheetForQC>>
    {

        private readonly IQCTimesheetQueryRepository _queryRepository;
        public GetQCListHandler(IQCTimesheetQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<TimeSheetForQC>> Handle(GetQCTimeSheetQuery request, CancellationToken cancellationToken)
        {
            return (List<TimeSheetForQC>)await _queryRepository.GetAllAsync();
        }
    }
    public class EditStatusHandler : IRequestHandler<UpdateStatusQuery, long>
    {
        private readonly IQCTimesheetQueryRepository _qcqueryRepository;
        public EditStatusHandler(IQCTimesheetQueryRepository qcqueryRepository)
        {
            _qcqueryRepository = qcqueryRepository;
        }

        public async Task<long> Handle(UpdateStatusQuery request, CancellationToken cancellationToken)
        {
            var req = await _qcqueryRepository.UpdateStatus(request.ID,request.StatusID,request.ModifiedByUserID);
            return req;
        }
    }

    public class GetActivityListHandler : IRequestHandler<GetActivityQuery, List<TimeSheetForQC>>
    {

        private readonly IQCTimesheetQueryRepository _queryRepository;
        public GetActivityListHandler(IQCTimesheetQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<TimeSheetForQC>> Handle(GetActivityQuery request, CancellationToken cancellationToken)
        {
            return (List<TimeSheetForQC>)await _queryRepository.GetMultipleQueryAsync(request.QCID);
        }
    }
}
