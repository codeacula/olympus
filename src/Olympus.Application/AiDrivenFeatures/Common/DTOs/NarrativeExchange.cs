namespace Olympus.Application.AiDrivenFeatures.Common.DTOs;

/// <summary>
/// DTO representing a single exchange in the narrative context.
/// </summary>
public sealed record class NarrativeExchange(string SpeakerId, string Text, DateTime Timestamp);
