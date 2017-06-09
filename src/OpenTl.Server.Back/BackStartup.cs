using System;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;

namespace OpenTl.Server.Back
{
    public class BackStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }
    }
}