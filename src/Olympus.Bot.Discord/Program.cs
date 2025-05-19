using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using Olympus.Application;
using Olympus.Application.Grpc;
using Olympus.Bot.Discord.Modules;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging();

builder.Services.AddSingleton(new GrpcHostConfig
{
  Host = "localhost",
  Port = 5000,
  UseHttps = false
});

builder.Services
  .AddGrpcServices()
  .AddDiscordGateway()
  .AddApplicationCommands();

var host = builder.Build();

host.AddModules(typeof(BaseInteractionModule<>).Assembly);
host.UseGatewayEventHandlers();

await host.RunAsync();
