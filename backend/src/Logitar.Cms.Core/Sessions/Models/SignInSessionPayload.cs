namespace Logitar.Cms.Core.Sessions.Models;

public record SignInSessionPayload
{
  public Guid? Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public bool IsPersistent { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public SignInSessionPayload()
  {
  }

  public SignInSessionPayload(string uniqueName, string password, Guid? id = null, bool isPersistent = false, IEnumerable<CustomAttribute>? customAttributes = null)
  {
    Id = id;

    UniqueName = uniqueName;
    Password = password;
    IsPersistent = isPersistent;

    if (customAttributes != null)
    {
      CustomAttributes.AddRange(customAttributes);
    }
  }
}
