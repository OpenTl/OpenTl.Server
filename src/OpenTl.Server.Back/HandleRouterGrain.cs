using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTl.Server.Back.Contracts;
using OpenTl.Server.Back.Contracts.Auth;
using Orleans;

namespace OpenTl.Server.Back
{
    using OpenTl.Server.Back.Contracts.Requests;

    public class HandleRouterGrain : Grain, IPackageRouterGrain
    {
        private readonly Dictionary<uint, IObjectHandler> _handlers = new Dictionary<uint, IObjectHandler>();

        public override Task OnActivateAsync()
        {
            _handlers[0x60469778] = GrainFactory.GetGrain<IRequestReqPqHandler>(0);
            _handlers[0xd712e4be] = GrainFactory.GetGrain<IRequestReqDhParamsHandler>(0);
            _handlers[0xf5045f1f] = GrainFactory.GetGrain<IRequestSetClientDhParamsHandler>(0);
            _handlers[0xda9b0d0d] = GrainFactory.GetGrain<IRequestInvokeWithLayerHandler>(0);
            _handlers[0x86aef0ec] = GrainFactory.GetGrain<IRequestSendCodeHandler>(0);
            _handlers[0x1b067634] = GrainFactory.GetGrain<IRequestSignUpHandler>(0);
            _handlers[0xbcd51581] = GrainFactory.GetGrain<IRequestSignInHandler>(0);

            return base.OnActivateAsync();
        }

        public async Task<byte[]> Handle(Guid clientId, byte[] encryptedRequest)
        {
            var encryptionHandler = GrainFactory.GetGrain<IEncryptionHandler>(0);

            var requestData = await encryptionHandler.TryDecrypt(encryptedRequest);

            var request = requestData.Item1;
            var objectId = BitConverter.ToUInt32(request, 0);

            var responce = await _handlers[objectId].Handle(clientId, request);

            var encryptedResponse = await encryptionHandler.TryEncrypt(responce, requestData.Item2);

            return encryptedResponse;
        }
    }
}