using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartNumbers.Data;
using PartNumbers.Models;

namespace ProductosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartNumbersController : ControllerBase
{
    private readonly PartNumbersDbContext _context;

    private readonly ILogger<PartNumbersController> _logger;

    public PartNumbersController(PartNumbersDbContext context, ILogger<PartNumbersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // PUT api/PartNumbers/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePartNumber(int id, PartNumberClass updatedPart)
    {
        try
        {
            if (id != updatedPart.Id)
                return BadRequest("El Id proporcionado no coincide con el Id del Numero de Parte.");

            var existingPart = await _context.PartNumbers.FindAsync(id);
            if (existingPart == null)
                return NotFound($"No se encontró PartNumber con Id: {id}.");

            // Normalización y validación opcional
            var nuevoPN = updatedPart.PartNumber.Trim().ToLower();
            var existeOtro = await _context.PartNumbers
                .AnyAsync(p => p.Id != id && p.PartNumber.Trim().ToLower() == nuevoPN);

            if (existeOtro)
                return Conflict($"Ya existe otro Numero de Parte: \"{updatedPart.PartNumber}\".");

            existingPart.PartNumber = updatedPart.PartNumber;
            existingPart.Description = updatedPart.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            // Error al guardar en la base de datos
            return StatusCode(503, new { mensaje = "Error al acceder a la base de datos. Intenta más tarde.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            // Cualquier otro error inesperado
            return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
        }
    }

    // GET api/PartNumbers/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PartNumberClass>> GetPartNumberById(int id)
    {
        try
        {
            var part = await _context.PartNumbers.FindAsync(id);
            if (part == null)
                return NotFound($"No se encontró Numero de Parte con Id: {id}.");
            return Ok(part);
        }
        catch (DbUpdateException ex)
        {
            // Error al guardar en la base de datos
            return StatusCode(503, new { mensaje = "Error al acceder a la base de datos. Intenta más tarde.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            // Cualquier otro error inesperado
            return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
        }
    }

    // GET api/PartNumbers/by-pn/{pn}
    [HttpGet("by-pn/{pn}")]
    public async Task<ActionResult<PartNumberClass>> GetPartNumberByPN(string pn)
    {
        try
        {
            var pnNorm = pn.Trim().ToLower();
            var part = await _context.PartNumbers
                .FirstOrDefaultAsync(p => p.PartNumber.Trim().ToLower() == pnNorm);

            if (part == null)
                return NotFound($"No se encontró Numero de Parte con: \"{pn}\".");
            return Ok(part);
        }
        catch (DbUpdateException ex)
        {
            // Error al guardar en la base de datos
            return StatusCode(503, new { mensaje = "Error al acceder a la base de datos. Intenta más tarde.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            // Cualquier otro error inesperado
            return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<PartNumberClass>> PostPartNumber(PartNumberClass partNumber)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Normaliza el nombre para evitar que "Camiseta" y "camiseta" se consideren distintos
            var nombreNormalizado = partNumber.PartNumber.Trim().ToLower();

            var existe = await _context.PartNumbers
                .AnyAsync(p => p.PartNumber.Trim().ToLower() == nombreNormalizado);

            if (existe)
                return Conflict($"Ya existe un Numero de Parte con: \"{partNumber.PartNumber}\".");

            var pn = new PartNumberClass
            {
                PartNumber = partNumber.PartNumber,
                Description = partNumber.Description
            };

            _context.PartNumbers.Add(pn);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPartNumbers), new { id = partNumber.Id }, partNumber);
        }
        catch (DbUpdateException ex)
        {
            // Error al guardar en la base de datos
            return StatusCode(503, new { mensaje = "Error al acceder a la base de datos. Intenta más tarde.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            // Cualquier otro error inesperado
            return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PartNumberClass>>> GetPartNumbers()
    {
        try
        {
            return await _context.PartNumbers.ToListAsync();
        }
        catch (DbUpdateException ex)
        {
            // Error al guardar en la base de datos
            return StatusCode(503, new { mensaje = "Error al acceder a la base de datos. Intenta más tarde.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            // Cualquier otro error inesperado
            return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePartNumber(int id)
    {
        try
        {
            var part = await _context.PartNumbers.FindAsync(id);
            if (part == null)
            {
                return NotFound($"No se encontró Numero de Parte con Id: {id}.");
            }

            _context.PartNumbers.Remove(part);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            // Error al guardar en la base de datos
            return StatusCode(503, new { mensaje = "Error al acceder a la base de datos. Intenta más tarde.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            // Cualquier otro error inesperado
            return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
        }
    }
}