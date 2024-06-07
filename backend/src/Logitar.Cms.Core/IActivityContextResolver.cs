namespace Logitar.Cms.Core;

public interface IActivityContextResolver
{
  Task<ActivityContext> ResolveAsync(CancellationToken cancellationToken = default);
}
