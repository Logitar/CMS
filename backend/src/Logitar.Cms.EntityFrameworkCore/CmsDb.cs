using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore;

public static class CmsDb
{
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();

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
}
