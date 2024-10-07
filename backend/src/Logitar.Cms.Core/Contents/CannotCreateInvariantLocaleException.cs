using Logitar.Cms.Contracts.Errors;

namespace Logitar.Cms.Core.Contents;

public class CannotCreateInvariantLocaleException : BadRequestException
{
  private const string ErrorMessage = "A locale cannot be created for an invariant content.";

  public string ContentId
  {
    get => (string)Data[nameof(ContentId)]!;
    private set => Data[nameof(ContentId)] = value;
  }
  public string ContentTypeId
  {
    get => (string)Data[nameof(ContentTypeId)]!;
    private set => Data[nameof(ContentTypeId)] = value;
  }

  public override Error Error => new(this.GetErrorCode(), ErrorMessage);

  public CannotCreateInvariantLocaleException(ContentAggregate content)
    : base(BuildMessage(content))
  {
    ContentId = content.Id.Value;
    ContentTypeId = content.ContentTypeId.Value;
  }

  private static string BuildMessage(ContentAggregate content) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ContentId), content.Id)
    .AddData(nameof(ContentTypeId), content.ContentTypeId)
    .Build();
}
