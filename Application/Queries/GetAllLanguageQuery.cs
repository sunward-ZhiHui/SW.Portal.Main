using Application.Queries.Base;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class GetAllLanguageQuery : PagedRequest, IRequest<List<LanguageMaster>>
    {
        public string SearchString { get; set; }
    }
}
