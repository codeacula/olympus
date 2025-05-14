namespace Olympus.Application.AiDrivenFeatures.Common.DTOs;

/// <summary>
/// DTO representing the narrative context for a session.
/// </summary>
/// <param name="SessionId"></param>
/// <param name="Exchanges"></param>
public sealed record class NarrativeContext(string SessionId, IReadOnlyList<NarrativeExchange> Exchanges);
