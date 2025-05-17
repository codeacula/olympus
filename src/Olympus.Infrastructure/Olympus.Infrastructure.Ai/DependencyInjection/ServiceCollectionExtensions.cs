using Microsoft.Extensions.DependencyInjection;
using Olympus.Application.Ai.Interactions.TalkWithGm;
using Olympus.Application.Ai.Services.AiInteractionService;
using Olympus.Infrastructure.Ai.Handlers;
using Olympus.Infrastructure.Ai.Services;

namespace Olympus.Infrastructure.Ai.DependencyInjection;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddAiInfrastructure(this IServiceCollection services)
  {
    // Register the AI interaction service
    _ = services.AddSingleton<IAiInteractionService, AiInteractionService>();

    // Register all AI request handlers
    _ = services.AddTransient<IAiRequestHandler<TalkWithGmRequest, TalkWithGmResponse>, TalkWithGmHandler>();

    // Add additional handlers here as needed

    return services;
  }
}
