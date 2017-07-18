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
        protected override Task<IServerDHParams> HandleProtected(Guid clientId, RequestReqDHParams obj)
        {
            var cache = AuthCache.GetCache(clientId);

            cache.Nonce = obj.Nonce;
                
            Step2ServerHelper.GetResponse(obj, RsaHelper.PrivateKey, out var parameters);

            return null;
        }
    }
}