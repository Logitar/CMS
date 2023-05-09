using Logitar.Cms.Contracts.Actors;

namespace Logitar.Cms.Core.Mapping;

internal class DriverLicense
{
  public string Number { get; set; } = string.Empty;
  public Actor? RegisteredBy { get; set; }
}
