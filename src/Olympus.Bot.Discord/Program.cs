using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using Olympus.Application;
using Olympus.Application.Common.Grpc;
using Olympus.Bot.Discord.Modules;
using ProtoBuf.Grpc.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(cfg => cfg.AddFilter("Grpc", LogLevel.Debug));

var grpcSection = builder.Configuration.GetSection(nameof(GrpcHostConfig)) ?? throw new InvalidOperationException(
    "The configuration section for GrpcHostConfig is missing."
  );

builder.Services.Configure<GrpcHostConfig>(
  grpcSection
);

Console.WriteLine(
  $"Using gRPC host: {grpcSection[nameof(GrpcHostConfig.Host)]}:{grpcSection[nameof(GrpcHostConfig.Port)]}"
);

GrpcClientFactory.AllowUnencryptedHttp2 = true;

builder.Services
  .AddSingleton<GrpcClientLoggingInterceptor>()
  .AddSingleton<IGrpcClient, Olympus.Application.Common.Grpc.GrpcClient>()
  .AddDiscordGateway()
  .AddApplicationCommands();

var host = builder.Build();

host.AddModules(typeof(BaseInteractionModule<>).Assembly);
host.UseGatewayEventHandlers();

await host.RunAsync();
