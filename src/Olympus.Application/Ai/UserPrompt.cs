namespace Olympus.Application.Ai;

public sealed record UserPrompt(string Prompt)
{
  public static implicit operator string(UserPrompt userPrompt)
  {
    return userPrompt.ToString();
  }
}
