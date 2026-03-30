using System.Text.Json;
using System.Text.Json.Nodes;
using evidence_zamestancu.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace evidence_zamestancu.Data;

public class DataSeeder
{
    public static void Seed(AppDbContext context)
    {
        context.Database.EnsureCreated();
        
        if (!context.Positions.Any())
        {
            var positionsJson = File.ReadAllText("SeedData/postions.json");
            
            var rootNode = JsonNode.Parse(positionsJson); //dynamicke drevo

            var positionArray = rootNode?["positions"]?.AsArray();
            
            if (positionArray != null)
            {
                foreach (var item in positionArray)
                {
                    string positionName = item.ToString();
                    context.Positions.Add(new Position{PositionName =  positionName});
                }
                context.SaveChanges();
            }
        }

        if (!context.Employees.Any())
        {
            var employeesJson = File.ReadAllText("SeedData/employes.json");
            var rootNode = JsonNode.Parse(employeesJson);
            
            var employeesArray = rootNode?["employees"]?.AsArray();

            if (employeesArray != null)
            {
                var dbPosition = context.Positions.ToList();
                
                foreach (var node in employeesArray)
                {
                    string name =  node?["Name"]?.ToString() ?? "Unknown";
                    string surname =  node?["Surname"]?.ToString() ?? "Unknown";
                    string datebirth = node?["BirthDate"]?.ToString() ?? "";
                    string position =  node?["Position"]?.ToString() ?? "";
                    string ipAddress =  node?["IpAddress"]?.ToString() ?? "0.0.0.0";
                    
                    var positionDB = dbPosition.FirstOrDefault(x => x.PositionName == position); //zjistime id pozice 
                    int positionID = positionDB != null ? positionDB.PositionID : 1;
                    
                    DateTime.TryParse(datebirth, out DateTime parsedDate);

                    context.Employees.Add(new Employee
                    {
                        Name = name,
                        Surname = surname,
                        BirthDate = parsedDate,
                        PositionID = positionID,
                        IPaddress = ipAddress,
                        IPCountryCode = ""
                    });

                }
                context.SaveChanges();
            }
        }
    }
}