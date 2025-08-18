using Microsoft.EntityFrameworkCore;

namespace RDOXMES.Models
{
    [Keyless]
    public class ViewHistoric
    {
        public int IdPartNumber { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string Movement { get; set; }
        public int Qty { get; set; }
        public DateTime DateTime { get; set; }        
    }
}
