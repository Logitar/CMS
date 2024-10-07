using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.Search;
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

  public async Task<FieldType> ReadAsync(FieldTypeAggregate fieldType, CancellationToken cancellationToken)
  {
    return await ReadAsync(fieldType.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The field type entity 'AggregateId={fieldType.Id.AggregateId}' could not be found.");
  }
  public async Task<FieldType?> ReadAsync(FieldTypeId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<FieldType?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _fieldTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueId == id, cancellationToken);

    return fieldType == null ? null : await MapAsync(fieldType, cancellationToken);
  }

  public async Task<FieldType?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = CmsDb.Normalize(uniqueName);

    FieldTypeEntity? fieldType = await _fieldTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return fieldType == null ? null : await MapAsync(fieldType, cancellationToken);
  }

  public async Task<SearchResults<FieldType>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(CmsDb.FieldTypes.Table).SelectAll(CmsDb.FieldTypes.Table)
      .ApplyIdInFilter(CmsDb.FieldTypes.UniqueId, payload);
    _searchHelper.ApplyTextSearch(builder, payload.Search, CmsDb.FieldTypes.UniqueName, CmsDb.FieldTypes.DisplayName);

    if (payload.DataType.HasValue)
    {
      builder.Where(CmsDb.FieldTypes.DataType, Operators.IsEqualTo(payload.DataType.Value.ToString()));
    }

    IQueryable<FieldTypeEntity> query = _fieldTypes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<FieldTypeEntity>? ordered = null;
    if (payload.Sort != null)
    {
      foreach (FieldTypeSortOption sort in payload.Sort)
      {
        switch (sort.Field)
        {
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
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    FieldTypeEntity[] fieldTypes = await query.ToArrayAsync(cancellationToken);
    IEnumerable<FieldType> items = await MapAsync(fieldTypes, cancellationToken);

    return new SearchResults<FieldType>(items, total);
  }

  private async Task<FieldType> MapAsync(FieldTypeEntity fieldType, CancellationToken cancellationToken)
    => (await MapAsync([fieldType], cancellationToken)).Single();
  private async Task<IReadOnlyCollection<FieldType>> MapAsync(IEnumerable<FieldTypeEntity> fieldTypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = fieldTypes.SelectMany(fieldType => fieldType.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return fieldTypes.Select(mapper.ToFieldType).ToArray();
  }
}
