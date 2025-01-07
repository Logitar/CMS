using Logitar.Cms.Core.Contents.Events;

namespace Logitar.Cms.Core.Contents;

internal class ContentTypeManager : IContentTypeManager
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;

  public ContentTypeManager(IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
  }

  public async Task SaveAsync(ContentType contentType, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = contentType.Changes.Any(change => change is ContentTypeCreated || change is ContentTypeUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      ContentTypeId? conflictId = await _contentTypeQuerier.FindIdAsync(contentType.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(contentType.Id))
      {
        throw new UniqueNameAlreadyUsedException(contentType, conflictId.Value);
      }
    }

    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);
  }
}
