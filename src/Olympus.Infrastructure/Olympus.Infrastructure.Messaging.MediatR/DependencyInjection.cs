using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Olympus.Application.Common.Messaging;
using Olympus.Infrastructure.Messaging.MediatR.Adapters;
using Olympus.Infrastructure.Messaging.MediatR.Handlers;

namespace Olympus.Infrastructure.Messaging.MediatR;

/// <summary>
/// Extension methods for registering MediatR messaging infrastructure in dependency injection
/// </summary>
public static class DependencyInjection
{
  /// <summary>
  /// Adds MediatR-based messaging infrastructure to the service collection
  /// </summary>
  /// <param name="services">The service collection</param>
  /// <param name="applicationAssembly">The assembly containing the command and query handlers</param>
  /// <returns>The service collection for chaining</returns>
  public static IServiceCollection AddMediatRMessaging(
      this IServiceCollection services,
      Assembly applicationAssembly)
  {
    // Register MediatR
    _ = services.AddMediatR(cfg => _ = cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

    // Register our Olympus dispatcher and publisher
    _ = services.AddTransient<IOlympusDispatcher, MediatROlympusDispatcher>();
    _ = services.AddTransient<IOlympusEventPublisher, MediatROlympusEventPublisher>();

    // Scan for command handlers
    RegisterCommandHandlers(services, applicationAssembly);

    // Scan for query handlers
    RegisterQueryHandlers(services, applicationAssembly);

    // Scan for event handlers
    RegisterEventHandlers(services, applicationAssembly);

    return services;
  }

  private static void RegisterCommandHandlers(IServiceCollection services, Assembly applicationAssembly)
  {
    var commandHandlerTypes = applicationAssembly.GetTypes()
        .Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces().Any(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IOlympusCommandHandler<,>)));

    foreach (var handlerType in commandHandlerTypes)
    {
      var handlerInterface = handlerType.GetInterfaces()
          .First(i => i.GetGenericTypeDefinition() == typeof(IOlympusCommandHandler<,>));

      var commandType = handlerInterface.GetGenericArguments()[0];
      var resultType = handlerInterface.GetGenericArguments()[1];

      // Register the handler itself
      _ = services.AddTransient(handlerType);
      _ = services.AddTransient(handlerInterface, sp => sp.GetRequiredService(handlerType));

      // Register the MediatR adapter
      var adapterType = typeof(MediatRCommandHandlerAdapter<,>).MakeGenericType(commandType, resultType);
      _ = services.AddTransient(adapterType);

      // Register the adapter type as IRequestHandler for the command
      var requestType = typeof(OlympusCommandToMediatRRequest<,>).MakeGenericType(commandType, resultType);
      var requestHandlerInterface = typeof(IRequestHandler<,>).MakeGenericType(requestType, resultType);
      _ = services.AddTransient(requestHandlerInterface, sp => sp.GetRequiredService(adapterType));
    }
  }

  private static void RegisterQueryHandlers(IServiceCollection services, Assembly applicationAssembly)
  {
    var queryHandlerTypes = applicationAssembly.GetTypes()
        .Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces().Any(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IOlympusQueryHandler<,>)));

    foreach (var handlerType in queryHandlerTypes)
    {
      var handlerInterface = handlerType.GetInterfaces()
          .First(i => i.GetGenericTypeDefinition() == typeof(IOlympusQueryHandler<,>));

      var queryType = handlerInterface.GetGenericArguments()[0];
      var resultType = handlerInterface.GetGenericArguments()[1];

      // Register the handler itself
      _ = services.AddTransient(handlerType);
      _ = services.AddTransient(handlerInterface, sp => sp.GetRequiredService(handlerType));

      // Register the MediatR adapter
      var adapterType = typeof(MediatRQueryHandlerAdapter<,>).MakeGenericType(queryType, resultType);
      _ = services.AddTransient(adapterType);

      // Register the adapter type as IRequestHandler for the query
      var requestType = typeof(OlympusQueryToMediatRRequest<,>).MakeGenericType(queryType, resultType);
      var requestHandlerInterface = typeof(IRequestHandler<,>).MakeGenericType(requestType, resultType);
      _ = services.AddTransient(requestHandlerInterface, sp => sp.GetRequiredService(adapterType));
    }
  }

  private static void RegisterEventHandlers(IServiceCollection services, Assembly applicationAssembly)
  {
    var eventHandlerTypes = applicationAssembly.GetTypes()
        .Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces().Any(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IOlympusEventHandler<>)));

    foreach (var handlerType in eventHandlerTypes)
    {
      var handlerInterfaces = handlerType.GetInterfaces()
          .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOlympusEventHandler<>));

      foreach (var handlerInterface in handlerInterfaces)
      {
        var eventType = handlerInterface.GetGenericArguments()[0];

        // Register the handler itself
        _ = services.AddTransient(handlerType);
        _ = services.AddTransient(handlerInterface, sp => sp.GetRequiredService(handlerType));

        // Register the MediatR adapter
        var adapterType = typeof(MediatREventHandlerAdapter<>).MakeGenericType(eventType);
        _ = services.AddTransient(adapterType);

        // Register the adapter type as INotificationHandler for the event
        var notificationType = typeof(OlympusEventToMediatRNotification<>).MakeGenericType(eventType);
        var notificationHandlerInterface = typeof(INotificationHandler<>).MakeGenericType(notificationType);
        _ = services.AddTransient(notificationHandlerInterface, sp => sp.GetRequiredService(adapterType));
      }
    }
  }
}
