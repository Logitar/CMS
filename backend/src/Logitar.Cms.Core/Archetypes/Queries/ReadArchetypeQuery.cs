using Logitar.Cms.Contracts.Archetypes;
using MediatR;

namespace Logitar.Cms.Core.Archetypes.Queries;

public record ReadArchetypeQuery(Guid? Id, string? Identifier) : IRequest<Archetype?>;
