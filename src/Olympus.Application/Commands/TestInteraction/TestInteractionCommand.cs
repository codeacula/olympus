namespace Olympus.Application.Commands.TestInteraction;

public sealed record TestInteractionCommand(string InteractionText) : IRequest<TestInteractionResult>;
