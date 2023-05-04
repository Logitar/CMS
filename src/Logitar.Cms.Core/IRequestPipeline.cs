using MediatR;

namespace Logitar.Cms.Core;

internal interface IRequestPipeline
{
  Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
}
