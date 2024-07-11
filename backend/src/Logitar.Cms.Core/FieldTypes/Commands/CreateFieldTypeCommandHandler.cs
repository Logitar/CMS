using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Commands;

internal class CreateFieldTypeCommandHandler : IRequestHandler<CreateFieldTypeCommand, FieldType>
{
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly ISender _sender;

  public CreateFieldTypeCommandHandler(IFieldTypeQuerier fieldTypeQuerier, ISender sender)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
    _sender = sender;
  }

  public async Task<FieldType> Handle(CreateFieldTypeCommand command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = FieldTypeAggregate.UniqueNameSettings;

    CreateFieldTypePayload payload = command.Payload;
    new CreateFieldTypeValidator(uniqueNameSettings).ValidateAndThrow(payload);

    ReadOnlyStringProperties properties = new(payload.StringProperties ?? new());

    UniqueNameUnit uniqueName = new(uniqueNameSettings, payload.UniqueName);
    FieldTypeAggregate fieldType = new(uniqueName, properties, command.ActorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    fieldType.Update(command.ActorId);

    await _sender.Send(new SaveFieldTypeCommand(fieldType), cancellationToken);

    return await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
  }
}
