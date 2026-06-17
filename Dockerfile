# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore
COPY MySampleApp.csproj ./
RUN dotnet restore

# Copy source code
COPY Program.cs ./
COPY appsettings.json ./
COPY appsettings.Development.json ./
COPY Properties ./Properties

# Publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "MySampleApp.dll"]
