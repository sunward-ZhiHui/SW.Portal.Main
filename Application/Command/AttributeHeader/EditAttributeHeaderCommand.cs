using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.AttributeHeader
{
    public class EditAttributeHeaderCommand : IRequest<AttributeHeaderResponse>
    {
        public int AttributeID { get; set; }
        public string Description { get; set; }
        public int ControlType { get; set; }
        public string EntryMask { get; set; }
        public string RegExp { get; set; }
        public string ListDefault { get; set; }
        public bool IsInternal { get; set; }
        public bool ContainsPersonalData { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public EditAttributeHeaderCommand()
        {
                
        }
    }
}
