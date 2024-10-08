﻿namespace Logitar.Cms.Contracts.ContentTypes;

public record UpdateContentTypePayload
{
  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }
}
