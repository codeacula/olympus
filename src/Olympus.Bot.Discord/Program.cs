using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using Olympus.Application;
using Olympus.Application.Common.Grpc;
using Olympus.Bot.Discord.Modules;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging();

builder.Services.Configure<GrpcHostConfig>(
  builder.Configuration.GetSection(nameof(GrpcHostConfig))
);

builder.Services
  .AddGrpcServices()
  .AddDiscordGateway()
  .AddApplicationCommands();

var host = builder.Build();

host.AddModules(typeof(BaseInteractionModule<>).Assembly);
host.UseGatewayEventHandlers();

await host.RunAsync();
