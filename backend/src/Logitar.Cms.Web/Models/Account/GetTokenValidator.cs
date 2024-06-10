using FluentValidation;

namespace Logitar.Cms.Web.Models.Account;

public class GetTokenValidator : AbstractValidator<GetTokenPayload>
{
  public GetTokenValidator()
  {
    RuleFor(x => x).Must(BeAValidPayload).WithErrorCode(nameof(GetTokenValidator))
      .WithMessage(x => $"Exactly one of the following properties must be specified: {nameof(x.RefreshToken)}, {x.Credentials}.");
  }

  private static bool BeAValidPayload(GetTokenPayload payload)
  {
    int count = 0;
    if (payload.RefreshToken != null)
    {
      count++;
    }
    if (payload.Credentials != null)
    {
      count++;
    }
    return count == 1;
  }
}
