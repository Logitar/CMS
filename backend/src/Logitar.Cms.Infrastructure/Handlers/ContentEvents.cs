using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Handlers;

internal class ContentEvents : INotificationHandler<ContentCreated>,
  INotificationHandler<ContentLocaleChanged>,
  INotificationHandler<ContentLocalePublished>,
  INotificationHandler<ContentLocaleUnpublished>
{
  private readonly ICommandHelper _commandHelper;
  private readonly CmsContext _context;

  public ContentEvents(ICommandHelper commandHelper, CmsContext context)
  {
    _commandHelper = commandHelper;
    _context = context;
  }

  public async Task Handle(ContentCreated @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await _context.Contents.AsNoTracking()
      .Include(x => x.ContentType).ThenInclude(x => x!.Fields).ThenInclude(x => x.FieldType)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content == null)
    {
      ContentTypeEntity contentType = await _context.ContentTypes
        .Include(x => x.Fields).ThenInclude(x => x.FieldType)
        .SingleOrDefaultAsync(x => x.StreamId == @event.ContentTypeId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The content type entity 'StreamId={@event.ContentTypeId}' could not be found.");

      content = new(contentType, @event);

      _context.Contents.Add(content);

      await _context.SaveChangesAsync(cancellationToken);

      await UpdateIndicesAsync(content.Locales.Single(), ContentStatus.Latest, cancellationToken);
    }
  }

  public async Task Handle(ContentLocaleChanged @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await _context.Contents
      .Include(x => x.ContentType).ThenInclude(x => x!.Fields).ThenInclude(x => x.FieldType)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content != null && content.Version == (@event.Version - 1))
    {
      LanguageEntity? language = null;
      if (@event.LanguageId.HasValue)
      {
        language = await _context.Languages
          .SingleOrDefaultAsync(x => x.StreamId == @event.LanguageId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The language entity 'StreamId={@event.LanguageId}' could not be found.");
      }

      content.SetLocale(language, @event);

      await _context.SaveChangesAsync(cancellationToken);

      ContentLocaleEntity locale = content.Locales.Single(l => l.LanguageId == language?.LanguageId);

      ICommand command = _commandHelper.Update()
        .Set(new Update(CmsDb.FieldIndex.ContentLocaleName, locale.UniqueNameNormalized))
        .Where(new OperatorCondition(CmsDb.FieldIndex.ContentLocaleId, Operators.IsEqualTo(locale.ContentLocaleId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

      command = _commandHelper.Update()
        .Set(new Update(CmsDb.UniqueIndex.ContentLocaleName, locale.UniqueNameNormalized))
        .Where(new OperatorCondition(CmsDb.UniqueIndex.ContentLocaleId, Operators.IsEqualTo(locale.ContentLocaleId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

      await UpdateIndicesAsync(locale, ContentStatus.Latest, cancellationToken);
    }
  }

  public async Task Handle(ContentLocalePublished @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await _context.Contents
      .Include(x => x.ContentType).ThenInclude(x => x!.Fields).ThenInclude(x => x.FieldType)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .Include(x => x.Locales).ThenInclude(x => x.PublishedContent)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content != null && content.Version == (@event.Version - 1))
    {
      ContentLocaleEntity? locale = content.Publish(@event)
        ?? throw new InvalidOperationException($"The content 'StreamId={@event.StreamId}' locale 'LanguageId={@event.LanguageId}' could not be found.");

      await _context.SaveChangesAsync(cancellationToken);

      await UpdateIndicesAsync(locale, ContentStatus.Published, cancellationToken);
    }
  }

  public async Task Handle(ContentLocaleUnpublished @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await _context.Contents
      .Include(x => x.ContentType).ThenInclude(x => x!.Fields).ThenInclude(x => x.FieldType)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .Include(x => x.Locales).ThenInclude(x => x.PublishedContent)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content != null && content.Version == (@event.Version - 1))
    {
      ContentLocaleEntity? locale = content.Unpublish(@event)
        ?? throw new InvalidOperationException($"The content 'StreamId={@event.StreamId}' locale 'LanguageId={@event.LanguageId}' could not be found.");

      if (locale.PublishedContent != null)
      {
        _context.PublishedContents.Remove(locale.PublishedContent);
      }

      await _context.SaveChangesAsync(cancellationToken);

      ICommand command = _commandHelper.Delete(CmsDb.FieldIndex.Table)
        .Where(
          new OperatorCondition(CmsDb.FieldIndex.ContentLocaleId, Operators.IsEqualTo(locale.ContentLocaleId)),
          new OperatorCondition(CmsDb.FieldIndex.Status, Operators.IsEqualTo(ContentStatus.Published.ToString())))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

      command = _commandHelper.Delete(CmsDb.UniqueIndex.Table)
        .Where(
          new OperatorCondition(CmsDb.UniqueIndex.ContentLocaleId, Operators.IsEqualTo(locale.ContentLocaleId)),
          new OperatorCondition(CmsDb.UniqueIndex.Status, Operators.IsEqualTo(ContentStatus.Published.ToString())))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);
    }
  }

  private async Task UpdateIndicesAsync(ContentLocaleEntity locale, ContentStatus status, CancellationToken cancellationToken)
  {
    ContentEntity content = locale.Content ?? throw new ArgumentException("The content is required.", nameof(locale));
    LanguageEntity? language = locale.LanguageId.HasValue
      ? (locale.Language ?? throw new ArgumentException("The language is required.", nameof(locale)))
      : null;
    ContentTypeEntity contentType = content.ContentType ?? throw new ArgumentException("The content type is required.", nameof(locale));
    Dictionary<Guid, FieldDefinitionEntity> fieldDefinitions = contentType.Fields.ToDictionary(x => x.Id, x => x);

    Dictionary<Guid, FieldIndexEntity> indexedFields = await _context.FieldIndex
      .Include(x => x.FieldType)
      .Where(x => x.ContentLocaleId == locale.ContentLocaleId && x.Status == status)
      .ToDictionaryAsync(x => x.FieldDefinitionUid, x => x, cancellationToken);
    Dictionary<Guid, UniqueIndexEntity> uniqueFields = await _context.UniqueIndex
      .Where(x => x.ContentLocaleId == locale.ContentLocaleId && x.Status == status)
      .ToDictionaryAsync(x => x.FieldDefinitionUid, x => x, cancellationToken);

    Dictionary<Guid, string> fieldValues = locale.GetFieldValues();

    foreach (KeyValuePair<Guid, FieldIndexEntity> indexedField in indexedFields)
    {
      if (!fieldValues.ContainsKey(indexedField.Key))
      {
        _context.FieldIndex.Remove(indexedField.Value);
      }
    }
    foreach (KeyValuePair<Guid, UniqueIndexEntity> uniqueField in uniqueFields)
    {
      if (!fieldValues.ContainsKey(uniqueField.Key))
      {
        _context.UniqueIndex.Remove(uniqueField.Value);
      }
    }

    foreach (KeyValuePair<Guid, string> fieldValue in fieldValues)
    {
      FieldDefinitionEntity fieldDefinition = fieldDefinitions[fieldValue.Key];
      FieldTypeEntity fieldType = fieldDefinition.FieldType ?? throw new ArgumentException($"The field definition 'Id={fieldDefinition.Id}' did not include a field type.", nameof(locale));

      if (fieldDefinition.IsIndexed)
      {
        if (indexedFields.TryGetValue(fieldValue.Key, out FieldIndexEntity? indexedField))
        {
          indexedField.Update(fieldValue.Value);
        }
        else
        {
          indexedField = new(contentType, language, fieldType, fieldDefinition, content, locale, status, fieldValue.Value);
          indexedFields[fieldValue.Key] = indexedField;

          _context.FieldIndex.Add(indexedField);
        }
      }

      if (fieldDefinition.IsUnique)
      {
        if (uniqueFields.TryGetValue(fieldValue.Key, out UniqueIndexEntity? uniqueField))
        {
          uniqueField.Update(fieldValue.Value);
        }
        else
        {
          uniqueField = new(contentType, language, fieldType, fieldDefinition, content, locale, status, fieldValue.Value);
          uniqueFields[fieldValue.Key] = uniqueField;

          _context.UniqueIndex.Add(uniqueField);
        }
      }
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
