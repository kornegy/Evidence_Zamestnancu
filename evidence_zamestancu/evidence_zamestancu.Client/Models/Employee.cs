using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace evidence_zamestancu.Client.Models;

public class Employee
{
    [Key]
    public int EmployeeID { get; set; }
    
    [Required(ErrorMessage = "You must enter a name.")]
    public string Name {get; set;}
    
    [Required(ErrorMessage = "You must enter a surname.")]
    public string Surname {get; set;}
    
    [Required]
    public DateTime BirthDate {get; set;}
    
    [Required]
    public int PositionID {get; set;}
    
    [ForeignKey(nameof(PositionID))]
    public Position? Position {get; set;}
    
    [Required]
    [RegularExpression(@"^(\d{1,3}\.){3}\d{1,3}$",ErrorMessage ="Invalid format of IP address")]
    public string IPaddress {get; set;}
    
    [Required]
    public string? IPCountryCode {get; set;}
}