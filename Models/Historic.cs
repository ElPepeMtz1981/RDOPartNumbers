using System.ComponentModel.DataAnnotations;

namespace RDOXMES.Models
{
    public class Historic
    {
        [Key]
        public int Id { get; set; }

        [Required]        
        public int PartNumberId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Movement { get; set; } // "Entrada" o "Salida"

        [Required]
        public int Qty { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]        
        public int UserNameId { get; set; }
    }
}
