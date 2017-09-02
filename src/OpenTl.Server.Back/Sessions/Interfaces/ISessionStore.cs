namespace OpenTl.Server.Back.Sessions.Interfaces
{
    using OpenTl.Common.Interfaces;

    public interface ISessionStore
    {
        void SetSession(ISession session);

        ISession GetSession(ulong authKeyId);
        
        bool ContainsSession(ulong authKeyId);
    }
}