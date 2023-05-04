using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.SubSections
{
    public class CreateSubSectionCommand : IRequest<SubSectionResponse>
    {
        public long SubSectionId { get; set; }
        public long? SectionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? HeadCount { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ProfileCode { get; set; }
        public CreateSubSectionCommand()
        {
            this.AddedDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
            this.AddedByUserId = 1;
            this.ModifiedByUserId = 1;
            this.HeadCount = 1;
        }
    }
}
