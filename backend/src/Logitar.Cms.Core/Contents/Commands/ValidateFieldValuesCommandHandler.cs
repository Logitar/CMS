using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.FieldTypes;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

internal class ValidateFieldValuesCommandHandler : IRequestHandler<ValidateFieldValuesCommand, Unit>
{
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public ValidateFieldValuesCommandHandler(IFieldTypeRepository fieldTypeRepository)
  {
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task<Unit> Handle(ValidateFieldValuesCommand command, CancellationToken cancellationToken)
  {
    ContentTypeAggregate contentType = command.ContentType;

    int capacity = contentType.FieldDefinitions.Count;
    HashSet<FieldTypeId> fieldTypeIds = new(capacity);
    HashSet<Guid> missingFieldIds = new(capacity);
    foreach (KeyValuePair<Guid, FieldDefinitionUnit> field in contentType.FieldDefinitions)
    {
      if (field.Value.IsInvariant == command.IsInvariant)
      {
        fieldTypeIds.Add(field.Value.FieldTypeId);

        if (field.Value.IsRequired)
        {
          missingFieldIds.Add(field.Key);
        }
      }
    }

    Dictionary<FieldTypeId, FieldTypeAggregate> fieldTypes = (await _fieldTypeRepository.LoadAsync(fieldTypeIds, cancellationToken))
      .ToDictionary(f => f.Id, f => f);

    List<ValidationFailure> errors = new(capacity: command.Fields.Count);

    foreach (FieldValuePayload field in command.Fields)
    {
      missingFieldIds.Remove(field.Id);

      FieldDefinitionUnit? definition = contentType.TryGetFieldDefinition(field.Id);
      if (definition == null)
      {
        errors.Add(new ValidationFailure(command.PropertyName, "The field definition could not be found.", field.Id)
        {
          ErrorCode = "FieldDefinitionNotFound"
        });
        continue;
      }
      else if (definition.IsInvariant != command.IsInvariant)
      {
        string errorMessage = command.IsInvariant
          ? "The field is invariant, but the current locale is not."
          : "The field is not invariant, but the current locale is invariant.";
        errors.Add(new ValidationFailure(command.PropertyName, errorMessage, field.Id)
        {
          ErrorCode = "InvalidFieldLocale"
        });
        continue;
      }

      FieldTypeAggregate fieldType = fieldTypes[definition.FieldTypeId];
      // TODO(fpion): Parsing
      // TODO(fpion): FieldType properties

      // TODO(fpion): FieldDefinition.IsUnique (conflits)
    }

    foreach (Guid missingFieldId in missingFieldIds)
    {
      errors.Insert(0, new ValidationFailure(command.PropertyName, "The field value is required.", missingFieldId)
      {
        ErrorCode = "RequiredFieldValue"
      });
    }

    if (errors.Count > 0)
    {
      throw new ValidationException(errors);
    }

    return Unit.Value;
  }
}
