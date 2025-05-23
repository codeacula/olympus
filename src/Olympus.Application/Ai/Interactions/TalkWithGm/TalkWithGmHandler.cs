using Olympus.Application.Ai.Interfaces;

namespace Olympus.Application.Ai.Interactions.TalkWithGm;

internal sealed class TalkWithGmHandler(IOrb orb) : IRequestHandler<TalkWithGmCommand, TalkWithGmResult>
{
  public async Task<TalkWithGmResult> Handle(TalkWithGmCommand request, CancellationToken cancellationToken)
  {
    var userPrompt = new UserPrompt(request.Value);
    var response = await orb.GreetGmAsync(userPrompt);
    return new TalkWithGmResult(response.Value);
  }
}
