namespace RDOXMES.DTO
{
    namespace RDOXMES.DTO
    {
        public class InventoryHistoryDto
        {
            public int IdPartNumber { get; set; }
            public string PartNumber { get; set; }
            public string Description { get; set; }
            public string Movement { get; set; } // "Entrada" o "Salida"
            public int Qty { get; set; }
            public DateTime DateTime { get; set; }            
        }
    }
}
