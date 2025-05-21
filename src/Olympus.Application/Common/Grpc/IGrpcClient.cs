using Olympus.Application.Ai.Services;

namespace Olympus.Application.Common.Grpc;

public interface IGrpcClient
{
  IAiGrpcService AiApiService { get; }
}
