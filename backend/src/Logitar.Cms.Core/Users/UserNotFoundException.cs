using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Users;

internal class UserNotFoundException : InvalidCredentialsException
{
  private new const string ErrorMessage = "The specified user could not be found.";

  public string Username
  {
    get => (string)Data[nameof(Username)]!;
    private set => Data[nameof(Username)] = value;
  }

  public UserNotFoundException(string username) : base(BuildMessage(username))
  {
    Username = username;
  }

  private static string BuildMessage(string username) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Username), username)
    .Build();
}
