using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Cms.Core.Indexing;
using Logitar.Cms.Core.Languages;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

internal class ValidateFieldValuesCommandHandler : IRequestHandler<ValidateFieldValuesCommand, IReadOnlyCollection<ValidationFailure>>
{
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly IIndexService _indexService;

  public ValidateFieldValuesCommandHandler(IFieldTypeRepository fieldTypeRepository, IIndexService indexService)
  {
    _fieldTypeRepository = fieldTypeRepository;
    _indexService = indexService;
  }

  public async Task<IReadOnlyCollection<ValidationFailure>> Handle(ValidateFieldValuesCommand command, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<FieldValuePayload> fields = command.Fields;
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
    List<FieldValuePayload> uniqueValues = new(capacity: fields.Count);

    foreach (FieldValuePayload field in fields)
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
          ? "The field is invariant, but the current locale is not."
          : "The field is not invariant, but the current locale is invariant.";
        errors.Add(new ValidationFailure(propertyName, errorMessage, field.Id)
        {
          ErrorCode = "InvalidFieldLocale"
        });
        continue;
      }

      FieldTypeAggregate fieldType = fieldTypes[definition.FieldTypeId];
      ValidationResult result = fieldType.Validate(field.Value);
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
      IReadOnlyCollection<FieldValueConflict> conflicts = await _indexService.GetConflictsAsync(uniqueValues, content.Id, language?.Id, cancellationToken);
      foreach (FieldValueConflict conflict in conflicts)
      {
        string errorMessage = $"The field value is already used by the content 'Id={conflict.ContentId.ToGuid()}'.";
        errors.Add(new ValidationFailure(propertyName, errorMessage, conflict.FieldId)
        {
          ErrorCode = "FieldValueConflict"
        });
      }
    }

    if (errors.Count > 0 && command.ThrowOnFailure)
    {
      throw new ValidationException(errors);
    }

    return errors.AsReadOnly();
  }
}
