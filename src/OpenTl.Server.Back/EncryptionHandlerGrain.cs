namespace OpenTl.Server.Back
{
    using System;
    using System.Threading.Tasks;

    using BarsGroup.CodeGuard;

    using OpenTl.Common.GuardExtensions;
    using OpenTl.Common.MtProto;
    using OpenTl.Server.Back.Contracts;
    using OpenTl.Server.Back.Sessions;

    using Orleans;

    public class EncryptionHandlerGrain : Grain, IEncryptionHandler
    {
        public Task<byte[]> TryEncrypt(byte[] package, ulong authKeyId)
        {
            Guard.That(package.Length).IsGreaterThan(4);

            if (!SessionStore.TryGetSession(authKeyId, out var session))
            {
                return Task.FromResult(package);
            }
            
            var data = MtProtoHelper.FromServerEncrypt(package, session, 0);

            return Task.FromResult(data);
        }

        public Task<Tuple<byte[], ulong>> TryDecrypt(byte[] package)
        {
            Guard.That(package.Length).IsGreaterThan(8);

            var authKeyId = BitConverter.ToUInt64(package, 0);

            if (!SessionStore.TryGetSession(authKeyId, out var session))
            {
                return Task.FromResult(Tuple.Create(package, (ulong)0));
            }

            var data = MtProtoHelper.FromClientDecrypt(package, session, out authKeyId, out var serverSalt, out var sessionId, out var messageId, out var seqNumber);

            Guard.That(authKeyId).IsEqual(session.AuthKey.Id);
            Guard.That(serverSalt).IsItemsEquals(session.ServerSalt);
            Guard.That(sessionId).IsEqual(session.SessionId);
            
            //Guard.That(messageId).IsEqual(session.SessionId);
            //Guard.That(seqNumber).IsEqual(session.SessionId);
            
            return Task.FromResult(Tuple.Create(data, authKeyId));
        }
    }
}