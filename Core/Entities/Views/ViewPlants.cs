using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewPlants
    {
        [Key]
        public long PlantID { get; set; }
        public long CompanyID { get; set; }

        public string? PlantCode { get; set; }

        public string? Description { get; set; }
        public long? RegisteredCountryID { get; set; }
        public string? RegistrationNo { get; set; }

        public DateTime? EstablishedDate { get; set; }
        public string? GSTNo { get; set; }
        public string? NavSoapAddress { get; set; }

        public bool IsLinkNav { get; set; }

        public string? NavCompanyName { get; set; }
        public string? NavUserName { get; set; }
        public string? NavPassword { get; set; }
        public string? NavOdataAddress { get; set; }
        public string? CompanyName { get; set; }
        public int? CodeId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? AddedByUserID { get; set; }
        public int? StatusCodeID { get; set; }
        public string? StatusCode { get; set; }
        public string? ModifiedBy { get; set; }
        public string? AddedBy { get; set; }
    }
}
