namespace OpenTl.Server.UnitTests
{
    using AutoMapper;

    using OpenTl.Server.Back;

    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.AutoMoq;

    public abstract class BaseTest
    {
        protected Fixture Fixture { get; }

        protected BaseTest()
        {
            Fixture = new Fixture();
            Fixture.Customize(new AutoMoqCustomization());

            Mapper.Initialize(
                cfg =>
                {
                    cfg.ConstructServicesUsing();
                    cfg.AddProfiles(typeof(ServerStartup).Assembly)
                });

        }
    }
}