using Microsoft.AspNetCore.Mvc;
using Weather.Api.Services;

namespace Weather.Api.Controllers;

[ApiController]
public class WeatherController(IWeatherService weatherService) : ControllerBase
{
    [HttpGet("weather/{city}")]
    public async Task<IActionResult> GetWeatherForCity(string city)
    {
        var weather = await weatherService.GetCurrentWeatherAsync(city);
        if (weather is null)
        {
            return NotFound();
        }

        return Ok(weather);
    }
}