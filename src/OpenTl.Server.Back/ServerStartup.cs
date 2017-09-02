using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTl.Server.Back
{
    using System.ComponentModel;
    using System.Linq;

    using AutoMapper;

    using OpenTl.Server.Back.BLL;
    using OpenTl.Server.Back.BLL.Interfaces;
    using OpenTl.Server.Back.DAL;
    using OpenTl.Server.Back.DAL.Interfaces;
    using OpenTl.Server.Back.Sessions;
    using OpenTl.Server.Back.Sessions.Interfaces;

    public class ServerStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IRepository<>), typeof(MemoryRepository<>));
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ISessionStore, SessionStore>();


            Mapper.Initialize(
                cfg =>
                {

                    var profileTypes = typeof(ServerStartup).Assembly.GetTypes().Where(typeof(Profile).IsAssignableFrom);
                    foreach (var profileType in profileTypes)
                    {
                        services.AddSingleton(typeof(Profile), profileType);
                    }

                    var provider = services.BuildServiceProvider();

                    cfg.ConstructServicesUsing(provider.GetService);

                    foreach (var profile in provider.GetServices<Profile>())
                    {
                        cfg.AddProfile(profile);
                    }
                });

            return services.BuildServiceProvider();
        }
    }
}