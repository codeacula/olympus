using Olympus.Application.Ai.Interfaces;

namespace Olympus.Application.Grpc;

public interface IGrpcClient
{
  IAiGrpcService AiApiService { get; }
}
