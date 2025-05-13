using MediatR;
using Olympus.Application.Common.Messaging;
using Olympus.Infrastructure.Messaging.MediatR.Adapters;

namespace Olympus.Infrastructure.Messaging.MediatR.Handlers;

/// <summary>
/// Adapts the Olympus query handler interface to MediatR's request handler
/// </summary>
internal class MediatRQueryHandlerAdapter<TQuery, TResult>
    : IRequestHandler<OlympusQueryToMediatRRequest<TQuery, TResult>, TResult>
    where TQuery : IOlympusQuery<TResult>
{
    private readonly IOlympusQueryHandler<TQuery, TResult> _handler;

    public MediatRQueryHandlerAdapter(IOlympusQueryHandler<TQuery, TResult> handler)
    {
        _handler = handler;
    }

    public Task<TResult> Handle(OlympusQueryToMediatRRequest<TQuery, TResult> request, CancellationToken cancellationToken)
    {
        return _handler.Handle(request.Query, cancellationToken);
    }
}
