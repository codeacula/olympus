namespace Olympus.Application.Grpc.Ai.GreetGm;

[ProtoContract]
public sealed record GreetGmRequest : IRequest<GreetGmResponse>
{
  [ProtoMember(1)]
  public string InteractionText { get; init; } = string.Empty;

  public GreetGmRequest() { }

  public GreetGmRequest(string interactionText)
  {
    InteractionText = interactionText;
  }
}
