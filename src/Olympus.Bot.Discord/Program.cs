using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using Olympus.Bot.Discord.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging();
builder.Services.AddSingleton<IOlympusApiGrpcClient, OlympusApiGrpcClient>();

builder.Services
  .AddDiscordGateway()
  .AddApplicationCommands();

var host = builder.Build();

host.AddModules(typeof(Program).Assembly);
host.UseGatewayEventHandlers();

await host.RunAsync();
