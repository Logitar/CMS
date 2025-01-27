﻿using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Localization.Models;

namespace Logitar.Cms.Core.Contents.Models;

public record ContentLocaleModel
{
  public ContentModel Content { get; set; }
  public LanguageModel? Language { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<FieldValue> FieldValues { get; set; } = [];

  public long Revision { get; set; }

  public ActorModel CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public ActorModel UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public bool IsPublished { get; set; }
  public long? PublishedRevision { get; set; }
  public ActorModel? PublishedBy { get; set; }
  public DateTime? PublishedOn { get; set; }

  public ContentLocaleModel(ContentModel content)
  {
    Content = content;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName}";
}
