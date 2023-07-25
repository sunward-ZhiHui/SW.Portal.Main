using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class CreateEmailTopics : EmailTopicsResponse, IRequest<long>
    {
    }

    public class EditEmailTopics : EmailTopicsResponse, IRequest<long>
    {
    }
}
