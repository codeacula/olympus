FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine@sha256:30fdbd1b5963bba6ed66190d72d877b750d4203a671c9b54592f4551b8c5a087 AS base
WORKDIR /app
EXPOSE 5034

ENV ASPNETCORE_URLS=http://+:5034

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine@sha256:2244f80ac7179b0feaf83ffca8fe82d31fbced5b7e353755bf9515a420eba711 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Olympus.Api/Olympus.Api.csproj", "src/Olympus.Api/"]
RUN dotnet restore "src\Olympus.Api\Olympus.Api.csproj"
COPY . .
WORKDIR "/src/src/Olympus.Api"
RUN dotnet build "Olympus.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Olympus.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Olympus.Api.dll"]
