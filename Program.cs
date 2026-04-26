var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Shared RNG used by endpoints to simulate occasional errors.
var rnd = Random.Shared;

// Weather endpoint: sometimes returns normal forecast, sometimes logs and returns a simulated error (500).
app.MapGet("/weatherforecast", (ILogger<Program> logger) =>
{
    // 20% chance to simulate an error (adjust probability as needed)
    if (rnd.NextDouble() < 0.20)
    {
        var ex = new InvalidOperationException("Simulated random error in WeatherForecast generation");
        // Log full exception so external log collectors (Grafana/Loki) capture it
        logger.LogError(ex, "Simulated error generated for testing");
        // Return a 500 response with a problem detail payload
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }

    var now = DateTime.UtcNow;
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(now.AddDays(index)),
            rnd.Next(-20, 55),
            summaries[rnd.Next(summaries.Length)]
        ))
        .ToArray();

    return Results.Ok(forecast);
})
.WithName("GetWeatherForecast");

// Manual test endpoint: immediately emits a logged error and 500 response
app.MapGet("/generate-error", (ILogger<Program> logger) =>
{
    var ex = new Exception("Manual test error generated via /generate-error");
    logger.LogError(ex, "Manual error endpoint invoked");
    return Results.Problem(detail: ex.Message, statusCode: 500);
})
.WithName("GenerateError");

app.MapGet("/", () => "App is running successfully!");
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
