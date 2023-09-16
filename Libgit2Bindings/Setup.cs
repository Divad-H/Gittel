using Microsoft.Extensions.DependencyInjection;

namespace Libgit2Bindings;

public static class Setup
{
  public static void RegisterLibgit2Bindings(this IServiceCollection services)
  {
    services.AddSingleton<ILibgit2, Libgit2>();
  }
}
