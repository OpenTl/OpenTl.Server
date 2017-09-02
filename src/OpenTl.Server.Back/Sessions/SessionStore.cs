namespace OpenTl.Server.Back.Sessions
{
    using System.Collections.Generic;

    using OpenTl.Common.Interfaces;
    using OpenTl.Server.Back.Sessions.Interfaces;

    public class SessionStore : ISessionStore
    {
        private readonly Dictionary<ulong, ISession> _sessions  = new Dictionary<ulong, ISession>(); 

        public ISession GetSession(ulong authKeyId)
        {
            return _sessions[authKeyId];
        }

        public bool ContainsSession(ulong authKeyId)
        {
            return _sessions.ContainsKey(authKeyId);
        }

        public void SetSession(ISession session)
        {
            _sessions.Add(session.AuthKey.Id, session);
        }
    }
}