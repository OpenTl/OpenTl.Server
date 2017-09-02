namespace OpenTl.Server.IntegrationTests.Entities
{
    using System.Net.Sockets;

    using OpenTl.Schema;

    public class ConnectionInfo
    {
        public TUser User { get; }

        public TestSession Session { get; }

        public NetworkStream NetworkStream { get; }

        public ConnectionInfo(TUser user, TestSession session, NetworkStream networkStream)
        {
            User = user;
            Session = session;
            NetworkStream = networkStream;
        }
    }
}