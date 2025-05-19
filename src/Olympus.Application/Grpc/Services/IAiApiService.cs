using System.ServiceModel;
using Olympus.Application.Ai.Interactions.TalkWithGm;
using ProtoBuf.Grpc;

namespace Olympus.Application.Grpc.Services;

[ServiceContract]
public interface IAiApiService
{
  ValueTask<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CallContext callContext = default);
}
