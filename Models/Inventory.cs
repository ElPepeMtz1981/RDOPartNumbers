using System.ComponentModel.DataAnnotations;

namespace RDOXMES.Models
{
    public class Inventory
    {
        [Key]
        [Required]       
        public int PartNumberId { get; set; }

        [Required]
        public int Qty { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
