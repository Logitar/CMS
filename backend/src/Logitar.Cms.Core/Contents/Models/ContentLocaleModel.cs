using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Localization.Models;

namespace Logitar.Cms.Core.Contents.Models;

public record ContentLocaleModel
{
  public ContentModel Content { get; set; } = new();
  public LanguageModel? Language { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public ActorModel CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public ActorModel UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public override string ToString() => $"{DisplayName ?? UniqueName}";
}
