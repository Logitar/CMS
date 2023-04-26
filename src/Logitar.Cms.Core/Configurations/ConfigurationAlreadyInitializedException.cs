namespace Logitar.Cms.Core.Configurations;

public class ConfigurationAlreadyInitializedException : Exception
{
  public ConfigurationAlreadyInitializedException() : base("The configuration has already been initialized.")
  {
  }
}
