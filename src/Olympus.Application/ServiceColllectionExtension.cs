using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Olympus.Application.Grpc;
using Olympus.Application.Grpc.Services;
using ProtoBuf.Grpc.Server;

namespace Olympus.Application;

public static class ServiceCollectionExtension
{
  public static IServiceCollection AddGrpcServices(this IServiceCollection services)
  {
    _ = services.AddSingleton<IGrpcClient, GrpcClient>();

    services.AddCodeFirstGrpc();
    return services;
  }

  public static WebApplication AddGrpcServices(this WebApplication builder)
  {
    _ = builder.MapGrpcService<AiApiService>();
    return builder;
  }

  public static IServiceCollection AddOlympusServices(this IServiceCollection services)
  {
    _ = services.AddGrpcServices();
    return services;
  }

  public static WebApplication AddOlympusServices(this WebApplication builder)
  {
    _ = builder.AddGrpcServices();
    return builder;
  }
}
