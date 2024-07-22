using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Commands;

internal class ReplaceFieldTypeCommandHandler : IRequestHandler<ReplaceFieldTypeCommand, FieldType?>
{
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly ISender _sender;

  public ReplaceFieldTypeCommandHandler(IFieldTypeQuerier fieldTypeQuerier, IFieldTypeRepository fieldTypeRepository, ISender sender)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
    _sender = sender;
  }

  public async Task<FieldType?> Handle(ReplaceFieldTypeCommand command, CancellationToken cancellationToken)
  {
    FieldTypeId id = new(command.Id);
    FieldTypeAggregate? fieldType = await _fieldTypeRepository.LoadAsync(id, cancellationToken);
    if (fieldType == null)
    {
      return null;
    }

    IUniqueNameSettings uniqueNameSettings = FieldTypeAggregate.UniqueNameSettings;

    ReplaceFieldTypePayload payload = command.Payload;
    new ReplaceFieldTypeValidator(uniqueNameSettings, fieldType.DataType).ValidateAndThrow(payload);

    fieldType.UniqueName = new UniqueNameUnit(uniqueNameSettings, payload.UniqueName);
    fieldType.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    fieldType.Description = DescriptionUnit.TryCreate(payload.Description);
    fieldType.Update(command.ActorId);

    if (payload.BooleanProperties != null)
    {
      fieldType.SetProperties(new ReadOnlyBooleanProperties(payload.BooleanProperties), command.ActorId);
    }
    if (payload.DateTimeProperties != null)
    {
      fieldType.SetProperties(new ReadOnlyDateTimeProperties(payload.DateTimeProperties), command.ActorId);
    }
    if (payload.NumberProperties != null)
    {
      fieldType.SetProperties(new ReadOnlyNumberProperties(payload.NumberProperties), command.ActorId);
    }
    if (payload.StringProperties != null)
    {
      fieldType.SetProperties(new ReadOnlyStringProperties(payload.StringProperties), command.ActorId);
    }
    if (payload.TextProperties != null)
    {
      fieldType.SetProperties(new ReadOnlyTextProperties(payload.TextProperties), command.ActorId);
    }

    await _sender.Send(new SaveFieldTypeCommand(fieldType), cancellationToken);

    return await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
  }
}
