namespace OpenTl.Server.UnitTests.Builders
{
    using System.Linq;

    using OpenTl.Common.Auth;
    using OpenTl.Server.Back.Contracts.Entities;

    using Ploeh.AutoFixture;

    internal static class SessionBuilder
    {
        public static ServerSession BuildSession(this BaseTest baseTest, User currentUser )
        {
            var authKey = new AuthKey(baseTest.Fixture.CreateMany<byte>(256).ToArray());
            
            var session = 
                    baseTest.Fixture.Build<ServerSession>()
                    .With(serverSession => serverSession.AuthKey, authKey)
                    .With(serverSession => serverSession.UserId, currentUser.UserId)
                    .With(serverSession => serverSession.Id, authKey.ToGuid())
                    .Create();

            baseTest.BuildRepository(session);

            return session;
        }
    }
}