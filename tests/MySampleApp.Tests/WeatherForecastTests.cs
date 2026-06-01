using Xunit;

namespace MySampleApp.Tests;

/// <summary>
/// Unit tests for WeatherForecast business logic.
/// </summary>
public class WeatherForecastTests
{
    [Theory]
    [InlineData(0, 32)]
    [InlineData(100, 212)]
    [InlineData(-40, -40)]
    [InlineData(20, 68)]
    [InlineData(-10, 14)]
    public void TemperatureF_ConvertsCorrectly(int celsius, int expectedFahrenheit)
    {
        // Arrange & Act
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.UtcNow), celsius, "Test");

        // Assert
        Assert.Equal(expectedFahrenheit, forecast.TemperatureF);
    }

    [Fact]
    public void TemperatureF_RoundsToNearestInteger()
    {
        // Arrange: 15°C should be 59°F (15 * 9/5 + 32 = 59)
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.UtcNow), 15, "Mild");

        // Act & Assert
        Assert.Equal(59, forecast.TemperatureF);
    }

    [Fact]
    public void WeatherForecast_CreatesWithAllProperties()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        const int tempC = 25;
        const string summary = "Warm";

        // Act
        var forecast = new WeatherForecast(date, tempC, summary);

        // Assert
        Assert.Equal(date, forecast.Date);
        Assert.Equal(tempC, forecast.TemperatureC);
        Assert.Equal(summary, forecast.Summary);
    }

    [Fact]
    public void WeatherForecast_AllowsNullSummary()
    {
        // Arrange & Act
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.UtcNow), 20, null);

        // Assert
        Assert.Null(forecast.Summary);
    }
}
