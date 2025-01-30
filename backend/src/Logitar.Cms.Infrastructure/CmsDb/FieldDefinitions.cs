using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;

namespace Logitar.Cms.Infrastructure.CmsDb;

public static class FieldDefinitions
{
  public static readonly TableId Table = new(CmsContext.Schema, nameof(CmsContext.FieldDefinitions), alias: null);

  public static readonly ColumnId ContentTypeId = new(nameof(FieldDefinitionEntity.ContentTypeId), Table);
  public static readonly ColumnId Description = new(nameof(FieldDefinitionEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(FieldDefinitionEntity.DisplayName), Table);
  public static readonly ColumnId FieldDefinitionId = new(nameof(FieldDefinitionEntity.FieldDefinitionId), Table);
  public static readonly ColumnId FieldTypeId = new(nameof(FieldDefinitionEntity.FieldTypeId), Table);
  public static readonly ColumnId Id = new(nameof(FieldDefinitionEntity.Id), Table);
  public static readonly ColumnId IsIndexed = new(nameof(FieldDefinitionEntity.IsIndexed), Table);
  public static readonly ColumnId IsInvariant = new(nameof(FieldDefinitionEntity.IsInvariant), Table);
  public static readonly ColumnId IsRequired = new(nameof(FieldDefinitionEntity.IsRequired), Table);
  public static readonly ColumnId IsUnique = new(nameof(FieldDefinitionEntity.IsUnique), Table);
  public static readonly ColumnId Order = new(nameof(FieldDefinitionEntity.Order), Table);
  public static readonly ColumnId Placeholder = new(nameof(FieldDefinitionEntity.Placeholder), Table);
  public static readonly ColumnId UniqueName = new(nameof(FieldDefinitionEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(FieldDefinitionEntity.UniqueNameNormalized), Table);
}
