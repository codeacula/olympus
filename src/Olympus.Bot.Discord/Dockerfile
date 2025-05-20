FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dev
WORKDIR /app

# Copy csproj and restore dependencies
COPY ["src/Olympus.Bot.Discord/Olympus.Bot.Discord.csproj", "src/Olympus.Bot.Discord/"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "src/Olympus.Bot.Discord/Olympus.Bot.Discord.csproj"

# Copy the application source code
COPY ["src/Olympus.Application/", "src/Olympus.Application/"]
COPY ["src/Olympus.Domain/", "src/Olympus.Domain/"]
COPY ["src/Olympus.Bot.Discord/", "src/Olympus.Bot.Discord/"]

# Add metadata labels
LABEL description="Development container for Olympus Discord Bot"
LABEL project="olympus"

# Use dotnet watch for hot reload
ENTRYPOINT ["dotnet", "watch", "run", "--project", "src/Olympus.Bot.Discord/Olympus.Bot.Discord.csproj"]
