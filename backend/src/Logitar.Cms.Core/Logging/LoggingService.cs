using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Core.Logging;

public class LoggingService : ILoggingService
{
  public virtual void SetApiKey(ApiKey? apiKey)
  {
    // TODO(fpion): implement
  }

  public virtual void SetSession(Session? session)
  {
    // TODO(fpion): implement
  }

  public virtual void SetUser(User? user)
  {
    // TODO(fpion): implement
  }
}
