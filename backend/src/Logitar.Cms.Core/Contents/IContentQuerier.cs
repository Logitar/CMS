using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Search;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Contents;

public interface IContentQuerier
{
  Task<IReadOnlyDictionary<Guid, ContentId>> FindConflictsAsync(
    ContentTypeId contentTypeId,
    LanguageId? languageId,
    IReadOnlyDictionary<Guid, string> fieldValues,
    ContentId contentId,
    CancellationToken cancellationToken = default);

  Task<IReadOnlyDictionary<Guid, Guid>> FindContentTypeIdsAsync(IEnumerable<Guid> contentIds, CancellationToken cancellationToken = default);

  Task<ContentId?> FindIdAsync(ContentTypeId contentTypeId, LanguageId? languageId, UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<ContentModel> ReadAsync(Content content, CancellationToken cancellationToken = default);
  Task<ContentModel?> ReadAsync(ContentId id, CancellationToken cancellationToken = default);
  Task<ContentModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<ContentLocaleModel>> SearchAsync(SearchContentsPayload payload, CancellationToken cancellationToken = default);
}
