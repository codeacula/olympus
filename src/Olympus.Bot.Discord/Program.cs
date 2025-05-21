using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using Olympus.Application;
using Olympus.Application.Grpc;
using Olympus.Bot.Discord.Modules;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(cfg => cfg.AddFilter("Grpc", LogLevel.Debug));

var grpcSection = builder.Configuration.GetSection(nameof(GrpcHostConfig)) ?? throw new InvalidOperationException(
    "The configuration section for GrpcHostConfig is missing."
  );

builder.Services.Configure<GrpcHostConfig>(
  grpcSection
);

var useHttps = grpcSection.GetValue<bool>(nameof(GrpcHostConfig.UseHttps));
Console.WriteLine(
  $"Using gRPC host: {grpcSection[nameof(GrpcHostConfig.Host)]}:{grpcSection[nameof(GrpcHostConfig.Port)]} (HTTPS: {useHttps})"
);

builder.Services
  .AddGrpcClientServices()
  .AddDiscordGateway()
  .AddApplicationCommands();

var host = builder.Build();

host.AddModules(typeof(BaseInteractionModule<>).Assembly);
host.UseGatewayEventHandlers();

await host.RunAsync();
