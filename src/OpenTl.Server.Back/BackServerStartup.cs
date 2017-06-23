using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTl.Server.Back
{
    public class BackServerStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }
    }
}