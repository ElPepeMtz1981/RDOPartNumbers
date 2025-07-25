using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RDOXMES.PartNumbers;

namespace RDOXMES.PartNumbers;

[ApiController]
[Route("api/partnumbers")]
public class PartNumbersController : ControllerBase
{
    private readonly PartNumbersDbContext partNumberDbContext;    

    public PartNumbersController(PartNumbersDbContext partNumberDbContext)
    {
        this.partNumberDbContext = partNumberDbContext;        
    }

    // PUT api/PartNumbers/{id}
    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> UpdatePartNumber(int id, PartNumberClass updatedPart)
    {
        try
        {
            if (id != updatedPart.Id)
            {
                return BadRequest("El Id proporcionado no coincide con el Id del Numero de Parte.");
            }

            var existingPart = await partNumberDbContext.PartNumbers.FindAsync(id);
            if (existingPart == null)
            {
                return NotFound($"No se encontró PartNumber con Id: {id}.");
            }

            // Normalización y validación opcional
            var newPartNumber = updatedPart.PartNumber.Trim().ToLower();
            var alreadyExist = await partNumberDbContext.PartNumbers
                .AnyAsync(p => p.Id != id && p.PartNumber.Trim().ToLower() == newPartNumber);

            if (alreadyExist)
                return Conflict($"Ya existe otro Numero de Parte: \"{updatedPart.PartNumber}\".");

            existingPart.PartNumber = updatedPart.PartNumber;
            existingPart.Description = updatedPart.Description;

            await partNumberDbContext.SaveChangesAsync();
            return NoContent();
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

    // GET api/PartNumbers/getbyid/{id}
    [HttpGet("getbyid/{id:int}")]
    public async Task<ActionResult<PartNumberClass>> GetPartNumberById(int id)
    {
        try
        {
            var part = await partNumberDbContext.PartNumbers.FindAsync(id);
            if (part == null)
            {
                return NotFound($"No se encontró Numero de Parte con Id: {id}.");
            }

            return Ok(part);
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

    // GET api/PartNumbers/getbypn/{pn}
    [HttpGet("getbypn/{pn}")]
    public async Task<ActionResult<PartNumberClass>> GetPartNumberByPN(string pn)
    {
        try
        {
            var partnumberUpper = pn.Trim().ToLower();
            var partNumber = await partNumberDbContext.PartNumbers
                .FirstOrDefaultAsync(p => p.PartNumber.Trim().ToLower() == partnumberUpper);

            if (partNumber == null)
            {
                return NotFound($"No se encontró Numero de Parte con: \"{pn}\".");
            }

            return Ok(partNumber);
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

            var existe = await partNumberDbContext.PartNumbers
                .AnyAsync(p => p.PartNumber.Trim().ToLower() == nombreNormalizado);

            if (existe)
                return Conflict($"Ya existe un Numero de Parte con: \"{partNumber.PartNumber}\".");

            var pn = new PartNumberClass
            {
                PartNumber = partNumber.PartNumber,
                Description = partNumber.Description
            };

            partNumberDbContext.PartNumbers.Add(pn);
            await partNumberDbContext.SaveChangesAsync();

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

    [HttpGet("getall")]
    public async Task<ActionResult<IEnumerable<PartNumberClass>>> GetPartNumbers()
    {
        try
        {
            return await partNumberDbContext.PartNumbers.ToListAsync();
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

    [HttpDelete("deletebyid/{id:int}")]
    public async Task<IActionResult> DeletePartNumber(int id)
    {
        try
        {
            var part = await partNumberDbContext.PartNumbers.FindAsync(id);
            if (part == null)
            {
                return NotFound($"No se encontró Numero de Parte con Id: {id}.");
            }

            partNumberDbContext.PartNumbers.Remove(part);
            await partNumberDbContext.SaveChangesAsync();

            return NoContent();
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
}