using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Logging;

public interface ILoggingService
{
  void Open(string? correlationId = null, string? method = null, string? destination = null, string? source = null, string? additionalInformation = null, DateTime? startedOn = null);
  void Report(DomainEvent @event);
  void Report(Exception exception); // TODO(fpion): exception reported in duplicate? See ActivityPipeline and LoggingMiddleware
  void SetActivity(IActivity activity); // TODO(fpion): ActivityPipeline
  void SetOperation(Operation operation);
  void SetApiKey(ApiKey? apiKey);
  void SetSession(Session? session);
  void SetUser(User? user);
  Task CloseAndSaveAsync(int statusCode, CancellationToken cancellationToken = default);
}
