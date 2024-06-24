using Logitar.Cms.Core.Fields;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.ContentTypes;

public record FieldDefinitionUnit
{
  public FieldTypeId FieldTypeId { get; }

  public bool IsInvariant { get; }
  public bool IsRequired { get; }
  public bool IsIndexed { get; }
  public bool IsUnique { get; }

  public IdentifierUnit UniqueName { get; set; }
  public DisplayNameUnit? DisplayName { get; set; }
  public DescriptionUnit? Description { get; set; }
  public PlaceholderUnit? Placeholder { get; set; }

  public FieldDefinitionUnit(FieldTypeId fieldTypeId, IdentifierUnit uniqueName, DisplayNameUnit? displayName = null, DescriptionUnit? description = null,
    PlaceholderUnit? placeholder = null, bool isInvariant = true, bool isRequired = false, bool isIndexed = false, bool isUnique = false)
  {
    FieldTypeId = fieldTypeId;

    IsInvariant = isInvariant;
    IsRequired = isRequired;
    IsIndexed = isIndexed;
    IsUnique = isUnique;

    UniqueName = uniqueName;
    DisplayName = displayName;
    Description = description;
    Placeholder = placeholder;
  }
}
