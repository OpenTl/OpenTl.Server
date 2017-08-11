namespace OpenTl.Server.Back.Sessions
{
    using System.Collections.Generic;

    using OpenTl.Common.Interfaces;

    public static class SessionStore
    {
        private static readonly Dictionary<ulong, ISession> Sessions  = new Dictionary<ulong, ISession>(); 

        public static bool TryGetSession(ulong authKeyId, out ISession sessionStore)
        {
            return Sessions.TryGetValue(authKeyId, out sessionStore);
        }
        
        public static void SetSession(ISession session)
        {
            Sessions.Add(session.AuthKey.Id, session);
        }
    }
}