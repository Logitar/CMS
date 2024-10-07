using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Sessions;

public class InvalidRefreshTokenException : InvalidCredentialsException
{
  public new const string ErrorMessage = "The specified refresh token is not valid.";

  public string RefreshToken
  {
    get => (string)Data[nameof(RefreshToken)]!;
    private set => Data[nameof(RefreshToken)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public InvalidRefreshTokenException(string xApiKey, string? propertyName = null, Exception? innerException = null)
    : base(BuildMessage(xApiKey, propertyName), innerException)
  {
    RefreshToken = xApiKey;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string xApiKey, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RefreshToken), xApiKey)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
