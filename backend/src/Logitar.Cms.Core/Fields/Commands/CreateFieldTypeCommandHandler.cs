using FluentValidation;
using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Core.Fields.Properties;
using Logitar.Cms.Core.Fields.Validators;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

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

    FieldTypeProperties properties = GetProperties(payload);
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(uniqueNameSettings, payload.UniqueName), properties, command.ActorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    fieldType.Update(command.ActorId);

    await _sender.Send(new SaveFieldTypeCommand(fieldType), cancellationToken);

    return await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
  }

  private static FieldTypeProperties GetProperties(CreateFieldTypePayload payload)
  {
    List<FieldTypeProperties> properties = new(capacity: 2);
    if (payload.StringProperties != null)
    {
      properties.Add(new ReadOnlyStringProperties(payload.StringProperties));
    }
    else if (payload.TextProperties != null)
    {
      properties.Add(new ReadOnlyTextProperties(payload.TextProperties));
    }

    if (properties.Count < 1)
    {
      throw new ArgumentException("No field type properties has been specified.", nameof(payload));
    }
    else if (properties.Count > 1)
    {
      throw new ArgumentException("Multiple field type properties have been specified.", nameof(payload));
    }

    return properties.Single();
  }
}
