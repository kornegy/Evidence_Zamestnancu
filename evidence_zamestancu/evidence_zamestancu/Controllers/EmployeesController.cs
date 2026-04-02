using evidence_zamestancu.Client.Models;
using evidence_zamestancu.Data;
using evidence_zamestancu.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace evidence_zamestancu.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IIpService _ipService;

    public EmployeesController(AppDbContext context, IIpService ipService)
    {
        _context = context;
        _ipService = ipService;
    }
 
    [HttpGet] //read employye 
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
        return await _context.Employees
            .Include(e => e.Position)
            .ToListAsync(); // return result only when all the settings are done
    }

    [HttpGet("{id}")] //search for 1 employee in db
    public async Task<ActionResult<Employee>> GetOneEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }
        return employee;
    }

    [HttpGet("position")] //getting Positions from DB
    public async Task<ActionResult<IEnumerable<Position>>> GetPositions()
    {
        return await _context.Positions.ToListAsync();
    }

    [HttpPost] //create employee
    public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
    {
        bool exists = await _context.Employees.AnyAsync(e => //dublicate check
                e.Name == employee.Name &&
                e.Surname == employee.Surname &&
                e.BirthDate == employee.BirthDate
            );
        
        if(exists) return BadRequest("Employee already exists");

        string ipToSearch = employee.IPaddress ?? "0.0.0.0";
        string countryCode = await _ipService.GetCountryCodeAsync(ipToSearch);

        employee.IPCountryCode = countryCode;
        
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        
        return Ok(employee); //return status 200 and created employee back to client
    }

    [HttpDelete("{id}")] //delete employee from db
    public async Task<ActionResult<Employee>> DeleteEmployee(int id)
    {
        var employees = await _context.Employees.FindAsync(id);

        if (employees == null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employees);
        await _context.SaveChangesAsync();

        return Ok(employees);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Employee>> UpdateEmployee(Employee employee, int id)
    {
        if (id != employee.EmployeeID)
        {
            return BadRequest();
        }

        bool exists = await _context.Employees.AnyAsync(e =>
        
            e.Name == employee.Name &&
            e.Surname == employee.Surname &&
            e.BirthDate == employee.BirthDate &&
            e.EmployeeID != id
        );
        
        if(exists) return BadRequest("Employee with this Name and DateBirth already exists");
        
        string  ipToSearch = employee.IPaddress ?? "0.0.0.0";
        employee.IPCountryCode = await _ipService.GetCountryCodeAsync(ipToSearch);
        
        _context.Entry(employee).State =  EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!_context.Employees.Any(e => e.EmployeeID == id))
                return NotFound();
            else
                throw;
        }
        
        return NoContent(); //status code 204
    }
    
}




























