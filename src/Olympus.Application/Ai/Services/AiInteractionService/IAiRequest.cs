namespace Olympus.Application.Ai.Services.AiInteractionService;

public interface IAiRequest<TResponseType>
    where TResponseType : IAiResponse;
