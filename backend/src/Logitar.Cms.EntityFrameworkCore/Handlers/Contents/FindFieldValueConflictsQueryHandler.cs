using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Queries;
using Logitar.Cms.Core.Languages;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;
using Logitar.Identity.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Contents;

internal class FindFieldValueConflictsQueryHandler : IRequestHandler<FindFieldValueConflictsQuery, IReadOnlyCollection<FieldValueConflict>>
{
  private readonly DbSet<UniqueFieldIndexEntity> _uniqueFieldIndex;
  private readonly ISqlHelper _sqlHelper;

  public FindFieldValueConflictsQueryHandler(CmsContext context, ISqlHelper sqlHelper)
  {
    _uniqueFieldIndex = context.UniqueFieldIndex;
    _sqlHelper = sqlHelper;
  }

  public async Task<IReadOnlyCollection<FieldValueConflict>> Handle(FindFieldValueConflictsQuery query, CancellationToken cancellationToken)
  {
    IEnumerable<FieldValue> fieldValues = query.FieldValues;
    ContentAggregate content = query.Content;
    LanguageAggregate? language = query.Language;

    int capacity = fieldValues.Count();
    if (capacity < 1)
    {
      return [];
    }

    List<AndCondition> conditions = new(capacity);
    foreach (FieldValue field in fieldValues)
    {
      conditions.Add(new AndCondition(new OperatorCondition(CmsDb.UniqueFieldIndex.FieldTypeUid, Operators.IsEqualTo(field.Id)),
        new OperatorCondition(CmsDb.UniqueFieldIndex.Value, Operators.IsEqualTo(field.Value.Truncate(UniqueFieldIndexEntity.MaximumLength)))));
    }

    IQueryBuilder builder = _sqlHelper.QueryFrom(CmsDb.UniqueFieldIndex.Table)
      .Where(CmsDb.UniqueFieldIndex.ContentItemUid, Operators.IsNotEqualTo(content.Id.ToGuid()))
      .Where(CmsDb.UniqueFieldIndex.LanguageUid, language == null ? Operators.IsNull() : Operators.IsEqualTo(language.Id.ToGuid()))
      .WhereOr([.. conditions])
      .Select(CmsDb.UniqueFieldIndex.FieldDefinitionUid, CmsDb.UniqueFieldIndex.ContentItemUid);

    return (await _uniqueFieldIndex.FromQuery(builder).AsNoTracking()
      .Select(x => new { x.FieldDefinitionUid, x.ContentItemUid })
      .ToArrayAsync(cancellationToken)
    ).Select(x => new FieldValueConflict(x.FieldDefinitionUid, new ContentId(x.ContentItemUid))).ToList().AsReadOnly();
  }
}
