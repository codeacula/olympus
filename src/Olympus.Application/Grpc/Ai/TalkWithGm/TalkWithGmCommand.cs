namespace Olympus.Application.Grpc.Ai.TalkWithGm;

public sealed record TalkWithGmCommand(string InteractionText) : IRequest<TalkWithGmResponse>;
