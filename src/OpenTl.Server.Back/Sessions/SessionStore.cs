namespace OpenTl.Server.Back.Sessions
{
    using System.Collections.Generic;

    using OpenTl.Common.Interfaces;
    using OpenTl.Server.Back.Sessions.Interfaces;

    public class SessionStore : ISessionStore
    {
        private readonly Dictionary<ulong, ISession> _sessions  = new Dictionary<ulong, ISession>(); 

        public bool TryGetSession(ulong authKeyId, out ISession sessionStore)
        {
            return _sessions.TryGetValue(authKeyId, out sessionStore);
        }
        
        public void SetSession(ISession session)
        {
            _sessions.Add(session.AuthKey.Id, session);
        }
    }
}