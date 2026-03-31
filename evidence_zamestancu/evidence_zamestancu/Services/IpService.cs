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
    
    public IpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetCountryCodeAsync(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) 
            return "Bad IP";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<IpResponse>($"http://ip-api.com/json/{ipAddress}");

            return response?.CountryCode ?? "Unknown";
        }
        catch
        {
            return "Error";
        }
    }

    public class IpResponse() //prijima odpoved od servera ip-api (DTO)
    {
        public string? CountryCode { get; set; }
    }
}