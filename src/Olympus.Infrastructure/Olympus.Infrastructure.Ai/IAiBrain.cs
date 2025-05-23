using Olympus.Application.Ai;

namespace Olympus.Infrastructure.Ai;

public interface IAiBrain
{
  Task<ChatMessageResponse> GetChatMessageAsync(IOrb speakingOrb, ChatMessageRequest request);
}
