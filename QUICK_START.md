# Quick Reference: Running MySampleApp

## TL;DR (30 seconds)

```powershell
# 1. Navigate to project
cd C:\Users\Vijay\sre_demo\MySampleApp

# 2. Run the app
dotnet run

# 3. Open in browser
# http://localhost:5000/
# http://localhost:5000/health
# http://localhost:5000/weatherforecast
```

## Full Commands Cheat Sheet

### Build & Run
```powershell
dotnet restore              # Install NuGet packages
dotnet build                # Compile project
dotnet run                  # Run application
dotnet clean                # Remove build artifacts
```

### Run with Environment
```powershell
# Development (default)
dotnet run

# Production
$env:ASPNETCORE_ENVIRONMENT = "Production"; dotnet run

# Custom port
dotnet run --urls="http://localhost:5555"
```

### Test
```powershell
dotnet test tests/MySampleApp.Tests                    # All tests
dotnet test tests/MySampleApp.Tests -v detailed        # Verbose
dotnet test tests/MySampleApp.Tests/WeatherForecastTests.cs  # Specific file
```

### Publish & Deploy
```powershell
# Publish for Azure
dotnet publish -c Release -o ./publish

# Package as NuGet
dotnet pack

# Create Docker image
docker build -t mysampleapp:latest .
docker run -p 5000:8080 mysampleapp:latest
```

## API Endpoints

### Using Browser
```
http://localhost:5000/
http://localhost:5000/health
http://localhost:5000/weatherforecast
```

### Using curl / PowerShell
```powershell
curl http://localhost:5000/
curl http://localhost:5000/health
curl http://localhost:5000/weatherforecast

# PowerShell with JSON formatting
(Invoke-WebRequest -Uri http://localhost:5000/weatherforecast).Content | ConvertFrom-Json | ConvertTo-Json
```

### Using VS Code REST Client
Open `MySampleApp.http` and click "Send Request"

### Using Postman / Insomnia
- Method: `GET`
- URL: `http://localhost:5000/weatherforecast`

## Expected Output When Running

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**Note:** Use `http://localhost:5000` (not HTTPS) in development.

## Stop Application

Press `Ctrl+C` in the terminal.

## Port Already in Use?

```powershell
# Find process using port 5000
netstat -ano | findstr :5000

# Kill process (replace PID with actual number)
taskkill /PID <PID> /F

# Or use different port
dotnet run --urls="http://localhost:5555"
```

## Sample Responses

### GET `/weatherforecast`
```json
[
  {"date":"2024-12-20","temperatureC":12,"summary":"Mild","temperatureF":54},
  {"date":"2024-12-21","temperatureC":-5,"summary":"Freezing","temperatureF":23}
]
```

### GET `/health`
```json
{"status":"healthy"}
```

### GET `/`
```
"App is running successfully!"
```

## Endpoints Reference

| Path | Method | Returns | Purpose |
|------|--------|---------|---------|
| `/` | GET | String | Basic health check |
| `/health` | GET | JSON object | Liveness probe (K8s/Docker) |
| `/weatherforecast` | GET | JSON array | 5-day forecast |
| `/openapi/v1.json` | GET | OpenAPI schema | API docs (dev only) |

## Development vs Production

| Feature | Development | Production |
|---------|-------------|-----------|
| HTTP Port | 5000 | 80 |
| HTTPS Port | 5001 | 443 |
| OpenAPI/Swagger | ? Enabled | ? Disabled |
| Logging Level | Verbose | Minimal |
| Error Details | Full | Redacted |

## Troubleshooting

| Problem | Solution |
|---------|----------|
| App won't start | Check .NET SDK: `dotnet --version` (need 10.0+) |
| Port 5000 in use | Kill process or use `--urls` flag |
| Connection refused | Wait 2-3 seconds, app may still be starting |
| HTTPS error | Use `http://` instead in development |
| Packages missing | Run `dotnet restore` |
| Build fails | Run `dotnet clean` then `dotnet build` |

---

**Last Updated:** December 2024  
**Framework:** .NET 10  
**Project:** MySampleApp
