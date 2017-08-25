using System;
using System.Threading.Tasks;

using OpenTl.Schema;
using OpenTl.Server.Back.Cache;
using OpenTl.Server.Back.Contracts.Auth;
using OpenTl.Server.Back.Helpers;

namespace OpenTl.Server.Back.Auth
{
    using OpenTl.Common.Auth.Server;

    public class RequestReqDhParamsHandlerGrain : BaseObjectHandlerGrain<RequestReqDHParams, IServerDHParams>,
        IRequestReqDhParamsHandler
    {
        protected override Task<IServerDHParams> HandleProtected(ulong clientId, RequestReqDHParams obj)
        {
            var cache = AuthCache.GetCache(clientId);

            cache.Nonce = obj.Nonce;
            
            var response = Step2ServerHelper.GetResponse(obj, RsaKeyHelper.PrivateKey, out var keyPair, out var newNonce);

            cache.NewNonse = newNonce;
            cache.KeyPair = keyPair;
            
            return Task.FromResult(response);
        }
    }
}