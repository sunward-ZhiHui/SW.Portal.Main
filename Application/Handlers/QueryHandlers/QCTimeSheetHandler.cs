using Application.Queries;
using Application.Queries.Base;
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
}
