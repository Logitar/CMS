﻿using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.Fields;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Contents;

internal class ContentManager : IContentManager
{
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public ContentManager(
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    IContentTypeRepository contentTypeRepository,
    IFieldTypeRepository fieldTypeRepository)
  {
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
    _contentTypeRepository = contentTypeRepository;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task SaveAsync(Content content, CancellationToken cancellationToken)
  {
    ContentType contentType = await _contentTypeRepository.LoadAsync(content, cancellationToken);
    await SaveAsync(content, contentType, cancellationToken);
  }
  public async Task SaveAsync(Content content, ContentType contentType, CancellationToken cancellationToken)
  {
    if (contentType.Id != content.ContentTypeId)
    {
      throw new ArgumentException($"The content type 'Id={contentType.Id}' was not expected. The expected content type for content 'Id={content.Id}' is '{content.ContentTypeId}'.", nameof(contentType));
    }

    HashSet<LanguageId?> languageIds = new(capacity: content.Locales.Count + 1);
    foreach (IEvent change in content.Changes)
    {
      if (change is ContentCreated)
      {
        languageIds.Add(null);
      }
      else if (change is ContentLocaleChanged localeChanged)
      {
        languageIds.Add(localeChanged.LanguageId);
      }
    }

    if (languageIds.Count > 0)
    {
      HashSet<FieldTypeId> fieldTypeIds = contentType.FieldDefinitions.Select(x => x.FieldTypeId).ToHashSet();
      Dictionary<FieldTypeId, FieldType> fieldTypes = (await _fieldTypeRepository.LoadAsync(fieldTypeIds, cancellationToken))
        .ToDictionary(x => x.Id, x => x);

      foreach (LanguageId? languageId in languageIds)
      {
        ContentLocale invariantOrLocale = languageId.HasValue ? content.FindLocale(languageId.Value) : content.Invariant;

        ContentId? conflictId = await _contentQuerier.FindIdAsync(content.ContentTypeId, languageId, invariantOrLocale.UniqueName, cancellationToken);
        if (conflictId.HasValue && !conflictId.Value.Equals(content.Id))
        {
          throw new ContentUniqueNameAlreadyUsedException(content, languageId, invariantOrLocale, conflictId.Value);
        }

        await ValidateAsync(contentType, fieldTypes, content.Id, languageId, invariantOrLocale.FieldValues, cancellationToken);
      }
    }

    await _contentRepository.SaveAsync(content, cancellationToken);
  }

  private async Task ValidateAsync(
    ContentType contentType,
    Dictionary<FieldTypeId, FieldType> fieldTypes,
    ContentId contentId,
    LanguageId? languageId,
    IReadOnlyDictionary<Guid, string> fieldValues,
    CancellationToken cancellationToken)
  {
    bool isInvariant = languageId == null;
    int capacity = contentType.FieldDefinitions.Count;
    Dictionary<Guid, FieldDefinition> fieldDefinitions = new(capacity);
    HashSet<Guid> requiredIds = new(capacity);
    HashSet<Guid> uniqueIds = new(capacity);
    foreach (FieldDefinition fieldDefinition in contentType.FieldDefinitions)
    {
      if (fieldDefinition.IsInvariant == isInvariant)
      {
        fieldDefinitions[fieldDefinition.Id] = fieldDefinition;

        if (fieldDefinition.IsRequired)
        {
          requiredIds.Add(fieldDefinition.Id);
        }
        if (fieldDefinition.IsUnique)
        {
          uniqueIds.Add(fieldDefinition.Id);
        }
      }
    }

    capacity = fieldValues.Count;
    List<Guid> unexpectedIds = new(capacity);
    List<ValidationFailure> validationFailures = [];
    Dictionary<Guid, string> uniqueValues = new(capacity);
    foreach (KeyValuePair<Guid, string> fieldValue in fieldValues)
    {
      if (!fieldDefinitions.TryGetValue(fieldValue.Key, out FieldDefinition? fieldDefinition))
      {
        unexpectedIds.Add(fieldValue.Key);
        continue;
      }

      requiredIds.Remove(fieldValue.Key);

      FieldType fieldType = fieldTypes[fieldDefinition.FieldTypeId];
      ValidationResult result = fieldType.Validate(fieldValue.Value);
      if (!result.IsValid)
      {
        validationFailures.AddRange(result.Errors);
        continue;
      }

      if (uniqueIds.Contains(fieldValue.Key))
      {
        uniqueValues[fieldValue.Key] = fieldValue.Value;
      }
    }

    if (unexpectedIds.Count > 0)
    {
      IEnumerable<ValidationFailure> unexpectedFailures = unexpectedIds.Select(id => new ValidationFailure("FieldValues", "The specified field identifiers were not expected.", id)
      {
        ErrorCode = "UnexpectedFieldValidator"
      });
      throw new ValidationException(unexpectedFailures);
    }
    if (requiredIds.Count > 0) // TODO(fpion): we don't care when saving content draft.
    {
      IEnumerable<ValidationFailure> requiredFailures = requiredIds.Select(id => new ValidationFailure("FieldValues", "The specified field identifiers are missing.", id)
      {
        ErrorCode = "RequiredFieldValidator"
      });
      throw new ValidationException(requiredFailures);
    }
    if (validationFailures.Count > 0)
    {
      throw new ValidationException(validationFailures);
    }

    if (uniqueValues.Count > 0)
    {
      IReadOnlyDictionary<Guid, ContentId> conflicts = await _contentQuerier.FindConflictsAsync(contentType.Id, languageId, uniqueValues, contentId, cancellationToken);
      if (conflicts.Count > 0)
      {
        throw new NotImplementedException(); // TODO(fpion): typed exception
      }
    }
  }
}