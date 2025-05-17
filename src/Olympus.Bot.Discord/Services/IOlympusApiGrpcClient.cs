namespace Olympus.Bot.Discord.Services;

public interface IOlympusApiGrpcClient
{
  Task<NarrativeResponseProto?> InteractAsync(string sessionId, string userId, string input);
}
