﻿using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Fields;

public record FieldDefinition(
  FieldTypeId FieldTypeId,
  bool IsInvariant,
  bool IsRequired,
  bool IsIndexed,
  bool IsUnique,
  Identifier UniqueName,
  DisplayName? DisplayName,
  Description? Description,
  Placeholder? Placeholder);