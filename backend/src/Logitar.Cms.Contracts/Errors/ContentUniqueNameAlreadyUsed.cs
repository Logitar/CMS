namespace Logitar.Cms.Contracts.Errors;

public record ContentUniqueNameAlreadyUsed : ValidationFailure
{
  public Guid? LanguageId { get; set; }

  public ContentUniqueNameAlreadyUsed() : base(string.Empty, string.Empty)
  {
  }

  public ContentUniqueNameAlreadyUsed(string code, string message) : base(code, message)
  {
  }
}
