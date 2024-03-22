using Logitar.Cms.Contracts.Archetypes;

namespace Logitar.Cms.Core.Archetypes;

public interface IArchetypeQuerier
{
  Task<Archetype> ReadAsync(ArchetypeAggregate archetype, CancellationToken cancellationToken = default);
  Task<Archetype?> ReadAsync(ArchetypeId id, CancellationToken cancellationToken = default);
  Task<Archetype?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Archetype?> ReadAsync(string identifier, CancellationToken cancellationToken = default);
}
