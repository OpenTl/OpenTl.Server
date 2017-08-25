namespace OpenTl.Server.UnitTests
{
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.AutoMoq;

    public abstract class BaseTest
    {
        protected Fixture Fixture { get; }

        protected BaseTest()
        {
            Fixture = new Fixture();
            Fixture.Customize(new AutoMoqCustomization());
        }
    }
}