using Olympus.Application.Ai.Errors;
using Olympus.Application.Ai.Interactions.TalkWithGm;
using Olympus.Application.Common.Types;
using ProtoBuf.Grpc;

namespace Olympus.Application.Grpc.Services;

public interface IAiApiService
{
  Task<OlympusResult<TalkWithGmResponse, FailedToGetResponseError>> TalkWithGmAsync(
    TalkWithGmRequest request,
    CallContext callContext = default);
}
