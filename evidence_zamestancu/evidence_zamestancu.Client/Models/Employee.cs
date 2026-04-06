using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace evidence_zamestancu.Client.Models;

public class Employee
{
    [Key]
    public int EmployeeID { get; set; }
    [Required(ErrorMessage = "You must enter a name!")]
    public string Name {get; set;}
    
    [Required(ErrorMessage = "You must enter a surname!")]
    public string Surname {get; set;}
    
    [Required(ErrorMessage = "You must enter a birthdate!")]
    public DateTime BirthDate {get; set;}
    
    [ForeignKey("PositionID")]
    public Position? Position {get; set;}
    
    [Range(1, int.MaxValue, ErrorMessage = "Please, choose a position!")]
    public int? PositionID {get; set;}
    
    [Required(ErrorMessage = "You must enter an IPaddress!")]
    public string IPaddress {get; set;}
    
    public string? IPCountryCode {get; set;}
}