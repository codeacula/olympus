using Olympus.Application.AiDrivenFeatures.Common.DTOs;
using Olympus.Application.Common.Messaging;
using Olympus.Application.Common.Types;

namespace Olympus.Application.AiDrivenFeatures.ProcessPlayerNarrativeInput;

/// <summary>
/// Command to process a player's narrative input for AI-driven GM response.
/// </summary>
public sealed record class ProcessPlayerNarrativeInputCommand(
    string SessionId,
    string PlayerId,
    string InputText
) : IOlympusCommand<Result<NarrativeResponseDto, Error>>;
