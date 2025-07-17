using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDOXMES.Models;

[Table("dtPartNumbers")]
public class PartNumberClass
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string PartNumber { get; set; }

    [Required]
    [MaxLength(200)]
    public string Description { get; set; }
}