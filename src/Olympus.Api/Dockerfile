FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5034
EXPOSE 5035

ENV ASPNETCORE_URLS=http://+:5034;https://+:5035
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

COPY ./dev-certs/aspnetapp.pfx /https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=YourSecurePassword

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Development
WORKDIR /src
COPY ["src/Olympus.Api/Olympus.Api.csproj", "src/Olympus.Api/"]
RUN dotnet restore "src/Olympus.Api/Olympus.Api.csproj"
COPY . .
WORKDIR "/src/src/Olympus.Api"
RUN dotnet build "Olympus.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Development
RUN dotnet publish "Olympus.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Olympus.Api.dll"]
