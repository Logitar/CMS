namespace Logitar.Cms.Core.Contents.Models;

public class PublishedContent
{
  public Guid Id { get; set; }

  public ContentTypeModel ContentType { get; set; } = new(); // TODO(fpion): ContentTypeSummary

  public PublishedContentLocale Invariant { get; set; }
  public List<PublishedContentLocale> Locales { get; set; } = [];

  public PublishedContent()
  {
    Invariant = new(this);
  }

  // TODO(fpion): FindLocale
  // TODO(fpion): HasLocale
  // TODO(fpion): TryGetLocale

  public override bool Equals(object? obj) => obj is PublishedContent content && content.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Invariant} | {GetType()} (Id={Id})";
}
