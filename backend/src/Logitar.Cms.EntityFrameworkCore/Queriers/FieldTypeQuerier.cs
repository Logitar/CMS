using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class FieldTypeQuerier : IFieldTypeQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<FieldTypeEntity> _fieldTypes;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public FieldTypeQuerier(IActorService actorService, CmsContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _fieldTypes = context.FieldTypes;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<FieldTypeId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = CmsDb.Helper.Normalize(uniqueName.Value);

    string? aggregateId = await _fieldTypes.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.AggregateId)
      .SingleOrDefaultAsync(cancellationToken);

    return aggregateId == null ? null : new FieldTypeId(aggregateId);
  }

  public async Task<FieldTypeModel> ReadAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    return await ReadAsync(fieldType.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The field type entity 'AggregateId={fieldType.Id}' could not be found.");
  }
  public async Task<FieldTypeModel?> ReadAsync(FieldTypeId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<FieldTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _fieldTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return fieldType == null ? null : await MapAsync(fieldType, cancellationToken);
  }
  public async Task<FieldTypeModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = CmsDb.Helper.Normalize(uniqueName);

    FieldTypeEntity? fieldType = await _fieldTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return fieldType == null ? null : await MapAsync(fieldType, cancellationToken);
  }

  public async Task<SearchResults<FieldTypeModel>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(CmsDb.FieldTypes.Table).SelectAll(CmsDb.FieldTypes.Table)
      .ApplyIdFilter(payload, CmsDb.FieldTypes.Id);
    _searchHelper.ApplyTextSearch(builder, payload.Search, CmsDb.FieldTypes.UniqueName, CmsDb.FieldTypes.DisplayName);

    if (payload.DataType.HasValue)
    {
      builder.Where(CmsDb.FieldTypes.DataType, Operators.IsEqualTo(payload.DataType.Value.ToString()));
    }

    IQueryable<FieldTypeEntity> query = _fieldTypes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<FieldTypeEntity>? ordered = null;
    foreach (FieldTypeSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case FieldTypeSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case FieldTypeSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case FieldTypeSort.UniqueName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case FieldTypeSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    FieldTypeEntity[] fieldTypes = await query.ToArrayAsync(cancellationToken);
    IEnumerable<FieldTypeModel> items = await MapAsync(fieldTypes, cancellationToken);

    return new SearchResults<FieldTypeModel>(items, total);
  }

  private async Task<FieldTypeModel> MapAsync(FieldTypeEntity fieldType, CancellationToken cancellationToken)
  {
    return (await MapAsync([fieldType], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<FieldTypeModel>> MapAsync(IEnumerable<FieldTypeEntity> fieldTypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = fieldTypes.SelectMany(fieldType => fieldType.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return fieldTypes.Select(mapper.ToFieldType).ToArray().AsReadOnly();
  }
}
