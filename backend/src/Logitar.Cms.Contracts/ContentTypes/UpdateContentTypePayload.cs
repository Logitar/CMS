using Logitar.Identity.Contracts;

namespace Logitar.Cms.Contracts.ContentTypes;

public record UpdateContentTypePayload
{
  public string? UniqueName { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }
}
