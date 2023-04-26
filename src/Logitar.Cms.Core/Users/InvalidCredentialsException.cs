namespace Logitar.Cms.Core.Users;

public class InvalidCredentialsException : Exception
{
  public InvalidCredentialsException(string message) : base(message) { }
}
