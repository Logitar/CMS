using System.Text;

namespace Logitar.Cms.Core;

public class InvalidUrlException : Exception, IPropertyFailure
{
  public InvalidUrlException(string url, string paramName, Exception innerException)
    : base(GetMessage(url, paramName), innerException)
  {
    Data[nameof(AttemptedValue)] = url;
    Data[nameof(PropertyName)] = paramName;
  }

  public string PropertyName => (string)Data[nameof(PropertyName)]!;
  public string AttemptedValue => (string)Data[nameof(AttemptedValue)]!;

  private static string GetMessage(string url, string paramName)
  {
    StringBuilder message = new();

    message.AppendLine("The specified URL is not valid.");
    message.Append("URL: ").AppendLine(url);
    message.Append("ParamName: ").AppendLine(paramName);

    return message.ToString();
  }
}
