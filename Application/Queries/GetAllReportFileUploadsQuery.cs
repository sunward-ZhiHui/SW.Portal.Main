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
    public class EditReportFileQuery : ReportDocuments, IRequest<long>
    {
        public byte[] FileContent { get; set; }
        public string ActFileName { get; set; }
    }
    public class DeleteReportdocumentQuery : ReportDocuments, IRequest<long>
    {
        public long ReportDocumentID { get; set; }
      public string ReportName { get; set; }

        public DeleteReportdocumentQuery(long Id, string reportName)
        {
            this.ReportDocumentID = Id;
           this.ReportName = reportName;
        }
    }
    public class CreateReportFileQueryNew : ReportDocuments, IRequest<Guid?>
    {

        //public byte[] FileContent { get; set; }
    }
}
