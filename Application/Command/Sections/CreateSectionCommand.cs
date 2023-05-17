using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.Sections
{
    public class CreateSectionCommand : IRequest<SectionResponse>
    {
        public long SectionId { get; set; }
        public long? DepartmentId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public bool? IsWiki { get; set; }
        public bool? IsTestPaper { get; set; }
        public int? HeadCount { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ProfileCode { get; set; }
        public CreateSectionCommand()
        {
        }
    }
}
