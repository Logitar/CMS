using FluentValidation;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Fields.Validators;

internal class CreateOrReplaceFieldTypeValidator : AbstractValidator<CreateOrReplaceFieldTypePayload>
{
  public CreateOrReplaceFieldTypeValidator()
  {
    RuleFor(x => x.UniqueName).UniqueName(FieldType.UniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x).Must(x => GetDataTypes(x).Count == 1)
      .WithErrorCode("CreateOrReplaceFieldTypeValidator")
      .WithMessage(x => $"Exactly one of the following must be specified: {string.Join(", ", nameof(x.Boolean), nameof(x.DateTime), nameof(x.Number), nameof(x.RichText), nameof(x.String))}.");
    When(x => x.Boolean != null, () => RuleFor(x => x.Boolean!).SetValidator(new BooleanSettingsValidator()));
    When(x => x.DateTime != null, () => RuleFor(x => x.DateTime!).SetValidator(new DateTimeSettingsValidator()));
    When(x => x.Number != null, () => RuleFor(x => x.Number!).SetValidator(new NumberSettingsValidator()));
    When(x => x.RichText != null, () => RuleFor(x => x.RichText!).SetValidator(new RichTextSettingsValidator()));
    When(x => x.Select != null, () => RuleFor(x => x.Select!).SetValidator(new SelectSettingsValidator()));
    When(x => x.String != null, () => RuleFor(x => x.String!).SetValidator(new StringSettingsValidator()));
    When(x => x.Tags != null, () => RuleFor(x => x.Tags!).SetValidator(new TagsSettingsValidator()));
  }

  private static IReadOnlyCollection<DataType> GetDataTypes(CreateOrReplaceFieldTypePayload payload)
  {
    List<DataType> dataTypes = new(capacity: 7);

    if (payload.Boolean != null)
    {
      dataTypes.Add(DataType.Boolean);
    }
    if (payload.DateTime != null)
    {
      dataTypes.Add(DataType.DateTime);
    }
    if (payload.Number != null)
    {
      dataTypes.Add(DataType.Number);
    }
    if (payload.RichText != null)
    {
      dataTypes.Add(DataType.RichText);
    }
    if (payload.Select != null)
    {
      dataTypes.Add(DataType.Select);
    }
    if (payload.String != null)
    {
      dataTypes.Add(DataType.String);
    }
    if (payload.Tags != null)
    {
      dataTypes.Add(DataType.Tags);
    }

    return dataTypes.AsReadOnly();
  }
}
