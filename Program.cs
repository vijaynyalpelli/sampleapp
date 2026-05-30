var builder = WebApplication.CreateBuilder(args);

// Add OpenAPI documentation (Swagger/API explorer)
builder.Services.AddOpenApi();

var app = builder.Build();

// Expose OpenAPI UI only in Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enforce HTTPS (relies on reverse proxy in production)
app.UseHttpsRedirection();

// Weather forecast endpoint: returns a 5-day forecast with random temperatures and conditions
app.MapGet("/weatherforecast", () =>
{
    const int forecastDays = 5;
    const int minTemp = -20;
    const int maxTemp = 55;

    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    var now = DateTime.UtcNow;
    var forecast = Enumerable.Range(1, forecastDays)
        .Select(index => new WeatherForecast(
            DateOnly.FromDateTime(now.AddDays(index)),
            Random.Shared.Next(minTemp, maxTemp + 1),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return Results.Ok(forecast);
})
.WithName("GetWeatherForecast")
.Produces<WeatherForecast[]>(StatusCodes.Status200OK)
.WithOpenApi();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("Health")
    .Produces(StatusCodes.Status200OK)
    .WithOpenApi();

app.Run();

/// <summary>
/// Enables test discovery for WebApplicationFactory{Program} in unit tests.
/// </summary>
public partial class Program { }

/// <summary>
/// Represents a weather forecast for a specific date.
/// </summary>
/// <param name="Date">The forecast date (UTC).</param>
/// <param name="TemperatureC">Temperature in Celsius.</param>
/// <param name="Summary">Weather condition summary.</param>
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// Converts Celsius to Fahrenheit using the canonical formula: F = C * 9/5 + 32.
    /// </summary>
    public int TemperatureF => 32 + (int)Math.Round(TemperatureC * 9.0 / 5.0);
}
