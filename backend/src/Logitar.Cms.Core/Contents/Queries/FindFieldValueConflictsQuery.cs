using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Languages;
using MediatR;

namespace Logitar.Cms.Core.Contents.Queries;

public record FindFieldValueConflictsQuery(IEnumerable<FieldValue> FieldValues, ContentAggregate Content, LanguageAggregate? Language)
  : IRequest<IReadOnlyCollection<FieldValueConflict>>;
