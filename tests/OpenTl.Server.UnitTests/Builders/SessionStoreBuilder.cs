namespace OpenTl.Server.UnitTests.Builders
{
    using Moq;

    using OpenTl.Common.Auth;
    using OpenTl.Common.Interfaces;
    using OpenTl.Server.Back.Entities;
    using OpenTl.Server.Back.Sessions.Interfaces;

    using Ploeh.AutoFixture;

    public static class SessionStoreBuilder
    {
        public static void BuildSessionStore(this BaseTest baseTest, User currentUser )
        {
            var mSession = baseTest.Fixture.Freeze<Mock<ISession>>();
            mSession.Setup(s => s.CurrentUserId).Returns(currentUser.UserId);
            mSession.Setup(s => s.AuthKey).Returns(baseTest.Fixture.Create<AuthKey>());
            var session = mSession.Object;

            baseTest.RegisterMockAndInstance(mSession);

            var mSessionStore = new Mock<ISessionStore>();
            mSessionStore.Setup(service => service.GetSession(session.AuthKey.Id)).Returns<ulong>(arg => session);

            baseTest.RegisterMockAndInstance(mSessionStore);
        }
    }
}