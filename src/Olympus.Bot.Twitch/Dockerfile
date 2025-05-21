# Olympus.Bot.Twitch - Production Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore packages for Twitch bot (and any shared projects it needs)
COPY Olympus.sln .
COPY src/Olympus.Bot.Twitch/*.csproj src/Olympus.Bot.Twitch/
COPY src/Olympus.Application/*.csproj src/Olympus.Application/
RUN dotnet restore src/Olympus.Bot.Twitch/Olympus.Bot.Twitch.csproj

# Build and publish the service
COPY . .
RUN dotnet publish src/Olympus.Bot.Twitch/Olympus.Bot.Twitch.csproj -c Release -o /app/out

# Runtime stage with .NET (no ASP.NET needed for a worker service)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out ./
ENV DOTNET_ENVIRONMENT=Production

# Run the Twitch bot service
ENTRYPOINT ["dotnet", "Olympus.Bot.Twitch.dll"]
