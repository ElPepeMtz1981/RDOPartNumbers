using System.ComponentModel.DataAnnotations;
namespace RDOXMES.PartNumbers;

public class PartNumbers
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string PartNumber { get; set; }

    [Required]
    [MaxLength(200)]
    public string Description { get; set; }
}