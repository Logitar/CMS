using GraphQL;
using GraphQL.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Schema;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsSchema(this IServiceCollection services, IConfiguration configuration)
  {
    GraphQLSettings settings = configuration.GetSection("GraphQL").Get<GraphQLSettings>() ?? new();

    return services.AddLogitarCmsSchema(settings);
  }

  public static IServiceCollection AddLogitarCmsSchema(this IServiceCollection services, GraphQLSettings settings)
  {
    return services.AddGraphQL(builder => builder
      .AddSchema<CmsSchema>()
      .AddSystemTextJson()
      .AddErrorInfoProvider(new ErrorInfoProvider(options =>
      {
        options.ExposeExceptionDetails = settings.ExposeExceptionDetails;
      }))
      .AddGraphTypes(typeof(CmsSchema).Assembly)
      .ConfigureExecutionOptions(options =>
      {
        options.EnableMetrics = settings.EnableMetrics;
      }));
  }
}
