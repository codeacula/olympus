namespace Olympus.Application.Grpc.Ai.TalkWithGm;

[ProtoContract]
public sealed record TalkWithGmRequest : IRequest<TalkWithGmResponse>
{
  [ProtoMember(1)]
  public string InteractionText { get; init; } = string.Empty;

  public TalkWithGmRequest() { }

  public TalkWithGmRequest(string interactionText)
  {
    InteractionText = interactionText;
  }
}
