namespace RDOXMES.DTO
{
    public class InventoryDTO
    {
        public int IdPartNumber { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}