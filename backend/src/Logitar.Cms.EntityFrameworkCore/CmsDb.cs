﻿using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore;

public static class CmsDb
{
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();

  public static class BooleanFieldIndex
  {
    public static readonly TableId Table = new(nameof(CmsContext.BooleanFieldIndex));

    public static readonly ColumnId ContentItemId = new(nameof(BooleanFieldIndexEntity.ContentItemId), Table);
    public static readonly ColumnId ContentItemUid = new(nameof(BooleanFieldIndexEntity.ContentItemUid), Table);
    public static readonly ColumnId ContentLocaleId = new(nameof(BooleanFieldIndexEntity.ContentLocaleId), Table);
    public static readonly ColumnId ContentLocaleName = new(nameof(BooleanFieldIndexEntity.ContentLocaleName), Table);
    public static readonly ColumnId ContentLocaleUid = new(nameof(BooleanFieldIndexEntity.ContentLocaleUid), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(BooleanFieldIndexEntity.ContentTypeId), Table);
    public static readonly ColumnId ContentTypeName = new(nameof(BooleanFieldIndexEntity.ContentTypeName), Table);
    public static readonly ColumnId ContentTypeUid = new(nameof(BooleanFieldIndexEntity.ContentTypeUid), Table);
    public static readonly ColumnId FieldDefinitionId = new(nameof(BooleanFieldIndexEntity.FieldDefinitionId), Table);
    public static readonly ColumnId FieldDefinitionName = new(nameof(BooleanFieldIndexEntity.FieldDefinitionName), Table);
    public static readonly ColumnId FieldDefinitionUid = new(nameof(BooleanFieldIndexEntity.FieldDefinitionUid), Table);
    public static readonly ColumnId FieldTypeId = new(nameof(BooleanFieldIndexEntity.FieldTypeId), Table);
    public static readonly ColumnId FieldTypeName = new(nameof(BooleanFieldIndexEntity.FieldTypeName), Table);
    public static readonly ColumnId FieldTypeUid = new(nameof(BooleanFieldIndexEntity.FieldTypeUid), Table);
    public static readonly ColumnId LanguageCode = new(nameof(BooleanFieldIndexEntity.LanguageCode), Table);
    public static readonly ColumnId LanguageId = new(nameof(BooleanFieldIndexEntity.LanguageId), Table);
    public static readonly ColumnId LanguageUid = new(nameof(BooleanFieldIndexEntity.LanguageUid), Table);
    public static readonly ColumnId BooleanFieldIndexId = new(nameof(BooleanFieldIndexId), Table);
    public static readonly ColumnId Value = new(nameof(BooleanFieldIndexEntity.Value), Table);
  }

  public static class ContentItems
  {
    public static readonly TableId Table = new(nameof(CmsContext.ContentItems));

    public static readonly ColumnId AggregateId = new(nameof(ContentItemEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ContentItemEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ContentItemEntity.CreatedOn), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ContentItemEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ContentItemEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(ContentItemEntity.Version), Table);

    public static readonly ColumnId ContentItemId = new(nameof(ContentItemEntity.ContentItemId), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(ContentItemEntity.ContentTypeId), Table);
    public static readonly ColumnId UniqueId = new(nameof(ContentItemEntity.UniqueId), Table);
  }

  public static class ContentLocales
  {
    public static readonly TableId Table = new(nameof(CmsContext.ContentLocales));

    public static readonly ColumnId CreatedBy = new(nameof(ContentLocaleEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ContentLocaleEntity.CreatedOn), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ContentLocaleEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ContentLocaleEntity.UpdatedOn), Table);

    public static readonly ColumnId ContentItemId = new(nameof(ContentLocaleEntity.ContentItemId), Table);
    public static readonly ColumnId ContentLocaleId = new(nameof(ContentLocaleEntity.ContentLocaleId), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(ContentLocaleEntity.ContentTypeId), Table);
    public static readonly ColumnId Fields = new(nameof(ContentLocaleEntity.Fields), Table);
    public static readonly ColumnId LanguageId = new(nameof(ContentLocaleEntity.LanguageId), Table);
    public static readonly ColumnId UniqueId = new(nameof(ContentLocaleEntity.UniqueId), Table);
    public static readonly ColumnId UniqueName = new(nameof(ContentLocaleEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(ContentLocaleEntity.UniqueNameNormalized), Table);
  }

  public static class ContentTypes
  {
    public static readonly TableId Table = new(nameof(CmsContext.ContentTypes));

    public static readonly ColumnId AggregateId = new(nameof(ContentTypeEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ContentTypeEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ContentTypeEntity.CreatedOn), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ContentTypeEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ContentTypeEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(ContentTypeEntity.Version), Table);

    public static readonly ColumnId ContentTypeId = new(nameof(ContentTypeEntity.ContentTypeId), Table);
    public static readonly ColumnId Description = new(nameof(ContentTypeEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(ContentTypeEntity.DisplayName), Table);
    public static readonly ColumnId IsInvariant = new(nameof(ContentTypeEntity.IsInvariant), Table);
    public static readonly ColumnId UniqueId = new(nameof(ContentTypeEntity.UniqueId), Table);
    public static readonly ColumnId UniqueName = new(nameof(ContentTypeEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(ContentTypeEntity.UniqueNameNormalized), Table);
  }

  public static class DateTimeFieldIndex
  {
    public static readonly TableId Table = new(nameof(CmsContext.DateTimeFieldIndex));

    public static readonly ColumnId ContentItemId = new(nameof(DateTimeFieldIndexEntity.ContentItemId), Table);
    public static readonly ColumnId ContentItemUid = new(nameof(DateTimeFieldIndexEntity.ContentItemUid), Table);
    public static readonly ColumnId ContentLocaleId = new(nameof(DateTimeFieldIndexEntity.ContentLocaleId), Table);
    public static readonly ColumnId ContentLocaleName = new(nameof(DateTimeFieldIndexEntity.ContentLocaleName), Table);
    public static readonly ColumnId ContentLocaleUid = new(nameof(DateTimeFieldIndexEntity.ContentLocaleUid), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(DateTimeFieldIndexEntity.ContentTypeId), Table);
    public static readonly ColumnId ContentTypeName = new(nameof(DateTimeFieldIndexEntity.ContentTypeName), Table);
    public static readonly ColumnId ContentTypeUid = new(nameof(DateTimeFieldIndexEntity.ContentTypeUid), Table);
    public static readonly ColumnId FieldDefinitionId = new(nameof(DateTimeFieldIndexEntity.FieldDefinitionId), Table);
    public static readonly ColumnId FieldDefinitionName = new(nameof(DateTimeFieldIndexEntity.FieldDefinitionName), Table);
    public static readonly ColumnId FieldDefinitionUid = new(nameof(DateTimeFieldIndexEntity.FieldDefinitionUid), Table);
    public static readonly ColumnId FieldTypeId = new(nameof(DateTimeFieldIndexEntity.FieldTypeId), Table);
    public static readonly ColumnId FieldTypeName = new(nameof(DateTimeFieldIndexEntity.FieldTypeName), Table);
    public static readonly ColumnId FieldTypeUid = new(nameof(DateTimeFieldIndexEntity.FieldTypeUid), Table);
    public static readonly ColumnId LanguageCode = new(nameof(DateTimeFieldIndexEntity.LanguageCode), Table);
    public static readonly ColumnId LanguageId = new(nameof(DateTimeFieldIndexEntity.LanguageId), Table);
    public static readonly ColumnId LanguageUid = new(nameof(DateTimeFieldIndexEntity.LanguageUid), Table);
    public static readonly ColumnId DateTimeFieldIndexId = new(nameof(DateTimeFieldIndexId), Table);
    public static readonly ColumnId Value = new(nameof(DateTimeFieldIndexEntity.Value), Table);
  }

  public static class FieldDefinitions
  {
    public static readonly TableId Table = new(nameof(CmsContext.FieldDefinitions));

    public static readonly ColumnId CreatedBy = new(nameof(FieldDefinitionEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(FieldDefinitionEntity.CreatedOn), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(FieldDefinitionEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(FieldDefinitionEntity.UpdatedOn), Table);

    public static readonly ColumnId ContentTypeId = new(nameof(FieldDefinitionEntity.ContentTypeId), Table);
    public static readonly ColumnId Description = new(nameof(FieldDefinitionEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(FieldDefinitionEntity.DisplayName), Table);
    public static readonly ColumnId FieldDefinitionId = new(nameof(FieldDefinitionEntity.FieldDefinitionId), Table);
    public static readonly ColumnId FieldTypeId = new(nameof(FieldDefinitionEntity.FieldTypeId), Table);
    public static readonly ColumnId IsIndexed = new(nameof(FieldDefinitionEntity.IsIndexed), Table);
    public static readonly ColumnId IsInvariant = new(nameof(FieldDefinitionEntity.IsInvariant), Table);
    public static readonly ColumnId IsRequired = new(nameof(FieldDefinitionEntity.IsRequired), Table);
    public static readonly ColumnId IsUnique = new(nameof(FieldDefinitionEntity.IsUnique), Table);
    public static readonly ColumnId Order = new(nameof(FieldDefinitionEntity.Order), Table);
    public static readonly ColumnId Placeholder = new(nameof(FieldDefinitionEntity.Placeholder), Table);
    public static readonly ColumnId UniqueId = new(nameof(FieldDefinitionEntity.UniqueId), Table);
    public static readonly ColumnId UniqueName = new(nameof(FieldDefinitionEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(FieldDefinitionEntity.UniqueNameNormalized), Table);
  }

  public static class FieldTypes
  {
    public static readonly TableId Table = new(nameof(CmsContext.FieldTypes));

    public static readonly ColumnId AggregateId = new(nameof(FieldTypeEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(FieldTypeEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(FieldTypeEntity.CreatedOn), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(FieldTypeEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(FieldTypeEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(FieldTypeEntity.Version), Table);

    public static readonly ColumnId DataType = new(nameof(FieldTypeEntity.DataType), Table);
    public static readonly ColumnId Description = new(nameof(FieldTypeEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(FieldTypeEntity.DisplayName), Table);
    public static readonly ColumnId FieldTypeId = new(nameof(FieldTypeEntity.FieldTypeId), Table);
    public static readonly ColumnId Properties = new(nameof(FieldTypeEntity.Properties), Table);
    public static readonly ColumnId UniqueId = new(nameof(FieldTypeEntity.UniqueId), Table);
    public static readonly ColumnId UniqueName = new(nameof(FieldTypeEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(FieldTypeEntity.UniqueNameNormalized), Table);
  }

  public static class Languages
  {
    public static readonly TableId Table = new(nameof(CmsContext.Languages));

    public static readonly ColumnId AggregateId = new(nameof(LanguageEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(LanguageEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(LanguageEntity.CreatedOn), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(LanguageEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(LanguageEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(LanguageEntity.Version), Table);

    public static readonly ColumnId Code = new(nameof(LanguageEntity.Code), Table);
    public static readonly ColumnId CodeNormalized = new(nameof(LanguageEntity.CodeNormalized), Table);
    public static readonly ColumnId DisplayName = new(nameof(LanguageEntity.DisplayName), Table);
    public static readonly ColumnId EnglishName = new(nameof(LanguageEntity.EnglishName), Table);
    public static readonly ColumnId IsDefault = new(nameof(LanguageEntity.IsDefault), Table);
    public static readonly ColumnId LanguageId = new(nameof(LanguageEntity.LanguageId), Table);
    public static readonly ColumnId LCID = new(nameof(LanguageEntity.LCID), Table);
    public static readonly ColumnId NativeName = new(nameof(LanguageEntity.NativeName), Table);
    public static readonly ColumnId UniqueId = new(nameof(LanguageEntity.UniqueId), Table);
  }

  public static class LogEvents
  {
    public static readonly TableId Table = new(nameof(CmsContext.LogEvents));

    public static readonly ColumnId EventId = new(nameof(LogEventEntity.EventId));
    public static readonly ColumnId LogId = new(nameof(LogEventEntity.LogId));
  }

  public static class LogExceptions
  {
    public static readonly TableId Table = new(nameof(CmsContext.LogExceptions));

    public static readonly ColumnId Data = new(nameof(LogExceptionEntity.Data));
    public static readonly ColumnId HResult = new(nameof(LogExceptionEntity.HResult));
    public static readonly ColumnId HelpLink = new(nameof(LogExceptionEntity.HelpLink));
    public static readonly ColumnId LogExceptionId = new(nameof(LogExceptionEntity.LogExceptionId));
    public static readonly ColumnId LogId = new(nameof(LogExceptionEntity.LogId));
    public static readonly ColumnId Message = new(nameof(LogExceptionEntity.Message));
    public static readonly ColumnId Source = new(nameof(LogExceptionEntity.Source));
    public static readonly ColumnId StackTrace = new(nameof(LogExceptionEntity.StackTrace));
    public static readonly ColumnId TargetSite = new(nameof(LogExceptionEntity.TargetSite));
    public static readonly ColumnId Type = new(nameof(LogExceptionEntity.Type));
  }

  public static class Logs
  {
    public static readonly TableId Table = new(nameof(CmsContext.Logs));

    public static readonly ColumnId ActivityData = new(nameof(LogEntity.ActivityData));
    public static readonly ColumnId ActivityType = new(nameof(LogEntity.ActivityType));
    public static readonly ColumnId ActorId = new(nameof(LogEntity.ActorId));
    public static readonly ColumnId AdditionalInformation = new(nameof(LogEntity.AdditionalInformation));
    public static readonly ColumnId ApiKeyId = new(nameof(LogEntity.ApiKeyId));
    public static readonly ColumnId CorrelationId = new(nameof(LogEntity.CorrelationId));
    public static readonly ColumnId Destination = new(nameof(LogEntity.Destination));
    public static readonly ColumnId Duration = new(nameof(LogEntity.Duration));
    public static readonly ColumnId EndedOn = new(nameof(LogEntity.EndedOn));
    public static readonly ColumnId HasErrors = new(nameof(LogEntity.HasErrors));
    public static readonly ColumnId IsCompleted = new(nameof(LogEntity.IsCompleted));
    public static readonly ColumnId Level = new(nameof(LogEntity.Level));
    public static readonly ColumnId LogId = new(nameof(LogEntity.LogId));
    public static readonly ColumnId Method = new(nameof(LogEntity.Method));
    public static readonly ColumnId OperationName = new(nameof(LogEntity.OperationName));
    public static readonly ColumnId OperationType = new(nameof(LogEntity.OperationType));
    public static readonly ColumnId SessionId = new(nameof(LogEntity.SessionId));
    public static readonly ColumnId Source = new(nameof(LogEntity.Source));
    public static readonly ColumnId StartedOn = new(nameof(LogEntity.StartedOn));
    public static readonly ColumnId StatusCode = new(nameof(LogEntity.StatusCode));
    public static readonly ColumnId TenantId = new(nameof(LogEntity.TenantId));
    public static readonly ColumnId UniqueId = new(nameof(LogEntity.UniqueId));
    public static readonly ColumnId UserId = new(nameof(LogEntity.UserId));
  }

  public static class NumberFieldIndex
  {
    public static readonly TableId Table = new(nameof(CmsContext.NumberFieldIndex));

    public static readonly ColumnId ContentItemId = new(nameof(NumberFieldIndexEntity.ContentItemId), Table);
    public static readonly ColumnId ContentItemUid = new(nameof(NumberFieldIndexEntity.ContentItemUid), Table);
    public static readonly ColumnId ContentLocaleId = new(nameof(NumberFieldIndexEntity.ContentLocaleId), Table);
    public static readonly ColumnId ContentLocaleName = new(nameof(NumberFieldIndexEntity.ContentLocaleName), Table);
    public static readonly ColumnId ContentLocaleUid = new(nameof(NumberFieldIndexEntity.ContentLocaleUid), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(NumberFieldIndexEntity.ContentTypeId), Table);
    public static readonly ColumnId ContentTypeName = new(nameof(NumberFieldIndexEntity.ContentTypeName), Table);
    public static readonly ColumnId ContentTypeUid = new(nameof(NumberFieldIndexEntity.ContentTypeUid), Table);
    public static readonly ColumnId FieldDefinitionId = new(nameof(NumberFieldIndexEntity.FieldDefinitionId), Table);
    public static readonly ColumnId FieldDefinitionName = new(nameof(NumberFieldIndexEntity.FieldDefinitionName), Table);
    public static readonly ColumnId FieldDefinitionUid = new(nameof(NumberFieldIndexEntity.FieldDefinitionUid), Table);
    public static readonly ColumnId FieldTypeId = new(nameof(NumberFieldIndexEntity.FieldTypeId), Table);
    public static readonly ColumnId FieldTypeName = new(nameof(NumberFieldIndexEntity.FieldTypeName), Table);
    public static readonly ColumnId FieldTypeUid = new(nameof(NumberFieldIndexEntity.FieldTypeUid), Table);
    public static readonly ColumnId LanguageCode = new(nameof(NumberFieldIndexEntity.LanguageCode), Table);
    public static readonly ColumnId LanguageId = new(nameof(NumberFieldIndexEntity.LanguageId), Table);
    public static readonly ColumnId LanguageUid = new(nameof(NumberFieldIndexEntity.LanguageUid), Table);
    public static readonly ColumnId NumberFieldIndexId = new(nameof(NumberFieldIndexId), Table);
    public static readonly ColumnId Value = new(nameof(NumberFieldIndexEntity.Value), Table);
  }

  public static class StringFieldIndex
  {
    public static readonly TableId Table = new(nameof(CmsContext.StringFieldIndex));

    public static readonly ColumnId ContentItemId = new(nameof(StringFieldIndexEntity.ContentItemId), Table);
    public static readonly ColumnId ContentItemUid = new(nameof(StringFieldIndexEntity.ContentItemUid), Table);
    public static readonly ColumnId ContentLocaleId = new(nameof(StringFieldIndexEntity.ContentLocaleId), Table);
    public static readonly ColumnId ContentLocaleName = new(nameof(StringFieldIndexEntity.ContentLocaleName), Table);
    public static readonly ColumnId ContentLocaleUid = new(nameof(StringFieldIndexEntity.ContentLocaleUid), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(StringFieldIndexEntity.ContentTypeId), Table);
    public static readonly ColumnId ContentTypeName = new(nameof(StringFieldIndexEntity.ContentTypeName), Table);
    public static readonly ColumnId ContentTypeUid = new(nameof(StringFieldIndexEntity.ContentTypeUid), Table);
    public static readonly ColumnId FieldDefinitionId = new(nameof(StringFieldIndexEntity.FieldDefinitionId), Table);
    public static readonly ColumnId FieldDefinitionName = new(nameof(StringFieldIndexEntity.FieldDefinitionName), Table);
    public static readonly ColumnId FieldDefinitionUid = new(nameof(StringFieldIndexEntity.FieldDefinitionUid), Table);
    public static readonly ColumnId FieldTypeId = new(nameof(StringFieldIndexEntity.FieldTypeId), Table);
    public static readonly ColumnId FieldTypeName = new(nameof(StringFieldIndexEntity.FieldTypeName), Table);
    public static readonly ColumnId FieldTypeUid = new(nameof(StringFieldIndexEntity.FieldTypeUid), Table);
    public static readonly ColumnId LanguageCode = new(nameof(StringFieldIndexEntity.LanguageCode), Table);
    public static readonly ColumnId LanguageId = new(nameof(StringFieldIndexEntity.LanguageId), Table);
    public static readonly ColumnId LanguageUid = new(nameof(StringFieldIndexEntity.LanguageUid), Table);
    public static readonly ColumnId StringFieldIndexId = new(nameof(StringFieldIndexId), Table);
    public static readonly ColumnId Value = new(nameof(StringFieldIndexEntity.Value), Table);
  }

  public static class TextFieldIndex
  {
    public static readonly TableId Table = new(nameof(CmsContext.TextFieldIndex));

    public static readonly ColumnId ContentItemId = new(nameof(TextFieldIndexEntity.ContentItemId), Table);
    public static readonly ColumnId ContentItemUid = new(nameof(TextFieldIndexEntity.ContentItemUid), Table);
    public static readonly ColumnId ContentLocaleId = new(nameof(TextFieldIndexEntity.ContentLocaleId), Table);
    public static readonly ColumnId ContentLocaleName = new(nameof(TextFieldIndexEntity.ContentLocaleName), Table);
    public static readonly ColumnId ContentLocaleUid = new(nameof(TextFieldIndexEntity.ContentLocaleUid), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(TextFieldIndexEntity.ContentTypeId), Table);
    public static readonly ColumnId ContentTypeName = new(nameof(TextFieldIndexEntity.ContentTypeName), Table);
    public static readonly ColumnId ContentTypeUid = new(nameof(TextFieldIndexEntity.ContentTypeUid), Table);
    public static readonly ColumnId FieldDefinitionId = new(nameof(TextFieldIndexEntity.FieldDefinitionId), Table);
    public static readonly ColumnId FieldDefinitionName = new(nameof(TextFieldIndexEntity.FieldDefinitionName), Table);
    public static readonly ColumnId FieldDefinitionUid = new(nameof(TextFieldIndexEntity.FieldDefinitionUid), Table);
    public static readonly ColumnId FieldTypeId = new(nameof(TextFieldIndexEntity.FieldTypeId), Table);
    public static readonly ColumnId FieldTypeName = new(nameof(TextFieldIndexEntity.FieldTypeName), Table);
    public static readonly ColumnId FieldTypeUid = new(nameof(TextFieldIndexEntity.FieldTypeUid), Table);
    public static readonly ColumnId LanguageCode = new(nameof(TextFieldIndexEntity.LanguageCode), Table);
    public static readonly ColumnId LanguageId = new(nameof(TextFieldIndexEntity.LanguageId), Table);
    public static readonly ColumnId LanguageUid = new(nameof(TextFieldIndexEntity.LanguageUid), Table);
    public static readonly ColumnId TextFieldIndexId = new(nameof(TextFieldIndexId), Table);
    public static readonly ColumnId Value = new(nameof(TextFieldIndexEntity.Value), Table);
  }

  public static class UniqueFieldIndex
  {
    public static readonly TableId Table = new(nameof(CmsContext.UniqueFieldIndex));

    public static readonly ColumnId ContentItemId = new(nameof(UniqueFieldIndexEntity.ContentItemId), Table);
    public static readonly ColumnId ContentItemUid = new(nameof(UniqueFieldIndexEntity.ContentItemUid), Table);
    public static readonly ColumnId ContentLocaleId = new(nameof(UniqueFieldIndexEntity.ContentLocaleId), Table);
    public static readonly ColumnId ContentLocaleName = new(nameof(UniqueFieldIndexEntity.ContentLocaleName), Table);
    public static readonly ColumnId ContentLocaleUid = new(nameof(UniqueFieldIndexEntity.ContentLocaleUid), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(UniqueFieldIndexEntity.ContentTypeId), Table);
    public static readonly ColumnId ContentTypeName = new(nameof(UniqueFieldIndexEntity.ContentTypeName), Table);
    public static readonly ColumnId ContentTypeUid = new(nameof(UniqueFieldIndexEntity.ContentTypeUid), Table);
    public static readonly ColumnId FieldDefinitionId = new(nameof(UniqueFieldIndexEntity.FieldDefinitionId), Table);
    public static readonly ColumnId FieldDefinitionName = new(nameof(UniqueFieldIndexEntity.FieldDefinitionName), Table);
    public static readonly ColumnId FieldDefinitionUid = new(nameof(UniqueFieldIndexEntity.FieldDefinitionUid), Table);
    public static readonly ColumnId FieldTypeId = new(nameof(UniqueFieldIndexEntity.FieldTypeId), Table);
    public static readonly ColumnId FieldTypeName = new(nameof(UniqueFieldIndexEntity.FieldTypeName), Table);
    public static readonly ColumnId FieldTypeUid = new(nameof(UniqueFieldIndexEntity.FieldTypeUid), Table);
    public static readonly ColumnId LanguageCode = new(nameof(UniqueFieldIndexEntity.LanguageCode), Table);
    public static readonly ColumnId LanguageId = new(nameof(UniqueFieldIndexEntity.LanguageId), Table);
    public static readonly ColumnId LanguageUid = new(nameof(UniqueFieldIndexEntity.LanguageUid), Table);
    public static readonly ColumnId UniqueFieldIndexId = new(nameof(UniqueFieldIndexId), Table);
    public static readonly ColumnId Value = new(nameof(UniqueFieldIndexEntity.Value), Table);
  }
}
