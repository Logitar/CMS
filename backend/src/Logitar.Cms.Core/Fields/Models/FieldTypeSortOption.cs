using Logitar.Cms.Core.Search;

namespace Logitar.Cms.Core.Fields.Models;

public record FieldTypeSortOption : SortOption
{
  public new FieldTypeSort Field
  {
    get => Enum.Parse<FieldTypeSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public FieldTypeSortOption() : this(FieldTypeSort.DisplayName, isDescending: true)
  {
  }

  public FieldTypeSortOption(FieldTypeSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
