using Logitar.Identity.Domain.Sessions;

namespace Logitar.Cms.Core.Sessions;

public static class SessionExtensions
{
  public const string AdditionalInformationKey = "AdditionalInformation";
  public const string IpAddressKey = "IpAddress";

  public static void SetAdditionalInformation(this SessionAggregate session, string? additionalInformation)
  {
    if (string.IsNullOrWhiteSpace(additionalInformation))
    {
      session.RemoveCustomAttribute(AdditionalInformationKey);
    }
    else
    {
      session.SetCustomAttribute(AdditionalInformationKey, additionalInformation);
    }
  }
  public static void SetIpAddress(this SessionAggregate session, string? ipAddress)
  {
    if (string.IsNullOrWhiteSpace(ipAddress))
    {
      session.RemoveCustomAttribute(IpAddressKey);
    }
    else
    {
      session.SetCustomAttribute(IpAddressKey, ipAddress);
    }
  }
}
