namespace Logitar.Cms.Contracts.Errors;

public record ValidationError : Error
{
  public List<PropertyError> Errors { get; set; }

  public ValidationError() : this("Validation", "Validation failed.")
  {
  }

  public ValidationError(string code, string message, IEnumerable<ErrorData>? data = null, IEnumerable<PropertyError>? errors = null)
    : base(code, message, data)
  {
    Errors = errors?.ToList() ?? [];
  }

  public void Add(PropertyError error) => Errors.Add(error);
}
