namespace Logitar.Cms.Contracts.Configurations;

public class InitializeConfigurationInput
{
  public string DefaultLocale { get; set; } = string.Empty;
  public InitialUserInput User { get; set; } = new();
}
