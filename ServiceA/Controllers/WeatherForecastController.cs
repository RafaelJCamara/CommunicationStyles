using Common;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using ServiceA.Clients;

namespace ServiceA.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ServiceBHttpClient _httpClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public WeatherForecastController(ServiceBHttpClient httpClient, IPublishEndpoint publishEndpoint)
    {
        _httpClient = httpClient;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        await _publishEndpoint.Publish(new SomeMessage("Rafael", "lordofnoring@email.com"));

        return await _httpClient.GetWfAsync();
    }
}
