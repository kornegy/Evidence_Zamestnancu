# Employee Management System (Evidence Zamestnancu)

A full-stack web application for managing employees and their job positions. Built with **Blazor WebAssembly** (Client) and **ASP.NET Core Web API** (Server), utilizing **Entity Framework Core** for database management.

## Project Overview
This project was developed to fulfill specific business requirements regarding data import, relational database management, and external API integration. It allows users to import `.json` files containing positions and employees, validates the data, and stores it in a SQL database.

## Technologies Used
* **Frontend:** Blazor WebAssembly, Bootstrap 5 (Flexbox layout)
* **Backend:** ASP.NET Core Web API, C# 12 / .NET 8
* **Database:** Microsoft SQL Server (LocalDB), Entity Framework Core (Code-First)
* **Logging:** Serilog (File & Console sinks)
* **External Services:** Integration with a third-party IP Geolocation API.

---

## Setup and Installation

### 1. Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
* IDE: JetBrains Rider or Visual Studio 2022
* SQL Server LocalDB (installed with Visual Studio/Rider)

### 2. Database Initialization
The project uses EF Core migrations. Run the following commands in the terminal of the **Server** project:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
### 3. Run the Application
Start the Server project. The Blazor WebAssembly client will be served automatically.
Note: Ensure the connection string in appsettings.json matches your local SQL instance.

## How to Test the Import Feature
To strictly follow relational database logic, files must be imported in this specific order:

Positions first: Click Choose file under Positions and upload positions.json.

Employees second: Click Choose file under Employees and upload employees.json.

Why? The system dynamically links employees to positions. If positions are not in the database yet, employees will be imported with a null position according to the requirements.

## Project Structure & Architecture
### Data Transfer Objects (DTOs)

* Files: EmployeeDTO.cs, PositionRootDTO.cs, EmployeeRootDTO.cs

Reasoning: Acting as a security and validation layer, DTOs decouple external JSON structures from internal Database Entities. This allows the system to handle varied naming conventions and nested data without exposing the database schema.

### Service Layer (IP Geolocation)

* Files: IIpService.cs, IpService.cs

Reasoning: Implements the Dependency Injection principle. By encapsulating external API logic (ip-api.com) within a dedicated service, the application remains modular, making it easy to swap providers or update integration logic in the future.

### Audit & Logging (Serilog)

Reasoning: Provides a robust "Black Box" for the server. Using a Rolling File approach (daily intervals), it creates a comprehensive audit trail of all imports, tracking added records, skipped duplicates, and runtime exceptions.

## Core Business Logic
### Smart Data Normalization

Strict Date Parsing: To prevent corruption from varying regional formats (US/EU), dates are strictly parsed using DateTime.ParseExact with InvariantCulture.

Relational Mapping: During import, the system performs a dynamic lookup to resolve string-based position names into PositionID foreign keys, maintaining relational integrity.

### Data Enrichment & Integrity

Automated Geolocation: Every new employee entry triggers an asynchronous enrichment process where IP addresses are resolved into standard ISO Country Codes before being persisted.

Triple-Check De-duplication: To prevent database bloating, uniqueness is enforced by verifying a composite key: Name + Surname + BirthDate.

### Conflict & Error Management

Concurrency Control: The update logic implements DbUpdateConcurrencyException handling to protect data from being accidentally overwritten during simultaneous administrative actions.

Fail-Safe Imports: The import logic is designed to be "resilient" — if one record fails (e.g., due to a malformed date), the system logs the error and continues processing the rest of the file instead of crashing the entire batch.
