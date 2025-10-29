using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.LeveMasters
{
    public class CreateLevelMasterCommand : IRequest<LevelMasterResponse>
    {
  
        public long LevelId { get; set; }
        public long? CompanyId { get; set; }
        public string Name { get; set; }
        public int? Priority { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? DivisionID { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? DivisionName { get; set; }
        [NotMapped]
        public string? CompanyName { get; set; }
        public CreateLevelMasterCommand()
        {

        }
    }
}
