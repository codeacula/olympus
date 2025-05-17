using Olympus.Application.Ai.Services.AiInteractionService;

namespace Olympus.Application.Ai.Requests;

public sealed record TalkWithGmRequest(string Message) : IAiRequest;
