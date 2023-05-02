using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.designations
{
    public class EditDesignationCommand : IRequest<DesignationResponse>
    {
        public long DesignationId { get; set; }
        public long? LevelId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? HeadCount { get; set; }
        public long? CompanyId { get; set; }
        public long? SubSectionTid { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public EditDesignationCommand()
        {
            this.ModifiedDate = DateTime.Now;
            this.ModifiedByUserId = 1;
        }
    }
}
