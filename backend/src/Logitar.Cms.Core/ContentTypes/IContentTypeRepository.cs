namespace Logitar.Cms.Core.ContentTypes;

public interface IContentTypeRepository
{
  Task<IReadOnlyCollection<ContentTypeAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<ContentTypeAggregate?> LoadAsync(ContentTypeId id, CancellationToken cancellationToken = default);
  Task<ContentTypeAggregate?> LoadAsync(IdentifierUnit uniqueName, CancellationToken cancellationToken = default);

  Task SaveAsync(ContentTypeAggregate contentType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ContentTypeAggregate> contentTypes, CancellationToken cancellationToken = default);
}
