using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using Olympus.Application;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging();

builder.Services
  .AddDiscordGateway()
  .AddApplicationCommands()
  .AddGrpcServices();

var host = builder.Build();

host.AddModules(typeof(Program).Assembly);
host.UseGatewayEventHandlers();

await host.RunAsync();
