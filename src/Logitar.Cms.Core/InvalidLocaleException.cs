using System.Text;

namespace Logitar.Cms.Core;

public class InvalidLocaleException : Exception, IPropertyFailure
{
  public InvalidLocaleException(string locale, string paramName, Exception innerException)
    : base(GetMessage(locale, paramName), innerException)
  {
    Data[nameof(AttemptedValue)] = locale;
    Data[nameof(PropertyName)] = paramName;
  }

  public string PropertyName => (string)Data[nameof(PropertyName)]!;
  public string AttemptedValue => (string)Data[nameof(AttemptedValue)]!;

  private static string GetMessage(string locale, string paramName)
  {
    StringBuilder message = new();

    message.AppendLine("The specified locale is not valid.");
    message.Append("Locale: ").AppendLine(locale);
    message.Append("ParamName: ").AppendLine(paramName);

    return message.ToString();
  }
}
