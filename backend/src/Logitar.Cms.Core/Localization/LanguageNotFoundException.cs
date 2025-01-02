namespace Logitar.Cms.Core.Localization;

public class LanguageNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified language could not be found.";

  public Guid LanguageId
  {
    get => (Guid)Data[nameof(LanguageId)]!;
    private set => Data[nameof(LanguageId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data.Add(nameof(LanguageId), LanguageId);
      error.Data.Add(nameof(PropertyName), PropertyName);
      return error;
    }
  }

  public LanguageNotFoundException(Guid contentTypeId, string propertyName) : base(BuildMessage(contentTypeId, propertyName))
  {
    LanguageId = contentTypeId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Guid contentTypeId, string propertyName) => new ErrorMessageBuilder()
    .AddData(nameof(LanguageId), contentTypeId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
