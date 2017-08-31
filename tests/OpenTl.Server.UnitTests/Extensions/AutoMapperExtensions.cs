namespace OpenTl.Server.UnitTests.Extensions
{
    using System.Linq;

    using AutoMapper;

    using Castle.MicroKernel.Registration;

    using OpenTl.Server.Back;

    public static class AutoMapperExtensions
    {
        public static void RegisterAllMaps(this BaseTest test)
        {
            Mapper.Initialize(
                cfg =>
                {
                    cfg.ConstructServicesUsing(test.Container.Resolve);

                    var profileTypes = typeof(ServerStartup).Assembly.GetTypes().Where(typeof(Profile).IsAssignableFrom);
                    foreach (var profileType in profileTypes)
                    {
                        test.Container.Register(Component.For<Profile>().ImplementedBy(profileType).LifestyleSingleton());
                    }

                    foreach (var profile in test.Container.ResolveAll<Profile>())
                    {
                        cfg.AddProfile(profile);
                    }
                });
        }
    }
}