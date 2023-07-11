using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.ApplicationMasterDetails
{
    public class CreateApplicationMasterDetailCommand : IRequest<ApplicationMasterDetailResponse>
    {
        public long ApplicationMasterDetailId { get; set; }
        public long ApplicationMasterId { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ProfileId { get; set; }
        public long? FileProfileTypeId { get; set; }
        public CreateApplicationMasterDetailCommand()
        {
        }
    }
}
