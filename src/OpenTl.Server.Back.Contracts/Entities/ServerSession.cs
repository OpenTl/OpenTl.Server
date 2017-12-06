namespace OpenTl.Server.Back.Contracts.Entities
{
    using System;

    using OpenTl.Common.Auth;
    using OpenTl.Common.Interfaces;

    public class ServerSession: ISession, IEntity
    {
        public AuthKey AuthKey { get; set; }

        public ulong SessionId { get; set; }

        public byte[] ServerSalt { get; set; }

        public ulong MessageId { get; set; }

        public int UserId { get; set; }

        public Guid Id { get; set; }
    }
}