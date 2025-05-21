using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Olympus.Application.Ai.Services;
using Olympus.Application.Common.Behaviors;
using Olympus.Application.Common.Grpc;
using ProtoBuf.Grpc.Server;

namespace Olympus.Application;

public static class ServiceCollectionExtension
{
  public static IServiceCollection AddGrpcServices(this IServiceCollection services)
  {
    _ = services.AddSingleton<IGrpcClient, GrpcClient>()
      .AddSingleton<GrpcClientLoggingInterceptor>();

    services.AddCodeFirstGrpc();
    return services;
  }

  public static WebApplication AddGrpcServices(this WebApplication builder)
  {
    _ = builder.MapGrpcService<AiGrpcService>();
    return builder;
  }

  public static IServiceCollection AddOlympusServices(this IServiceCollection services)
  {
    _ = services.AddGrpcServices();
    _ = services.AddMediatR(cfg => cfg
        .RegisterServicesFromAssemblyContaining<GrpcClient>()
        .AddOpenBehavior(typeof(ValidationBehavior<,>))
        .AddOpenBehavior(typeof(ErrorHandlingBehavior<,>))
    );
    return services;
  }

  public static WebApplication AddOlympusServices(this WebApplication builder)
  {
    _ = builder.AddGrpcServices();
    return builder;
  }
}
