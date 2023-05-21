using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using DevExpress.Blazor;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace Application.Queries
{
    public class UploadFilesRequest : IRequest<bool>
    {
        public List<IFormFile> Files { get; set; }
        public Guid SessionId { get; set; }
    }  

}
