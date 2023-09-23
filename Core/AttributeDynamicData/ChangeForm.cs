using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.AttributeDynamicData
{
    public class ChangeForm
    {
        [FieldType(FieldType.Text)]
        [Display(Name = "UserName", Description = "User Name")]
        public string CCFCAPprovalID { get; set; }

        [FieldType(FieldType.CheckBox)]
        [Display(Name = "Approve")]
        public bool IsApproved { get; set; }

        [FieldType(FieldType.CheckBox)]
        [Display(Name = "Not Approve")]
        public bool IsNotApproved { get; set; }
        [FieldType(FieldType.Text)]
        [Display(Name = "Comments")]
        public string? Comments { get; set; }
        [FieldType(FieldType.ComboBox)]
        [Display(Name = "VerifiedBy")]
        public string VerifiedBy { get; set; }

       

    }
}
