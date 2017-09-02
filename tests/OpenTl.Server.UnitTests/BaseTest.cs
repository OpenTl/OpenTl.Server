namespace OpenTl.Server.UnitTests
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Moq;

    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.AutoMoq;

    public abstract class BaseTest
    {
        protected Fixture Fixture { get; }

        internal IWindsorContainer Container { get; } = new WindsorContainer();

        protected BaseTest()
        {
            Fixture = new Fixture();
            Fixture.Customize(new AutoMoqCustomization());
        }


        protected TService Resolve<TService>() => Container.Resolve<TService>();

        protected Mock<TService> ResolveMock<TService>()
            where TService : class
        {
            return Container.Resolve<Mock<TService>>();
        }

        protected void RegisterSingleton<TService>()
            where TService : class
        {
            RegisterSingleton<TService, TService>();
        }

        protected void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : TService
        {
            Container.Register(Component.For<TService>().ImplementedBy<TImplementation>().LifestyleSingleton());
        }
        protected void RegisterMockAndInstance<TService>(Mock<TService> mock)
            where TService : class
        {
            Container.Register(Component.For<Mock<TService>>().Instance(mock));
            Container.Register(Component.For<TService>().Instance(mock.Object));
        }

    }
}