using Logitar.EventSourcing;
using System.Globalization;

namespace Logitar.Cms.Core.Users.Events;

public abstract record UserSaved : DomainEvent
{
  public string? FirstName { get; init; }
  public string? LastName { get; init; }
  public string? FullName { get; init; }

  public CultureInfo? Locale { get; init; }

  public Uri? Picture { get; init; }
}
