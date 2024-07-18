using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Languages;
using MediatR;

namespace Logitar.Cms.Core.Contents.Queries;

internal record FindFieldValueConflictsCommand(IEnumerable<FieldValuePayload> FieldValues, ContentAggregate Content, LanguageAggregate? Language)
  : IRequest<IReadOnlyCollection<FieldValueConflict>>;
