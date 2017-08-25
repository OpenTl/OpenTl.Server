namespace OpenTl.Server.Back.Sessions.Interfaces
{
    using OpenTl.Common.Interfaces;

    public interface ISessionStore
    {
        bool TryGetSession(ulong authKeyId, out ISession sessionStore);

        void SetSession(ISession session);
    }
}