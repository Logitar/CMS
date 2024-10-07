namespace Logitar.Cms.Contracts.Errors;

public record Error
{
  public string Code { get; set; }
  public string Message { get; set; }
  public List<ErrorData> Data { get; set; }

  public Error() : this(string.Empty, string.Empty)
  {
  }

  public Error(string code, string message) : this(code, message, data: [])
  {
  }

  public Error(string code, string message, IEnumerable<ErrorData> data)
  {
    Code = code;
    Message = message;
    Data = data.ToList();
  }

  public void AddData(KeyValuePair<string, string> data) => Data.Add(new ErrorData(data));
  public void AddData(string key, string value) => Data.Add(new ErrorData(key, value));
}
