using MediatR;

namespace Logitar.Cms.Core.Archetypes.Commands;

public record SaveArchetypeCommand(ArchetypeAggregate Archetype) : IRequest;
