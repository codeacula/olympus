using Olympus.Application.AiDrivenFeatures.Common.DTOs;
using Olympus.Application.Common.Messaging;
using Olympus.Application.Common.Types;
using Olympus.Domain.SharedKernel.ValueObjects;

namespace Olympus.Application.AiDrivenFeatures.ProcessPlayerNarrativeInput;

/// <summary>
/// Command to process a player's narrative input and generate an AI response.
/// </summary>
/// <param name="SessionId"></param>
/// <param name="UserId"></param>
/// <param name="Input"></param>
public sealed record class ProcessPlayerNarrativeInputCommand(
    SessionId SessionId,
    UserId UserId,
    string Input) : IOlympusCommand<Result<NarrativeResponseDto, Error>>;
