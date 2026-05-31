# MySampleApp

A minimal .NET 10 Weather API sample application demonstrating best practices for ASP.NET Core minimal APIs, error handling, and deployment to Azure App Service.

## Features

- **Minimal API**: Uses .NET 10 minimal APIs for lightweight, fast HTTP endpoints
- **Weather Forecast Endpoint**: Returns a 5-day forecast with random temperatures
- **Health Check**: Liveness probe for container orchestration
- **OpenAPI/Swagger**: Built-in API documentation in development
- **Docker Support**: Ready-to-deploy container image
- **Unit & Integration Tests**: Comprehensive test coverage (xUnit)

## Project Structure

```
MySampleApp/
??? Program.cs                     # Application entry point
??? MySampleApp.csproj             # Project configuration
??? Dockerfile                     # Container build
??? Properties/
?   ??? launchSettings.json       # Launch profiles
??? appsettings.json              # Configuration
??? appsettings.Development.json  # Development config
??? tests/
    ??? MySampleApp.Tests/
        ??? MySampleApp.Tests.csproj
        ??? WeatherForecastTests.cs
        ??? IntegrationTests.cs
```

## Getting Started

### Prerequisites
- .NET 10 SDK or later
- Docker (optional, for containerized deployment)

### Build & Run Locally

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run

# Run tests
dotnet test tests/MySampleApp.Tests
```

The application will start on `https://localhost:5001` (or http://localhost:5000 in development).

### Endpoints

- **GET `/`** — Health check
- **GET `/health`** — Liveness probe (returns `{ "status": "healthy" }`)
- **GET `/weatherforecast`** — Returns a 5-day forecast
- **GET `/openapi/v1.json`** — OpenAPI schema (development only)

## Docker Build & Run

```bash
# Build Docker image
docker build -t mysampleapp:latest .

# Run container
docker run -p 5000:8080 mysampleapp:latest
```

## Deployment to Azure

### Option 1: Using GitHub Actions CI/CD Pipeline

1. Create an Azure App Service with .NET 10 runtime (Windows or Linux)
2. Get the publish profile and store it in GitHub secrets as `AZURE_WEBAPP_PUBLISH_PROFILE`
3. Push to `main` branch — the workflow automatically deploys

### Option 2: Using Azure CLI

```bash
# Create resource group
az group create --name sre-demo-rg --location eastus

# Create App Service plan (Windows)
az appservice plan create --name sre-demo-plan --resource-group sre-demo-rg --sku B1

# Create Web App
az webapp create --resource-group sre-demo-rg --plan sre-demo-plan --name mysampleapp --runtime 'DOTNET|10.0'

# Deploy
dotnet publish -c Release -o ./publish
az webapp deploy --resource-group sre-demo-rg --name mysampleapp --src-path ./publish
```

## Configuration

### Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Set to `Development` to enable Swagger UI |
| `ASPNETCORE_URLS` | `https://+:443;http://+:80` | Listening URLs |

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Error Handling & Logging

- Errors are logged using Microsoft.Extensions.Logging
- Log levels: Critical, Error, Warning, Information, Debug, Trace
- In production, consider integrating with Grafana Loki, Application Insights, or similar

## Testing

Unit tests cover:
- Temperature Celsius-to-Fahrenheit conversion accuracy
- WeatherForecast record creation and nullability
- Correct rounding of temperature conversion

Integration tests cover:
- HTTP status codes and response formats
- Date sequence correctness
- Temperature range validation
- Health check endpoint

Run tests:
```bash
dotnet test tests/MySampleApp.Tests -v minimal
```

## Code Quality & Standards

- **C# 13 features**: Records, top-level statements, implicit usings
- **Nullable reference types**: Enabled for type safety
- **OpenAPI/Swagger**: Built-in API documentation
- **Structured logging**: Semantic, queryable logs
- **Tests**: xUnit with theory-based parameterized tests

## Performance Considerations

- **Minimal APIs**: Significantly faster than traditional ASP.NET routing
- **Random.Shared**: Thread-safe, lock-free random number generation
- **UTF-8 Everywhere**: Efficient default string encoding in .NET 10

## Security

- HTTPS is enforced via `app.UseHttpsRedirection()`
- Nullable reference types prevent null-pointer exceptions
- Input validation is built into endpoint definitions
- Secrets should be stored in Azure Key Vault, not in code or config files

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-feature`)
3. Commit changes (`git commit -am 'Add feature'`)
4. Push to the branch (`git push origin feature/my-feature`)
5. Create a Pull Request

## License

MIT License — see LICENSE file for details

## Support

For issues, questions, or contributions, please open a GitHub issue.

---

**Last Updated**: December 2024  
**Target Framework**: .NET 10  
**License**: MIT
