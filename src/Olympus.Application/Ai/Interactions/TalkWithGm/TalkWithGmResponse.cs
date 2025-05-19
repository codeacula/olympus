using System.Runtime.Serialization;

namespace Olympus.Application.Ai.Interactions.TalkWithGm;

[DataContract]
public sealed record TalkWithGmResponse
{
  [DataMember(Order = 1)]
  public string Response { get; init; } = string.Empty;

  public TalkWithGmResponse(string response)
  {
    Response = response;
  }

  public override string ToString() => $"Response: {Response}";
}
