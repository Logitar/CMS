using Logitar.Cms.Contracts.Configurations;

namespace Logitar.Cms.Core.Caching;

public interface ICacheService
{
  Configuration? Configuration { get; set; }
}
