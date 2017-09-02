namespace OpenTl.Server.Back
{
    using System;
    using System.Threading.Tasks;

    using BarsGroup.CodeGuard;

    using OpenTl.Common.GuardExtensions;
    using OpenTl.Common.Interfaces;
    using OpenTl.Common.MtProto;
    using OpenTl.Server.Back.Contracts;
    using OpenTl.Server.Back.Sessions.Interfaces;

    using Orleans;

    public class EncryptionHandlerGrain : Grain, IEncryptionHandler
    {
        private readonly ISessionStore _sessionStore;

        public EncryptionHandlerGrain(ISessionStore sessionStore)
        {
            _sessionStore = sessionStore;
        }
        public Task<byte[]> TryEncrypt(byte[] package, ulong authKeyId)
        {
            Guard.That(package.Length).IsGreaterThan(4);

            var session = _sessionStore.GetSession(authKeyId);
            
            var data = MtProtoHelper.FromServerEncrypt(package, session, 0);

            return Task.FromResult(data);
        }

        public Task<byte[]> TryDecrypt(byte[] package, ulong authKeyId)
        {
            Guard.That(package.Length).IsGreaterThanOrEqualTo(8);

            var session = _sessionStore.GetSession(authKeyId);

            var data = MtProtoHelper.FromClientDecrypt(package, session, out authKeyId, out var serverSalt, out var sessionId, out var messageId, out var seqNumber);

            Guard.That(authKeyId).IsEqual(session.AuthKey.Id);
            Guard.That(serverSalt).IsItemsEquals(session.ServerSalt);
            Guard.That(sessionId).IsEqual(session.SessionId);
            
            //Guard.That(messageId).IsEqual(session.SessionId);
            //Guard.That(seqNumber).IsEqual(session.SessionId);
            
            return Task.FromResult(data);
        }
    }
}