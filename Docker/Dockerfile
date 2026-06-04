# Build stage
# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project file and restore dependencies
COPY MySampleApp.csproj .
RUN dotnet restore "MySampleApp.csproj"

# Copy entire source code
COPY . .

# Build the application
RUN dotnet build "MySampleApp.csproj" -c Release -o /app/build

# Stage 2: Publish stage
FROM build AS publish

# Publish the application
RUN dotnet publish "MySampleApp.csproj" -c Release -o /app/publish

# Stage 3: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

# Copy published app from publish stage
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 5000 5001

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "MySampleApp.dll"]