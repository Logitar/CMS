﻿namespace Logitar.Cms.Contracts.Errors;

public record Error
{
  public string Code { get; set; }
  public string Message { get; set; }
  public List<ErrorData> Data { get; set; }

  public Error() : this(string.Empty, string.Empty)
  {
  }

  public Error(string code, string message)
  {
    Code = code;
    Message = message;
    Data = [];
  }

  public Error(string code, string message, IEnumerable<ErrorData> data) : this(code, message)
  {
    Data.AddRange(data);
  }
}
