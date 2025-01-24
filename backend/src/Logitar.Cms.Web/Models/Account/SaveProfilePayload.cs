using Logitar.Cms.Core;
using Logitar.Cms.Core.Users.Models;

namespace Logitar.Cms.Web.Models.Account;

public record SaveProfilePayload
{
  public ChangePasswordInput? Password { get; set; }

  public ChangeModel<string>? FirstName { get; set; }
  public ChangeModel<string>? MiddleName { get; set; }
  public ChangeModel<string>? LastName { get; set; }

  public ChangeModel<string>? EmailAddress { get; set; }
  public ChangeModel<string>? PictureUrl { get; set; }

  public UpdateUserPayload ToUpdateUserPayload()
  {
    UpdateUserPayload payload = new()
    {
      Password = Password?.ToChangePasswordPayload(),
      FirstName = FirstName,
      MiddleName = MiddleName,
      LastName = LastName,
      Picture = PictureUrl
    };

    if (EmailAddress != null)
    {
      payload.Email = EmailAddress.Value == null
        ? new ChangeModel<EmailPayload>()
        : new ChangeModel<EmailPayload>(new EmailPayload(EmailAddress.Value, isVerified: false));
    }

    return payload;
  }
}
