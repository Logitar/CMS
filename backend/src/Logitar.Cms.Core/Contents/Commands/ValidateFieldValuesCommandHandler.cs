using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents.Queries;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Cms.Core.Languages;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

internal class ValidateFieldValuesCommandHandler : IRequestHandler<ValidateFieldValuesCommand, ValidationResult>
{
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly ISender _sender;

  public ValidateFieldValuesCommandHandler(IFieldTypeRepository fieldTypeRepository, ISender sender)
  {
    _fieldTypeRepository = fieldTypeRepository;
    _sender = sender;
  }

  public async Task<ValidationResult> Handle(ValidateFieldValuesCommand command, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<FieldValue> fields = command.Fields;
    ContentTypeAggregate contentType = command.ContentType;
    ContentAggregate content = command.Content;
    LanguageAggregate? language = command.Language;
    string propertyName = command.PropertyName;

    int capacity = contentType.FieldDefinitions.Count;
    bool isInvariant = language == null;
    HashSet<FieldTypeId> fieldTypeIds = new(capacity);
    HashSet<Guid> missingFieldIds = new(capacity);
    foreach (KeyValuePair<Guid, FieldDefinitionUnit> field in contentType.FieldDefinitions)
    {
      if (field.Value.IsInvariant == isInvariant)
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

    List<ValidationFailure> errors = new(capacity: fields.Count);
    List<FieldValue> uniqueValues = new(capacity: fields.Count);

    foreach (FieldValue field in fields)
    {
      missingFieldIds.Remove(field.Id);

      FieldDefinitionUnit? definition = contentType.TryGetFieldDefinition(field.Id);
      if (definition == null)
      {
        errors.Add(new ValidationFailure(propertyName, "The field definition could not be found.", field.Id)
        {
          ErrorCode = "FieldDefinitionNotFound"
        });
        continue;
      }
      else if (definition.IsInvariant != isInvariant)
      {
        string errorMessage = isInvariant
          ? "The field is not invariant, but the current locale is invariant."
          : "The field is invariant, but the current locale is not.";
        errors.Add(new ValidationFailure(propertyName, errorMessage, field.Id)
        {
          ErrorCode = "InvalidFieldLocale"
        });
        continue;
      }

      FieldTypeAggregate fieldType = fieldTypes[definition.FieldTypeId];
      ValidationResult result = fieldType.Validate(field.Value, propertyName); // ISSUE: https://github.com/Logitar/CMS/issues/3 (FieldId)
      if (result.IsValid)
      {
        if (definition.IsUnique)
        {
          uniqueValues.Add(field);
        }
      }
      else
      {
        errors.AddRange(result.Errors);
      }
    }

    foreach (Guid missingFieldId in missingFieldIds)
    {
      errors.Insert(0, new ValidationFailure(propertyName, "The field value is required.", missingFieldId)
      {
        ErrorCode = "RequiredFieldValue"
      });
    }

    if (uniqueValues.Count > 0)
    {
      FindFieldValueConflictsQuery findFieldValueConflicts = new(fields, content, language);
      IReadOnlyCollection<FieldValueConflict> conflicts = await _sender.Send(findFieldValueConflicts, cancellationToken);
      foreach (FieldValueConflict conflict in conflicts)
      {
        string errorMessage = $"The field value is already used by the content 'Id={conflict.ContentId.ToGuid()}'.";
        errors.Add(new ValidationFailure(propertyName, errorMessage, conflict.FieldDefinitionId)
        {
          ErrorCode = "FieldValueConflict"
        });
      }
    }

    if (errors.Count > 0 && command.ThrowOnFailure)
    {
      throw new ValidationException(errors);
    }

    return new ValidationResult(errors);
  }
}
