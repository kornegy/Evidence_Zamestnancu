using evidence_zamestancu.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace evidence_zamestancu.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    // vyrvareni tabulky v DB
    public DbSet<Employee> Employees {get; set;}
    public DbSet<Position> Positions {get; set;}
}