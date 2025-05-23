namespace Olympus.Application.Ai.TalkWithGm;

internal sealed class TalkWithGmHandler(IOrb orb) : IRequestHandler<TalkWithGmCommand, TalkWithGmResult>
{
  public async Task<TalkWithGmResult> Handle(TalkWithGmCommand request, CancellationToken cancellationToken)
  {
    try
    {
      var userPrompt = new UserPrompt(request.InteractionText);
      var response = await orb.GreetGmAsync(userPrompt);
      return new TalkWithGmResult(response.Response);
    }
    catch (Exception ex)
    {
      throw new OlympusInvalidResponseException("An error occurred while processing the GM request.", ex);
    }
  }
}
