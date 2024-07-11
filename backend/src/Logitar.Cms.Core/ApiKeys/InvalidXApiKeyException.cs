using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.ApiKeys;

public class InvalidXApiKeyException : InvalidCredentialsException
{
  public new const string ErrorMessage = "The specified X-API-Key is not valid.";

  public string XApiKey
  {
    get => (string)Data[nameof(XApiKey)]!;
    private set => Data[nameof(XApiKey)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public InvalidXApiKeyException(string xApiKey, string? propertyName = null, Exception? innerException = null)
    : base(BuildMessage(xApiKey, propertyName), innerException)
  {
    XApiKey = xApiKey;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string xApiKey, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(XApiKey), xApiKey)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
