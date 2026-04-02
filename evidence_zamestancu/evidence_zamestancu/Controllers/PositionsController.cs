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

    public PositionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Position>>> GetPositions()
    {
        return await _context.Positions.ToListAsync();
    }
}