using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTl.Server.Back
{
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
            Mapper.Initialize(cfg => cfg.AddProfiles(typeof(ServerStartup).Assembly));

            services.AddSingleton(typeof(IRepository<>), typeof(MemoryRepository<>));
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ISessionStore, SessionStore>();
            
            return services.BuildServiceProvider();
        }
    }
}