using Logitar.Cms.Core.Archetypes.Events;
using Logitar.Cms.Core.Shared;
using MediatR;

namespace Logitar.Cms.Core.Archetypes.Commands;

internal class SaveArchetypeCommandHandler : IRequestHandler<SaveArchetypeCommand>
{
  private readonly IArchetypeRepository _archetypeRepository;

  public SaveArchetypeCommandHandler(IArchetypeRepository archetypeRepository)
  {
    _archetypeRepository = archetypeRepository;
  }

  public async Task Handle(SaveArchetypeCommand command, CancellationToken cancellationToken)
  {
    ArchetypeAggregate archetype = command.Archetype;

    if (archetype.Changes.Any(change => change is ArchetypeCreatedEvent))
    {
      ArchetypeAggregate? other = await _archetypeRepository.LoadAsync(archetype.UniqueName, cancellationToken);
      if (other != null && !other.Equals(archetype))
      {
        throw new UniqueNameAlreadyUsedException<ArchetypeAggregate>(archetype.UniqueName, nameof(archetype.UniqueName));
      }
    }

    await _archetypeRepository.SaveAsync(archetype, cancellationToken);
  }
}
