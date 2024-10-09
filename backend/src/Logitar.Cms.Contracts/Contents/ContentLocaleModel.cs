using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Languages;

namespace Logitar.Cms.Contracts.Contents;

public record ContentLocaleModel
{
  public ContentModel Content { get; set; }
  public LanguageModel? Language { get; set; }

  public string UniqueName { get; set; }

  public Actor CreatedBy { get; set; } = Actor.System;
  public DateTime CreatedOn { get; set; }
  public Actor UpdatedBy { get; set; } = Actor.System;
  public DateTime UpdatedOn { get; set; }

  public ContentLocaleModel()
  {
    Content = new()
    {
      Invariant = this
    };

    UniqueName = string.Empty;
  }

  public ContentLocaleModel(ContentModel content, string uniqueName)
  {
    Content = content;

    UniqueName = uniqueName;
  }

  public override string ToString() => $"{UniqueName} | {GetType()} (ContentId={Content.Id}, LanguageId={Language?.Id.ToString() ?? "<null>"})";
}
