using Logitar.Cms.Core.FieldTypes;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.ContentTypes;

public record FieldDefinitionUnit(
  FieldTypeId FieldTypeId,
  bool IsInvariant,
  bool IsRequired,
  bool IsIndexed,
  bool IsUnique,
  IdentifierUnit UniqueName,
  DisplayNameUnit? DisplayName,
  DescriptionUnit? Description,
  PlaceholderUnit? Placeholder);
