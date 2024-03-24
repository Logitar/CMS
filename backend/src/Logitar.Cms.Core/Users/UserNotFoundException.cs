using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Users;

public class UserNotFoundException : InvalidCredentialsException
{
  public new const string ErrorMessage = "The specified user could not be found.";

  public string Username
  {
    get => (string)Data[nameof(Username)]!;
    private set => Data[nameof(Username)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public UserNotFoundException(string username, string? propertyName = null) : base(BuildMessage(username, propertyName))
  {
    Username = username;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string username, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Username), username)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
