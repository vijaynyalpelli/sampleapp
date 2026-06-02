var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Root endpoint
app.MapGet("/", () => "App is running successfully!");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// Weather forecast endpoint
app.MapGet("/weatherforecast", () =>
{
    var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
    var forecast = Enumerable.Range(1, 5)
        .Select(index => new WeatherForecast(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(index)),
            Random.Shared.Next(-20, 56),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return Results.Ok(forecast);
});

app.Run();

public partial class Program { }

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)Math.Round(TemperatureC * 9.0 / 5.0);
}
