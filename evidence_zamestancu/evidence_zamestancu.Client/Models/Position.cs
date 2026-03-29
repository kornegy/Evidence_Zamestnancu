using System.ComponentModel.DataAnnotations;

namespace evidence_zamestancu.Client.Models;

public class Position
{
    [Key]
    public int PositionID {get; set;}
    [Required]
    public string? PositionName {get; set;}
    
    // navigace pro EF (relace One-To-Many)
    public ICollection<Employee> Employees {get; set;}
}