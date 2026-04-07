using evidence_zamestancu.Client.Models;
using evidence_zamestancu.Client.Models.DTO;
using evidence_zamestancu.Data;
using evidence_zamestancu.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;


namespace evidence_zamestancu.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IIpService _ipService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(AppDbContext context, IIpService ipService, ILogger<EmployeesController> logger)
    {
        _context = context;
        _ipService = ipService;
        _logger = logger;
    }
 
    [HttpGet] //zobrazeni zamestnanca
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
        return await _context.Employees
            .Include(e => e.Position)
            .ToListAsync();
    }

    [HttpGet("{id}")] //hledani jednoho zamestanca z db
    public async Task<ActionResult<Employee>> GetOneEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }
        return employee;
    }

    [HttpGet("position")]
    public async Task<ActionResult<IEnumerable<Position>>> GetPositions()
    {
        return await _context.Positions.ToListAsync();
    }

    [HttpPost] //vytvoreni zamestancaa
    public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
    {
        bool exists = await _context.Employees.AnyAsync(e => //overeni duplikatu
            e.Name == employee.Name &&
            e.Surname == employee.Surname &&
            e.BirthDate == employee.BirthDate
        );
            
        if(exists) return BadRequest("Employee already exists");
        
        try
        {
            string ipToSearch = employee.IPaddress ?? "0.0.0.0";
            string countryCode = await _ipService.GetCountryCodeAsync(ipToSearch);

            employee.IPCountryCode = countryCode;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Employee with ID {employee.EmployeeID} successfully created");
            
            return Ok(employee); //vraci statuscode 200 a zamestnanca
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while adding employee with ID {employee.EmployeeID}");
            
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")] //vymazani zamestanca
    public async Task<ActionResult<Employee>> DeleteEmployee(int id)
    {
        var employees = await _context.Employees.FindAsync(id);

        if (employees == null)
        {
            return NotFound();
        }

        try
        {
            _context.Employees.Remove(employees);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Employee with ID {employees.EmployeeID} successfully deleted");
            return Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee with ID {employees.EmployeeID}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")] //aktualizace data pro zamestanca
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

        if (exists) return BadRequest("Employee with this Name and DateBirth already exists");
        
        try
        {
            string ipToSearch = employee.IPaddress ?? "0.0.0.0";
            employee.IPCountryCode = await _ipService.GetCountryCodeAsync(ipToSearch);

            _context.Entry(employee).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Employee with ID {ID} successfully updated", employee.EmployeeID);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!_context.Employees.Any(e => e.EmployeeID == id))
            {
                _logger.LogWarning("Employee with ID {ID} not found", employee.EmployeeID);
                return NotFound();
            }
            else
            {
                _logger.LogError("Concurrency while updating Employee with ID {ID}", employee.EmployeeID);
                throw;
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error while updating Employee with ID {ID} and Name {NAME}", employee.EmployeeID, employee.Name);
            return StatusCode(500, "Internal server error");
        }
        return NoContent();
    }

    [HttpPost("import")]
    public async Task<ActionResult> ImportEmployees([FromBody]List<EmployeeDTO> importedEmployee)
    {
        if(importedEmployee == null || !importedEmployee.Any())
            return BadRequest("File has wrong format or is empty!");
        
        foreach (var item in importedEmployee)
        {
            try
            {
                DateTime parsedData = DateTime.ParseExact(item.BirthDate, "yyyy/mm/dd", CultureInfo.InvariantCulture); //format ne zalezi na nastaveni pocitace
                
                bool exists = await _context.Employees.AnyAsync(e =>
                    e.Name == item.Name &&
                    e.Surname == item.Surname &&
                    e.BirthDate == parsedData);

                if (exists) continue;

                int? positionId = null;
                if (!string.IsNullOrWhiteSpace(item.Position))
                {
                    var pos = await _context.Positions
                        .FirstOrDefaultAsync(p => p.PositionName == item.Position);

                    if (pos != null)
                    {
                        positionId = pos.PositionID;
                    }
                }

                string ipToSearch = item.IpAddress ?? "0.0.0.0";
                string countryCode = await _ipService.GetCountryCodeAsync(ipToSearch);

                var newEmployee = new Employee
                {
                    Name = item.Name,
                    Surname = item.Surname,
                    BirthDate = parsedData,
                    PositionID = positionId,
                    IPaddress = item.IpAddress,
                    IPCountryCode = countryCode
                };

                _context.Employees.Add(newEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while importing Employees");
            }
        }
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Employees imported successfully");
        return Ok();
    }
}




























