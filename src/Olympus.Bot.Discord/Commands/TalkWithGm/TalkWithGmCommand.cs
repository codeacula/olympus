using MediatR;

namespace Olympus.Bot.Discord.Commands.TalkWithGm;

public sealed class TalkWithGmCommand(string InteractionText) : IRequest<TalkWithGmResult>;
