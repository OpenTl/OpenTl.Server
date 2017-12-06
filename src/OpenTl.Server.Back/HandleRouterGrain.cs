using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTl.Server.Back.Contracts;
using OpenTl.Server.Back.Contracts.Auth;
using Orleans;

namespace OpenTl.Server.Back
{
    using System.Linq;

    using OpenTl.Server.Back.Contracts.Entities;
    using OpenTl.Server.Back.Contracts.Requests;
    using OpenTl.Server.Back.Contracts.Requests.Contacts;
    using OpenTl.Server.Back.DAL.Interfaces;

    public class HandleRouterGrain : Grain, IPackageRouterGrain
    {
        private readonly IRepository<ServerSession> _sessionRepository;

        private readonly Dictionary<uint, IObjectHandler> _handlers = new Dictionary<uint, IObjectHandler>();

        public HandleRouterGrain(IRepository<ServerSession> sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }
        
        public override Task OnActivateAsync()
        {
            _handlers[0x60469778] = GrainFactory.GetGrain<IRequestReqPqHandler>(0);
            _handlers[0xd712e4be] = GrainFactory.GetGrain<IRequestReqDhParamsHandler>(0);
            _handlers[0xf5045f1f] = GrainFactory.GetGrain<IRequestSetClientDhParamsHandler>(0);
            _handlers[0xda9b0d0d] = GrainFactory.GetGrain<IRequestInvokeWithLayerHandler>(0);
            _handlers[0x86aef0ec] = GrainFactory.GetGrain<IRequestSendCodeHandler>(0);
            _handlers[0x1b067634] = GrainFactory.GetGrain<IRequestSignUpHandler>(0);
            _handlers[0xbcd51581] = GrainFactory.GetGrain<IRequestSignInHandler>(0);
            _handlers[0x22c6aa08] = GrainFactory.GetGrain<IRequestGetContactsHandler>(0);
            _handlers[0xda30b32d] = GrainFactory.GetGrain<IRequestImportContactsHandler>(0);

            return base.OnActivateAsync();
        }

        public async Task<Tuple<Guid?, byte[]>> Handle(Guid keyId, byte[] request)
        {
            var encryptionHandler = GrainFactory.GetGrain<IEncryptionHandler>(0);

            var guid = new byte[16];
            Array.Copy(request, 0, guid, 8, 8);
            var authKeyId = new Guid(guid);
            
            var hasSession = _sessionRepository.GetAll().Any(s => s.Id == authKeyId);

            if (hasSession)
            {
                keyId = authKeyId;
                request = await encryptionHandler.TryDecrypt(request, authKeyId);
            }

            var objectId = BitConverter.ToUInt32(request, 0);

            var responce = await _handlers[objectId].Handle(keyId, request);

            if (hasSession)
            {
                responce = await encryptionHandler.TryEncrypt(responce, keyId);
            }

            return hasSession
                       ? Tuple.Create((Guid?)authKeyId, responce)
                       : Tuple.Create((Guid?)null, responce); 
        }
    }
}