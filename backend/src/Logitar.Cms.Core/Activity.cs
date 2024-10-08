using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;

namespace Logitar.Cms.Core;

public abstract record Activity
{
  public ActorId GetActorId() => new(); // TODO(fpion): implement
  public IUserSettings GetUserSettings() => new UserSettings(); // TODO(fpion): implement
}
