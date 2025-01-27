﻿using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;

namespace Logitar.Cms.Infrastructure.CmsDb;

public static class UniqueIndex
{
  public static readonly TableId Table = new(nameof(CmsContext.UniqueIndex));

  public static readonly ColumnId ContentId = new(nameof(UniqueIndexEntity.ContentId), Table);
  public static readonly ColumnId ContentLocaleId = new(nameof(UniqueIndexEntity.ContentLocaleId), Table);
  public static readonly ColumnId ContentLocaleName = new(nameof(UniqueIndexEntity.ContentLocaleName), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(UniqueIndexEntity.ContentTypeId), Table);
  public static readonly ColumnId ContentTypeName = new(nameof(UniqueIndexEntity.ContentTypeName), Table);
  public static readonly ColumnId ContentTypeUid = new(nameof(UniqueIndexEntity.ContentTypeUid), Table);
  public static readonly ColumnId ContentUid = new(nameof(UniqueIndexEntity.ContentUid), Table);
  public static readonly ColumnId FieldDefinitionId = new(nameof(UniqueIndexEntity.FieldDefinitionId), Table);
  public static readonly ColumnId FieldDefinitionName = new(nameof(UniqueIndexEntity.FieldTypeName), Table);
  public static readonly ColumnId FieldDefinitionUid = new(nameof(UniqueIndexEntity.FieldDefinitionUid), Table);
  public static readonly ColumnId FieldTypeId = new(nameof(UniqueIndexEntity.FieldTypeId), Table);
  public static readonly ColumnId FieldTypeName = new(nameof(UniqueIndexEntity.FieldTypeName), Table);
  public static readonly ColumnId FieldTypeUid = new(nameof(UniqueIndexEntity.FieldTypeUid), Table);
  public static readonly ColumnId Key = new(nameof(UniqueIndexEntity.Key), Table);
  public static readonly ColumnId LanguageCode = new(nameof(UniqueIndexEntity.LanguageCode), Table);
  public static readonly ColumnId LanguageId = new(nameof(UniqueIndexEntity.LanguageId), Table);
  public static readonly ColumnId LanguageIsDefault = new(nameof(UniqueIndexEntity.LanguageIsDefault), Table);
  public static readonly ColumnId LanguageUid = new(nameof(UniqueIndexEntity.LanguageUid), Table);
  public static readonly ColumnId Revision = new(nameof(UniqueIndexEntity.Revision), Table);
  public static readonly ColumnId Status = new(nameof(UniqueIndexEntity.Status), Table);
  public static readonly ColumnId UniqueIndexId = new(nameof(UniqueIndexEntity.UniqueIndexId), Table);
  public static readonly ColumnId Value = new(nameof(UniqueIndexEntity.Value), Table);
  public static readonly ColumnId ValueNormalized = new(nameof(UniqueIndexEntity.ValueNormalized), Table);
}
