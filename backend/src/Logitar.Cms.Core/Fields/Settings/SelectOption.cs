using FluentValidation;

namespace Logitar.Cms.Core.Fields.Settings;

public record SelectOption : ISelectOption
{
  public bool IsDisabled { get; }
  public bool IsSelected { get; }
  public string Text { get; }
  public string? Label { get; }
  public string? Value { get; }

  [JsonConstructor]
  public SelectOption(string text, bool isDisabled = false, bool isSelected = false, string? label = null, string? value = null)
  {
    IsDisabled = isDisabled;
    IsSelected = isSelected;
    Text = text.Trim();
    Label = label?.CleanTrim();
    Value = value?.CleanTrim();
    new Validator().ValidateAndThrow(this);
  }

  public SelectOption(ISelectOption option)
  {
    IsDisabled = option.IsDisabled;
    IsSelected = option.IsSelected;
    Text = option.Text.Trim();
    Label = option.Label?.CleanTrim();
    Value = option.Value?.CleanTrim();
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<SelectOption>
  {
    public Validator()
    {
      RuleFor(x => x.Text).NotEmpty();
      When(x => x.Label != null, () => RuleFor(x => x.Label).NotEmpty());
      When(x => x.Value != null, () => RuleFor(x => x.Value).NotEmpty());
    }
  }
}
