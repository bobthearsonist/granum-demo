# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Granum.sln ./
COPY Granum.Api/Granum.Api.csproj ./Granum.Api/
COPY Granum.Tests/Granum.Tests.csproj ./Granum.Tests/
COPY Granum.IntegrationTests/Granum.IntegrationTests.csproj ./Granum.IntegrationTests/
COPY global.json ./

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build the application
WORKDIR /src/Granum.Api
RUN dotnet build -c Release -o /app/build --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Expose port
EXPOSE 5134

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5134
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:5134/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "Granum.Api.dll"]
