using Microsoft.AspNetCore.Mvc;
using Olympus.Application.AiDrivenFeatures.Common.DTOs;

namespace Olympus.Api.Controllers;

[ApiController]
[Route("api/ai")]
public class PlayerNarrativeController : ControllerBase
{
  [HttpPost("interact")]
  public ActionResult<NarrativeResponseDto> Interact([FromBody] PlayerInteractRequest request)
  {
    // Simple mock response for testing
    var mockResponse = new NarrativeResponseDto(
        $"Mock response to: {request.Input}",
        ["Context item 1", "Context item 2"]
    );

    return Ok(mockResponse);
  }
}

public record PlayerInteractRequest(string SessionId, string UserId, string Input);
