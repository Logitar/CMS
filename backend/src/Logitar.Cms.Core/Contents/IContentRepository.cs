namespace Logitar.Cms.Core.Contents;

public interface IContentRepository
{
  Task SaveAsync(ContentAggregate content, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ContentAggregate> contents, CancellationToken cancellationToken = default);
}
