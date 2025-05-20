using System.Runtime.Serialization;

namespace Olympus.Application.Ai.Interactions.TalkWithGm;

[DataContract]
public sealed record TalkWithGmRequest : IRequest<TalkWithGmResponse>
{
  [DataMember(Order = 1)]
  public string InteractionText { get; init; } = string.Empty;

  public TalkWithGmRequest() { }

  public TalkWithGmRequest(string interactionText)
  {
    InteractionText = interactionText;
  }

  public override string ToString() => $"InteractionText: {InteractionText}";
}
