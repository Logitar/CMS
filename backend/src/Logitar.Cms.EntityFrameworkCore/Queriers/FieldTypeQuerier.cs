using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class FieldTypeQuerier : IFieldTypeQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<FieldTypeEntity> _fieldTypes;

  public FieldTypeQuerier(IActorService actorService, CmsContext context)
  {
    _actorService = actorService;
    _fieldTypes = context.FieldTypes;
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
