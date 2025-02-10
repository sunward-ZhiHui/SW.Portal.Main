using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllIncidentAppSettingsQuery : PagedRequest, IRequest<List<IncidentAppSettings>>
    {
    }
    public class CreateincidentApp : IncidentAppSettings, IRequest<long>
    {
    }

    public class EditIncidentApp : IncidentAppSettings, IRequest<long>
    {
    }
}
