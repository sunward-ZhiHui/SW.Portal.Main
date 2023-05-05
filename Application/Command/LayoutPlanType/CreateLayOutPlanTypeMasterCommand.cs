using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.LayoutPlanType
{
    public class CreateLayOutPlanTypeMasterCommand: IRequest<LayOutPlanTypeResponse>
    {
        public long LayoutPlanTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VersionNo { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public Guid? SessionId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}
