using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllReportFileUploadsQuery : PagedRequest, IRequest<List<ReportDocuments>>
    {
    }
    public class CreateReportFileQuery : ReportDocuments, IRequest<long>
    {
        
        public byte[] FileContent { get; set; }
    }
}
