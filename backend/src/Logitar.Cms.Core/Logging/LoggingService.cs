using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Core.Logging;

public class LoggingService : ILoggingService // TODO(fpion): Logging
{
  public virtual void SetApiKey(ApiKey? apiKey)
  {
  }

  public virtual void SetSession(Session? session)
  {
  }

  public virtual void SetUser(User? user)
  {
  }
}
