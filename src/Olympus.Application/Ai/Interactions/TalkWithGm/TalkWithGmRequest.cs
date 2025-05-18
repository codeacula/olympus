namespace Olympus.Application.Ai.Interactions.TalkWithGm;

public sealed record TalkWithGmRequest(string Message) : IRequest<TalkWithGmResponse>;
