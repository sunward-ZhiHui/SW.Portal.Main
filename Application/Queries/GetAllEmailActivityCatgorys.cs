using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllEmailActivityCatgorys : PagedRequest, IRequest<List<EmailActivityCatgorys>>
    {
    }

    public class CreateEmailActivityCatgorysQuery : EmailActivityCatgorys, IRequest<long>
    {
    }
}
