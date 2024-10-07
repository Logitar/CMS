using Logitar.Cms.Core.Logging;
using Logitar.Cms.Infrastructure;
using Logitar.Cms.MongoDB.Repositories;
using Logitar.Cms.MongoDB.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Logitar.Cms.MongoDB;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWithMongoDB(this IServiceCollection services, IConfiguration configuration)
  {
    MongoDBSettings settings = configuration.GetSection(MongoDBSettings.SectionKey).Get<MongoDBSettings>() ?? new();
    return services.AddLogitarCmsWithMongoDB(settings);
  }
  public static IServiceCollection AddLogitarCmsWithMongoDB(this IServiceCollection services, MongoDBSettings settings)
  {
    if (!string.IsNullOrWhiteSpace(settings.ConnectionString) && !string.IsNullOrWhiteSpace(settings.DatabaseName))
    {
      MongoClient client = new(settings.ConnectionString.Trim());
      IMongoDatabase database = client.GetDatabase(settings.DatabaseName.Trim());
      services.AddSingleton(database).AddTransient<ILogRepository, LogRepository>();
    }

    return services.AddLogitarCmsInfrastructure();
  }
}
