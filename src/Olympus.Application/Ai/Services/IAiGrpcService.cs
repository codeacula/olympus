using System.ServiceModel;
using Olympus.Application.Ai.Interactions.TalkWithGm;
using ProtoBuf.Grpc;

namespace Olympus.Application.Ai.Services;

[ServiceContract]
public interface IAiGrpcService
{
  Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CallContext callContext = default);
}
