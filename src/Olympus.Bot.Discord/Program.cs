using System.Net.Security;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
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

var useHttps = grpcSection.GetValue<bool>(nameof(GrpcHostConfig.UseHttps));
Console.WriteLine(
  $"Using gRPC host: {grpcSection[nameof(GrpcHostConfig.Host)]}:{grpcSection[nameof(GrpcHostConfig.Port)]} (HTTPS: {useHttps})"
);

// Enable HTTP/2 without TLS when using plain HTTP
GrpcClientFactory.AllowUnencryptedHttp2 = true;

// Configure GrpcChannel explicitly
builder.Services.AddSingleton(services =>
{
  var options = services.GetRequiredService<IOptions<GrpcHostConfig>>().Value;
  var loggerFactory = services.GetRequiredService<ILoggerFactory>();

  // Create channel options
  var channelOptions = new GrpcChannelOptions
  {
    LoggerFactory = loggerFactory,
    ServiceProvider = services,
    HttpHandler = new SocketsHttpHandler
    {
      MaxConnectionsPerServer = 100,
      PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
      EnableMultipleHttp2Connections = true,
      SslOptions = new SslClientAuthenticationOptions
      {
        EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12,
        RemoteCertificateValidationCallback = (_, _, _, _) => true,
      },
    },
  };

  // Construct the appropriate URI based on config
  var scheme = options.UseHttps ? "https" : "http";
  var address = $"{scheme}://{options.Host}:{options.Port}";
  Console.WriteLine($"Creating gRPC channel with address: {address}");

  return GrpcChannel.ForAddress(address, channelOptions);
});

builder.Services
  .AddSingleton<GrpcClientLoggingInterceptor>()
  .AddSingleton<IGrpcClient, Olympus.Application.Common.Grpc.GrpcClient>()
  .AddDiscordGateway()
  .AddApplicationCommands();

var host = builder.Build();

host.AddModules(typeof(BaseInteractionModule<>).Assembly);
host.UseGatewayEventHandlers();

await host.RunAsync();
