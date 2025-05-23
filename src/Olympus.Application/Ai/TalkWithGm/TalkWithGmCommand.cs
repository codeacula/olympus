namespace Olympus.Application.Ai.TalkWithGm;

public sealed record TalkWithGmCommand(string InteractionText) : IRequest<TalkWithGmResult>;
