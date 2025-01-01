using FluentValidation;

namespace Logitar.Cms.Core;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> ContentType<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(byte.MaxValue); // TODO(fpion): Validator
  }
}
