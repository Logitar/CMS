using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Contents;

internal class ContentManager : IContentManager
{
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;

  public ContentManager(IContentQuerier contentQuerier, IContentRepository contentRepository, IContentTypeRepository contentTypeRepository)
  {
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
    _contentTypeRepository = contentTypeRepository;
  }

  public async Task SaveAsync(Content content, CancellationToken cancellationToken)
  {
    ContentType contentType = await _contentTypeRepository.LoadAsync(content, cancellationToken);
    await SaveAsync(content, contentType, cancellationToken);
  }
  public async Task SaveAsync(Content content, ContentType contentType, CancellationToken cancellationToken)
  {
    if (contentType.Id != content.ContentTypeId)
    {
      throw new ArgumentException($"The content type 'Id={contentType.Id}' was not expected. The expected content type for content 'Id={content.Id}' is '{content.ContentTypeId}'.", nameof(contentType));
    }

    HashSet<LanguageId?> languageIds = new(capacity: content.Locales.Count + 1);
    foreach (IEvent change in content.Changes)
    {
      if (change is ContentCreated)
      {
        languageIds.Add(null);
      }
      else if (change is ContentLocaleChanged localeChanged)
      {
        languageIds.Add(localeChanged.LanguageId);
      }
    }

    foreach (LanguageId? languageId in languageIds)
    {
      ContentLocale invariantOrLocale = languageId.HasValue ? content.FindLocale(languageId.Value) : content.Invariant;

      ContentId? conflictId = await _contentQuerier.FindIdAsync(content.ContentTypeId, languageId, invariantOrLocale.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(content.Id))
      {
        throw new ContentUniqueNameAlreadyUsedException(content, languageId, invariantOrLocale, conflictId.Value);
      }

      // TODO(fpion): validate field values
    }

    await _contentRepository.SaveAsync(content, cancellationToken);
  }
}
