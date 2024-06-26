﻿using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore;

internal static class CmsDb
{
  public static class ContentItems
  {
    public static readonly TableId Table = new(nameof(CmsContext.ContentItems));

    public static readonly ColumnId AggregateId = new(nameof(ContentItemEntity.AggregateId), Table);
    public static readonly ColumnId ContentItemId = new(nameof(ContentItemEntity.ContentItemId), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(ContentItemEntity.ContentTypeId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ContentItemEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ContentItemEntity.CreatedOn), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ContentItemEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ContentItemEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(ContentItemEntity.Version), Table);
  }

  public static class ContentLocales
  {
    public static readonly TableId Table = new(nameof(CmsContext.ContentLocales));

    public static readonly ColumnId ContentItemId = new(nameof(ContentLocaleEntity.ContentItemId), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(ContentLocaleEntity.ContentTypeId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ContentLocaleEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ContentLocaleEntity.CreatedOn), Table);
    public static readonly ColumnId LanguageId = new(nameof(ContentLocaleEntity.LanguageId), Table);
    public static readonly ColumnId UniqueName = new(nameof(ContentLocaleEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(ContentLocaleEntity.UniqueNameNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ContentLocaleEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ContentLocaleEntity.UpdatedOn), Table);
  }

  public static class ContentTypes
  {
    public static readonly TableId Table = new(nameof(CmsContext.ContentTypes));

    public static readonly ColumnId AggregateId = new(nameof(ContentTypeEntity.AggregateId), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(ContentTypeEntity.ContentTypeId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ContentTypeEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ContentTypeEntity.CreatedOn), Table);
    public static readonly ColumnId Description = new(nameof(ContentTypeEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(ContentTypeEntity.DisplayName), Table);
    public static readonly ColumnId IsInvariant = new(nameof(ContentTypeEntity.IsInvariant), Table);
    public static readonly ColumnId UniqueName = new(nameof(ContentTypeEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(ContentTypeEntity.UniqueNameNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ContentTypeEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ContentTypeEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(ContentTypeEntity.Version), Table);
  }

  public static class FieldDefinitions
  {
    public static readonly TableId Table = new(nameof(CmsContext.FieldDefinitions));

    public static readonly ColumnId ContentTypeId = new(nameof(FieldDefinitionEntity.ContentTypeId), Table);
    public static readonly ColumnId ContentTypeName = new(nameof(FieldDefinitionEntity.ContentTypeName), Table);
    public static readonly ColumnId CreatedBy = new(nameof(FieldDefinitionEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(FieldDefinitionEntity.CreatedOn), Table);
    public static readonly ColumnId Description = new(nameof(FieldDefinitionEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(FieldDefinitionEntity.DisplayName), Table);
    public static readonly ColumnId FieldDataType = new(nameof(FieldDefinitionEntity.FieldDataType), Table);
    public static readonly ColumnId FieldDefinitionId = new(nameof(FieldDefinitionEntity.FieldDefinitionId), Table);
    public static readonly ColumnId FieldTypeId = new(nameof(FieldDefinitionEntity.FieldTypeId), Table);
    public static readonly ColumnId FieldTypeName = new(nameof(FieldDefinitionEntity.FieldTypeName), Table);
    public static readonly ColumnId Id = new(nameof(FieldDefinitionEntity.Id), Table);
    public static readonly ColumnId IsIndexed = new(nameof(FieldDefinitionEntity.IsIndexed), Table);
    public static readonly ColumnId IsInvariant = new(nameof(FieldDefinitionEntity.IsInvariant), Table);
    public static readonly ColumnId IsRequired = new(nameof(FieldDefinitionEntity.IsRequired), Table);
    public static readonly ColumnId IsUnique = new(nameof(FieldDefinitionEntity.IsUnique), Table);
    public static readonly ColumnId Order = new(nameof(FieldDefinitionEntity.Order), Table);
    public static readonly ColumnId Placeholder = new(nameof(FieldDefinitionEntity.Placeholder), Table);
    public static readonly ColumnId UniqueName = new(nameof(FieldDefinitionEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(FieldDefinitionEntity.UniqueNameNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(FieldDefinitionEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(FieldDefinitionEntity.UpdatedOn), Table);
  }

  public static class FieldTypes
  {
    public static readonly TableId Table = new(nameof(CmsContext.FieldTypes));

    public static readonly ColumnId AggregateId = new(nameof(FieldTypeEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(FieldTypeEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(FieldTypeEntity.CreatedOn), Table);
    public static readonly ColumnId DataType = new(nameof(FieldTypeEntity.DataType), Table);
    public static readonly ColumnId Description = new(nameof(FieldTypeEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(FieldTypeEntity.DisplayName), Table);
    public static readonly ColumnId FieldTypeId = new(nameof(FieldTypeEntity.FieldTypeId), Table);
    public static readonly ColumnId Properties = new(nameof(FieldTypeEntity.Properties), Table);
    public static readonly ColumnId UniqueName = new(nameof(FieldTypeEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(FieldTypeEntity.UniqueNameNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(FieldTypeEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(FieldTypeEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(FieldTypeEntity.Version), Table);
  }

  public static class Languages
  {
    public static readonly TableId Table = new(nameof(CmsContext.Languages));

    public static readonly ColumnId AggregateId = new(nameof(LanguageEntity.AggregateId), Table);
    public static readonly ColumnId Code = new(nameof(LanguageEntity.Code), Table);
    public static readonly ColumnId CodeNormalized = new(nameof(LanguageEntity.CodeNormalized), Table);
    public static readonly ColumnId CreatedBy = new(nameof(LanguageEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(LanguageEntity.CreatedOn), Table);
    public static readonly ColumnId DisplayName = new(nameof(LanguageEntity.DisplayName), Table);
    public static readonly ColumnId EnglishName = new(nameof(LanguageEntity.EnglishName), Table);
    public static readonly ColumnId IsDefault = new(nameof(LanguageEntity.IsDefault), Table);
    public static readonly ColumnId LCID = new(nameof(LanguageEntity.LCID), Table);
    public static readonly ColumnId LanguageId = new(nameof(LanguageEntity.LanguageId), Table);
    public static readonly ColumnId NativeName = new(nameof(LanguageEntity.NativeName), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(LanguageEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(LanguageEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(LanguageEntity.Version), Table);
  }

  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
