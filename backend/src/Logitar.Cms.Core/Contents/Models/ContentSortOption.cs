using Logitar.Cms.Core.Search;

namespace Logitar.Cms.Core.Contents.Models;

public record ContentSortOption : SortOption
{
  public new ContentSort Field
  {
    get => Enum.Parse<ContentSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ContentSortOption() : this(ContentSort.DisplayName, isDescending: true)
  {
  }

  public ContentSortOption(ContentSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
