using Olympus.Application.Ai.Interfaces;

namespace Olympus.Infrastructure.Ai;

public interface IAiBrain
{
  Task<ChatMessageResponse> GetChatMessageAsync(IOrb speakingOrb, ChatMessageRequest request);
}
