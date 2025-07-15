using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartNumbers.Data;
using PartNumbers.Models;

namespace ProductosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartNumbersController : ControllerBase
{
    private readonly PartNumbersDbContext context;

    private readonly ILogger<PartNumbersController> logger;

    public PartNumbersController(PartNumbersDbContext context, ILogger<PartNumbersController> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    // PUT api/PartNumbers/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePartNumber(int id, PartNumberClass updatedPart)
    {
        try
        {
            if (id != updatedPart.Id)
                return BadRequest("El Id proporcionado no coincide con el Id del Numero de Parte.");

            var existingPart = await context.PartNumbers.FindAsync(id);
            if (existingPart == null)
                return NotFound($"No se encontró PartNumber con Id: {id}.");

            // Normalización y validación opcional
            var newPartNumber = updatedPart.PartNumber.Trim().ToLower();
            var alreadyExist = await context.PartNumbers
                .AnyAsync(p => p.Id != id && p.PartNumber.Trim().ToLower() == newPartNumber);

            if (alreadyExist)
                return Conflict($"Ya existe otro Numero de Parte: \"{updatedPart.PartNumber}\".");

            existingPart.PartNumber = updatedPart.PartNumber;
            existingPart.Description = updatedPart.Description;

            await context.SaveChangesAsync();
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
            var part = await context.PartNumbers.FindAsync(id);
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
            var part = await context.PartNumbers
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

    [HttpPost("new")]
    public async Task<ActionResult<PartNumberClass>> PostPartNumber(PartNumberClass partNumber)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nombreNormalizado = partNumber.PartNumber.Trim().ToLower();

            var existe = await context.PartNumbers
                .AnyAsync(p => p.PartNumber.Trim().ToLower() == nombreNormalizado);

            if (existe)
                return Conflict($"Ya existe un Numero de Parte con: \"{partNumber.PartNumber}\".");

            var pn = new PartNumberClass
            {
                PartNumber = partNumber.PartNumber,
                Description = partNumber.Description
            };

            context.PartNumbers.Add(pn);
            await context.SaveChangesAsync();

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
            return await context.PartNumbers.ToListAsync();
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
            var part = await context.PartNumbers.FindAsync(id);
            if (part == null)
            {
                return NotFound($"No se encontró Numero de Parte con Id: {id}.");
            }

            context.PartNumbers.Remove(part);
            await context.SaveChangesAsync();

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