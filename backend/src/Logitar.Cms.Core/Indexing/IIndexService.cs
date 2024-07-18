using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Languages;

namespace Logitar.Cms.Core.Indexing;

public interface IIndexService
{
  Task<IReadOnlyCollection<FieldValueConflict>> GetConflictsAsync(IEnumerable<FieldValuePayload> values,
    ContentId contentId, LanguageId? languageId, CancellationToken cancellationToken = default);
}
