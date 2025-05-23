using System.Net.Security;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Olympus.Application.Ai.Services;
using Olympus.Application.Common.Behaviors;
using Olympus.Application.Common.Grpc;
using Olympus.Application.Grpc;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc.Server;

using OlympusGrpcClient = Olympus.Application.Grpc.GrpcClient;

namespace Olympus.Application;

public static class ServiceCollectionExtension
{
  public static IServiceCollection AddGrpcClientServices(this IServiceCollection services)
  {
    _ = services.AddGrpcCommonServices();
    return services;
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5359:Do Not Disable Certificate Validation", Justification = "Needed for development purposes")]
  private static IServiceCollection AddGrpcCommonServices(this IServiceCollection services)
  {
    _ = services.AddSingleton<IGrpcClient, OlympusGrpcClient>()
      .AddSingleton<GrpcClientLoggingInterceptor>()
      .AddSingleton(services =>
    {
      var options = services.GetRequiredService<IOptions<GrpcHostConfig>>().Value;
      var loggerFactory = services.GetRequiredService<ILoggerFactory>();

      // Enable HTTP/2 without TLS when using plain HTTP
      GrpcClientFactory.AllowUnencryptedHttp2 = true;

      // Create channel options
      var channelOptions = new GrpcChannelOptions
      {
        LoggerFactory = loggerFactory
      };

      // Only configure SSL options when using HTTPS
      if (options.UseHttps)
      {
        channelOptions.HttpHandler = new SocketsHttpHandler
        {
          SslOptions = new SslClientAuthenticationOptions
          {
            RemoteCertificateValidationCallback = options.ValidateSslCertificate ? null : static (_, _, _, _) => true,
          },
        };
      }

      // Construct the appropriate URI based on config
      var scheme = options.UseHttps ? "https" : "http";
      var address = $"{scheme}://{options.Host}:{options.Port}";
      Console.WriteLine($"Creating gRPC channel with address: {address}");

      return GrpcChannel.ForAddress(address, channelOptions);
    });

    return services;
  }

  public static IServiceCollection AddGrpcServerServices(this IServiceCollection services)
  {
    services
      .AddGrpcCommonServices()
      .AddCodeFirstGrpc();
    return services;
  }

  public static WebApplication AddGrpcServerServices(this WebApplication builder)
  {
    _ = builder.MapGrpcService<AiGrpcService>();
    return builder;
  }

  public static IServiceCollection AddOlympusServices(this IServiceCollection services)
  {
    _ = services.AddMediatR(cfg => cfg
      .RegisterServicesFromAssemblyContaining<IOlympusApplication>()
      .AddOpenBehavior(typeof(ValidationBehavior<,>))
      .AddOpenBehavior(typeof(ErrorHandlingBehavior<,>))
      .AddOpenBehavior(typeof(RequestLoggingBehavior<,>))
    );
    return services;
  }

  public static WebApplication AddOlympusServices(this WebApplication builder)
  {
    _ = builder.AddGrpcServerServices();
    return builder;
  }
}
