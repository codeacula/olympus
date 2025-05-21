# Olympus.Api - Production Dockerfile (multi-stage)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and csproj files, then restore dependencies
COPY Olympus.sln .
COPY src/Olympus.Api/*.csproj src/Olympus.Api/
COPY src/Olympus.Application/*.csproj src/Olympus.Application/
COPY src/Olympus.Infrastructure/*/*.csproj src/Olympus.Infrastructure/
RUN dotnet restore src/Olympus.Api/Olympus.Api.csproj

# Copy the entire source and build the application
COPY . .
RUN dotnet publish src/Olympus.Api/Olympus.Api.csproj -c Release -o /app/out

# Use a smaller runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/out ./

# Set environment to Production and expose port
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5000

# Run the web API
ENTRYPOINT ["dotnet", "Olympus.Api.dll"]
