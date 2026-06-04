# Docker Setup Guide for MySampleApp

## Overview
This guide explains how to containerize and run your .NET 10 ASP.NET Core application using Docker.

---

## Files Created

### 1. **Dockerfile** - Multi-stage build configuration
### 2. **.dockerignore** - Excludes unnecessary files from Docker build context
### 3. **docker-compose.yml** - Orchestration for running the container

---

## Understanding the Dockerfile (Multi-Stage Build)

### **Stage 1: Build Stage**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY MySampleApp.csproj .
RUN dotnet restore "MySampleApp.csproj"
COPY . .
RUN dotnet build "MySampleApp.csproj" -c Release -o /app/build
```
- Uses the **SDK image** (larger, ~500MB) which includes build tools
- Copies only the project file first for better layer caching
- Restores NuGet dependencies
- Builds the application in Release mode

**Why this step?** To compile your C# code into IL (Intermediate Language) and check for errors.

---

### **Stage 2: Publish Stage**
```dockerfile
FROM build AS publish
RUN dotnet publish "MySampleApp.csproj" -c Release -o /app/publish
```
- Inherits from the build stage (reuses the built artifacts)
- Publishes the application to `/app/publish` folder
- Creates a self-contained or framework-dependent package

**Why this step?** Separates the publish output from build artifacts to optimize the final image size.

---

### **Stage 3: Runtime Stage**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000 5001
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1
ENTRYPOINT ["dotnet", "MySampleApp.dll"]
```
- Uses **Runtime image** (smaller, ~200MB) - no SDK or build tools
- Copies only the published binaries from the publish stage
- Exposes ports 5000 (HTTP) and 5001 (HTTPS)
- Sets environment variables for production
- Includes a health check that calls the `/health` endpoint
- Starts the application by running the DLL

**Benefits of this stage:**
- Final image is much smaller (no build tools)
- More secure (less attack surface)
- Optimized for production

---

## Why Multi-Stage Builds?

| Stage | Size | Purpose |
|-------|------|---------|
| SDK (Build) | ~500MB | Compile and test code |
| ASP.NET Runtime (Final) | ~200MB | Run the app only |
| **Old method (single stage)** | **~700MB** | Keeps everything (bloated) |
| **New method (multi-stage)** | **~200MB** | Only runtime needed |

**Result:** ~70% smaller Docker image! ??

---

## .dockerignore File

Prevents unnecessary files from being copied into the Docker build context:
- `.git`, `.github` - Version control files
- `bin/`, `obj/` - Build artifacts
- `.vs/`, `.vscode/` - IDE files
- `node_modules/` - Node dependencies (if any)
- `Dockerfile`, `docker-compose.yml` - Docker files themselves

**Benefits:** Faster builds, smaller context size, cleaner images.

---

## docker-compose.yml Configuration

```yaml
services:
  sampleapp:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: mysampleapp
    ports:
      - "5000:5000"
      - "5001:5001"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5000
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 3s
      retries: 3
      start_period: 40s
```

**Explanation:**
- `build.context: .` - Builds from current directory
- `build.dockerfile: Dockerfile` - Uses the Dockerfile we created
- `container_name: mysampleapp` - Names the running container
- `ports: "5000:5000"` - Maps host port 5000 to container port 5000
- `environment` - Sets environment variables inside container
- `restart: unless-stopped` - Auto-restarts if container crashes
- `healthcheck` - Periodically checks if app is healthy

---

## How to Build and Run

### **Method 1: Using Docker Compose (Recommended)**

#### Build the image:
```bash
docker-compose build
```

#### Run the container:
```bash
docker-compose up
```

#### Run in detached mode (background):
```bash
docker-compose up -d
```

#### Stop the container:
```bash
docker-compose down
```

---

### **Method 2: Using Docker Commands Directly**

#### Build the image:
```bash
docker build -t mysampleapp:latest .
```

#### Run the container:
```bash
docker run -d -p 5000:5000 -p 5001:5001 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  --name mysampleapp \
  --health-cmd="curl -f http://localhost:5000/health || exit 1" \
  --health-interval=30s \
  --health-timeout=3s \
  --health-retries=3 \
  --health-start-period=40s \
  mysampleapp:latest
```

---

## Testing the Containerized App

### **Check if container is running:**
```bash
docker ps
```

### **View container logs:**
```bash
docker logs mysampleapp
```

### **Test endpoints:**

#### Root endpoint:
```bash
curl http://localhost:5000/
```
Expected: `App is running successfully!`

#### Health check:
```bash
curl http://localhost:5000/health
```
Expected: `{"status":"healthy"}`

#### Weather forecast:
```bash
curl http://localhost:5000/weatherforecast
```
Expected: JSON array with 5 weather forecasts

### **Check container health:**
```bash
docker ps --format "table {{.Names}}\t{{.Status}}"
```

---

## Common Docker Commands

| Command | Purpose |
|---------|---------|
| `docker build -t name:tag .` | Build image from Dockerfile |
| `docker run -d -p 8080:5000 image` | Run container in detached mode |
| `docker ps` | List running containers |
| `docker logs container_name` | View container logs |
| `docker exec -it container_name bash` | Enter container shell |
| `docker stop container_name` | Stop a running container |
| `docker rm container_name` | Remove a stopped container |
| `docker images` | List all images |
| `docker rmi image_name` | Remove an image |
| `docker-compose up -d` | Start services in background |
| `docker-compose down` | Stop and remove services |

---

## Environment Variables in Container

The Dockerfile sets these environment variables:

```dockerfile
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
```

**What they do:**
- `ASPNETCORE_URLS=http://+:5000` - Listens on all interfaces (0.0.0.0) on port 5000
- `ASPNETCORE_ENVIRONMENT=Production` - Runs in production mode (disables development UI, error pages)

---

## Health Check Explanation

```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1
```

- **interval=30s** - Check every 30 seconds
- **timeout=3s** - Fails if no response within 3 seconds
- **start-period=40s** - Grace period before first health check
- **retries=3** - Fails container after 3 consecutive failed checks
- **CMD curl -f http://localhost:5000/health** - Calls your `/health` endpoint

Your app already has this endpoint that returns `{"status":"healthy"}` ?

---

## Production Deployment Tips

1. **Use specific version tags:**
   ```bash
   docker build -t mysampleapp:1.0.0 .
   ```
   Instead of `:latest` (which is ambiguous)

2. **Use environment variables for configuration:**
   ```bash
   docker run -e ASPNETCORE_ENVIRONMENT=Production \
     -e LOG_LEVEL=Information \
     mysampleapp:1.0.0
   ```

3. **Run as non-root user** (optional security enhancement):
   ```dockerfile
   USER app
   ```

4. **Set resource limits:**
   ```bash
   docker run -m 512m --cpus="1" mysampleapp:latest
   ```
   (512MB RAM, 1 CPU core max)

5. **Use Docker registries:**
   ```bash
   docker tag mysampleapp:latest myregistry.azurecr.io/mysampleapp:latest
   docker push myregistry.azurecr.io/mysampleapp:latest
   ```

---

## Troubleshooting

### **Container exits immediately:**
```bash
docker logs mysampleapp
```
Check the logs for errors.

### **Port already in use:**
```bash
docker run -p 8080:5000 mysampleapp:latest
```
Map to a different host port (8080 ? 5000).

### **Health check failing:**
Ensure the `/health` endpoint is returning the expected response.

### **Large image size:**
Use `.dockerignore` to exclude unnecessary files, or use Alpine Linux base images (smaller but less tested).

---

## Next Steps

1. ? Build the Docker image
2. ? Test locally with `docker run` or `docker-compose up`
3. ? Push to Docker registry (Docker Hub, Azure Container Registry, etc.)
4. ? Deploy to orchestration platform (Kubernetes, Azure Container Instances, etc.)

---

## Summary

Your app is now fully containerized with:
- ? Multi-stage optimized Dockerfile (200MB final image)
- ? Health checks for production monitoring
- ? Docker Compose for easy orchestration
- ? Environment-based configuration
- ? .dockerignore for optimized build context

Happy containerizing! ??
