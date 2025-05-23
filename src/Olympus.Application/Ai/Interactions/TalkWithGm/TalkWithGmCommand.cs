namespace Olympus.Application.Ai.Interactions.TalkWithGm;

public sealed record TalkWithGmCommand(string Value) : IRequest<TalkWithGmResult>;
