using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace evidence_zamestancu.Services;

public interface IIpService
{
    Task<string> GetCountryCodeAsync(string ipAddress);
}

public class IpService : IIpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IpService> _logger;
    
    public IpService(HttpClient httpClient, ILogger<IpService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetCountryCodeAsync(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return "Bad IP";
        
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IpResponse>($"http://ip-api.com/json/{ipAddress}");
            
            _logger.LogInformation("API for address {address} was successfully retrieved",  ipAddress);

            return response?.CountryCode ?? "Unknown";
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving country code for {ipAddress}", ipAddress);
            return "Error";
        }
    }

    public class IpResponse() //prijima odpoved od servera ip-api (DTO)
    {
        public string? CountryCode { get; set; }
    }
}