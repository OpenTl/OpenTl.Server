namespace OpenTl.Server.Back
{
    using System;
    using System.Threading.Tasks;

    using BarsGroup.CodeGuard;

    using OpenTl.Common.GuardExtensions;
    using OpenTl.Common.MtProto;
    using OpenTl.Server.Back.Contracts;
    using OpenTl.Server.Back.Contracts.Entities;
    using OpenTl.Server.Back.DAL.Interfaces;

    using Orleans;

    public class EncryptionHandlerGrain : Grain, IEncryptionHandler
    {
        private readonly IRepository<ServerSession> _sessionRepository;

        public EncryptionHandlerGrain(IRepository<ServerSession> sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }
        public Task<byte[]> TryEncrypt(byte[] package, Guid authKeyId)
        {
            Guard.That(package.Length).IsGreaterThan(4);

            var session = _sessionRepository.Get(authKeyId);
            
            var data = MtProtoHelper.FromServerEncrypt(package, session, 0);

            return Task.FromResult(data);
        }

        public Task<byte[]> TryDecrypt(byte[] package, Guid authKeyId)
        {
            Guard.That(package.Length).IsGreaterThanOrEqualTo(8);

            var session = _sessionRepository.Get(authKeyId);

            var data = MtProtoHelper.FromClientDecrypt(package, session, out _, out var serverSalt, out var sessionId, out var messageId, out var seqNumber);

            Guard.That(authKeyId).IsEqual(session.AuthKey.ToGuid());
            Guard.That(serverSalt).IsItemsEquals(session.ServerSalt);
            Guard.That(sessionId).IsEqual(session.SessionId);
            
            //Guard.That(messageId).IsEqual(session.SessionId);
            //Guard.That(seqNumber).IsEqual(session.SessionId);
            
            return Task.FromResult(data);
        }
    }
}