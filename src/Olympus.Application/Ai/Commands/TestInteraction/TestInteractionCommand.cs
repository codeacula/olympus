using Olympus.Application.Common.Messaging;

namespace Olympus.Application.Ai.Commands.TestInteraction;

public sealed record TestInteractionCommand(string InteractionText) : IOlympusCommand<TestAiInteractionCommandResult>;
