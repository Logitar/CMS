﻿using Logitar.Cms.Core.Caching.Commands;
using Logitar.Cms.Infrastructure.Commands;
using MediatR;

namespace Logitar.Cms;

public class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseStaticWebAssets();

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    using IServiceScope scope = application.Services.CreateScope();

    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    await mediator.Publish(new InitializeDatabase());
    await mediator.Publish(new InitializeCaching());

    application.Run();
  }
}
