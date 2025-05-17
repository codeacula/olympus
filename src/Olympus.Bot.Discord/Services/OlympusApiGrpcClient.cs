using Grpc.Net.Client;

namespace Olympus.Bot.Discord.Services;

public class OlympusApiGrpcClient : IOlympusApiGrpcClient
{
  private readonly AiInteraction.AiInteractionClient _client;
  private readonly ILogger<OlympusApiGrpcClient> _logger;

  public OlympusApiGrpcClient(IConfiguration configuration, ILogger<OlympusApiGrpcClient> logger)
  {
    _logger = logger;
    var apiAddress = configuration["OlympusApi:Address"] ?? "https://localhost:7001"; // Example: Get from config
    var channel = GrpcChannel.ForAddress(apiAddress);
    _client = new AiInteraction.AiInteractionClient(channel);
  }

  public async Task<NarrativeResponseProto?> InteractAsync(string sessionId, string userId, string input)
  {
    try
    {
      var request = new PlayerInteractRequestProto
      {
        SessionId = sessionId,
        UserId = userId,
        Input = input
      };
      _logger.LogInformation("Sending gRPC Interact request for user {UserId}", userId);
      return await _client.InteractAsync(request);
    }
    catch (Grpc.Core.RpcException ex)
    {
      _logger.LogError(ex, "Error calling Olympus API via gRPC for user {UserId}", userId);
      return null;
    }
  }
}
