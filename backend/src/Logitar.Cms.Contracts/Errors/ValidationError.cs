namespace Logitar.Cms.Contracts.Errors;

public record ValidationError : Error
{
  public List<PropertyError> Errors { get; set; }

  public ValidationError() : this("Validation", "Validation failed.")
  {
  }

  public ValidationError(string code, string message) : this(code, message, [])
  {

  }
  public ValidationError(string code, string message, IEnumerable<PropertyError> errors) : base(code, message)
  {
    Errors = errors.ToList();
  }

  public void Add(PropertyError error) => Errors.Add(error);
}
