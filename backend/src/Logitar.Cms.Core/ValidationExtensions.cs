using FluentValidation;
using Logitar.Cms.Core.Fields.Validators;

namespace Logitar.Cms.Core;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> ContentType<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(byte.MaxValue).SetValidator(new ContentTypeValidator<T>());
  }
}
