namespace OpenTl.Server.IntegrationTests.Entities
{
    using OpenTl.Common.Auth;
    using OpenTl.Common.Interfaces;
    
    public class TestSession: ISession 
    {
        public AuthKey AuthKey { get; set; }

        public ulong SessionId { get; set; }

        public byte[] ServerSalt { get; set; }

        public ulong MessageId { get; set; }

        public int UserId { get; set; }

        public int SeqNumber { get; set; }
    }
}