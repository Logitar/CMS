using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Fields;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Search;
using Logitar.Cms.Infrastructure.Actors;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Queriers;

internal class FieldTypeQuerier : IFieldTypeQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<FieldTypeEntity> _fieldTypes;
  private readonly IQueryHelper _queryHelper;

  public FieldTypeQuerier(IActorService actorService, CmsContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _fieldTypes = context.FieldTypes;
    _queryHelper = queryHelper;
  }

  public async Task<FieldTypeId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName.Value);

    string? streamId = await _fieldTypes.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new FieldTypeId(streamId);
  }

  public async Task<FieldTypeModel> ReadAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    return await ReadAsync(fieldType.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The field type entity 'StreamId={fieldType.Id}' could not be found.");
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
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    FieldTypeEntity? fieldType = await _fieldTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return fieldType == null ? null : await MapAsync(fieldType, cancellationToken);
  }

  public async Task<SearchResults<FieldTypeModel>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(CmsDb.FieldTypes.Table).SelectAll(CmsDb.FieldTypes.Table)
      .ApplyIdFilter(CmsDb.FieldTypes.Id, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, CmsDb.FieldTypes.UniqueName, CmsDb.FieldTypes.DisplayName);

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
    IReadOnlyCollection<FieldTypeModel> items = await MapAsync(fieldTypes, cancellationToken);

    return new SearchResults<FieldTypeModel>(items, total);
  }

  private async Task<FieldTypeModel> MapAsync(FieldTypeEntity fieldType, CancellationToken cancellationToken)
  {
    return (await MapAsync([fieldType], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<FieldTypeModel>> MapAsync(IEnumerable<FieldTypeEntity> fieldTypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = fieldTypes.SelectMany(fieldType => fieldType.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return fieldTypes.Select(mapper.ToFieldType).ToArray();
  }
}
