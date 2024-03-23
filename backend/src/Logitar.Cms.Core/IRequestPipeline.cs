using MediatR;

namespace Logitar.Cms.Core;

public interface IRequestPipeline
{
  Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
}
