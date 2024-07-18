using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Indexing;
using Logitar.Cms.Core.Languages;

namespace Logitar.Cms.EntityFrameworkCore.Indexing;

internal class IndexService : IIndexService
{
  public Task<IReadOnlyCollection<FieldValueConflict>> GetConflictsAsync(IEnumerable<FieldValuePayload> values,
    ContentId contentId, LanguageId? languageId, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
