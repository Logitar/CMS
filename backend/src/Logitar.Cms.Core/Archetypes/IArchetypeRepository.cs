using Logitar.Cms.Core.Shared;

namespace Logitar.Cms.Core.Archetypes;

public interface IArchetypeRepository
{
  Task<ArchetypeAggregate?> LoadAsync(IdentifierUnit identifier, CancellationToken cancellationToken = default);

  Task SaveAsync(ArchetypeAggregate archetype, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ArchetypeAggregate> archetypes, CancellationToken cancellationToken = default);
}
