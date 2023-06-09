﻿using MediatR;

namespace Logitar.Cms.Core;

internal class RequestPipeline : IRequestPipeline
{
  private readonly IMediator _mediator;

  public RequestPipeline(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    return await _mediator.Send(request, cancellationToken);
  }
}
