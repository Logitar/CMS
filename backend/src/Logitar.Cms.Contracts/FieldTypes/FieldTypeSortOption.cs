using Logitar.Cms.Contracts.Search;

namespace Logitar.Cms.Contracts.FieldTypes;

public record FieldTypeSortOption : SortOption
{
  public new FieldTypeSort Field
  {
    get => Enum.Parse<FieldTypeSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public FieldTypeSortOption() : this(FieldTypeSort.DisplayName)
  {
  }

  public FieldTypeSortOption(FieldTypeSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
