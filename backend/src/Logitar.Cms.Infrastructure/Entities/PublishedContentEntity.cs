using Logitar.Cms.Core.Contents.Events;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Cms.Infrastructure.Entities;

public class PublishedContentEntity : AggregateEntity // TODO(fpion): is it an aggregate, really?
{
  public ContentLocaleEntity? ContentLocale { get; private set; }
  public int ContentLocaleId { get; private set; }

  public int ContentTypeId { get; private set; }
  public int ContentId { get; private set; }
  public Guid ContentUid { get; private set; }
  public int? LanguageId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string? FieldValues { get; private set; }

  public PublishedContentEntity(ContentLocaleEntity contentLocale, ContentLocalePublished @event) : base(@event)
  {
    ContentEntity content = contentLocale.Content ?? throw new ArgumentException("The content is required.", nameof(contentLocale));

    ContentLocale = contentLocale;
    ContentLocaleId = contentLocale.ContentLocaleId;

    ContentTypeId = content.ContentTypeId;
    ContentId = content.ContentId;
    ContentUid = content.Id;
    LanguageId = contentLocale.LanguageId;

    Update(@event);
  }

  private PublishedContentEntity()
  {
  }

  public void Update(ContentLocalePublished @event)
  {
    #region TODO(fpion): implement
    UniqueName = "";
    DisplayName = null;
    Description = null;
    FieldValues = null;
    #endregion
  }
}
