namespace Olympus.Application.AiDrivenFeatures.Common.DTOs;

/// <summary>
/// Data Transfer Object for narrative response from the AI system.
/// </summary>
/// <param name="Response"></param>
/// <param name="UpdatedContext"></param>
public sealed record class NarrativeResponseDto(
    string Response,
    IEnumerable<string> UpdatedContext);
