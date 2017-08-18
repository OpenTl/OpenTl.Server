using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTl.Server.Back
{
    using OpenTl.Server.Back.DAL;
    using OpenTl.Server.Back.DAL.Interfaces;

    public class BackServerStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IRepository<>), typeof(MemoryRepository<>));
            
            return services.BuildServiceProvider();
        }
    }
}