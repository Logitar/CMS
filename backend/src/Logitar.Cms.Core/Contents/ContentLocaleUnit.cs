using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Contents;

public record ContentLocaleUnit(UniqueNameUnit UniqueName, DisplayNameUnit? DisplayName = null, DescriptionUnit? Description = null);
