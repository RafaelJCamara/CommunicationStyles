using Microsoft.AspNetCore.Mvc;
using ServiceA.Clients;

namespace ServiceA.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ServiceBHttpClient _httpClient;

    public WeatherForecastController(ServiceBHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        return await _httpClient.GetWfAsync();
    }
}
