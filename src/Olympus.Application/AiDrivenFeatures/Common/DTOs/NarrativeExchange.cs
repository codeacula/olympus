namespace Olympus.Application.AiDrivenFeatures.Common.DTOs;

/// <summary>
/// DTO representing a single exchange in the narrative context.
/// </summary>
/// <param name="SpeakerId"></param>
/// <param name="Text"></param>
/// <param name="Timestamp"></param>
public sealed record class NarrativeExchange(string SpeakerId, string Text, DateTime Timestamp);
