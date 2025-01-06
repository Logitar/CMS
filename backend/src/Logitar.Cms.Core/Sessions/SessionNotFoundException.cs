using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;

namespace Logitar.Cms.Core.Sessions;

public class SessionNotFoundException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified session could not be found.";

  public Guid SessionId
  {
    get => (Guid)Data[nameof(SessionId)]!;
    private set => Data[nameof(SessionId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public SessionNotFoundException(SessionId sessionId, string propertyName) : base(BuildMessage(sessionId, propertyName))
  {
    SessionId = sessionId.EntityId.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(SessionId sessionId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(SessionId), sessionId.EntityId.ToGuid())
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
