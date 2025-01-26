using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Localization.Models;

namespace Logitar.Cms.Core.Contents.Models;

public record PublishedContentLocale
{
  public PublishedContent Content { get; set; }
  public LanguageModel? Language { get; set; } // TODO(fpion): LanguageSummary

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<FieldValue> FieldValues { get; set; } = [];

  public ActorModel PublishedBy { get; set; } = new();
  public DateTime PublishedOn { get; set; }

  public PublishedContentLocale(PublishedContent content)
  {
    Content = content;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName}";
}
