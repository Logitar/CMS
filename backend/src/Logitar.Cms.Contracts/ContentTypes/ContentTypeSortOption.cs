using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Contracts.ContentTypes;

public record ContentTypeSortOption : SortOption
{
  public new ContentTypeSort Field
  {
    get => Enum.Parse<ContentTypeSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ContentTypeSortOption() : this(ContentTypeSort.UpdatedOn, isDescending: true)
  {
  }

  public ContentTypeSortOption(ContentTypeSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
