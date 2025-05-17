using Olympus.Application.Grpc.Services;

namespace Olympus.Application.Grpc;

public interface IGrpcClient
{
  IAiApiService AiApiService { get; }
}
