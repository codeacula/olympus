using Olympus.Application.Grpc.Ai;

namespace Olympus.Application.Grpc;

public interface IGrpcClient
{
  IAiGrpcService AiApiService { get; }
}
