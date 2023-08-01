using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class NavProductionInformationResponse
    {
        public long NavProductionInformationId { get; set; }
        public long? ItemId { get; set; }
        public int? NoOfBUOMInOneCarton { get; set; }
    }
}
