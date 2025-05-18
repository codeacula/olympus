namespace Olympus.Application.Ai.Commands.TestInteraction;

public sealed record TestInteractionCommand(string InteractionText) : IRequest<TestInteractionResult>;
