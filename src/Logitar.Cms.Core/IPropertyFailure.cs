namespace Logitar.Cms.Core;

public interface IPropertyFailure
{
  string PropertyName { get; }
  string AttemptedValue { get; }
}
