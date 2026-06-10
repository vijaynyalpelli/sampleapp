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

#### 1. Restore & Build

```bash
# Navigate to project directory
cd C:\Users\Vijay\sre_demo\MySampleApp

# Restore dependencies
dotnet restore

# Build the project
dotnet build
```

#### 2. Run the Application

```bash
# Run in development mode (default)
dotnet run

# Or run in production mode
$env:ASPNETCORE_ENVIRONMENT = "Production"
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

The application will be available at:
- **Development**: `http://localhost:5000` (HTTP)
- **Production**: `https://localhost:5001` (HTTPS)

#### 3. Access the API Endpoints

**Option A: Browser (Easiest)**

Open these URLs directly in your browser:

| Endpoint | URL | Response |
|----------|-----|----------|
| Root Health | `http://localhost:5000/` | `"App is running successfully!"` |
| Liveness Probe | `http://localhost:5000/health` | `{"status":"healthy"}` |
| Weather Forecast | `http://localhost:5000/weatherforecast` | JSON array of 5 forecasts |

**Option B: PowerShell / Terminal (curl)**

```powershell
# Root health check
curl http://localhost:5000/

# Liveness probe
curl http://localhost:5000/health

# Weather forecast (all 5 days)
curl http://localhost:5000/weatherforecast

# Pretty-print forecast as JSON
(Invoke-WebRequest -Uri http://localhost:5000/weatherforecast).Content | ConvertFrom-Json | ConvertTo-Json
```

**Option C: VS Code REST Client**

Open the `MySampleApp.http` file in VS Code and click **"Send Request"** on any endpoint:

```http
### Health Check
GET http://localhost:5000/

### Liveness Probe
GET http://localhost:5000/health

### Weather Forecast
GET http://localhost:5000/weatherforecast
```

**Option D: Postman or Insomnia**

1. Create a new GET request
2. Paste URL: `http://localhost:5000/weatherforecast`
3. Click Send

#### 4. Stop the Application

Press `Ctrl+C` in the terminal to stop the running application.

### Sample API Responses

**GET `/weatherforecast`** returns:
```json
[
  {
    "date": "2024-12-20",
    "temperatureC": 12,
    "summary": "Mild",
    "temperatureF": 54
  },
  {
    "date": "2024-12-21",
    "temperatureC": -5,
    "summary": "Freezing",
    "temperatureF": 23
  },
  ...
]
```

**GET `/health`** returns:
```json
{
  "status": "healthy"
}
```

### Troubleshooting

| Issue | Solution |
|-------|----------|
| **Port 5000 already in use** | Kill the process using `netstat -ano \| findstr :5000` or set custom port: `dotnet run --urls="http://localhost:5555"` |
| **"Connection refused"** | Ensure app is running with `dotnet run` and wait 2-3 seconds for startup |
| **HTTPS certificate error** | Use `http://` (not `https://`) in development; use `http://localhost:5000` |
| **"dotnet" not recognized** | Install .NET 10 SDK from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) |

### Run Tests

```bash
# Run all unit & integration tests
dotnet test tests/MySampleApp.Tests

# Run with verbose output
dotnet test tests/MySampleApp.Tests -v detailed

# Run specific test file
dotnet test tests/MySampleApp.Tests/WeatherForecastTests.cs
```

### Endpoints

- **GET `/`** � Health check (returns `"App is running successfully!"`)
- **GET `/health`** � Liveness probe (returns `{ "status": "healthy" }`)
- **GET `/weatherforecast`** � 5-day forecast with random temperatures
- **GET `/openapi/v1.json`** � OpenAPI/Swagger schema (development only)

## Docker Build & Run

```bash
# Build Docker image
docker build -t mysampleapp:latest .

# Run container
docker run -p 5000:8080 mysampleapp:latest
```

## Push Docker Image to GitHub Container Registry (GHCR)

### Prerequisites
- GitHub account with repository access
- Personal Access Token (PAT) with `write:packages` and `read:packages` scopes
  - Create at: https://github.com/settings/tokens

### Steps

#### 1. Create Personal Access Token (PAT)

1. Go to GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Click **"Generate new token (classic)"**
3. Select scopes:
   - ✅ `write:packages`
   - ✅ `read:packages`
4. Click **"Generate token"** and copy the token

#### 2. Login to GHCR

```bash
# Using --password-stdin (recommended)
echo "YOUR_PAT" | docker login ghcr.io -u YOUR_GITHUB_USERNAME --password-stdin

# Example:
# echo "ghp_xxxxxxxxxxxx" | docker login ghcr.io -u vijaynyalpelli --password-stdin
```

#### 3. Tag the Docker Image

```bash
# Format: ghcr.io/OWNER/IMAGE_NAME:TAG
docker tag mysampleapp:latest ghcr.io/YOUR_GITHUB_USERNAME/mysampleapp:latest

# Optional: Add version tag
docker tag mysampleapp:latest ghcr.io/YOUR_GITHUB_USERNAME/mysampleapp:v1.0.0

# Example:
# docker tag mysampleapp:latest ghcr.io/vijaynyalpelli/mysampleapp:latest
```

#### 4. Push the Image to GHCR

```bash
docker push ghcr.io/YOUR_GITHUB_USERNAME/mysampleapp:latest

# Optional: Push version tag
docker push ghcr.io/YOUR_GITHUB_USERNAME/mysampleapp:v1.0.0

# Example:
# docker push ghcr.io/vijaynyalpelli/mysampleapp:latest
```

#### 5. Make Package Public (Optional)

1. Go to GitHub → Your profile → Packages
2. Click on `mysampleapp`
3. Click **"Package settings"**
4. Change visibility to **"Public"** if you want the image publicly accessible

#### 6. Pull the Image

```bash
# Pull from GHCR
docker pull ghcr.io/YOUR_GITHUB_USERNAME/mysampleapp:latest

# Example:
# docker pull ghcr.io/vijaynyalpelli/mysampleapp:latest
```

### Troubleshooting

| Issue | Solution |
|-------|----------|
| **"context deadline exceeded"** | Check VPN/proxy settings; ensure Docker Desktop is running |
| **"unauthorized: authentication required"** | Verify PAT has `write:packages` scope; use username (not email) |
| **"denied: permission_denied"** | Ensure PAT has correct permissions; regenerate token if needed |

## Deployment to Azure

### Option 1: Using GitHub Actions CI/CD Pipeline

1. Create an Azure App Service with .NET 10 runtime (Windows or Linux)
2. Get the publish profile and store it in GitHub secrets as `AZURE_WEBAPP_PUBLISH_PROFILE`
3. Push to `main` branch � the workflow automatically deploys

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

MIT License � see LICENSE file for details

## Support

For issues, questions, or contributions, please open a GitHub issue.

---

**Last Updated**: December 2024  
**Target Framework**: .NET 10  
**License**: MIT
