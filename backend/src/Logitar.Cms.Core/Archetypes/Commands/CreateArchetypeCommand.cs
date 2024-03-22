using Logitar.Cms.Contracts.Archetypes;
using MediatR;

namespace Logitar.Cms.Core.Archetypes.Commands;

public record CreateArchetypeCommand(CreateArchetypePayload Payload) : Request, IRequest<Archetype>;
