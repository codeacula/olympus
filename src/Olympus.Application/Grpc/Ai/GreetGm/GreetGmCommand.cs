namespace Olympus.Application.Grpc.Ai.GreetGm;

public sealed record GreetGmCommand(string InteractionText) : IRequest<GreetGmResponse>;
