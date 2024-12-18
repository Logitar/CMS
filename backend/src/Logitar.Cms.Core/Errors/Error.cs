namespace Logitar.Cms.Core.Errors;

public record Error
{
  public string Code { get; init; } = string.Empty;
  public string Message { get; init; } = string.Empty;
  public IReadOnlyCollection<ErrorData> Data { get; init; } = [];

  public Error()
  {
  }

  public Error(string code, string message, IEnumerable<ErrorData>? data = null)
  {
    Code = code;
    Message = message;

    if (data != null)
    {
      Data = data.ToArray();
    }
  }
}
