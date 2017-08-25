namespace OpenTl.Server.Back.Sessions
{
    using OpenTl.Common.Auth;
    using OpenTl.Common.Interfaces;

    internal sealed class ServerSession: ISession
    {
        public AuthKey AuthKey { get; set; }

        public ulong SessionId { get; set; }

        public byte[] ServerSalt { get; set; }

        public ulong MessageId { get; set; }

        public int CurrentUserId { get; set; }
    }
}