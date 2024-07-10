using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Users;

public class UserNotFoundException : InvalidCredentialsException
{
  public new const string ErrorMessage = "The specified user could not be found.";

  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public UserNotFoundException(string uniqueName, string? propertyName = null) : base(BuildMessage(uniqueName, propertyName))
  {
    UniqueName = uniqueName;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string uniqueName, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(UniqueName), uniqueName)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
