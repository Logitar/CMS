using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Contents;

public interface IContentRepository
{
  Task<ContentAggregate?> LoadAsync(ContentTypeId contentTypeId, LanguageId? languageId, UniqueNameUnit uniqueName, CancellationToken cancellationToken = default);

  Task SaveAsync(ContentAggregate content, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ContentAggregate> contents, CancellationToken cancellationToken = default);
}
