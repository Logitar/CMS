using FluentValidation;
using Logitar.Cms.Contracts.Archetypes;
using Logitar.Cms.Core.Archetypes.Validators;
using Logitar.Cms.Core.Shared;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Archetypes.Commands;

internal class CreateArchetypeCommandHandler : IRequestHandler<CreateArchetypeCommand, Archetype>
{
  private readonly IArchetypeQuerier _archetypeQuerier;
  private readonly ISender _sender;

  public CreateArchetypeCommandHandler(IArchetypeQuerier archetypeQuerier, ISender sender)
  {
    _archetypeQuerier = archetypeQuerier;
    _sender = sender;
  }

  public async Task<Archetype> Handle(CreateArchetypeCommand command, CancellationToken cancellationToken)
  {
    CreateArchetypePayload payload = command.Payload;
    new CreateArchetypeValidator().ValidateAndThrow(payload);

    ArchetypeAggregate archetype = new(new IdentifierUnit(payload.UniqueName), command.ActorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    archetype.Update(command.ActorId);

    await _sender.Send(new SaveArchetypeCommand(archetype), cancellationToken);

    return await _archetypeQuerier.ReadAsync(archetype, cancellationToken);
  }
}
