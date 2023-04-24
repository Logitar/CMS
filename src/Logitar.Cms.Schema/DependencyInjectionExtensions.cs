using GraphQL;
using GraphQL.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Schema;

public static class DependencyInjectionExtensions
{
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
