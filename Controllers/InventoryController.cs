using Microsoft.AspNetCore.Mvc;
using RDOXMES.DTO;
using RDOXMES.Data;
using RDOXMES.Models;
using Microsoft.EntityFrameworkCore;

namespace RDOXMES.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryDbContext inventoryDBContext;

        public InventoryController(InventoryDbContext context)
        {
            inventoryDBContext = context;
        }
        
        [HttpPost("registermovement")]
        public async Task<IActionResult> RegisterMovement([FromBody] MovementDto dto)
        {
            try
            {
                using var transaction = await inventoryDBContext.Database.BeginTransactionAsync();

                var inventory = await inventoryDBContext.Inventory.FindAsync(dto.PartNumberId);
                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        PartNumberId = dto.PartNumberId,
                        Qty = 0,
                        UpdatedAt = DateTime.UtcNow
                    };
                    inventoryDBContext.Inventory.Add(inventory);
                }

                if (dto.Movement == "Salida" && inventory.Qty < dto.Qty)
                {
                    return BadRequest("No hay suficiente inventario para realizar la salida.");
                }

                inventory.Qty += dto.Movement == "Entrada" ? dto.Qty : -dto.Qty;
                inventory.UpdatedAt = DateTime.UtcNow;
                
                var historic = new Historic
                {
                    PartNumberId = dto.PartNumberId,
                    Movement = dto.Movement,
                    Qty = dto.Qty,
                    DateTime = DateTime.UtcNow,
                    UserNameId = dto.UserNameId
                };
                inventoryDBContext.Historic.Add(historic);

                await inventoryDBContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    PartNumberId = inventory.PartNumberId,
                    Qty = inventory.Qty,
                    UpdatedAt = inventory.UpdatedAt
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(503, new { mensaje = "Error al acceder a la base de datos. Intenta más tarde.", detalle = ex.Message });
            }
            catch (Exception ex)
            {             
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

                
        [HttpGet("allinventoryhistory")]
        public async Task<IActionResult> GetInventoryHistory()
        {
            var inventory = await inventoryDBContext.ViewHistoric.ToListAsync();
            return Ok(inventory);
        }

        
        [HttpGet("historybyid/{idPartNumber}")]
        public async Task<IActionResult> GetHistoryById(int idPartNumber)
        {
            var history = await inventoryDBContext.ViewHistoric
                .Where(h => h.IdPartNumber == idPartNumber)
                .OrderByDescending(h => h.DateTime)
                .ToListAsync();

            return Ok(history);
        }


        [HttpGet("historybypn/{partNumber}")]
        public async Task<IActionResult> GetHistoryByPN(string partNumber)
        {
            var history = await inventoryDBContext.ViewHistoric
                .Where(h => h.PartNumber == partNumber)
                .OrderByDescending(h => h.DateTime)
                .ToListAsync();

            return Ok(history);
        }


        [HttpGet("allinventory")]
        public async Task<ActionResult<IEnumerable<InventoryDTO>>> GetInventory()
        {
            var result = await inventoryDBContext.ViewInventory
                .Select(x => new InventoryDTO
                {
                    IdPartNumber = x.IdPartNumber,
                    PartNumber = x.PartNumber,
                    Description = x.Description,
                    Qty = x.Qty,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return Ok(result);
        }


        [HttpGet("inventorybyid/{idPartNumber}")]
        public async Task<ActionResult<InventoryDTO>> GetInventoryById(int idPartNumber)
        {
            var result = await inventoryDBContext.ViewInventory
                .Where(i => i.IdPartNumber == idPartNumber)
                .Select(i => new InventoryDTO
                {
                    IdPartNumber = i.IdPartNumber,
                    PartNumber = i.PartNumber,
                    Description = i.Description,
                    Qty = i.Qty,
                    UpdatedAt = i.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpGet("inventorybypn/{partNumber}")]
        public async Task<ActionResult<IEnumerable<InventoryDTO>>> GetInventoryByPartNumber(string partNumber)
        {
            var result = await inventoryDBContext.ViewInventory
                .Where(i => i.PartNumber == partNumber)
                .Select(i => new InventoryDTO
                {
                    IdPartNumber = i.IdPartNumber,
                    PartNumber = i.PartNumber,
                    Description = i.Description,
                    Qty = i.Qty,
                    UpdatedAt = i.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpGet("inventory/search")]
        public async Task<IActionResult> SearchParts([FromQuery] string term)
        {
            var results = await inventoryDBContext.ViewInventory
                .Where(p => p.PartNumber.Contains(term) || p.Description.Contains(term))
                .Select(p => new {
                    value = p.IdPartNumber,
                    label = $"{p.PartNumber} - {p.Description}"
                })
                .ToListAsync();

            return Ok(results);
        }
    }
}
