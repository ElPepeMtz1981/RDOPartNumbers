namespace RDOXMES.DTO
{
    public class MovementDto
    {
        public int PartNumberId { get; set; }
        public string Movement { get; set; } // "Entrada" o "Salida"
        public int Qty { get; set; }
        public int UserNameId { get; set; }
    }

    public enum MovementType
    {
        Entrada = 1,
        Salida = 2
    }
}
