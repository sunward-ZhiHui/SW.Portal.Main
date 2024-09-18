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
}
