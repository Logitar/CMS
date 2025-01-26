﻿namespace Logitar.Cms.Core.Contents.Models;

public record LanguageFilter
{
  public List<int> Ids { get; set; } = [];
  public List<Guid> Uids { get; set; } = [];
  public List<string> Codes { get; set; } = [];

  public bool? IsDefault { get; set; }
}
