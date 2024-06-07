using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Core.Logging;

public interface ILoggingService
{
  void SetApiKey(ApiKey? apiKey);
  void SetSession(Session? session);
  void SetUser(User? user);
}
