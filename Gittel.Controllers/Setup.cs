using Microsoft.Extensions.DependencyInjection;

namespace Gittel.Controllers;

public static class Setup
{
  public static void RegisterControllers(this IServiceCollection services)
  {
    services.AddScoped<RepositoryController>();
    services.AddScoped<SampleController>();
  }
}
