using Logitar.Cms.Contracts.Archetypes;
using Logitar.Cms.Core.Shared;
using MediatR;

namespace Logitar.Cms.Core.Archetypes.Queries;

internal class ReadArchetypeQueryHandler : IRequestHandler<ReadArchetypeQuery, Archetype?>
{
  private readonly IArchetypeQuerier _archetypeQuerier;

  public ReadArchetypeQueryHandler(IArchetypeQuerier archetypeQuerier)
  {
    _archetypeQuerier = archetypeQuerier;
  }

  public async Task<Archetype?> Handle(ReadArchetypeQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Archetype> archetypes = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Archetype? archetype = await _archetypeQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (archetype != null)
      {
        archetypes[archetype.Id] = archetype;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Identifier))
    {
      Archetype? archetype = await _archetypeQuerier.ReadAsync(query.Identifier, cancellationToken);
      if (archetype != null)
      {
        archetypes[archetype.Id] = archetype;
      }
    }

    if (archetypes.Count > 1)
    {
      throw TooManyResultsException<Archetype>.ExpectedSingle(archetypes.Count);
    }

    return archetypes.SingleOrDefault().Value;
  }
}
