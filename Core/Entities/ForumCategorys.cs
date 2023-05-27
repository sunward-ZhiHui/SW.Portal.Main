using Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class ForumCategorys : BaseEntity
    {
        [Key]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please Enter Name.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Please Select Type.")]
        public long TypeId { get; set; }
        [NotMapped]
        public int Index { get; set; }
    }
   
}

