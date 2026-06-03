using System.Net;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

public class ApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
        });
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        if (_factory != null)
        {
            await _factory.DisposeAsync();
        }
    }

    [Fact]
    public async Task RootEndpoint_ReturnsSuccessMessage()
    {
        // Act
        var response = await _client!.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("App is running successfully!", content);
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthyStatus()
    {
        // Act
        var response = await _client!.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
        Assert.NotNull(result);
        Assert.True(result.ContainsKey("status"));
        Assert.Equal("healthy", result["status"]);
    }

    [Fact]
    public async Task WeatherForecastEndpoint_ReturnsFiveDaysOfForecasts()
    {
        // Act
        var response = await _client!.GetAsync("/weatherforecast");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var forecasts = JsonSerializer.Deserialize<List<WeatherForecastData>>(content);
        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Count);
    }

    [Fact]
    public async Task WeatherForecastEndpoint_ReturnsForecastsWithValidData()
    {
        // Act
        var response = await _client!.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var forecasts = JsonSerializer.Deserialize<List<WeatherForecastData>>(content, options);

        // Assert
        Assert.NotNull(forecasts);
        foreach (var forecast in forecasts)
        {
            Assert.True(forecast.TemperatureC >= -20 && forecast.TemperatureC <= 55);
            Assert.NotNull(forecast.Summary);
            Assert.NotEmpty(forecast.Summary);
        }
    }

    [Fact]
    public async Task WeatherForecastEndpoint_TemperatureConversionIsCorrect()
    {
        // Act
        var response = await _client!.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();
        var forecasts = JsonSerializer.Deserialize<List<WeatherForecastData>>(content);

        // Assert
        Assert.NotNull(forecasts);
        foreach (var forecast in forecasts)
        {
            var expectedF = 32 + (int)Math.Round(forecast.TemperatureC * 9.0 / 5.0);
            Assert.Equal(expectedF, forecast.TemperatureF);
        }
    }
}

record WeatherForecastData(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)Math.Round(TemperatureC * 9.0 / 5.0);
}
