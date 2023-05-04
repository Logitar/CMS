namespace Logitar.Cms.Core.Mapping;

public interface IMappingService
{
  T Map<T>(object? value);
}
