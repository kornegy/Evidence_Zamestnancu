using evidence_zamestancu.Client.Models;
using evidence_zamestancu.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace evidence_zamestancu.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PositionsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<PositionsController> _logger;

    public PositionsController(AppDbContext context, ILogger<PositionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Position>>> GetPositions()
    {
        return await _context.Positions.ToListAsync();
    }

    [HttpPost("import")]
    public async Task<ActionResult> ImportPositions([FromBody] List<string> importedPositions)
    {
        if (importedPositions == null || !importedPositions.Any())
        {
            return BadRequest("File is empty or bad format!");
        }

        foreach (var posName in importedPositions)
        {
            try
            {
                bool exists = await _context.Positions.AnyAsync(e =>
                    e.PositionName == posName
                );

                if (exists) continue;

                var newPosition = new Position
                {
                    PositionName = posName
                };
                    
                _context.Positions.Add(newPosition);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while importing positions");
            }
        }
        await _context.SaveChangesAsync();
        
        return Ok();
    }
}