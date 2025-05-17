using Olympus.Application.Ai.Services.AiInteractionService;

namespace Olympus.Application.Ai.Interactions.TalkWithGm;

public sealed record TalkWithGmResponse(string Response) : IAiResponse;
